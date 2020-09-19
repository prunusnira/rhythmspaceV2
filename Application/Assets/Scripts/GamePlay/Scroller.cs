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

        // 공푸어 처리용
        private int notesInTimingWindow = 0; // 개수가 0보다 크면 노트가 있음

        private int speed = 200;
        private int speedfl = 200;
        private SpdType spdType;

        // 체력 관리
        private JudgeType judgeType;
        private HPController hpController;

        // 패드용 눌림 체크
        private bool[] isAxisPushed;

        private bool BGAOnChecked = false;

        // 노트 스크롤
        private double noteTiming;
        private bool[] btnPushState; // 라인별 처리중 상태 on/off
        private bool[] btnPushSound; // 라인 눌렀을 때 소리를 재생했는지 여부 확인
        private int syncControl = 0;

        // 롱노트 처리 변수
        private bool[] isLnWorking; // 라인별 처리중 상태 on/off
        private double[] lnTiming;
        private Longnote[] lnInProcess;

        // 소리 재생
        private ISoundController soundController;

        // 외부 클래스 연계
        private PlayUI ui;

        public void Init(int noteCount)
        {
            ui = GetComponent<PlayUI>();

            speed = Const.GetSpeedFixed();
            speedfl = Const.GetSpeedFluid();
            spdType = Const.GetSpdType();

            isAxisPushed = new bool[8] { false, false, false, false, false, false, false, false };
            btnPushState = new bool[8] { false, false, false, false, false, false, false, false };
            btnPushSound = new bool[8] { false, false, false, false, false, false, false, false };
            isLnWorking = new bool[42];
            lnTiming = new double[8];
            lnInProcess = new Longnote[8];
            syncControl = Const.GetSync() * 5;

            for (int i = 0; i < 42; i++)
            {
                isLnWorking[i] = false;
            }
            for (int i = 0; i < 8; i++)
            {
                lnTiming[i] = 0;
            }

            // 초기 HP 지정
            hpController = GetComponent<HPController>();
            judgeType = Const.GetJudgeType();
            hpController.SetHPJudgeType(judgeType, noteCount);

            // 사운드 컨트롤러 정의
            /*if (Const.GetAudio() == 0)
            {
                soundController = GetComponent<SoundControllerUnity>();
            }
            else
            {*/
                soundController = GetComponent<SoundControllerFMOD>();
            //}
        }

        public void MoveNotes(List<Note>[] notePlay, List<Longnote> lnlist,
            double noteMove, ref double bps)
        {
            double linepos = lastLine.transform.localPosition.z;

            // 기본적으로 롱노트든 뭐든 모든 노트를 내림
            foreach(List<Note> notes in notePlay)
            {
                List<Note> removeCandidate = new List<Note>();

                for (int i = 0; i < notes.Count; i++)
                {
                    Note current = notes[i];
                    float scrpos = MoveActualNotePosition(ref current, noteMove);

                    // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                    if (!current.isReleased() && current.getScrPos() < 3000)
                    {
                        ui.displayNote(ref current, ref lnlist);
                    }

                    // 실제 오브젝트가 존재할 때 위치를 이동시킴
                    if (current.noteobj != null)
                    {
                        GameObject noteobj = current.noteobj;
                        noteobj.transform.localPosition = new Vector3(
                            noteobj.transform.localPosition.x,
                            noteobj.transform.localPosition.y,
                            scrpos);
                    }

                    // 롱노트 가운데 노트의 위치와 넓이 설정
                    if (current.isLnEnd() && current.isReleased() &&
                        !current.isUsed())
                    {
                        // isSpeedChanged는 모든 롱노트를 변경한 후에
                        // false로 바꾸어야하므로 for문 아래에 둠
                        Longnote ln = lnlist[current.getLnNum()];
                        double startRealPos = ln.getStart().getScrPos();
                        double endRealPos = ln.getEnd().getScrPos();

                        Note middle = ln.getMiddle();

                        // 스케일 변경
                        Vector3 middleScale = middle.noteobj.transform.localScale;
                        middle.noteobj.transform.localScale = new Vector3(
                            middleScale.x, middleScale.y, (float)(endRealPos - startRealPos));

                        // 위치 변경
                        Vector3 middlePos = middle.noteobj.transform.localPosition;
                        middle.noteobj.transform.localPosition = new Vector3(
                            middlePos.x, middlePos.y, (float)((startRealPos + endRealPos) / 2));
                    }

                    
                    double time = GetJudgeTiming(current.getScrPos(), linepos, bps);

                    // 노트가 타이밍윈도우 내에 진입하면 공푸어 계산을 위해 값을 추가
                    if(time < TIMING)
                    {
                        notesInTimingWindow++;
                    }

                    // 노트가 판정선 이하로 내려가면 사라짐
                    // 노트의 위치는 판정선과 노트 사이의 도달 시간으로 계산
                    /*
                     * 노트 위치에 대해 판정선까지의 시간계산하기
                     * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                     */
                    if (time < TIMING * -1 &&
                        current.getNotetype() == Note.NOTETYPE.PLAYABLE &&
                        current.isReleased())
                    {
                        if (!current.isLong() && !current.isUsed())
                        {
                            // 데이터 처리
                            miss++;
                            combo = 0;
                            cb++;
                            processedNotes++;
                            hpController.hpChangeMiss();
                            UpdateTiming(time, true);
                            current.setUsed(true);
                            ui.UpdateHP(hpController.GetHP());

                            // 노트 삭제
                            Destroy(current.noteobj);
                            removeCandidate.Add(current);
                        }
                        else if (current.isLnStart() && !current.isUsed() &&
                            !lnlist[current.getLnNum()].isInProgress())
                        {
                            // 시작 위치에서는 롱노트 전체 형태를 유지해야 하므로 없애지 않음
                            miss++;
                            combo = 0;
                            cb++;
                            hpController.hpChangeMiss();
                            UpdateTiming(time, true);
                        }
                        else if (current.isLnMid() && !current.isUsed())
                        {
                            current.setUsed(true);
                        }
                        else if (current.isLnEnd() && !current.isUsed())
                        {
                            // 롱노트 관련 데이터 처리
                            int lnNum = current.getLnNum();
                            isLnWorking[current.getLane()] = false;
                            lnlist[lnNum].getStart().setUsed(true);
                            lnlist[lnNum].setUsed(true);
                            current.setUsed(true);
                            processedNotes++;

                            // 롱노트 구성요소를 모두 삭제처리
                            Destroy(lnlist[lnNum].getStart().noteobj);
                            Destroy(lnlist[lnNum].getMiddle().noteobj);
                            Destroy(current.noteobj);
                            removeCandidate.Add(current);
                            removeCandidate.Add(lnlist[lnNum].getStart());
                            removeCandidate.Add(lnlist[lnNum].getMiddle());
                        }
                    }
                }
                
                // 삭제
                foreach (Note n in removeCandidate)
                {
                    notes.Remove(n);
                }
            }
        }

        private float MoveActualNotePosition(ref Note n, double noteMove)
        {
            double posz = n.getPosition() - noteMove;
            float scrpos = (float)posz * speed / 100;
            n.setPosition(posz);
            n.setScrPos(scrpos);
            return scrpos;
        }

        public void PlayBGA(List<Note> noteBGA, BMS bms, double noteMove)
        {
            double linepos = lastLine.transform.localPosition.z;
            List<Note> removeCandidate = new List<Note>();

            for (int i = 0; i < noteBGA.Count; i++)
            {
                Note current = noteBGA[i];
                MoveActualNotePosition(ref current, noteMove);

                if (current.getScrPos() <= linepos)
                {
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
                }
            }

            foreach (Note n in removeCandidate)
            {
                noteBGA.Remove(n);
            }
        }

        public void PlayBGM(List<Note> noteBGM, BMS bms, double noteMove)
        {
            double linepos = lastLine.transform.localPosition.z;
            List<Note> removeCandidate = new List<Note>();

            for (int i = 0; i < noteBGM.Count; i++)
            {
                Note current = noteBGM[i];
                MoveActualNotePosition(ref current, noteMove);

                if (current.getScrPos() <= linepos)
                {
                    soundController.PlayKeySound(current.getLane(), current.getWav(), ref bms);
                    current.setUsed(true);
                    removeCandidate.Add(current);
                }
            }

            foreach (Note n in removeCandidate)
            {
                noteBGM.Remove(n);
            }
        }

        public void PlayBPM(List<Note> noteBPM, BMS bms, double noteMove,
            ref double bpm, ref double bps, ref double spb)
        {
            double linepos = lastLine.transform.localPosition.z;
            List<Note> removeCandidate = new List<Note>();

            for (int i = 0; i < noteBPM.Count; i++)
            {
                Note current = noteBPM[i];
                MoveActualNotePosition(ref current, noteMove);

                if (current.getScrPos() <= linepos)
                {
                    switch (current.getNotetype())
                    {
                        case Note.NOTETYPE.BPM:
                            bpm = Convert.ToInt32(current.getWav(), 16);
                            bps = bpm / 240;
                            spb = (double)(240 * 1000) / bpm;
                            current.setUsed(true);
                            removeCandidate.Add(current);
                            ui.SetGearCurBPM(bpm);
                            break;
                        case Note.NOTETYPE.BPMT2:
                            bpm = bms.mBPMNum[current.getWav()];
                            bps = bpm / 240;
                            spb = (double)(240 * 1000) / bpm;
                            current.setUsed(true);
                            removeCandidate.Add(current);
                            ui.SetGearCurBPM(bpm);
                            break;
                    }
                }
            }

            foreach (Note n in removeCandidate)
            {
                noteBPM.Remove(n);
            }
        }

        public void AutoPlay(List<Note>[] notePlay, List<Longnote> lnlist,
            BMS bms, ref double bps, int totalNotes)
        {
            double linepos = lastLine.transform.localPosition.z;
            double additionalPos = bps * PERFECT;
            List<Note> removeCandidate = new List<Note>();

            // 각 라인에 대해 검사
            for (int i = 0; i < Const.LINE; i++)
            {
                // 1. 현재 라인을 가져옴
                List<Note> currentLine = notePlay[i];

                // 2. 현재 라인의 첫 노트를 가져옴
                if (currentLine.Count == 0) continue;

                Note cnote = currentLine[0];

                if (cnote.isLnStart() && lnlist[cnote.getLnNum()].isInProgress())
                {
                    cnote = currentLine[1];
                }

                // 3. 해당 노트의 위치가 판정선 아래인지 확인
                if (cnote.getScrPos() <= linepos + additionalPos)
                {
                    // 롱노트 진행중이 아닐 때의 처리
                    // (일반노트 & 롱노트 시작)
                    if (!isLnWorking[i])
                    {
                        // 눌림 상태 처리
                        btnPushState[i] = true;

                        // 소리 내기
                        if (!btnPushSound[i])
                        {
                            soundController.PlayKeySound(cnote.getLane(), cnote.getWav(), ref bms);
                            btnPushSound[i] = true;
                        }

                        double time = GetJudgeTiming(cnote.getScrPos(), linepos, bps);
                        
                        if (cnote.isLnStart())
                        {
                            ProcessLNStartNote(ref cnote, ref lnlist, time, i);
                            lnlist[cnote.getLnNum()].setInProgress(true);
                        }
                        else if(!cnote.isLong())
                        {
                            ProcessSingleNote(i, time, totalNotes, ref bms, ref cnote, ref removeCandidate, ref lnlist);
                            StartCoroutine(AutoTurnoffBeam(i));
                        }

                        btnPushSound[i] = false;
                    }
                    // 롱노트의 끝노트일 때 처리
                    else
                    {
                        if (cnote.isLnEnd())
                        {
                            double time = lnTiming[cnote.getLane()];
                            AfterTouchLongEnd(time, totalNotes);

                            btnPushState[i] = false;
                            isLnWorking[i] = false;
                            btnPushSound[i] = false;
                            cnote.setUsed(true);

                            removeCandidate.Add(lnlist[cnote.getLnNum()].getStart());
                            removeCandidate.Add(lnlist[cnote.getLnNum()].getMiddle());
                            removeCandidate.Add(cnote);
                        }
                    }
                }

                foreach (Note n in removeCandidate)
                {
                    Destroy(n.noteobj);
                    notePlay[i].Remove(n);
                }
            }

            /*foreach (List<Note> notes in notePlay)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    Note current = notes[i];
                    Note.NOTETYPE type = current.getNotetype();

                    if (type == Note.NOTETYPE.PLAYABLE)
                    {
                        int line = current.getLane();

                        // (일반노트 혹은 롱노트의 시작)
                        if (!isLnWorking[line] &&
                            current.getScrPos() <= linepos + additionalPos &&
                            !current.isUsed())
                        {
                            btnPushState[line] = true;

                            if (!btnPushSound[line])
                            {
                                soundController.PlayKeySound(current.getLane(), current.getWav(), ref bms);
                                btnPushSound[line] = true;
                            }

                            double time = GetJudgeTiming(notes[i].getScrPos(), linepos, bps);
                            ProcessSingleNote(line, time, totalNotes, ref bms, ref current, ref removeCandidate, ref lnlist);

                            if (!current.isLong())
                            {
                                StartCoroutine(AutoTurnoffBeam(line));
                                btnPushSound[line] = false;
                                notes.Remove(current);
                            }
                        }
                        // (롱노트가 끝날때의 처리)
                        else if (isLnWorking[line] &&
                            current.getScrPos() <= linepos + additionalPos &&
                            !current.isUsed())
                        {
                            if (current.isLnEnd())
                            {
                                //double time = GetJudgeTiming(current.getScrPos(), linepos, bps);
                                double time = lnTiming[current.getLane()];
                                AfterTouchLongEnd(time, totalNotes);

                                //ShowBeam(line, false);
                                isLnWorking[line] = false;
                                btnPushState[line] = false;
                                btnPushSound[line] = false;
                                current.setUsed(true);
                                notes.Remove(current);
                            }
                        }
                    }
                }
            }*/
        }

        public void SpeedChangeAndBeam(double bpm)
        {
            if (Input.GetButtonDown("spdup"))
            {
                if (spdType == SpdType.FIXED)
                {
                    SpeedUpFixed(bpm);
                }
            }
            if (Input.GetButtonDown("spddn"))
            {
                if (spdType == SpdType.FIXED)
                {
                    SpeedDownFixed(bpm);
                }
            }
            if (Input.GetButton("spdup"))
            {
                if (spdType == SpdType.FLUID)
                {
                    SpeedUpFluid(bpm);
                }
            }
            if (Input.GetButton("spddn"))
            {
                if (spdType == SpdType.FLUID)
                {
                    SpeedDownFluid(bpm);
                }
            }

            // 버튼 푸시 상태에 따라 빔 표시 상태 변경
            for (int i = 0; i < btnPushState.Length; i++)
            {
                if (btnPushState[i])
                {
                    ShowBeam(i, true);
                }
                else
                {
                    ShowBeam(i, false);
                }
            }
        }

        public void Play(List<Note>[] notePlay, List<Longnote> lnlist, BMS bms,
            ref double bpm, ref double bps, ref double spb, int totalNotes)
        {
            double linepos = lastLine.transform.localPosition.z;

            /**
             * 노트 처리하기
             * 
             * Step 1.
             * 노트를 일단 누른다 (ProcessBeamOnTouch())
             * 
             * Step 2.
             * 눌려진 노트 라인을 확인하여 그 리스트의 제일 첫 노트를 확인한다
             * 
             * Step 3.
             * 해당 노트의 소리를 낸다
             * 
             * Step 4.
             * Timing 계산 후 판정 처리
             */

            // 각 라인에 대해 검사
            for (int i = 0; i < Const.LINE; i++)
            {
                // 현재 라인이 눌려진 상태이면
                if (btnPushState[i])
                {
                    // 1. 현재 라인을 가져옴
                    List<Note> currentLine = notePlay[i];

                    // 2. 현재 라인의 첫 노트를 가져옴
                    Note cnote = currentLine[0];

                    // 3. 노트 존재 유무와 상관없이 소리를 냄
                    if (!btnPushSound[i])
                    {
                        soundController.PlayKeySound(cnote.getLane(), cnote.getWav(), ref bms);
                        btnPushSound[i] = true;
                    }

                    // 4. 판정 처리하기
                }
            }

            /*foreach (List<Note> notes in notePlay)
            {
                List<Note> removeCandidate = new List<Note>();
                for (int i = 0; i < notes.Count; i++)
                {
                    Note current = notes[i];
                    Note.NOTETYPE type = current.getNotetype();

                    if (type == Note.NOTETYPE.PLAYABLE && !isAuto)
                    {
                        int line = current.getLane();
                        double additionalPos = bps * PERFECT;

                        // 라인에 버튼 눌렸는지 확인
                        if (btnPushState[line])
                        {
                            // 일반노트 or 롱노트 시작
                            if (!isLnWorking[line] && !current.isUsed())
                            {
                                double time = GetJudgeTiming(current.getScrPos(), linepos, bps);
                                if (time > TIMING) continue;


                                // 노트 처리
                                ProcessSingleNote(line, time, totalNotes, ref bms, ref current, ref removeCandidate, ref lnlist);
                            }
                        }
                        else
                        {
                            // 라인에 버튼이 눌리지 않았는데 롱노트가 처리중이었던 경우
                            if (isLnWorking[line])
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
                                        isLnWorking[line] = false;
                                        lnInProcess[line].getEnd().setUsed(true);
                                        //lnstartproc[line - 1].getStart().setPlayed(true);
                                        lnInProcess[line].setProcessed(true);
                                    }
                                }
                            }
                            // 아니면 하는 동작 없음 (미스처리는 moveNote에서 수행)
                        }
                    }
                }
                
                // 삭제
                foreach (Note n in removeCandidate)
                {
                    notes.Remove(n);
                }
            }*/

            if (Const.DEBUG)
            {
                DebugConsole.text =
                    "Num / Push / Long status\n" +
                    "T - " + btnPushState[0] + " - " + isLnWorking[0] + "\n" +
                    "1 - " + btnPushState[1] + " - " + isLnWorking[1] + "\n" +
                    "2 - " + btnPushState[2] + " - " + isLnWorking[2] + "\n" +
                    "3 - " + btnPushState[3] + " - " + isLnWorking[3] + "\n" +
                    "4 - " + btnPushState[4] + " - " + isLnWorking[4] + "\n" +
                    "5 - " + btnPushState[5] + " - " + isLnWorking[5] + "\n" +
                    "6 - " + btnPushState[6] + " - " + isLnWorking[6] + "\n" +
                    "7 - " + btnPushState[7] + " - " + isLnWorking[7] + "\n";
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

            
            // Button Down Check
            for(int i = 0; i < Keys.btnkb.Length; i++)
            {
                if (Keys.GetKeyAxisDown(Keys.btnkb[i]))
                {
                    btnPushState[i] = true;
                }
                if (Keys.GetKeyAxisDown(Keys.btnct[i]))
                {
                    if (Keys.dpad[i] && !isAxisPushed[i])
                    {
                        btnPushState[i] = true;
                        isAxisPushed[i] = true;
                    }
                }

                if (Keys.GetKeyAxisUp(Keys.btnkb[i]))
                {
                    btnPushState[i] = false;
                    btnPushSound[i] = false;
                }
                if (Keys.GetKeyAxisUp(Keys.btnct[i]))
                {
                    if (Keys.dpad[i] && isAxisPushed[i])
                    {
                        btnPushState[i] = false;
                        btnPushSound[i] = false;
                        isAxisPushed[i] = false;
                    }
                }
            }
        }

        public void ProcessLNStartNote(ref Note note, ref List<Longnote> lnlist,
            double time, int line)
        {
            Longnote n = lnlist[note.getLnNum()];
            lnInProcess[line] = n;
            lnInProcess[line].setProcTime(time);
            isLnWorking[line] = true;
            lnTiming[line] = time;
        }

        public void ProcessSingleNote(int line, double time, int totalNotes,
            ref BMS bms, ref Note note, ref List<Note> removeCandidate,
            ref List<Longnote> lnlist)
        {
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
            notesInTimingWindow--;
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
            else if(rateadd)
            {
                sumRate += (1 - abstime / TIMING) * 100;

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
            yield return new WaitForSeconds(0.05f);
            btnPushState[i] = false;
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
            Const.SetResultScore(exscore);
            Const.SetResultMaxCombo(maxcombo);
        }

        private void SpeedUpFixed(double bpm)
        {
            if (speed < 1000)
            {
                speed += 25;
                Const.SetSpeedFixed(speed);
                Const.SetSpeedFluid((int)(speed * bpm / 100));
                ui.UpdateSpeed();
            }
        }

        private void SpeedDownFixed(double bpm)
        {
            if (speed > 50)
            {
                speed -= 25;
                Const.SetSpeedFixed(speed);
                Const.SetSpeedFluid((int)(speed * bpm / 100));
                ui.UpdateSpeed();
            }
        }

        private void SpeedUpFluid(double bpm)
        {
            if (speedfl < 1000)
            {
                speedfl += 1;
                speed = (int)(speedfl / bpm * 100);
                Const.SetSpeedFluid(speedfl);
                Const.SetSpeedFixed((int)(speedfl / bpm * 100));
                ui.UpdateSpeed();
            }
        }

        private void SpeedDownFluid(double bpm)
        {
            if (speedfl > 50)
            {
                speedfl -= 1;
                speed = (int)(speedfl / bpm * 100);
                Const.SetSpeedFluid(speedfl);
                Const.SetSpeedFixed((int)(speedfl / bpm * 100));
                ui.UpdateSpeed();
            }
        }

        public int GetProcessedNotes()
        {
            return processedNotes;
        }
    }
}
