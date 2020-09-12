using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using BMSCore;

namespace BMSPlayer
{
    // 노트 이동을 담당하는 클래스
    public class Scroller : MonoBehaviour
    {
        // ms단위 타이밍
        public static double TIMING = 10000d/60;
        public static double PERFECT = 1000d/60;
        public static double GREAT = 3000d/60;
        public static double GOOD = 8000d/60;

        public TextMesh DebugConsole;

        // Unity objects
        public GameObject startLine; // 노트가 생성되는 위치
        public GameObject lastLine; // 판정선
        public GameObject bgaVideo;
        public GameObject bgaImage;

        // user play info
        private int exscore = 0;
        private int combo = 0;
        private int maxcombo = 0;
        private int processedNotes = 0;
        private int perfect = 0;
        private int great = 0;
        private int good = 0;
        private int ok = 0;
        private int miss = 0;
        private int cb = 0;
        private double sumTimeDiff = 0;
        private double avgTimeDiff = 0;
        private double sumRate = 0;
        private double avgRate = 0;

        private int speed = 200;
        private SpdType spdType;

        // 배속 변경 후 롱노트 넓이 변경에 사용
        // 초기설정이 true여야 처음에 변경됨
        private bool isSpeedChanged = true;
        
        // 체력 관리
        private JudgeType judgeType;
        private HPController hpController;

        private bool isGameOver = false;
        private bool isGameOverEnabled = true;
        private bool[] isAxisPushed;

        private bool BGAOnChecked = false;

        // 노트 스크롤
        private double noteTiming;
        private double velocity;
        private bool[] btnPushState; // 라인별 처리중 상태 on/off
        private int syncControl = 0;

        // 롱노트 처리 변수
        private bool[] lnprocess; // 라인별 처리중 상태 on/off
        private long[] lncomboTime;
        private Longnote[] lnstartproc;

        // 소리 재생
        private ISoundController soundController;

        // 외부 클래스 연계
        private PlayUI ui;

        public void Init(int noteCount)
        {
            ui = GetComponent<PlayUI>();

            speed = Const.GetSpeedFixed();
            spdType = Const.GetSpdType();

            isAxisPushed = new bool[8] { false, false, false, false, false, false, false, false };
            btnPushState = new bool[8];
            lnprocess = new bool[42];
            lncomboTime = new long[8];
            lnstartproc = new Longnote[8];
            syncControl = Const.GetSync() * 5;

            for (int i = 0; i < 42; i++)
            {
                lnprocess[i] = false;
            }
            for (int i = 0; i < 8; i++)
            {
                lncomboTime[i] = 0;
            }

            // 초기 HP 지정
            hpController = GetComponent<HPController>();
            judgeType = Const.GetJudgeType();
            hpController.SetHPJudgeType(judgeType, noteCount);

            // 사운드 컨트롤러 정의
            if (Const.GetAudio() == 0)
            {
                soundController = GetComponent<SoundControllerUnity>();
            }
            else
            {
                soundController = GetComponent<SoundControllerFMOD>();
            }
        }

