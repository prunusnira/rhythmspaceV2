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
        public static double PERFECT;
        public static double GREAT;
        public static double GOOD;
        public static double BAD;
        public static double POOR;

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
        private int fast = 0;
        private int slow = 0;
        private double sumTimeDiff = 0;
        private double avgTimeDiff = 0;
        private double sumRate = 0;
        private double avgRate = 0;
        private double addStopTiming = 0;

        // 공푸어 처리용
        private int notesInTimingWindow = 0; // 개수가 0보다 크면 노트가 있음

        private int speed = 200;
        private int speedfl = 200;
        private SpdType spdType;

        // 체력 관리
        private GaugeType gaugeType;
        private HPController hpController;

        // 눌림 체크
        private bool[] isSet1Pushed;
        private bool[] isSet2Pushed;

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

        // 롱노트 처리 변수
        private bool[] isLnWorking; // 라인별 처리중 상태 on/off
        private double[] lnTiming;

        // 소리 재생
        private ISoundController soundController;

        // 외부 클래스 연계
        private PlayUI ui;
        private Graph Graph;

        public void Init(int noteCount, int rank)
        {
            // 판정 타입에 따른 판정 값 변경
            switch(Const.JudgeType)
            {
                case JudgeType.ARCADE:
                    PERFECT = Const.JudgeArcade[0];
                    GREAT = Const.JudgeArcade[1];
                    GOOD = Const.JudgeArcade[2];
                    BAD = Const.JudgeArcade[3];
                    POOR = Const.JudgeArcade[4];
                    break;
                case JudgeType.ORIGINAL:
                    switch(rank)
                    {
                        case 0:
                            PERFECT = Const.JudgeOriginVeryHard[0];
                            GREAT = Const.JudgeOriginVeryHard[1];
                            GOOD = Const.JudgeOriginVeryHard[2];
                            BAD = Const.JudgeOriginVeryHard[3];
                            POOR = Const.JudgeOriginVeryHard[4];
                            break;
                        case 1:
                            PERFECT = Const.JudgeOriginHard[0];
                            GREAT = Const.JudgeOriginHard[1];
                            GOOD = Const.JudgeOriginHard[2];
                            BAD = Const.JudgeOriginHard[3];
                            POOR = Const.JudgeOriginHard[4];
                            break;
                        case 3:
                            PERFECT = Const.JudgeOriginEasy[0];
                            GREAT = Const.JudgeOriginEasy[1];
                            GOOD = Const.JudgeOriginEasy[2];
                            BAD = Const.JudgeOriginEasy[3];
                            POOR = Const.JudgeOriginEasy[4];
                            break;
                        case 2:
                        default:
                            PERFECT = Const.JudgeOriginNormal[0];
                            GREAT = Const.JudgeOriginNormal[1];
                            GOOD = Const.JudgeOriginNormal[2];
                            BAD = Const.JudgeOriginNormal[3];
                            POOR = Const.JudgeOriginNormal[4];
                            break;
                    }
                    break;
                case JudgeType.LR2:
                    switch (rank)
                    {
                        case 0:
                            PERFECT = Const.JudgeLR2VeryHard[0];
                            GREAT = Const.JudgeLR2VeryHard[1];
                            GOOD = Const.JudgeLR2VeryHard[2];
                            BAD = Const.JudgeLR2VeryHard[3];
                            POOR = Const.JudgeLR2VeryHard[4];
                            break;
                        case 1:
                            PERFECT = Const.JudgeLR2Hard[0];
                            GREAT = Const.JudgeLR2Hard[1];
                            GOOD = Const.JudgeLR2Hard[2];
                            BAD = Const.JudgeLR2Hard[3];
                            POOR = Const.JudgeLR2Hard[4];
                            break;
                        case 3:
                            PERFECT = Const.JudgeLR2Easy[0];
                            GREAT = Const.JudgeLR2Easy[1];
                            GOOD = Const.JudgeLR2Easy[2];
                            BAD = Const.JudgeLR2Easy[3];
                            POOR = Const.JudgeLR2Easy[4];
                            break;
                        case 2:
                        default:
                            PERFECT = Const.JudgeLR2Normal[0];
                            GREAT = Const.JudgeLR2Normal[1];
                            GOOD = Const.JudgeLR2Normal[2];
                            BAD = Const.JudgeLR2Normal[3];
                            POOR = Const.JudgeLR2Normal[4];
                            break;
                    }
                    break;
                case JudgeType.BEATORAJA:
                    switch (rank)
                    {
                        case 0:
                            PERFECT = Const.JudgeBRVeryHard[0];
                            GREAT = Const.JudgeBRVeryHard[1];
                            GOOD = Const.JudgeBRVeryHard[2];
                            BAD = Const.JudgeBRVeryHard[3];
                            POOR = Const.JudgeBRVeryHard[4];
                            break;
                        case 1:
                            PERFECT = Const.JudgeBRHard[0];
                            GREAT = Const.JudgeBRHard[1];
                            GOOD = Const.JudgeBRHard[2];
                            BAD = Const.JudgeBRHard[3];
                            POOR = Const.JudgeBRHard[4];
                            break;
                        case 3:
                            PERFECT = Const.JudgeBREasy[0];
                            GREAT = Const.JudgeBREasy[1];
                            GOOD = Const.JudgeBREasy[2];
                            BAD = Const.JudgeBREasy[3];
                            POOR = Const.JudgeBREasy[4];
                            break;
                        case 2:
                        default:
                            PERFECT = Const.JudgeBRNormal[0];
                            GREAT = Const.JudgeBRNormal[1];
                            GOOD = Const.JudgeBRNormal[2];
                            BAD = Const.JudgeBRNormal[3];
                            POOR = Const.JudgeBRNormal[4];
                            break;
                    }
                    break;
            }

            ui = GetComponent<PlayUI>();
            Graph = GetComponent<Graph>();

            speed = Const.SpeedFixed;
            speedfl = Const.SpeedFluid;
            spdType = Const.SpdType;

            isSet1Pushed = new bool[9] { false, false, false, false, false, false, false, false, false };
            isSet2Pushed = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnPushState = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnProcState = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnPushSound = new bool[9] { false, false, false, false, false, false, false, false, false };
            isLnWorking = new bool[8];
            lnTiming = new double[8];

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
            gaugeType = Const.GaugeType;
            hpController.SetHPJudgeType(gaugeType, noteCount);

            // 사운드 컨트롤러 정의
            soundController = GetComponent<SoundControllerFMOD>();
        }

        private void SetNotePositionOnStop(
            NoteObject note, PlayData data, double timePassed, double bps)
        {
            note.OnScrPos = note.Position - data.CurrentStopPos;
        }

        private void SetNotePosition(
            NoteObject note, PlayData data, double timePassed, double bps)
        {
            // 현재 시간을 계산해야 하는 부분에 addStopTiming을 사용
            // addStopTiming은 BPM 변경 포인트간의 STOP 합을 정의한다
            if(note is PlayNote
                && (note as PlayNote).PlayNoteType == NoteType.LNSTART
                && note.Timing <= timePassed)
            {
                    note.OnScrPos = 0;
            }
            else if(data.BPMPositionFix.Count == 0)
            {
                note.OnScrPos =
                    note.Position - (timePassed - addStopTiming) * bps / 10;
            }
            else if (data.BPMNum == 0)
            {
                note.OnScrPos =
                    note.Position - (timePassed - addStopTiming) * bps / 10;
            }
            else
            {
                note.OnScrPos =
                    note.Position
                    - data.BPMPositionFix[data.BPMNum - 1]
                    - (timePassed - data.BPMTimingFix[data.BPMNum - 1] - addStopTiming) * bps / 10;
            }
        }

        public void MoveSplitLine(PlayData data,
            double playTime, double bps)
        {
            List<SplitLine> removeCandidate = new List<SplitLine>();

            foreach (SplitLine current in data.SplitLine)
            {
                // 화면에 표시되지 않은 상태라면 라인을 화면에 뿌림
                if (!current.OnScreen && current.OnScrPos * speed * Const.SPEEDMULTIPLIER < Const.NoteStartPos)
                {
                    ui.DisplaySplitLine(current);
                }

                // 시간에 따라 실제 라인이 표시될 위치 계산
                if (data.IsStopOn)
                {
                    SetNotePositionOnStop(current, data, playTime, bps);
                }
                else
                {
                    SetNotePosition(current, data, playTime, bps);
                }

                // 실제 오브젝트가 존재할 때 위치를 이동시킴
                if (current.OnScreen && current.NoteObject != null)
                {
                    GameObject noteobj = current.NoteObject;
                    noteobj.transform.localPosition = new Vector3(
                        noteobj.transform.localPosition.x,
                        (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER),
                        noteobj.transform.localPosition.z);
                }

                if (current.Timing < playTime)
                {
                    Destroy(current.NoteObject);
                    current.NoteObject = null;
                    current.OnScreen = false;
                    removeCandidate.Add(current);
                }
            }

            // 삭제
            foreach (SplitLine n in removeCandidate)
            {
                data.SplitLine.Remove(n);
            }
        }

        public void MoveMine(PlayData data,
            double playTime, double bps)
        {
            foreach (List<MineNote> notes in data.NoteMine)
            {
                List<MineNote> removeCandidate = new List<MineNote>();

                foreach (MineNote current in notes)
                {
                    // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                    if (!current.OnScreen && current.OnScrPos * speed * Const.SPEEDMULTIPLIER < Const.NoteStartPos)
                    {
                        ui.DisplayMineNote(current);
                    }

                    // 시간에 따라 실제 노트가 표시될 위치 계산
                    if (data.IsStopOn)
                    {
                        SetNotePositionOnStop(current, data, playTime, bps);
                    }
                    else
                    {
                        SetNotePosition(current, data, playTime, bps);
                    }

                    // 실제 오브젝트가 존재할 때 위치를 이동시킴
                    if (current.OnScreen && current.NoteObject != null)
                    {
                        GameObject noteobj = current.NoteObject;
                        noteobj.transform.localPosition = new Vector3(
                            noteobj.transform.localPosition.x,
                            (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER),
                            noteobj.transform.localPosition.z);
                    }

                    if(current.Timing < playTime)
                    {
                        Destroy(current.NoteObject);
                        current.NoteObject = null;
                        current.OnScreen = false;
                        removeCandidate.Add(current);
                    }
                }

                // 삭제
                foreach (MineNote n in removeCandidate)
                {
                    notes.Remove(n);
                }
            }
        }

        public void MoveNotes(PlayData data,
            double timePassed, ref double bps)
        {
            // 기본적으로 롱노트든 뭐든 모든 노트를 내림
            foreach (List<PlayNote> notes in data.NotePlay)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                foreach (PlayNote current in notes)
                {
                    // 노트가 화면에 표시되지 않은 상태라면 노트를 화면에 뿌림
                    if (!current.OnScreen && current.OnScrPos * speed * Const.SPEEDMULTIPLIER < Const.NoteStartPos)
                    {
                        ui.DisplayPlayNote(current, data.NoteLong);
                    }

                    // 시간에 따라 실제 노트가 표시될 위치 계산
                    if(data.IsStopOn)
                    {
                        SetNotePositionOnStop(current, data, timePassed, bps);
                    }
                    else
                    {
                        SetNotePosition(current, data, timePassed, bps);
                    }

                    // 실제 오브젝트가 존재할 때 위치를 이동시킴
                    if (current.OnScreen && current.NoteObject != null)
                    {
                        GameObject noteobj = current.NoteObject;
                        noteobj.transform.localPosition = new Vector3(
                            noteobj.transform.localPosition.x,
                            (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER),
                            noteobj.transform.localPosition.z);
                    }

                    // 롱노트 가운데 노트의 위치와 넓이 설정
                    if (current.PlayNoteType == NoteType.LNSTART &&
                        current.OnScreen &&
                        !current.Used)
                    {
                        // isSpeedChanged는 모든 롱노트를 변경한 후에
                        // false로 바꾸어야하므로 for문 아래에 둠
                        LongNote ln = data.NoteLong[current.LNNum];
                        double startRealPos = ln.Start.OnScrPos * speed * Const.SPEEDMULTIPLIER;
                        double endRealPos = ln.End.OnScrPos * speed * Const.SPEEDMULTIPLIER;

                        // 스케일 변경
                        Vector3 scale = ln.Mid.NoteObject.transform.localScale;
                        ln.Mid.NoteObject.transform.localScale = new Vector3(
                            scale.x, scale.y, (float)(endRealPos - startRealPos));

                        // 위치 변경
                        Vector3 pos = ln.Mid.NoteObject.transform.localPosition;
                        ln.Mid.NoteObject.transform.localPosition = new Vector3(
                            pos.x, (float)((startRealPos + endRealPos) / 2), pos.z);
                    }

                    double judgeTiming = GetJudgeTiming(current.Timing, timePassed);

                    // 노트가 타이밍윈도우 내에 진입하면 공푸어 계산을 위해 값을 추가
                    if (judgeTiming < BAD &&
                        !current.InTimingWindow &&
                        current.PlayNoteType == NoteType.SINGLE)
                    {
                        notesInTimingWindow++;
                        current.InTimingWindow = true;
                    }

                    // 노트가 판정선 이하로 내려가면 사라짐
                    // 노트의 위치는 판정선과 노트 사이의 도달 시간으로 계산
                    /*
                     * 노트 위치에 대해 판정선까지의 시간계산하기
                     * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                     */
                    if (judgeTiming < POOR * -1 &&
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
                            }

                            // 노트 삭제
                            removeCandidate.Add(current);
                        }
                        else if (current.PlayNoteType == NoteType.LNSTART &&
                            !current.Used &&
                            !data.NoteLong[current.LNNum].Processing)
                        {
                            // 시작 위치에서는 롱노트 전체 형태를 유지해야 하므로 없애지 않음
                            miss++;
                            cb++;
                            hpController.hpChangeMiss();
                            UpdateTiming(judgeTiming, true);
                            data.NoteLong[current.LNNum].Processing = true;
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
                            data.NoteLong[lnNum].Start.Used = true;
                            data.NoteLong[lnNum].Used = true;
                            current.Used = true;
                            processedNotes++;
                            data.NoteLong[current.LNNum].Processing = false;

                            // 롱노트 구성요소를 모두 삭제처리
                            removeCandidate.Add(current);
                            removeCandidate.Add(data.NoteLong[lnNum].Start);
                            removeCandidate.Add(data.NoteLong[lnNum].Mid);
                        }
                    }
                }
                
                // 삭제
                foreach (PlayNote n in removeCandidate)
                {
                    Destroy(n.NoteObject);
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
                        ui.BGAImageSetting(current.BGASprite);
                    }
                    else
                    {
                        ui.BGAVideoPlay(current.VideoFile);
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
                    Data.BPMNum++;
                    addStopTiming = 0;
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
                    // 지정된 시간동안 노트 움직임을 멈춤
                    double stop = current.StopDuration;
                    Data.Stop += stop / bps * 10;
                    addStopTiming += stop / bps * 10;
                    Data.CurrentStopPos = current.Position;
                    Data.IsStopOn = true;
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
            double additionalTiming = PERFECT;

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
                if (cnote.Timing <= timePassed + (additionalTiming/2) && !cnote.Used)
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
            if (Input.GetKey(KeyCode.Alpha2))
            {
                if (spdType == SpdType.FIXED)
                {
                    SpeedUpFixed(bpm);
                }
                if (spdType == SpdType.FLUID)
                {
                    SpeedUpFluid(bpm);
                }
            }
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (spdType == SpdType.FIXED)
                {
                    SpeedDownFixed(bpm);
                }
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
                    if(i == 0 || i == 8) ShowBeam(0, true);
                    else ShowBeam(i, true);
                }
                else
                {
                    if(i == 0 || i == 8)
                    {
                        if (!btnPushState[0] && !btnPushState[8])
                            ShowBeam(0, false);
                    }
                    else ShowBeam(i, false);
                }
            }
        }

        public void Play(List<PlayNote>[] notePlay, List<LongNote> lnlist, BMS bms,
            double timePassed,
            ref double bpm, ref double bps, int totalNotes)
        {
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
            for (int i = 0; i < Const.LINE + 1; i++)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                // 현재 라인이 눌려진 상태이면
                // (롱노트가 아닌 이상 눌러지자 마자 한 번만 체크해야 함)
                if (btnPushState[i])
                {
                    bool lnWorkCheck = false;
                    if (i == 8) lnWorkCheck = isLnWorking[0];
                    else lnWorkCheck = isLnWorking[i];

                    // 롱노트가 처리중인 상태가 아닐 때
                    if (!lnWorkCheck && !btnProcState[i])
                    {
                        // 버튼 눌림 및 처리 상태 설정
                        btnProcState[i] = true;

                        // 1. 현재 라인을 가져옴
                        List<PlayNote> currentLine;
                        if(i == 8) currentLine = notePlay[0];
                        else currentLine = notePlay[i];

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
                        double time = GetJudgeTiming(cnote.Timing + Const.Sync * 0.01, timePassed);

                        // Timing Window 내에 노트가 있으면
                        if (time < POOR && time >= BAD)
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
                                    processedNotes++;
                                    lnlist[cnote.LNNum].Used = true;
                                }
                                
                                lnlist[cnote.LNNum].Processing = true;
                            }
                            else if (cnote.PlayNoteType == NoteType.SINGLE)
                            {
                                ProcessSingleNote(i, time, totalNotes, bms, cnote, removeCandidate, lnlist);
                                notesInTimingWindow--;

                                // 노트 이펙트 켜기
                                ui.TurnOnNoteEffect(i);
                            }
                        }
                        else if(time < BAD * -1 && time >= POOR * -1)
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
                    List<PlayNote> currentLine;
                    if (i == 8) currentLine = notePlay[0];
                    else currentLine = notePlay[i];

                    // 2. 현재 라인의 첫 노트를 가져옴
                    if (currentLine.Count == 0) continue;

                    PlayNote cnote = currentLine[0];

                    if (cnote.PlayNoteType == NoteType.LNSTART &&
                        lnlist[cnote.LNNum].Processing)
                    {
                        cnote = currentLine[1];
                    }

                    bool lnWorkCheck = false;
                    if (i == 8) lnWorkCheck = isLnWorking[0];
                    else lnWorkCheck = isLnWorking[i];

                    if (lnWorkCheck)
                    {
                        double time = GetJudgeTiming(cnote.Timing + Const.Sync * 0.01, timePassed);

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

                                if(i == 8) isLnWorking[0] = false;
                                else isLnWorking[i] = false;

                                lnlist[lnNum].Processing = false;
                                
                                removeCandidate.Add(lnlist[lnNum].Start);
                                removeCandidate.Add(lnlist[lnNum].Mid);
                                removeCandidate.Add(cnote);
                            }
                            // 그 보다 위에 있으면 미스처리
                            else
                            {
                                // 노트 이펙트 켜기
                                processedNotes++;
                                ui.TurnOnNoteEffectLN(i, false);

                            }
                        }
                    }
                }

                foreach (PlayNote n in removeCandidate)
                {
                    Destroy(n.NoteObject);
                    notePlay[i].Remove(n);
                }
            }
        }

        public void ButtonPushState()
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
            for (int i = 0; i < Keys.btnSet1.Length; i++)
            {
                if (Keys.btnAxisSet1[i])
                {
                    // Axis 값일때
                    if(Keys.GetAxisValue(Keys.btnSet1[i]) != 0)
                    {
                        btnPushState[i] = true;
                        isSet1Pushed[i] = true;
                    }
                    else
                    {
                        if(!isSet2Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                        }
                        isSet1Pushed[i] = false;
                    }
                }
                else
                {
                    // 일반 버튼일때
                    if (Keys.GetKeyDown(Keys.btnSet1[i])
                        && !isSet1Pushed[i]
                        && !isSet2Pushed[i])
                    {
                        btnPushState[i] = true;
                        isSet1Pushed[i] = true;
                    }

                    if (Keys.GetKeyUp(Keys.btnSet1[i])
                        && isSet1Pushed[i])
                    {
                        btnPushState[i] = false;
                        btnPushSound[i] = false;
                        btnProcState[i] = false;
                        isSet1Pushed[i] = false;
                    }
                }

                if (Keys.btnAxisSet2[i])
                {
                    // Axis 값일때
                    if (Keys.GetAxisValue(Keys.btnSet2[i]) != 0)
                    {
                        btnPushState[i] = true;
                        isSet2Pushed[i] = true;
                    }
                    else
                    {
                        if (!isSet1Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                        }
                        isSet2Pushed[i] = false;
                    }
                }
                else
                {
                    // 일반 버튼일때
                    if (Keys.GetKeyDown(Keys.btnSet2[i])
                        && !isSet1Pushed[i]
                        && !isSet2Pushed[i])
                    {
                        btnPushState[i] = true;
                        isSet2Pushed[i] = true;
                    }

                    if (Keys.GetKeyUp(Keys.btnSet2[i])
                        && isSet2Pushed[i])
                    {
                        btnPushState[i] = false;
                        btnPushSound[i] = false;
                        btnProcState[i] = false;
                        isSet2Pushed[i] = false;
                    }
                }
            }
        }

        public void ProcessLNStartNote(PlayNote note, List<LongNote> lnlist,
            double time, int line)
        {
            LongNote n = lnlist[note.LNNum];

            if(line == 8)
            {
                isLnWorking[0] = true;
                lnTiming[0] = time;
            }
            else
            {
                isLnWorking[line] = true;
                lnTiming[line] = time;
            }
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

            processedNotes++;
            UpdateTiming(time, true);
            UpdateScore(totalNotes);
            ui.UpdateHP(hpController.CurrentHP);
        }

        private void UpdateTiming(double time, bool rateadd)
        {
            double abstime = Math.Abs(time);
            TimingType timingType = GetTimingType(abstime, false);

            double timefs = 0;
            if (time > PERFECT)
            {
                timefs = time - PERFECT;
                if(time <= GOOD) fast++;
            }
            else if(time < -PERFECT)
            {
                timefs = time + PERFECT;
                if(time >= -GOOD) slow++;
            }

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

                ui.UpdateJudge(timingType, combo, ((1 - abstime / BAD) * 100).ToString("0.00") + "%", (int)Math.Round(timefs*100));
            }

            avgRate = Math.Round(sumRate * 100 / processedNotes) / 100;
            avgTimeDiff = sumTimeDiff / processedNotes;
            ui.UpdateSideJudge(perfect, great, good, ok, miss, cb, fast, slow,
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
            ui.UpdateScore(score);
            Graph.UpdateGraph(score, processedNotes, totalNotes);
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
            Const.ResultFast = fast;
            Const.ResultSlow = slow;
            Const.ResultAvgRate = (float)avgRate;
            Const.ResultTimeDiff = (float)avgTimeDiff;
            Const.ResultScore = score;
            Const.ResultMaxCombo = maxcombo;
            Const.ResultRank = ui.GetRank(score, processedNotes);
        }

        private void SpeedUpFixed(double bpm)
        {
            if (speed < 2000)
            {
                speed += 1;
                Const.SpeedFixed = speed;
                Const.SpeedFluid = (int)(speed * bpm / 100);
                ui.UpdateSpeed();
            }
        }

        private void SpeedDownFixed(double bpm)
        {
            if (speed > 50)
            {
                speed -= 1;
                Const.SpeedFixed = speed;
                Const.SpeedFluid = (int)(speed * bpm / 100);
                ui.UpdateSpeed();
            }
        }

        private void SpeedUpFluid(double bpm)
        {
            if (speedfl < 2000)
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
            if (speedfl > 100)
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
