﻿using System.Collections;
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
        private int noteCount = 0;
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

        private int speed = 200;
        private int speedfl = 200;
        private double BPM = 0;
        private bool isSpeedChanged = true;

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
        private int[] lnNumInProcess;

        // 외부 클래스 연계
        private PlayUI UI;
        private HPBarUI HPUI;
        private Graph Graph;
        private BGAControl BGAControl;
        private JudgeUIProcess JudgeUI;
        private BeamUI BeamUI;
        private NoteObjectAdder NoteAdder;

        public void Update()
        {
            // 배속 조절시 속도 변경 적용
            if(isSpeedChanged)
            {
                isSpeedChanged = false;
                if (Const.SpdType == SpdType.STANDARD)
                {
                    Const.SpeedCon = (int)(speed * BPM / 100);
                }
                else if(Const.SpdType == SpdType.CONSTANT)
                {
                    speed = (int)(speedfl / BPM * 100);
                    Const.SpeedStd = (int)(speedfl / BPM * 100);
                }
                UI.UpdateSpeed();
            }
        }

        public void Init()
        {
            UI = GetComponent<PlayUI>();
            HPUI = GetComponent<HPBarUI>();
            Graph = GetComponent<Graph>();
            BGAControl = GetComponent<BGAControl>();
            JudgeUI = GetComponent<JudgeUIProcess>();
            BeamUI = GetComponent<BeamUI>();
            NoteAdder = GetComponent<NoteObjectAdder>();

            speed = Const.SpeedStd;
            speedfl = Const.SpeedCon;

            isSet1Pushed = new bool[9] { false, false, false, false, false, false, false, false, false };
            isSet2Pushed = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnPushState = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnProcState = new bool[9] { false, false, false, false, false, false, false, false, false };
            btnPushSound = new bool[9] { false, false, false, false, false, false, false, false, false };
            isLnWorking = new bool[8] { false, false, false, false, false, false, false, false };
            lnTiming = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            lnNumInProcess = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };


            // 초기 HP 지정
            hpController = HPController.Instance;
            gaugeType = Const.GaugeType;
        }

        public void PlaySetup(int noteCount, int totalval, int rank)
        {
            this.noteCount = noteCount;
            hpController.SetHPJudgeType(Const.JudgeType, rank, gaugeType, totalval, noteCount);

            // 판정 타입에 따른 판정 값 변경
            switch (Const.JudgeType)
            {
                case JudgeType.ARCADE:
                    PERFECT = Const.JudgeArcade[0];
                    GREAT = Const.JudgeArcade[1];
                    GOOD = Const.JudgeArcade[2];
                    BAD = Const.JudgeArcade[3];
                    POOR = Const.JudgeArcade[4];
                    break;
                case JudgeType.ORIGINAL:
                    switch (rank)
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
                    NoteAdder.DisplaySplitLine(current);
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
                        (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER) - (Const.SplitLineSizeCalc / 2),
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
                        NoteAdder.DisplayMineNote(current);
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
                        NoteAdder.DisplayPlayNote(current, data.NoteLong);
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
                        if(current.OnScrPos > 0)
                        {
                            noteobj.transform.localPosition = new Vector3(
                                noteobj.transform.localPosition.x,
                                (float)(current.OnScrPos * speed * Const.SPEEDMULTIPLIER),
                                noteobj.transform.localPosition.z);
                        }
                        else
                        {
                            noteobj.transform.localPosition = new Vector3(
                                noteobj.transform.localPosition.x,
                                0,
                                noteobj.transform.localPosition.z);
                        }
                    }

                    // 롱노트 가운데 노트의 위치와 넓이 설정
                    if (current.PlayNoteType == NoteType.LNSTART &&
                        current.OnScreen)
                    {
                        // isSpeedChanged는 모든 롱노트를 변경한 후에
                        // false로 바꾸어야하므로 for문 아래에 둠
                        LongNote ln = data.NoteLong[current.LNNum];
                        double startRealPos = ln.Start.OnScrPos * speed * Const.SPEEDMULTIPLIER;
                        double endRealPos = ln.End.OnScrPos * speed * Const.SPEEDMULTIPLIER;

                        // 스케일 변경
                        Vector3 scale = ln.Mid.NoteObject.transform.localScale;
                        ln.Mid.NoteObject.transform.localScale = new Vector3(
                            scale.x, (float)(endRealPos - startRealPos) / Const.LnHeightCalc, scale.z);

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
                        current.InTimingWindow = true;
                    }

                    // 노트가 판정선 이하로 내려가면 사라짐
                    // 노트의 위치는 판정선과 노트 사이의 도달 시간으로 계산
                    /*
                     * 노트 위치에 대해 판정선까지의 시간계산하기
                     * 시간 = (노트의 처음위치 - 노트의 현재위치) / 시간당 비트(bps)
                     */
                    if (judgeTiming < BAD * -1 &&
                        current.ObjType == ObjectType.PLAYABLE &&
                        current.OnScreen)
                    {
                        if (current.PlayNoteType == NoteType.SINGLE &&
                            !current.Used)
                        {
                            // 데이터 처리
                            if(Const.Auto != AutoPlayType.TURNTABLE
                                || current.Line != 0)
                            {
                                miss++;
                                cb++;
                                hpController.hpChangeMiss();
                                UpdateTiming(judgeTiming, true);
                            }
                            processedNotes++;
                            current.Used = true;
                            HPUI.UpdateHP(hpController.CurrentHP);

                            // 노트 삭제
                            removeCandidate.Add(current);
                        }
                        else if (current.PlayNoteType == NoteType.LNSTART &&
                            !current.Used &&
                            !data.NoteLong[current.LNNum].Processing)
                        {
                            // 시작 위치에서는 롱노트 전체 형태를 유지해야 하므로 없애지 않음
                            int lnNum = current.LNNum;
                            miss++;
                            cb++;
                            hpController.hpChangeMiss();
                            UpdateTiming(judgeTiming, true);

                            isLnWorking[current.Line] = false;
                            data.NoteLong[lnNum].End.Used = true;
                            data.NoteLong[lnNum].Used = true;
                            current.Used = true;
                            processedNotes++;
                            data.NoteLong[lnNum].Processing = false;

                            // 롱노트 구성요소를 모두 삭제처리
                            removeCandidate.Add(current);
                            removeCandidate.Add(data.NoteLong[lnNum].End);
                            removeCandidate.Add(data.NoteLong[lnNum].Mid);
                        }
                        else if (current.PlayNoteType == NoteType.LNMID &&
                            !current.Used)
                        {
                            current.Used = true;
                        }
                        else if (current.PlayNoteType == NoteType.LNEND &&
                            !current.Used
                            && Const.LNProcType == LNProcessType.CN)
                        {
                            // LN일 때는 Play에서 처리되므로 이 아래로 내려올 일이 없음

                            // 롱노트 관련 데이터 처리
                            // Used일 때는 Play에서 처리하므로 여기서는 놓쳤을 때 처리를 수행
                            int lnNum = current.LNNum;
                            isLnWorking[current.Line] = false;
                            data.NoteLong[lnNum].Start.Used = true;
                            data.NoteLong[lnNum].Used = true;
                            current.Used = true;
                            processedNotes++;
                            data.NoteLong[current.LNNum].Processing = false;

                            // 데이터
                            miss++;
                            cb++;
                            hpController.hpChangeMiss();
                            UpdateTiming(judgeTiming, true);

                            // 롱노트 구성요소를 모두 삭제처리
                            removeCandidate.Add(current);
                            removeCandidate.Add(data.NoteLong[lnNum].Start);
                            removeCandidate.Add(data.NoteLong[lnNum].Mid);
                        }
                        UpdateScore(); // 그래프 갱신용
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
                        BGAControl.BGAImageSetting(current.BGASprite);
                    }
                    else if(BGAControl.IsVLCNeeded())
                    {
                        BGAControl.BGAVideoPlayVLC();
                    }
                    else
                    {
                        BGAControl.BGAVideoPlayNM();
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

        public void PlayLayer(List<LayerNote> noteLayer, BMS bms, double time)
        {
            List<LayerNote> removeCandidate = new List<LayerNote>();

            for (int i = 0; i < noteLayer.Count; i++)
            {
                LayerNote current = noteLayer[i];

                if (current.Timing <= time && !current.Used)
                {
                    // 비디오 파일이 아닐 경우
                    BGAControl.LayerImageSetting(current.BGASprite);
                    current.Used = true;
                    removeCandidate.Add(current);
                }
            }

            foreach (LayerNote n in removeCandidate)
            {
                noteLayer.Remove(n);
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
                    SoundControllerFMOD.Instance.PlayKeySound(current.Wav, bms, 0);
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
                    UpdateBPM(bpm);
                    current.Used = true;
                    removeCandidate.Add(current);
                    UI.SetGearCurBPM(bpm);
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
            BMS bms, double timePassed, ref double bps)
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

                    double time = GetJudgeTiming(cnote.Timing, timePassed);

                    if (cnote.PlayNoteType == NoteType.SINGLE)
                    {
                        // 소리 내기
                        if (!btnPushSound[i])
                        {
                            SoundControllerFMOD.Instance.PlayKeySound(cnote.Wav, bms, cnote.Line);
                            btnPushSound[i] = true;
                        }

                        ProcessSingleNote(time, bms, cnote, removeCandidate, lnlist);
                        StartCoroutine(AutoTurnoffBeam(i));
                        // btnPushState false를 여기서 수행함

                        // 노트 이펙트 켜기
                        UI.TurnOnNoteEffect(i);
                        btnPushSound[i] = false;
                    }
                    // 롱노트 시작 처리
                    else if (cnote.PlayNoteType == NoteType.LNSTART)
                    {
                        // 소리 내기
                        if (!btnPushSound[i])
                        {
                            SoundControllerFMOD.Instance.PlayKeySound(cnote.Wav, bms, cnote.Line);
                            btnPushSound[i] = true;
                        }

                        ProcessLNStartNote(cnote, lnlist, time, i);
                        cnote.Used = true;

                        // 노트 이펙트 켜기
                        UI.TurnOnNoteEffectLN(i, true);

                        // InProgress 값은 중간에 미스가 나도 true로 유지하고
                        // 마지막 노트가 지나야 false로 변경한다
                        // movenote에서 계속 miss 판정이 나는 것을 방지
                        lnlist[cnote.LNNum].Processing = true;
                    }
                    // 롱노트의 끝노트일 때 처리
                    else if (cnote.PlayNoteType == NoteType.LNEND)
                    {
                        double endtiming = lnTiming[cnote.Line];
                        AfterTouchLongEnd(endtiming);

                        // 노트 이펙트 켜기
                        UI.TurnOnNoteEffectLN(i, false);

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

        public void Beam()
        {
            // 버튼 푸시 상태에 따라 빔 표시 상태 변경
            for (int i = 0; i < btnPushState.Length; i++)
            {
                if(i == 0 || i == 8)
                {
                    if (btnPushState[0] || btnPushState[8])
                    {
                        ShowBeam(0, true);
                    }
                    else
                    {
                        ShowBeam(0, false);
                    }
                }
                else
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
        }

        public void Play(List<PlayNote>[] notePlay, List<LongNote> lnlist, BMS bms,
            double timePassed, ref double bpm, ref double bps)
        {
            /**
             * 노트 처리하기
             * 
             * Step 1.
             * 노트를 일단 누른다
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

            // 각 버튼에 대해 검사
            for (int i = 0; i < Const.LINE + 1; i++)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                if (Const.Auto == AutoPlayType.TURNTABLE && i == 0)
                {
                    List<PlayNote> list = notePlay[0];
                    if (list.Count == 0) continue;
                    PlayNote note = list[0];
                    if (note.Timing < timePassed)
                    {
                        if (note.PlayNoteType == NoteType.SINGLE)
                        {
                            if (!btnPushSound[0])
                            {
                                SoundControllerFMOD.Instance.PlayKeySound(note.Wav, bms, note.Line);
                                btnPushSound[0] = true;
                            }

                            removeCandidate.Add(note);
                            processedNotes++;
                        }
                        else if (note.PlayNoteType == NoteType.LNSTART)
                        {
                            if (!btnPushSound[0])
                            {
                                SoundControllerFMOD.Instance.PlayKeySound(note.Wav, bms, note.Line);
                                btnPushSound[0] = true;
                            }
                        }
                        else if (note.PlayNoteType == NoteType.LNEND)
                        {
                            btnPushSound[0] = false;
                        }
                    }
                }
                else
                {
                    int line;
                    List<PlayNote> currentLine;
                    if (i == 8)
                    {
                        currentLine = notePlay[0];
                        line = 0;
                    }
                    else
                    {
                        currentLine = notePlay[i];
                        line = i;
                    }

                    // 현재 라인의 노트가 모두 소진되었으면 넘김
                    if (currentLine.Count == 0) continue;

                    // 현재 라인이 눌려진 상태이면
                    // (롱노트가 아닌 이상 눌러지자 마자 한 번만 체크해야 함)
                    if (btnPushState[i])
                    {
                        if (!isLnWorking[line] && !btnProcState[i])
                        {
                            btnProcState[i] = true;

                            // 최상단 노트를 가져옴
                            PlayNote cnote = currentLine[0];

                            // 노트 존재 유무와 상관없이 소리 냄
                            if (!btnPushSound[i])
                            {
                                SoundControllerFMOD.Instance.PlayKeySound(cnote.Wav, bms, cnote.Line);
                                btnPushSound[i] = true;
                            }

                            // 노트 타이밍 계산
                            double time = GetJudgeTiming(cnote.Timing + Const.Sync * 0.01, timePassed);

                            // + 범위 & POOR~BAD 사이 (공푸어)
                            if(time <= POOR && time > BAD)
                            {
                                ProcessEmptyPoor();
                            }
                            // 내부 판정 처리
                            else if(time <= BAD && time >= BAD * -1)
                            {
                                // 단노트
                                if(cnote.PlayNoteType == NoteType.SINGLE)
                                {
                                    ProcessSingleNote(time, bms, cnote, removeCandidate, lnlist);
                                    UI.TurnOnNoteEffect(line);
                                }
                                // 롱노트
                                else if(cnote.PlayNoteType == NoteType.LNSTART)
                                {
                                    TimingType type = GetTimingType(time);

                                    if (type != TimingType.BAD)
                                    {
                                        ProcessLNStartNote(cnote, lnlist, time, line);
                                    }
                                    else
                                    {
                                        int lnNum = cnote.LNNum;
                                        cnote.Used = true;
                                        isLnWorking[line] = false;
                                        lnlist[lnNum].Processing = false;
                                        lnlist[lnNum].Used = true;

                                        removeCandidate.Add(lnlist[lnNum].Start);
                                        removeCandidate.Add(lnlist[lnNum].Mid);
                                        removeCandidate.Add(lnlist[lnNum].End);

                                        AfterTouchLongEnd(time);
                                    }
                                }
                            }
                            // 뒷부분 공푸어는 처리 없음
                        }
                        else if(isLnWorking[line] && btnProcState[i])
                        {
                            // 롱노트가 진행 중일 때
                            // 턴테이블의 경우 0과 8이 같이 눌려진 상태인지 확인

                            // 최상단 노트를 가져옴
                            // (0은 LNStart이므로 LNEnd를 확인함)
                            PlayNote cnote = currentLine[1];

                            // 롱노트 타입이 LN이고 노트가 LNEnd이면 처리를 수행
                            if (cnote.LNNum == lnNumInProcess[line] &&
                                cnote.PlayNoteType == NoteType.LNEND &&
                                Const.LNProcType == LNProcessType.LN)
                            {
                                // 판정 타이밍은 LNStart의 타이밍으로 처리함
                                double time = lnTiming[line];

                                if (cnote.Timing <= timePassed)
                                {
                                    // 롱놋 처리된 것으로 처리
                                    int lnNum = cnote.LNNum;
                                    AfterTouchLongEnd(time);

                                    cnote.Used = true;
                                    isLnWorking[line] = false;
                                    lnlist[lnNum].Processing = false;
                                    lnlist[lnNum].Used = true;

                                    removeCandidate.Add(lnlist[lnNum].Start);
                                    removeCandidate.Add(lnlist[lnNum].Mid);
                                    removeCandidate.Add(lnlist[lnNum].End);
                                }
                            }
                        }
                    }
                    // 버튼을 떼었을 때
                    else if (!btnPushState[i])
                    {
                        // 턴테이블 한정:
                        // 롱노트가 입력중인 상태이면 다른 버튼의 처리는 스킵
                        if (i == 8 && btnProcState[0]) continue;
                        if (i == 0 && btnProcState[8]) continue;

                        // 사실 일반 노트는 볼 일 없음...
                        PlayNote cnote = currentLine[0];

                        if (cnote.PlayNoteType == NoteType.LNSTART &&
                            lnlist[cnote.LNNum].Processing)
                        {
                            cnote = currentLine[1];
                        }

                        double time = GetJudgeTiming(cnote.Timing + Const.Sync * 0.01, timePassed);
                        
                        if (isLnWorking[line])
                        {
                            int lnNum = cnote.LNNum;

                            // Timing Window 내에 롱노트 끝이 있으면 끝 판정 처리 (끝 판정은 들어갈 때 판정과 동일하게 처리함
                            // 롱노트 시작할 때 이미 틀린 판정처리 났으면 아무런 처리를 하지 않음
                            if (lnNum == lnNumInProcess[line] &&
                                cnote.PlayNoteType == NoteType.LNEND)
                            {
                                if (time > POOR) time = POOR;
                                AfterTouchLongEnd(time);
                                
                                cnote.Used = true;
                                isLnWorking[line] = false;
                                lnlist[lnNum].Processing = false;

                                removeCandidate.Add(lnlist[lnNum].Start);
                                removeCandidate.Add(lnlist[lnNum].Mid);
                                removeCandidate.Add(lnlist[lnNum].End);

                                UI.TurnOnNoteEffectLN(line, false);
                            }
                        }
                    }
                }

                foreach (PlayNote n in removeCandidate)
                {
                    Destroy(n.NoteObject);
                    if(i == 8)
                    {
                        notePlay[0].Remove(n);
                    }
                    else
                    {
                        notePlay[i].Remove(n);
                    }
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
                    if((Keys.btnSet1[i].EndsWith("p") &&
                        Keys.GetAxisValue(Keys.btnSet1[i]) > 0.5) ||
                        (Keys.btnSet1[i].EndsWith("m") &&
                        Keys.GetAxisValue(Keys.btnSet1[i]) < -0.5))
                    {
                        if (!isSet1Pushed[i]
                            && !isSet2Pushed[i])
                        {
                            btnPushState[i] = true;
                            isSet1Pushed[i] = true;
                        }
                    }

                    if((Keys.btnSet1[i].EndsWith("p") &&
                        Keys.GetAxisValue(Keys.btnSet1[i]) <= 0.5) ||
                        (Keys.btnSet1[i].EndsWith("m") &&
                        Keys.GetAxisValue(Keys.btnSet1[i]) >= -0.5))
                    {
                        if (isSet1Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                            isSet1Pushed[i] = false;
                        }
                    }
                }
                else
                {
                    // 일반 버튼일때
                    if (Keys.GetKeyDown(Keys.btnSet1[i]))
                    {
                        if(!isSet1Pushed[i]
                            && !isSet2Pushed[i])
                        {
                            btnPushState[i] = true;
                            isSet1Pushed[i] = true;
                        }
                    }

                    if (Keys.GetKeyUp(Keys.btnSet1[i]))
                    {
                        if (isSet1Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                            isSet1Pushed[i] = false;
                        }
                    }
                }

                if (Keys.btnAxisSet2[i])
                {
                    // Axis 값일때
                    if ((Keys.btnSet2[i].EndsWith("p") &&
                        Keys.GetAxisValue(Keys.btnSet2[i]) > 0.5) ||
                        (Keys.btnSet2[i].EndsWith("m") &&
                        Keys.GetAxisValue(Keys.btnSet2[i]) < -0.5))
                    {
                        if (!isSet1Pushed[i]
                            && !isSet2Pushed[i])
                        {
                            btnPushState[i] = true;
                            isSet2Pushed[i] = true;
                        }
                    }
                    
                    if((Keys.btnSet2[i].EndsWith("p") &&
                        Keys.GetAxisValue(Keys.btnSet2[i]) <= 0.5) ||
                        (Keys.btnSet2[i].EndsWith("m") &&
                        Keys.GetAxisValue(Keys.btnSet2[i]) >= -0.5))
                    {
                        if (isSet2Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                            isSet2Pushed[i] = false;
                        }
                    }
                }
                else
                {
                    // 일반 버튼일때
                    if (Keys.GetKeyDown(Keys.btnSet2[i]))
                    {
                        if (!isSet1Pushed[i]
                            && !isSet2Pushed[i])
                        {
                            btnPushState[i] = true;
                            isSet2Pushed[i] = true;
                        }
                    }

                    if (Keys.GetKeyUp(Keys.btnSet2[i]))
                    {
                        if (isSet2Pushed[i])
                        {
                            btnPushState[i] = false;
                            btnPushSound[i] = false;
                            btnProcState[i] = false;
                            isSet2Pushed[i] = false;
                        }
                    }
                }
            }
        }

        public void ProcessSingleNote(double time, BMS bms, PlayNote note,
            List<PlayNote> removeCandidate, List<LongNote> lnlist)
        {
            TimingType timingType = GetTimingType(time);
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

            HPUI.UpdateHP(hpController.CurrentHP);
            note.Used = true;
            processedNotes++;
            UpdateTiming(time, true);
            UpdateScore();
            removeCandidate.Add(note);
        }

        public void ProcessLNStartNote(PlayNote note, List<LongNote> lnlist,
            double time, int line)
        {
            int lnNum = note.LNNum;

            LongNote n = lnlist[lnNum];

            isLnWorking[line] = true;

            lnTiming[line] = time;
            lnNumInProcess[line] = note.LNNum;

            note.Used = true;
            lnlist[lnNum].Processing = true;
            UI.TurnOnNoteEffectLN(line, true);
        }

        // 롱노트의 끝 노트 타이밍을 처리하는 메소드
        public void AfterTouchLongEnd(double time)
        {
            switch(GetTimingType(time))
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
                    combo = 0;
                    hpController.hpChangeBad();
                    break;
                case TimingType.POOR:
                    miss++;
                    cb++;
                    combo = 0;
                    break;
            }

            processedNotes++;
            UpdateTiming(time, true);
            UpdateScore();
            HPUI.UpdateHP(hpController.CurrentHP);
        }

        // 공푸어 처리
        private void ProcessEmptyPoor()
        {
            // cb나 콤보 초기화 없이 miss 개수만 늘림
            epoor++;
            hpController.hpChangeEPoor();
            HPUI.UpdateHP(hpController.CurrentHP);
            JudgeUI.UpdateJudge(TimingType.EPOOR, combo, "0.00%", 0, score);
            UI.UpdateSideJudge(perfect, great, good, ok, miss, epoor, cb, fast, slow,
                    avgRate.ToString("0.00") + "%", (avgTimeDiff * 100).ToString("0.0") + "ms");
        }

        private void UpdateTiming(double time, bool rateadd)
        {
            double abstime = Math.Abs(time);
            TimingType timingType = GetTimingType(abstime);

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
                JudgeUI.UpdateJudge(timingType, combo, "0.00%", 0, score);
            }
            else if(timingType == TimingType.BAD)
            {
                sumRate += (1 - abstime / BAD) * 100;
                combo = 0;
                JudgeUI.UpdateJudge(timingType, combo, "0.00%", 0, score);
            }
            else if(rateadd)
            {
                sumRate += (1 - abstime / BAD) * 100;

                combo += 1;
                sumTimeDiff += (time - Const.Sync * 0.01);

                if (combo > maxcombo)
                {
                    maxcombo = combo;
                    UI.UpdateMaxCombo(maxcombo);
                }

                JudgeUI.UpdateJudge(timingType, combo, ((1 - abstime / BAD) * 100).ToString("0.00") + "%", (int)Math.Round(timefs*100), score);
            }

            avgRate = Math.Round(sumRate * 100 / processedNotes) / 100;
            avgTimeDiff = sumTimeDiff / processedNotes;
            UI.UpdateSideJudge(perfect, great, good, ok, miss, epoor, cb, fast, slow,
                avgRate.ToString("0.00") + "%", (avgTimeDiff * 100).ToString("0.0") + "ms");

            if(Const.AutoSync == AutoSyncType.ON)
            {
                if(processedNotes > 20)
                    Const.Sync = (int)(avgTimeDiff * -100);
            }
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
        private TimingType GetTimingType(double otime)
        {
            double time = Math.Abs(otime);
            if (time <= PERFECT) return TimingType.PERFECT;
            else if (time > PERFECT && time <= GREAT) return TimingType.GREAT;
            else if (time > GREAT && time <= GOOD) return TimingType.GOOD;
            else if (time > GOOD && time <= BAD) return TimingType.BAD;
            else if (time > BAD && time <= POOR) return TimingType.POOR;
            else return TimingType.NONE;
        }

        private void UpdateScore()
        {
            UI.UpdateScore(score);
            if (Const.GraphType != GraphType.OFFBGA
                && Const.GraphType != GraphType.OFFGEAR)
                Graph.UpdateGraph(score, processedNotes, noteCount);
        }

        private void ShowBeam(int lane, bool onoff)
        {
            BeamUI.ShowAndHideBeam(lane, onoff);
        }

        public void GetResultData(int totalNotes)
        {
            Const.ResultPerfect = perfect;
            Const.ResultGreat = great;
            Const.ResultGood = good;
            Const.ResultOk = ok;
            Const.ResultMiss = miss;
            Const.ResultEPoor = epoor;
            Const.ResultComboBreak = cb;
            Const.ResultFast = fast;
            Const.ResultSlow = slow;
            Const.ResultAvgRate = (float)avgRate;
            Const.ResultTimeDiff = (float)avgTimeDiff;
            Const.ResultScore = score;
            Const.ResultMaxCombo = maxcombo;
            Const.ResultRank = GetRank(score, totalNotes);

            Const.ResultTotalNote = totalNotes;
            Const.ResultProcessedNote = processedNotes;
        }

        public void SpeedUpFixed()
        {
            if (speed < 2000)
            {
                speed += 25;
                Const.SpeedStd = speed;
                isSpeedChanged = true;
            }
        }

        public void SpeedDownFixed()
        {
            if (speed > 50)
            {
                speed -= 25;
                Const.SpeedStd = speed;
                isSpeedChanged = true;
            }
        }

        public void SpeedUpFluid()
        {
            if (speedfl < 2000)
            {
                speedfl += 25;
                Const.SpeedCon = speedfl;
                isSpeedChanged = true;
            }
        }

        public void SpeedDownFluid()
        {
            if (speedfl > 100)
            {
                speedfl -= 25;
                Const.SpeedCon = speedfl;
                isSpeedChanged = true;
            }
        }

        public int GetProcessedNotes()
        {
            return processedNotes;
        }

        public void UpdateBPM(double bpm)
        {
            BPM = bpm;
        }

        public void RecalcSpd()
        {
            isSpeedChanged = true;
        }

        private string GetRank(int ex, int notes)
        {
            string rank = "f";
            float currentRankState = (float)ex / (notes * 2);

            if (currentRankState >= 8f / 9)
            {
                rank = "aaa";
            }
            else if (currentRankState >= 7f / 9)
            {
                rank = "aa";
            }
            else if (currentRankState >= 6f / 9)
            {
                rank = "a";
            }
            else if (currentRankState >= 5f / 9)
            {
                rank = "b";
            }
            else if (currentRankState >= 4f / 9)
            {
                rank = "c";
            }
            else if (currentRankState >= 3f / 9)
            {
                rank = "d";
            }
            else if (currentRankState >= 2f / 9)
            {
                rank = "e";
            }
            else
            {
                rank = "f";
            }

            return rank;
        }

        public int GetScore() { return score; }
        public int GetPerfect() { return perfect; }
        public int GetGreat() { return great; }
        public int GetGood() { return good; }
        public int GetBad() { return ok; }
        public int GetPoor() { return miss; }
        public int GetEPoor() { return epoor; }
        public int GetBreak() { return cb; }
    }
}