        public void moveNotes(double frametime, double bps, double spb,
            List<Longnote> lnlist, List<Note> notes)
        {
            /**
             * 1틱에 노트가 이동한 거리 (noteTimimg) = frametime * bps
             * (배속이 적용되지 않은 실제 노트의 위치임 = 이전 노트위치 - noteTiming)
             * 
             * 배속이 적용된 노트 위치 = (노트위치 - noteTiming) * 배속
             * 
             * 순간속도 (velocity) = noteTiming * 배속
             * 
             * ==========================================
             * 노트 위치에 대해 판정선까지의 시간계산하기
             * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
             */
            noteTiming = frametime * bps;
            velocity = noteTiming * speed / 10;

            double linepos = lastLine.transform.position.z;

            List<Note> removeCandidate = new List<Note>();

            // 기본적으로 롱노트든 뭐든 모든 노트를 내림
            foreach (Note n in notes)
            {
                double posz = n.getPosition() - noteTiming;
                float scrpos = (float)posz * speed / 100;
                n.setPosition(posz);
                n.setScrPos(scrpos);

                // 나중에 시작점 검사 들어가면 그 이후로 활성화
                //if (n.isReleased())
                //{
                if(n.getNotetype() == Note.NOTETYPE.PLAYABLE)
                {
                    GameObject noteobj = n.noteobj;
                    noteobj.transform.localPosition = new Vector3(
                        noteobj.transform.localPosition.x,
                        noteobj.transform.localPosition.y,
                        (float)posz * speed / 100);
                }
                //}

                // 노트가 판정범위 이하로 내려갔을 때의 처리
                if (!n.isLong())
                {
                    /*
                     * 노트 위치에 대해 판정선까지의 시간계산하기
                     * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                     */
                    // 일반노트 미스처리
                    //double time = (notes[i].getScrPos() - (linepos + syncControl)) / bps;
                    double time = GetJudgeTiming(n.getScrPos(), linepos, bps);
                    if (time < TIMING * -1)
                    {
                        miss++;
                        combo = 0;
                        cb++;
                        processedNotes++;
                        hpController.hpChangeMiss();
                        UpdateTiming(time, true);
                        if (n.isReleased()) Destroy(n.noteobj);
                        n.setUsed(true);
                        removeCandidate.Add(n);

                        ui.UpdateHP(hpController.GetHP());
                    }
                }
                else
                {
                    // 롱노트인 경우
                    for (int j = 0; j < lnlist.Count; j++)
                    {
                        // 시작노트가 처리되지 못하고 판정라인 아래로 내려간 경우
                        if (n == lnlist[j].getStart() && !n.isUsed())
                        {
                            /*
                             * 노트 위치에 대해 판정선까지의 시간계산하기
                             * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                             */
                            //double time = (notes[i].getScrPos() - (linepos + syncControl)) / bps;
                            double time = GetJudgeTiming(n.getScrPos(), linepos, bps);
                            if (time < TIMING * -1)
                            {
                                if (!lnprocess[n.getLane()])
                                {
                                    miss++;
                                    combo = 0;
                                    cb++;
                                    processedNotes++;
                                    hpController.hpChangeMiss();
                                    UpdateTiming(time, true);
                                    n.setUsed(true);

                                    ui.UpdateHP(hpController.GetHP());
                                }
                            }
                        }
                        // 롱노트의 끝부분이 판정라인 아래로 내려갔을 경우
                        else if (n == lnlist[j].getEnd())
                        {
                            /*
                             * 노트 위치에 대해 판정선까지의 시간계산하기
                             * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                             */
                            //double time = (notes[i].getScrPos() - (linepos + syncControl)) / bps;
                            double time = GetJudgeTiming(n.getScrPos(), linepos, bps);
                            if (time < TIMING * -1)
                            {
                                lnlist[j].setUsed(true);
                                Destroy(lnlist[j].getStart().noteobj);
                                Destroy(lnlist[j].getMiddle().noteobj);
                                Destroy(n.noteobj);
                                lnlist[j].getStart().setUsed(true);
                                n.setUsed(true);
                                removeCandidate.Add(n);
                                removeCandidate.Add(lnlist[j].getStart());
                                removeCandidate.Add(lnlist[j].getMiddle());
                                lnprocess[n.getLane()] = false;
                                btnPushState[n.getLane()] = false;
                            }
                        }
                    }
                }
            }

            if(isSpeedChanged)
            {
                foreach (Longnote ln in lnlist)
                {
                    if (!ln.isUsed())
                    {
                        float startRealPos = ln.getStart().noteobj.transform.localPosition.z;
                        float endRealPos = ln.getEnd().noteobj.transform.localPosition.z;

                        Note middle = ln.getMiddle();
                        Vector3 middleScale = middle.noteobj.transform.localScale;
                        middle.noteobj.transform.localScale = new Vector3(
                            middleScale.x, middleScale.y, (endRealPos - startRealPos));
                    }
                }
                isSpeedChanged = false;
            }

            // Middle노트 위치 변경
            foreach (Longnote ln in lnlist)
            {
                if(!ln.isUsed())
                {
                    float startRealPos = ln.getStart().noteobj.transform.localPosition.z;
                    float endRealPos = ln.getEnd().noteobj.transform.localPosition.z;

                    Note middle = ln.getMiddle();
                    Vector3 middlePos = middle.noteobj.transform.localPosition;
                    middle.noteobj.transform.localPosition = new Vector3(
                        middlePos.x, middlePos.y, (startRealPos + endRealPos) / 2);
                }
            }

            // 삭제
            foreach (Note n in removeCandidate)
            {
                notes.Remove(n);
            }
        }

