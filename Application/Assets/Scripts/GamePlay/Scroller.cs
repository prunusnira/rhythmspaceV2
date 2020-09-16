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
            btnPushState = new bool[8];
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
            if (Const.GetAudio() == 0)
            {
                soundController = GetComponent<SoundControllerUnity>();
            }
            else
            {
                soundController = GetComponent<SoundControllerFMOD>();
            }
        }

        public void moveNotes(double frametime, double bps,
            List<Longnote> lnlist, List<Note> notes)
        {
            /**
             * 1틱에 노트가 이동한 거리 (noteTimimg) = frametime * bps
             * (배속이 적용되지 않은 실제 노트의 위치임 = 이전 노트위치 - noteTiming)
             * 
             * 배속이 적용된 노트 위치 = (노트위치 - noteTiming) * 배속
             * 
             * 순간속도 = noteTiming * 배속
             * 
             * ==========================================
             * 노트 위치에 대해 판정선까지의 시간계산하기
             * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
             */
            noteTiming = frametime * bps;

            double linepos = lastLine.transform.localPosition.z;

            List<Note> removeCandidate = new List<Note>();

            // 기본적으로 롱노트든 뭐든 모든 노트를 내림
            for (int i = 0; i < notes.Count; i++)
            {
                Note n = notes[i];
                double posz = n.getPosition() - noteTiming;
                float scrpos = (float)posz * speed / 100;
                n.setPosition(posz);
                n.setScrPos(scrpos);

                // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                if(!n.isReleased() && scrpos < 3000)
                {
                    ui.displayNote(ref n, ref lnlist);
                }

                // 실제 오브젝트가 존재할 때 위치를 이동시킴
                if(n.noteobj != null)
                {
                    GameObject noteobj = n.noteobj;
                    noteobj.transform.localPosition = new Vector3(
                        noteobj.transform.localPosition.x,
                        noteobj.transform.localPosition.y,
                        (float)posz * speed / 100);
                }

                // 롱노트 가운데 노트의 초기 위치와 넓이 설정
                if(n.isLnEnd() && n.isReleased())
                {
                    // isSpeedChanged는 모든 롱노트를 변경한 후에
                    // false로 바꾸어야하므로 for문 아래에 둠
                    Longnote ln = lnlist[n.getLnNum()];
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

                // 노트가 판정선 이하로 내려가면 사라짐
                // 노트의 위치는 판정선과 노트 사이의 도달 시간으로 계산
                /*
                 * 노트 위치에 대해 판정선까지의 시간계산하기
                 * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                 */
                double time = GetJudgeTiming(n.getScrPos(), linepos, bps);
                if (time < TIMING * -1 &&
                    n.getNotetype() == Note.NOTETYPE.PLAYABLE)
                {
                    if(!n.isLong() && !n.isUsed())
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
                    else if(n.isLnStart() && !n.isUsed())
                    {
                        // 시작 위치에서는 롱노트 전체 형태를 유지해야 하므로 없애지 않음
                        miss++;
                        combo = 0;
                        cb++;
                        hpController.hpChangeMiss();
                        UpdateTiming(time, true);
                        n.setUsed(true);
                    }
                    else if(n.isLnMid() && !n.isUsed())
                    {
                        n.setUsed(true);
                    }
                    else if(n.isLnEnd())
                    {
                        // 롱노트 구성요소를 모두 삭제처리 하는 과정
                        processedNotes++;
                        int lnNum = n.getLnNum();
                        lnlist[lnNum].setUsed(true);
                        Destroy(lnlist[lnNum].getStart().noteobj);
                        Destroy(lnlist[lnNum].getMiddle().noteobj);
                        Destroy(n.noteobj);
                        lnlist[lnNum].getStart().setUsed(true);
                        n.setUsed(true);
                        removeCandidate.Add(n);
                        removeCandidate.Add(lnlist[lnNum].getStart());
                        removeCandidate.Add(lnlist[lnNum].getMiddle());
                        isLnWorking[n.getLane()] = false;
                        btnPushState[n.getLane()] = false;
                    }
                }

            }

            // 삭제
            foreach (Note n in removeCandidate)
            {
                notes.Remove(n);
            }
        }

        public void autoPlay(List<Note> notes, List<Longnote> lnlist,
            BMS bms, ref double bps, int totalNotes)
        {
            double linepos = lastLine.transform.localPosition.z;
            List<Note> removeCandidate = new List<Note>();

            for (int i = 0; i < notes.Count; i++)
            {
                Note current = notes[i];
                Note.NOTETYPE type = current.getNotetype();

                if(type == Note.NOTETYPE.PLAYABLE)
                {
                    int line = current.getLane();
                    double additionalPos = bps * PERFECT;

                    /**
                     * 롱노트를 처리중이 아니면서
                     * 판정선에 노트가 닿고
                     * 현재 노트가 미사용상태일 경우
                     * (일반노트 혹은 롱노트의 시작)
                     */
                    if (!isLnWorking[line] &&
                        current.getScrPos() <= linepos + additionalPos &&
                        !current.isUsed())
                    {
                        //ShowBeam(line, true);
                        btnPushState[line] = true;
                        double time = GetJudgeTiming(notes[i].getScrPos(), linepos, bps);
                        ProcessSingleNote(line, time, totalNotes, ref bms, ref current, ref removeCandidate, ref lnlist);

                        if (!current.isLong())
                        {
                            StartCoroutine(AutoTurnoffBeam(line));
                        }
                    }
                    /**
                     * 롱노트가 처리중이며
                     * 판정선에 노트가 닿고
                     * 현재 노트가 미사용상태일 경우
                     * (롱노트가 끝날때의 처리)
                     */
                    else if (isLnWorking[line] &&
                        current.getScrPos() <= linepos + additionalPos &&
                        !current.isUsed())
                    {
                        if(current.isLnEnd())
                        {
                            //double time = GetJudgeTiming(current.getScrPos(), linepos, bps);
                            double time = lnTiming[current.getLane()];
                            AfterTouchLongEnd(time, totalNotes);

                            //ShowBeam(line, false);
                            isLnWorking[line] = false;
                            btnPushState[line] = false;
                            current.setUsed(true);
                        }
                    }
                }
            }
        }

        public void play(List<Note> notes, List<Longnote> lnlist, BMS bms,
            ref double bpm, ref double bps, ref double spb, int totalNotes, bool isAuto)
        {
            double linepos = lastLine.transform.localPosition.z;

            List<Note> removeCandidate = new List<Note>();

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
            if(Input.GetButton("spdup"))
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
                if(btnPushState[i])
                {
                    ShowBeam(i, true);
                }
                else
                {
                    ShowBeam(i, false);
                }
            }

            for (int i = 0; i < notes.Count; i++)
            {
                Note current = notes[i];
                Note.NOTETYPE type = current.getNotetype();

                if(type == Note.NOTETYPE.PLAYABLE && !isAuto)
                {
                    int line = current.getLane();
                    // 라인 벗어나는 노트는 Controller에서 생성할 때 미리 MUSIC으로 바꿔둠

                    double additionalPos = bps * PERFECT;
                    
                    // 라인에 버튼 눌렸는지 확인
                    if (btnPushState[line])
                    {
                        // 일반노트 or 롱노트 시작
                        if (!isLnWorking[line])
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
                }
                if (Keys.GetKeyAxisUp(Keys.btnct[i]))
                {
                    if (Keys.dpad[i] && isAxisPushed[i])
                    {
                        btnPushState[i] = false;
                        isAxisPushed[i] = false;
                    }
                }
            }
        }

        public void ProcessSingleNote(int line, double time, int totalNotes,
            ref BMS bms, ref Note note, ref List<Note> removeCandidate,
            ref List<Longnote> lnlist)
        {
            // 일단 소리는 내고 시작
            soundController.PlayKeySound(note.getLane(), note.getWav(), ref bms);
            
            // 해당 노트가 롱노트의 시작노트라면
            if(note.isLong() && note.isLnStart()) {
                Longnote n = lnlist[note.getLnNum()];
                lnInProcess[line] = n;
                lnInProcess[line].setProcTime(time);
                isLnWorking[line] = true;
                lnTiming[line] = time;
                note.setUsed(true);
                AfterTouchLongStart(time);
            }
            // 롱노트가 아닌 일반 단노트 처리
            else if(!note.isLong()) {
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
            //btnPushState[line] = false;
        }

        public void AfterTouchLongStart(double time)
        {
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
            //ShowBeam(i, false);
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
            /*if (onoff) {
                btnPushState[lane] = true;
            }
            else {
                btnPushState[lane] = false;
            }*/
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
            Const.SetResultExScore(exscore);
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
