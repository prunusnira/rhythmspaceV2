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
        public static double PERFECT = 10d / 60;
        public static double GREAT = 30d / 60;
        public static double GOOD = 80d / 60;
        public static double BAD = 100d / 60;
        public static double EPOOR = 120d / 60;

        public TextMesh DebugConsole;

        // Unity objects
        public GameObject lastLine; // 판정선
        public GameObject bgaVideo;
        public GameObject bgaImage;

        // user play info
        private int score = 0;
        private int combo = 0;
        private int maxcombo = 0;
        private int processedNotes = 0;
        private int perfect = 0;
        private int great = 0;
        private int good = 0;
        private int ok = 0;
        private int miss = 0;
        private int epoor = 0;
        private int cb = 0;
        private double sumTimeDiff = 0;
        private double avgTimeDiff = 0;
        private double sumRate = 0;
        private double avgRate = 0;
        private double stopStartTime = 0;

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

        // 라인별 버튼 눌림 상태 확인
        // Beam 불 들어올 때 사용 (오토플레이 포함)
        private bool[] btnPushState;

        // 라인별 버튼 눌렸을 때 처리 되었는지 확인
        // 단노트에서 버튼을 계속 누르고 있을 때 처리가 지속되는것을 방지
        // 버튼을 뗄 때 초기화됨
        // 오토플레이에서는 필요없음
        private bool[] btnProcState;

        // 라인 눌렀을 때 소리를 재생했는지 여부 확인
        private bool[] btnPushSound;
        private int syncControl = 0;

        // 롱노트 처리 변수
        private bool[] isLnWorking; // 라인별 처리중 상태 on/off
        private double[] lnTiming;

        // 소리 재생
        private ISoundController soundController;

        // 외부 클래스 연계
        private PlayUI ui;

        public void Init(int noteCount)
        {
            ui = GetComponent<PlayUI>();

            speed = Const.SpeedFixed;
            speedfl = Const.SpeedFluid;
            spdType = Const.SpdType;

            isAxisPushed = new bool[8] { false, false, false, false, false, false, false, false };
            btnPushState = new bool[8] { false, false, false, false, false, false, false, false };
            btnProcState = new bool[8] { false, false, false, false, false, false, false, false };
            btnPushSound = new bool[8] { false, false, false, false, false, false, false, false };
            isLnWorking = new bool[8];
            lnTiming = new double[8];
            syncControl = Const.Sync * 5;

            for (int i = 0; i < 8; i++)
            {
                isLnWorking[i] = false;
            }
            for (int i = 0; i < 8; i++)
            {
                lnTiming[i] = 0;
            }

            // 초기 HP 지정
            hpController = GetComponent<HPController>();
            judgeType = Const.JudgeType;
            hpController.SetHPJudgeType(judgeType, noteCount);

            // 사운드 컨트롤러 정의
            soundController = GetComponent<SoundControllerFMOD>();
        }

        private void CalculateNotePosition(
            NoteObject note, double deltaTime, double bps)
        {
            note.OnScrPos = note.OnScrPos - (deltaTime * bps / 10);
        }

        public int currentBar = 0;
        public int currentStop = 0;

        public void MoveMine(List<MineNote>[] noteMine, double deltaTime, double bps)
        {
            foreach (List<MineNote> notes in noteMine)
            {
                List<MineNote> removeCandidate = new List<MineNote>();

                foreach (MineNote current in notes)
                {
                    // 시간에 따라 실제 노트가 표시될 위치 계산
                    CalculateNotePosition(current, deltaTime, bps);

                    // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                    if (!current.OnScreen)// && current.OnScrPos * speed * Const.SPEEDMULTIPLIER < 3000)
                    {
                        ui.DisplayMineNote(current);
                    }

                    // 실제 오브젝트가 존재할 때 위치를 이동시킴
                    if (current.OnScreen && current.NoteObject != null)
                    {
                        GameObject noteobj = current.NoteObject;
                        noteobj.transform.localPosition = new Vector3(
                            noteobj.transform.localPosition.x,
                            noteobj.transform.localPosition.y,
                            (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER));
                    }

                    if(current.OnScrPos < 0)
                    {
                        Destroy(current.NoteObject);
                        removeCandidate.Add(current);
                        current.NoteObject = null;
                    }
                }

                // 삭제
                foreach (MineNote n in removeCandidate)
                {
                    notes.Remove(n);
                }
            }
        }

        public void MoveNotes(List<PlayNote>[] notePlay, List<LongNote> lnlist,
            double timePassed, double deltaTime, ref double bps)
        {
            // 기본적으로 롱노트든 뭐든 모든 노트를 내림
            foreach (List<PlayNote> notes in notePlay)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                foreach (PlayNote current in notes)
                {
                    // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                    if (!current.OnScreen)// && current.OnScrPos * speed * Const.SPEEDMULTIPLIER < 3000)
                    {
                        ui.DisplayPlayNote(current, lnlist);
                    }

                    // 시간에 따라 실제 노트가 표시될 위치 계산
                    CalculateNotePosition(current, deltaTime, bps);

                    // 실제 오브젝트가 존재할 때 위치를 이동시킴
                    if (current.OnScreen && current.NoteObject != null)
                    {
                        GameObject noteobj = current.NoteObject;
                        noteobj.transform.localPosition = new Vector3(
                            noteobj.transform.localPosition.x,
                            noteobj.transform.localPosition.y,
                            (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER));
                    }

                    // 롱노트 가운데 노트의 위치와 넓이 설정
                    if (current.PlayNoteType == NoteType.LNSTART &&
                        current.OnScreen &&
                        !current.Used)
                    {
                        // isSpeedChanged는 모든 롱노트를 변경한 후에
                        // false로 바꾸어야하므로 for문 아래에 둠
                        LongNote ln = lnlist[current.LNNum];
                        double startRealPos = ln.Start.OnScrPos * speed * Const.SPEEDMULTIPLIER;
                        double endRealPos = ln.End.OnScrPos * speed * Const.SPEEDMULTIPLIER;

                        // 스케일 변경
                        Vector3 scale = ln.Mid.NoteObject.transform.localScale;
                        ln.Mid.NoteObject.transform.localScale = new Vector3(
                            scale.x, scale.y, (float)(endRealPos - startRealPos));

                        // 위치 변경
                        Vector3 pos = ln.Mid.NoteObject.transform.localPosition;
                        ln.Mid.NoteObject.transform.localPosition = new Vector3(
                            pos.x, pos.y, (float)((startRealPos + endRealPos) / 2));
                    }

                    double judgeTiming = GetJudgeTiming(current.Timing, timePassed);

                    // 노트가 타이밍윈도우 내에 진입하면 공푸어 계산을 위해 값을 추가
                    if (judgeTiming < BAD &&
                        !current.InTimingWindow &&
                        current.PlayNoteType == NoteType.SINGLE)
                    {
                        notesInTimingWindow++;
                        //Debug.Log("NotesInTimingWindow++: " + notesInTimingWindow);
                        current.InTimingWindow = true;
                    }

                    // 노트가 판정선 이하로 내려가면 사라짐
                    // 노트의 위치는 판정선과 노트 사이의 도달 시간으로 계산
                    /*
                     * 노트 위치에 대해 판정선까지의 시간계산하기
                     * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                     */
                    if (judgeTiming < EPOOR * -1 &&
                        current.ObjType == ObjectType.PLAYABLE &&
                        current.OnScreen)
                    {
                        if (current.PlayNoteType == NoteType.SINGLE &&
                            !current.Used)
                        {
                            // 데이터 처리
                            miss++;
                            cb++;
                            processedNotes++;
                            hpController.hpChangeMiss();
                            UpdateTiming(judgeTiming, true);
                            current.Used = true;
                            ui.UpdateHP(hpController.CurrentHP);

                            // 공푸어 위치 전에 BAD 위치를 지나면
                            // 타이밍 윈도우에서 없애야 함
                            if(judgeTiming < BAD * -1)
                            {
                                notesInTimingWindow--;
                                //Debug.Log("NotesInTimingWindow--: " + notesInTimingWindow);
                            }

                            // 노트 삭제
                            //Destroy(current.Noteobj);
                            removeCandidate.Add(current);
                        }
                        else if (current.PlayNoteType == NoteType.LNSTART &&
                            !current.Used &&
                            !lnlist[current.LNNum].Processing)
                        {
                            // 시작 위치에서는 롱노트 전체 형태를 유지해야 하므로 없애지 않음
                            miss++;
                            cb++;
                            hpController.hpChangeMiss();
                            UpdateTiming(judgeTiming, true);
                            lnlist[current.LNNum].Processing = true;
                        }
                        else if (current.PlayNoteType == NoteType.LNMID &&
                            !current.Used)
                        {
                            current.Used = true;
                        }
                        else if (current.PlayNoteType == NoteType.LNEND &&
                            !current.Used)
                        {
                            // 롱노트 관련 데이터 처리
                            int lnNum = current.LNNum;
                            isLnWorking[current.Line] = false;
                            lnlist[lnNum].Start.Used = true;
                            lnlist[lnNum].Used = true;
                            current.Used = true;
                            processedNotes++;
                            lnlist[current.LNNum].Processing = false;

                            // 롱노트 구성요소를 모두 삭제처리
                            //Destroy(lnlist[lnNum].StartNote.Noteobj);
                            //Destroy(lnlist[lnNum].MidNote.Noteobj);
                            //Destroy(current.Noteobj);
                            removeCandidate.Add(current);
                            removeCandidate.Add(lnlist[lnNum].Start);
                            removeCandidate.Add(lnlist[lnNum].Mid);
                        }
                    }
                }
                
                // 삭제
                foreach (PlayNote n in removeCandidate)
                {
                    notes.Remove(n);
                }
            }
        }

        public void PlayBGA(List<BGANote> noteBGA, BMS bms, double time)
        {
            List<BGANote> removeCandidate = new List<BGANote>();

            for (int i = 0; i < noteBGA.Count; i++)
            {
                BGANote current = noteBGA[i];

                if (current.Timing <= time && !current.Used)
                {
                    // 비디오 파일이 아닐 경우
                    if(current.VideoFile == null)
                    {
                        if (!BGAOnChecked) bgaImage.SetActive(true);
                        ui.BGAImageSetting(current.BGASprite);
                        BGAOnChecked = true;
                    }
                    else
                    {
                        if (!BGAOnChecked) bgaVideo.SetActive(true);
                        ui.BGAVideoPlay();
                        BGAOnChecked = true;
                    }
                    current.Used = true;
                    removeCandidate.Add(current);
                }
            }

            foreach (BGANote n in removeCandidate)
            {
                noteBGA.Remove(n);
            }
        }

        public void PlayBGM(List<BGMNote> noteBGM, BMS bms, double time)
        {
            List<BGMNote> removeCandidate = new List<BGMNote>();

            for (int i = 0; i < noteBGM.Count; i++)
            {
                BGMNote current = noteBGM[i];

                if (current.Timing <= time && !current.Used)
                {
                    soundController.PlayKeySound(current.Wav, bms, 0);
                    current.Used = true;
                    removeCandidate.Add(current);
                }
            }

            foreach (BGMNote n in removeCandidate)
            {
                noteBGM.Remove(n);
            }
        }

        public void PlayBPM(PlayData Data, double time, ref double bpm, ref double bps)
        {
            List<BPMNote> removeCandidate = new List<BPMNote>();

            for (int i = 0; i < Data.NoteBPM.Count; i++)
            {
                BPMNote current = Data.NoteBPM[i];

                if (current.Timing <= time && !current.Used)
                {
                    bpm = current.BPMValue;
                    bps = bpm / 240;
                    current.Used = true;
                    removeCandidate.Add(current);
                    ui.SetGearCurBPM(bpm);
                    Data.IsBPMChanged = true;
                    Data.BPMNum++;
                }
            }

            foreach (BPMNote n in removeCandidate)
            {
                Data.NoteBPM.Remove(n);
            }
        }

        /**
         * Stop 시간 계산하기
         * https://hitkey.nekokan.dyndns.info/cmdsJP.htm#STOPXX
         * 
         * STOPVALUE / 192 / BPS (단위 second)
         * ex) BPM 60 / STOP 48 -> 48 / 192 / 60 * 240 = 1초
         * ex) BPM 120 / STOP 48 -> 48 / 192 / 120 * 240 = 0.5초
         * ex) BPM 240 / STOP 48 -> 48 / 192 / 240 * 240 = 0.25초
         */
        public void PlayStop(PlayData Data, double time, double bps)
        {
            List<StopNote> removeCandidate = new List<StopNote>();

            for (int i = 0; i < Data.NoteStop.Count; i++)
            {
                StopNote current = Data.NoteStop[i];

                if (current.Timing <= time && !current.Used)
                {
                    currentStop++;
                    currentBar = current.Bar;
                    // 지정된 시간동안 노트 움직임을 멈춤
                    double stop = current.StopDuration;
                    Debug.Log("StopDuration:" + current.Bar + " " + (stop / bps * 10));
                    Data.Stop += stop / bps * 10;
                    current.Used = true;
                    removeCandidate.Add(current);
                }
            }

            foreach (StopNote n in removeCandidate)
            {
                Data.NoteStop.Remove(n);
            }
        }

        public void AutoPlay(List<PlayNote>[] notePlay, List<LongNote> lnlist,
            BMS bms, double timePassed, ref double bps, int totalNotes)
        {
            double linepos = lastLine.transform.localPosition.z;
            double additionalPos = PERFECT;

            // 각 라인에 대해 검사
            for (int i = 0; i < Const.LINE; i++)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                // 1. 현재 라인을 가져옴
                List<PlayNote> currentLine = notePlay[i];

                // 2. 현재 라인의 첫 노트를 가져옴
                if (currentLine.Count == 0) continue;

                PlayNote cnote = currentLine[0];

                if (cnote.PlayNoteType == NoteType.LNSTART &&
                    lnlist[cnote.LNNum].Processing)
                {
                    cnote = currentLine[1];
                }

                // 3. 해당 노트의 위치가 판정선 아래인지 확인
                if (cnote.Timing <= timePassed/* + additionalPos*/ && !cnote.Used)
                {
                    // 눌림 상태 처리
                    btnPushState[i] = true;

                    // 소리 내기
                    if (!btnPushSound[i])
                    {
                        soundController.PlayKeySound(cnote.Wav, bms, cnote.Line);
                        btnPushSound[i] = true;
                    }

                    double time = GetJudgeTiming(cnote.Timing, timePassed);

                    if (cnote.PlayNoteType == NoteType.SINGLE)
                    {
                        ProcessSingleNote(i, time, totalNotes, bms, cnote, removeCandidate, lnlist);
                        StartCoroutine(AutoTurnoffBeam(i));
                        // btnPushState false를 여기서 수행함

                        // 노트 이펙트 켜기
                        ui.TurnOnNoteEffect(i);
                        btnPushSound[i] = false;
                    }
                    // 롱노트 시작 처리
                    else if (cnote.PlayNoteType == NoteType.LNSTART)
                    {
                        ProcessLNStartNote(cnote, lnlist, time, i);

                        // 노트 이펙트 켜기
                        ui.TurnOnNoteEffectLN(i, true);

                        // InProgress 값은 중간에 미스가 나도 true로 유지하고
                        // 마지막 노트가 지나야 false로 변경한다
                        // movenote에서 계속 miss 판정이 나는 것을 방지
                        lnlist[cnote.LNNum].Processing = true;
                    }
                    // 롱노트의 끝노트일 때 처리
                    else if (cnote.PlayNoteType == NoteType.LNEND)
                    {
                        double endtiming = lnTiming[cnote.Line];
                        AfterTouchLongEnd(endtiming, totalNotes);

                        // 노트 이펙트 켜기
                        ui.TurnOnNoteEffectLN(i, false);

                        btnPushState[i] = false;
                        isLnWorking[i] = false;
                        btnPushSound[i] = false;

                        removeCandidate.Add(lnlist[cnote.LNNum].Start);
                        removeCandidate.Add(lnlist[cnote.LNNum].Mid);
                        removeCandidate.Add(cnote);
                    }
                }

                foreach (PlayNote n in removeCandidate)
                {
                    Destroy(n.NoteObject);
                    if(notePlay[i].Contains(n)) notePlay[i].Remove(n);
                    n.Used = true;
                }
            }
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

        public void Play(List<PlayNote>[] notePlay, List<LongNote> lnlist, BMS bms,
            double timePassed,
            ref double bpm, ref double bps, int totalNotes)
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
                List<PlayNote> removeCandidate = new List<PlayNote>();

                // 현재 라인이 눌려진 상태이면
                // (롱노트가 아닌 이상 눌러지자 마자 한 번만 체크해야 함)
                if (btnPushState[i])
                {
                    // 롱노트가 처리중인 상태가 아닐 때
                    if (!isLnWorking[i] && !btnProcState[i])
                    {
                        // 버튼 눌림 및 처리 상태 설정
                        btnProcState[i] = true;

                        // 1. 현재 라인을 가져옴
                        List<PlayNote> currentLine = notePlay[i];

                        // 2. 현재 라인의 첫 노트를 가져옴
                        if (currentLine.Count == 0) continue;

                        PlayNote cnote = currentLine[0];

                        // 3. 노트 존재 유무와 상관없이 소리를 냄
                        if (!btnPushSound[i])
                        {
                            soundController.PlayKeySound(cnote.Wav, bms, cnote.Line);
                            btnPushSound[i] = true;
                        }

                        // 4. 판정 처리하기
                        // 여기부터는 타이밍으로 간 봐야 할것
                        double time = GetJudgeTiming(cnote.Timing, timePassed);

                        // Timing Window 내에 노트가 있으면
                        if (time < EPOOR && time >= BAD)
                        {
                            // 빠른 공푸어 처리
                            // 이 경우에는 콤보 초기화가 있음
                            combo = 0;
                            epoor++;
                            cb++;
                        }
                        else if (time < BAD && time >= BAD * -1)
                        {
                            // 판정 처리 수행
                            if (cnote.PlayNoteType == NoteType.LNSTART)
                            {
                                // 롱노트는 애초에 공푸어에 관여하지 않음
                                TimingType timing = GetTimingType(time, false);
                                if(timing != TimingType.BAD && timing != TimingType.POOR)
                                {
                                    ProcessLNStartNote(cnote, lnlist, time, i);

                                    // 노트 이펙트 켜기
                                    ui.TurnOnNoteEffectLN(i, true);
                                }
                                else
                                {
                                    // 틀린 처리하고 넘어감
                                    lnlist[cnote.LNNum].Used = true;
                                }
                                
                                lnlist[cnote.LNNum].Processing = true;
                            }
                            else if (cnote.PlayNoteType == NoteType.SINGLE)
                            {
                                ProcessSingleNote(i, time, totalNotes, bms, cnote, removeCandidate, lnlist);
                                notesInTimingWindow--;
                                //Debug.Log("NotesInTimingWindow--: " + notesInTimingWindow);

                                // 노트 이펙트 켜기
                                ui.TurnOnNoteEffect(i);
                            }
                        }
                        else if(time < BAD * -1 && time >= EPOOR * -1)
                        {
                            // 느린 공푸어 처리
                            // 콤보 초기화 없음
                        }
                        // Timing Window 내에 노트가 없는데
                        else
                        {
                            // 다른 라인에 Timing Window 내에 노트가 있으면
                            if (notesInTimingWindow > 0)
                            {
                                // 공푸어 처리 (Type1 - 간접미스)
                                ProcessEmptyPoor();
                            }
                        }
                    }
                }
                // 롱노트인데 버튼을 떼었을 때
                else
                {
                    // 1. 현재 라인을 가져옴
                    List<PlayNote> currentLine = notePlay[i];

                    // 2. 현재 라인의 첫 노트를 가져옴
                    if (currentLine.Count == 0) continue;

                    PlayNote cnote = currentLine[0];

                    if (cnote.PlayNoteType == NoteType.LNSTART &&
                        lnlist[cnote.LNNum].Processing)
                    {
                        cnote = currentLine[1];
                    }

                    if (isLnWorking[i])
                    {
                        double time = GetJudgeTiming(cnote.Timing, timePassed);

                        if (time < BAD)
                        {
                            // Timing Window 내에 롱노트 끝이 있으면 끝 판정 처리
                            // 롱노트 시작할 때 이미 틀린 판정처리 났으면 아무런 처리를 하지 않음
                            if (cnote.PlayNoteType == NoteType.LNEND &&
                                !lnlist[cnote.LNNum].Used)
                            {
                                int lnNum = cnote.LNNum;
                                AfterTouchLongEnd(time, totalNotes);

                                // 노트 이펙트 켜기
                                ui.TurnOnNoteEffectLN(i, false);

                                cnote.Used = true;
                                isLnWorking[i] = false;
                                lnlist[lnNum].Processing = false;
                                
                                //Destroy(lnlist[lnNum].Start.Noteobj);
                                //Destroy(lnlist[lnNum].Mid.Noteobj);
                                //Destroy(cnote.Noteobj);
                                removeCandidate.Add(lnlist[lnNum].Start);
                                removeCandidate.Add(lnlist[lnNum].Mid);
                                removeCandidate.Add(cnote);
                            }
                            // 그 보다 위에 있으면 미스처리
                            else
                            {
                                // 노트 이펙트 켜기
                                ui.TurnOnNoteEffectLN(i, false);

                            }
                        }
                    }
                }

                foreach (PlayNote n in removeCandidate)
                {
                    //Destroy(n.Noteobj);
                    notePlay[i].Remove(n);
                }
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
                    btnPushSound[i] = false;
                    btnProcState[i] = false;
                }
                if (Keys.GetKeyAxisUp(Keys.btnct[i]))
                {
                    if (Keys.dpad[i] && isAxisPushed[i])
                    {
                        btnPushState[i] = false;
                        btnPushSound[i] = false;
                        isAxisPushed[i] = false;
                        btnProcState[i] = false;
                    }
                }
            }
        }

        public void ProcessLNStartNote(PlayNote note, List<LongNote> lnlist,
            double time, int line)
        {
            LongNote n = lnlist[note.LNNum];
            isLnWorking[line] = true;
            lnTiming[line] = time;
        }

        public void ProcessSingleNote(int line, double time, int totalNotes,
            BMS bms, PlayNote note, List<PlayNote> removeCandidate,
            List<LongNote> lnlist)
        {
            TimingType timingType = GetTimingType(time, false);
            switch (timingType)
            {
                case TimingType.PERFECT:
                    perfect++;
                    hpController.hpChangePerfect();
                    score += 2;
                    break;
                case TimingType.GREAT:
                    great++;
                    hpController.hpChangeGreat();
                    score += 1;
                    break;
                case TimingType.GOOD:
                    good++;
                    hpController.hpChangeGood();
                    break;
                case TimingType.BAD:
                    ok++;
                    cb++;
                    hpController.hpChangeBad();
                    break;
            }

            ui.UpdateHP(hpController.CurrentHP);
            processedNotes++;
            UpdateTiming(time, true);
            UpdateScore(totalNotes);
            removeCandidate.Add(note);
        }

        // 공푸어 처리
        private void ProcessEmptyPoor()
        {
            // cb나 콤보 초기화 없이 miss 개수만 늘림
            miss++;
            hpController.hpChangeEPoor();
            ui.UpdateHP(hpController.CurrentHP);
            ui.UpdateJudge(TimingType.EPOOR, combo, "0.00%", 0);
        }

        // 롱노트의 끝 노트 타이밍을 처리하는 메소드
        public void AfterTouchLongEnd(double time, int totalNotes)
        {
            switch(GetTimingType(time, true))
            {
                case TimingType.PERFECT:
                    perfect++;
                    hpController.hpChangePerfect();
                    score += 2;
                    break;
                case TimingType.GREAT:
                    great++;
                    hpController.hpChangeGreat();
                    score += 1;
                    break;
                case TimingType.GOOD:
                    good++;
                    hpController.hpChangeGood();
                    break;
                case TimingType.BAD:
                    ok++;
                    cb++;
                    hpController.hpChangeBad();
                    break;
                case TimingType.POOR:
                    miss++;
                    cb++;
                    break;
            }

            UpdateTiming(time, true);
            UpdateScore(totalNotes);
            ui.UpdateHP(hpController.CurrentHP);
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
                sumRate += (1 - abstime / BAD) * 100;
                combo = 0;
                ui.UpdateJudge(timingType, combo, "0.00%", 0);
            }
            else if(rateadd)
            {
                sumRate += (1 - abstime / BAD) * 100;

                combo += 1;
                sumTimeDiff += time;

                if (combo > maxcombo)
                {
                    maxcombo = combo;
                    ui.UpdateMaxCombo(maxcombo);
                }

                ui.UpdateJudge(timingType, combo, ((1 - abstime / BAD) * 100).ToString("0.00") + "%", (int)Math.Round(time));
            }

            avgRate = Math.Round(sumRate * 100 / processedNotes) / 100;
            avgTimeDiff = sumTimeDiff / processedNotes;
            ui.UpdateSideJudge(perfect, great, good, ok, miss, cb,
                avgRate.ToString("0.00") + "%", avgTimeDiff.ToString("0.0") + "ms");
        }

        IEnumerator AutoTurnoffBeam(int i)
        {
            yield return new WaitForSeconds(0.05f);
            btnPushState[i] = false;
        }

        public double GetJudgeTiming(double noteTime, double playTime)
        {
            return noteTime - playTime;
        }

        // Timing value로 현재 판정을 알아내는 메소드
        private TimingType GetTimingType(double otime, bool isLongEndCheck)
        {
            double time = Math.Abs(otime);
            if (time <= PERFECT) return TimingType.PERFECT;
            else if (time > PERFECT && time <= GREAT) return TimingType.GREAT;
            else if (time > GREAT && time <= GOOD) return TimingType.GOOD;
            else if (time > GOOD && time <= BAD) return TimingType.BAD;
            else if (time > BAD) return TimingType.POOR;
            else return TimingType.NONE;
        }

        private void UpdateScore(int totalNotes)
        {
            ui.UpdateExScore(score);
            ui.UpdateGraph(score, processedNotes, totalNotes);
        }

        private void ShowBeam(int lane, bool onoff)
        {
            ui.ShowAndHideBeam(lane, onoff);
        }

        public void GetResultData(int totalNotes)
        {
            Const.ResultPerfect = perfect;
            Const.ResultGreat = great;
            Const.ResultGood = good;
            Const.ResultOk = ok;
            Const.ResultMiss = miss;
            Const.ResultComboBreak = cb;
            Const.ResultAvgRate = (float)avgRate;
            Const.ResultTimeDiff = (float)avgTimeDiff;
            Const.ResultScore = score;
            Const.ResultMaxCombo = maxcombo;
            Const.ResultRank = ui.GetRank(score, processedNotes);
        }

        private void SpeedUpFixed(double bpm)
        {
            if (speed < 1000)
            {
                speed += 25;
                Const.SpeedFixed = speed;
                Const.SpeedFluid = (int)(speed * bpm / 100);
                ui.UpdateSpeed();
            }
        }

        private void SpeedDownFixed(double bpm)
        {
            if (speed > 50)
            {
                speed -= 25;
                Const.SpeedFixed = speed;
                Const.SpeedFluid = (int)(speed * bpm / 100);
                ui.UpdateSpeed();
            }
        }

        private void SpeedUpFluid(double bpm)
        {
            if (speedfl < 1000)
            {
                speedfl += 1;
                speed = (int)(speedfl / bpm * 100);
                Const.SpeedFluid = speedfl;
                Const.SpeedFixed = (int)(speedfl / bpm * 100);
                ui.UpdateSpeed();
            }
        }

        private void SpeedDownFluid(double bpm)
        {
            if (speedfl > 50)
            {
                speedfl -= 1;
                speed = (int)(speedfl / bpm * 100);
                Const.SpeedFluid = speedfl;
                Const.SpeedFixed = (int)(speedfl / bpm * 100);
                ui.UpdateSpeed();
            }
        }

        public int GetProcessedNotes()
        {
            return processedNotes;
        }
    }
}