        public void play(List<Note> notes, GameObject[] touches, List<Longnote> lnlist,
            BMS bms, ref double bpm, ref double bps, ref double spb,
            int totalNotes, double lines, double totalLen, bool isAuto)
        {
            double linepos = lastLine.transform.position.z;

            List<Note> removeCandidate = new List<Note>();

            if (Input.GetButtonDown("spdup"))
            {
                SpeedUp(bpm);
            }
            if (Input.GetButtonDown("spddn"))
            {
                SpeedDown(bpm);
            }

            for (int i = 0; i < notes.Count; i++)
            {
                Note current = notes[i];
                Note.NOTETYPE type = current.getNotetype();

                if(type == Note.NOTETYPE.PLAYABLE)
                {
                    int line = current.getLane();
                    // 라인 벗어나는 노트는 Controller에서 생성할 때 미리 MUSIC으로 바꿔둠

                    double additionalPos = bps * PERFECT;
                    
                    // 오토
                    if(isAuto)
                    {
                        if (!lnprocess[line] && current.getScrPos() <= linepos + additionalPos && !current.isUsed())
                        {
                            ShowBeam(line, true);
                            //double timing = GetJudgeTiming(current.getScrPos(), linepos);
                            double time = GetJudgeTiming(notes[i].getScrPos(), linepos, bps);
                            ProcessSingleNote(line, time, totalNotes, ref bms, ref current, ref removeCandidate, ref lnlist);//, ref unityAudio, ref channelGroup, ref fmodChannel);

                            if (!current.isLong())
                            {
                                StartCoroutine(AutoTurnoffBeam(line));
                            }
                        }
                        else if(lnprocess[line] && current.getScrPos() <= linepos + additionalPos && !current.isUsed())
                        {
                            foreach(Longnote n in lnlist)
                            {
                                if(n.getEnd() == current)
                                {
                                    double time = GetJudgeTiming(current.getScrPos(), linepos, bps);
                                    AfterTouchLongEnd(time, totalNotes);

                                    ShowBeam(line, false);
                                    current.setUsed(true);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 라인에 버튼 눌렸는지 확인
                        if (btnPushState[line])
                        {
                            // 일반노트 or 롱노트 시작
                            if (!lnprocess[line])
                            {
                                double time = GetJudgeTiming(current.getScrPos(), linepos, bps);

                                if (time > TIMING) continue;

                                // 노트 처리
                                ProcessSingleNote(line, time, totalNotes, ref bms, ref current, ref removeCandidate, ref lnlist);//, ref unityAudio, ref channelGroup, ref fmodChannel);
                            }
                            // 롱노트 처리하는 중간
                            else
                            {
                                // LNCombo 처리
                            }
                        }
                        else
                        {
                            // 라인에 버튼이 눌리지 않았는데 롱노트가 처리중이었던 경우
                            if (lnprocess[line])
                            {
                                // 롱노트 끝 노트의 위치를 확인하여
                                // 거리에 따라 판정 적용
                                foreach (Longnote n in lnlist)
                                {
                                    if (n.getEnd() == current)
                                    {
                                        // 이미 처리된 경우는 break
                                        if (current.isUsed()) break;

                                        // 거리를 계산하여 판정처리
                                        // 너무 일찍 떼는 경우를 생각해야 함

                                        double time = GetJudgeTiming(current.getScrPos(), linepos, bps);

                                        // 롱노트 처리
                                        AfterTouchLongEnd(time, totalNotes);

                                        // 끝 노트이므로 음은 재생하지 않음
                                        lnprocess[line] = false;
                                        lnstartproc[line].getEnd().setUsed(true);
                                        //lnstartproc[line - 1].getStart().setPlayed(true);
                                        lnstartproc[line].setProcessed(true);
                                    }
                                }
                            }
                            // 아니면 하는 동작 없음 (미스처리는 moveNote에서 수행)
                        }
                    }
                }
                else
                {
                    if(current.getScrPos() <= linepos + syncControl)
                    {
                        switch (current.getNotetype())
                        {
                            case Note.NOTETYPE.MUSIC:
                                //PlayKeySound(current.getWav(), current.getLane(), ref bms, ref unityAudio, ref channelGroup, ref fmodChannel);
                                soundController.PlayKeySound(current.getLane(), current.getWav(), ref bms);
                                current.setUsed(true);
                                removeCandidate.Add(current);
                                break;
                            case Note.NOTETYPE.BGA:
                                string notestr = current.getWav();
                                if (bms.mBGAImages.Count > 0)
                                {
                                    if (bms.mBGAImages.ContainsKey(notestr))
                                    {
                                        if (!BGAOnChecked) bgaImage.SetActive(true);
                                        ui.BGAImageSetting(bms.mBGAImages[notestr]);
                                    }
                                }
                                else if (bms.bgaVideoFile != null)
                                {
                                    if (!BGAOnChecked) bgaVideo.SetActive(true);
                                    ui.BGAVideoPlay();
                                }
                                current.setUsed(true);
                                removeCandidate.Add(current);
                                break;
                            case Note.NOTETYPE.BPM:
                                bpm = Convert.ToInt32(current.getWav(), 16);
                                bps = bpm / 240;
                                spb = (double)(240 * 1000) / bpm;
                                current.setUsed(true);
                                removeCandidate.Add(current);
                                break;
                        }
                    }
                }
            }

            // 삭제
            foreach (Note n in removeCandidate)
            {
                notes.Remove(n);
            }

            if (Const.DEBUG)
            {
                DebugConsole.text =
                    "Num / Push / Long status\n" +
                    "T - " + btnPushState[0] + " - " + lnprocess[0] + "\n" +
                    "1 - " + btnPushState[1] + " - " + lnprocess[1] + "\n" +
                    "2 - " + btnPushState[2] + " - " + lnprocess[2] + "\n" +
                    "3 - " + btnPushState[3] + " - " + lnprocess[3] + "\n" +
                    "4 - " + btnPushState[4] + " - " + lnprocess[4] + "\n" +
                    "5 - " + btnPushState[5] + " - " + lnprocess[5] + "\n" +
                    "6 - " + btnPushState[6] + " - " + lnprocess[6] + "\n" +
                    "7 - " + btnPushState[7] + " - " + lnprocess[7] + "\n";
            }
        }

        public void ProcessBeamOnTouch()
        {
            /**
             * 사용자의 플레이에 대한 처리
             * 1. 판정선과 가장 가까운 하나의 노트에 대해서만 처리
             * 2. 판정 범위는 +-TIMING으로 정의
             * 3. 판정 종류
             * 
             * PERFECT = 90% 이상 범위
             * GREAT = 75%~90%
             * GOOD = 40%~75%
             * OK = 0%~40%
             * 모든 게이지의 변화는 정확도를 기준으로 변동됨
             * 
             * s = vt를 이용해 현재 배속을 모두 고려하여 판정 범위를 계산
             * 
             * [처리방법]
             * 1. KeyDown -> 해당 키의 노트 처리 변수를 true로 한다
             * 1) 일반 노트의 노트 처리 변수값을 검사해 true이면 그 시점에 노트 위치를 기준으로 판정하고
             *    노트 처리 변수를 false로 바꾼다
             * 2) 롱노트의 경우 노트 처리 변수가 true이면 롱노트를 처리중임을 체크(bool으로)하고 그 값을 유지한다
             * 2. KeyUp
             * 1) 일반 노트의 경우는 KeyDown시 이미 노트 처리 변수가 false가 되므로 상관없다
             * 2) 롱노트의 경우는 KeyUp시 위치하는 노트를 검사하여 롱노트의 끝이면 판정하고
             *    롱노트의 끝이 닿기 전이면 미스처리한다
             * 
             * [미스처리]
             * 1. 일반노트는 하단 범위 지나면 그냥 미스
             * 2. 롱노트는 중간에 KeyUp이 되거나 끝을 지나치면 미스
             */

            // 유저 입력이 들어왔을 때 beam과 누름 상태 자체에 대해서만 처리

            // 노트의 처리와 상관없이 beam이 나오고 현재 채널의 음이 재생된다
            // 판정 범위 내에 노트가 없으면 채널에 음이 할당되어있지 않으면 (null이면) 무시
            if (Keys.GetKeyAxisDown(Keys.btn1k))
            {
                ShowBeam(0, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn1c))
            {
                if (Keys.dpad1 && !isAxisPushed[0])
                {
                    ShowBeam(0, true);
                    isAxisPushed[0] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn1k))
            {
                ShowBeam(0, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn1c))
            {
                if (Keys.dpad1 && isAxisPushed[0])
                {
                    ShowBeam(0, false);
                    isAxisPushed[0] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn2k))
            {
                ShowBeam(1, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn2c))
            {
                if (Keys.dpad2 && !isAxisPushed[1])
                {
                    ShowBeam(1, true);
                    isAxisPushed[1] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn2k))
            {
                ShowBeam(1, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn2c))
            {
                if (Keys.dpad2 && isAxisPushed[1])
                {
                    ShowBeam(1, false);
                    isAxisPushed[1] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn3k))
            {
                ShowBeam(2, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn3c))
            {
                if (Keys.dpad3 && !isAxisPushed[2])
                {
                    ShowBeam(2, true);
                    isAxisPushed[2] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn3k))
            {
                ShowBeam(2, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn3c))
            {
                if (Keys.dpad3 && isAxisPushed[2])
                {
                    ShowBeam(2, false);
                    isAxisPushed[2] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn4k))
            {
                ShowBeam(3, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn4c))
            {
                if (Keys.dpad4 && !isAxisPushed[3])
                {
                    ShowBeam(3, true);
                    isAxisPushed[3] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn4k))
            {
                ShowBeam(3, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn4c))
            {
                if (Keys.dpad4 && isAxisPushed[3])
                {
                    ShowBeam(3, false);
                    isAxisPushed[3] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn5k))
            {
                ShowBeam(4, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn5c))
            {
                if (Keys.dpad5 && !isAxisPushed[4])
                {
                    ShowBeam(4, true);
                    isAxisPushed[4] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn5k))
            {
                ShowBeam(4, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn5c))
            {
                if (Keys.dpad5 && isAxisPushed[4])
                {
                    ShowBeam(4, false);
                    isAxisPushed[4] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn6k))
            {
                ShowBeam(5, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn6c))
            {
                if (Keys.dpad6 && !isAxisPushed[5])
                {
                    ShowBeam(5, true);
                    isAxisPushed[5] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn6k))
            {
                ShowBeam(5, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn6c))
            {
                if (Keys.dpad6 && isAxisPushed[5])
                {
                    ShowBeam(5, false);
                    isAxisPushed[5] = false;
                }
            }

            // l1 r1 처리
            if (Keys.GetKeyAxisDown(Keys.btn7k))
            {
                ShowBeam(6, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn7c))
            {
                if (Keys.dpad7 && !isAxisPushed[6])
                {
                    ShowBeam(6, true);
                    isAxisPushed[6] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn7k))
            {
                ShowBeam(6, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn7c))
            {
                if (Keys.dpad7 && isAxisPushed[6])
                {
                    ShowBeam(6, false);
                    isAxisPushed[6] = false;
                }
            }

            if (Keys.GetKeyAxisDown(Keys.btn8k))
            {
                ShowBeam(7, true);
            }
            if (Keys.GetKeyAxisDown(Keys.btn8c))
            {
                if (Keys.dpad8 && !isAxisPushed[7])
                {
                    ShowBeam(7, true);
                    isAxisPushed[7] = true;
                }
            }

            if (Keys.GetKeyAxisUp(Keys.btn8k))
            {
                ShowBeam(7, false);
            }
            if (Keys.GetKeyAxisUp(Keys.btn8c))
            {
                if (Keys.dpad8 && isAxisPushed[7])
                {
                    ShowBeam(7, false);
                    isAxisPushed[7] = false;
                }
            }
        }

        public void ProcessSingleNote(int line, double time, int totalNotes,
            ref BMS bms, ref Note note, ref List<Note> removeCandidate,
            ref List<Longnote> lnlist)
            //, ref AudioSource[] unityAudio, ref FMOD.ChannelGroup channelGroup, ref FMOD.Channel[] fmodChannel)
        {
            // 일단 소리는 내고 시작
            //PlayKeySound(note.getWav(), note.getLane(), ref bms, ref unityAudio, ref channelGroup, ref fmodChannel);
            soundController.PlayKeySound(note.getLane(), note.getWav(), ref bms);
            
            // 해당 노트가 롱노트에 속한다면
            if(note.isLong()) {
                foreach(Longnote n in lnlist) {
                    // 화면에 있는 롱노트 중
                    if(note == n.getStart()) {
                        lnstartproc[line] = n;
                        lnstartproc[line].setProcTime(time);
                        lnprocess[line] = true;
                        note.setUsed(true);
                        AfterTouchLongStart(line, time, totalNotes, ref note, ref removeCandidate);
                        break;
                    }
                }
            }
            else {
                TimingType timingType = GetTimingType(time, false);
                switch (timingType)
                {
                    case TimingType.PERFECT:
                        perfect++;
                        hpController.hpChangePerfect();
                        exscore += 2;
                        AfterTouchSingle(line, time, totalNotes, ref note, ref removeCandidate);
                        break;
                    case TimingType.GREAT:
                        great++;
                        hpController.hpChangeGreat();
                        exscore += 1;
                        AfterTouchSingle(line, time, totalNotes, ref note, ref removeCandidate);
                        break;
                    case TimingType.GOOD:
                        good++;
                        hpController.hpChangeGood();
                        AfterTouchSingle(line, time, totalNotes, ref note, ref removeCandidate);
                        break;
                    case TimingType.BAD:
                        ok++;
                        hpController.hpChangeBad();
                        AfterTouchSingle(line, time, totalNotes, ref note, ref removeCandidate);
                        break;
                }

                ui.UpdateHP(hpController.GetHP());
            }
        }

        // 롱노트의 끝 노트 타이밍을 처리하는 메소드
        public void AfterTouchLongEnd(double time, int totalNotes)
        {
            switch(GetTimingType(time, true))
            {
                case TimingType.PERFECT:
                    perfect++;
                    hpController.hpChangePerfect();
                    exscore += 2;
                    break;
                case TimingType.GREAT:
                    great++;
                    hpController.hpChangeGreat();
                    exscore += 1;
                    break;
                case TimingType.GOOD:
                    good++;
                    hpController.hpChangeGood();
                    break;
                case TimingType.BAD:
                    ok++;
                    combo = 0;
                    cb++;
                    hpController.hpChangeBad();
                    break;
                case TimingType.POOR:
                    miss++;
                    combo = 0;
                    cb++;
                    break;
            }

            UpdateTiming(time, true);
            UpdateScore(totalNotes);
            ui.UpdateHP(hpController.GetHP());
        }

        // 싱글 노트 후처리
        public void AfterTouchSingle(int line, double time, int totalNotes, ref Note note, ref List<Note> removeCandidate)
        {
            processedNotes++;
            UpdateTiming(time, true);
            UpdateScore(totalNotes);
            Destroy(note.noteobj);
            note.setUsed(true);
            removeCandidate.Add(note);
            btnPushState[line] = false;
        }

        public void AfterTouchLongStart(int line, double time, int totalNotes, ref Note note, ref List<Note> removeCandidate)
        {
            processedNotes++;
            UpdateTiming(time, false);
        }

        private bool CheckAutoNearNotes()
        {
            bool noteExist = false;

            return noteExist;
        }

        private void UpdateTiming(double time, bool rateadd)
        {
            double abstime = Math.Abs(time);
            TimingType timingType = GetTimingType(abstime, false);

            if(timingType == TimingType.POOR)
            {
                combo = 0;
                ui.UpdateJudge(timingType, combo, "0.00%", 0);
            }
            else if(timingType == TimingType.BAD)
            {
                sumRate += (1 - abstime / TIMING) * 100;
                combo = 0;
                ui.UpdateJudge(timingType, combo, "0.00%", 0);
            }
            else
            {
                if (rateadd) sumRate += (1 - abstime / TIMING) * 100;

                combo += 1;
                sumTimeDiff += time;

                if (combo > maxcombo)
                {
                    maxcombo = combo;
                    ui.UpdateMaxCombo(maxcombo);
                }

                ui.UpdateJudge(timingType, combo, ((1 - abstime / TIMING) * 100).ToString("0.00") + "%", (int)Math.Round(time));
            }

            avgRate = Math.Round(sumRate * 100 / processedNotes) / 100;
            avgTimeDiff = sumTimeDiff / processedNotes;
            ui.UpdateSideJudge(perfect, great, good, ok, miss,
                avgRate.ToString("0.00") + "%", avgTimeDiff.ToString("0.0") + "ms");
        }

        IEnumerator AutoTurnoffBeam(int i)
        {
            yield return new WaitForSeconds(0.1f);
            ShowBeam(i, false);
        }

        public double GetJudgeTiming(double scrpos, double linepos, double bps)
        {
            // 판정선으로부터의 거리
            double distance = scrpos - (linepos + syncControl);

            // 현재 위치에서 판정선까지 걸리는 시간
            double time = distance / bps / 4;

            return time;
        }

        // Timing value로 현재 판정을 알아내는 메소드
        private TimingType GetTimingType(double time, bool isLongEndCheck)
        {
            double abstime = Math.Abs(time);
            if (abstime <= PERFECT) return TimingType.PERFECT;
            else if (abstime > PERFECT && abstime <= GREAT) return TimingType.GREAT;
            else if (abstime > GREAT && abstime <= GOOD) return TimingType.GOOD;
            else if (abstime > GOOD && abstime <= TIMING) return TimingType.BAD;
            else if (abstime > TIMING) return TimingType.POOR;
            else return TimingType.NONE;
        }

        private void UpdateScore(int totalNotes)
        {
            ui.UpdateExScore(exscore);
            ui.UpdateGraph(exscore, processedNotes, totalNotes);
        }

        private void ShowBeam(int lane, bool onoff)
        {
            ui.ShowAndHideBeam(lane, onoff);
            if (onoff) {
                btnPushState[lane] = true;
            }
            else {
                btnPushState[lane] = false;
            }
        }

        public bool CheckGameOver()
        {
            if (isGameOverEnabled)
            {
                if (hpController.isHpMin())
                {
                    isGameOver = true;
                    Const.SetClear(0);
                }
            }
            return isGameOver;
        }

        public void SetGameOver()
        {
            isGameOver = true;
        }

        public void GetResultData(int totalNotes)
        {
            Const.SetResultPerfect(perfect);
            Const.SetResultGreat(great);
            Const.SetResultGood(good);
            Const.SetResultOk(ok);
            Const.SetResultMiss(miss);
            Const.SetResultComboBreak(cb);
            Const.SetResultAvgRate((float)avgRate);
            Const.SetResultTimeDiff((float)avgTimeDiff);
            //Const.SetResultScore(score);
            Const.SetResultExScore(exscore);
            Const.SetResultMaxCombo(maxcombo);
        }

        public void SpeedUp(double bpm) {
            if (speed < 1000)
            {
                speed += 25;
                Const.SetSpeedFixed(speed);
                ui.UpdateSpeed(bpm);
            }
        }

        public void SpeedDown(double bpm) {
            if (speed > 50)
            {
                speed -= 25;
                Const.SetSpeedFixed(speed);
                ui.UpdateSpeed(bpm);
            }
        }
    }
}
