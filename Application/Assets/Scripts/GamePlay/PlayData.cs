using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
	public class PlayData
    {
        // BMS 관련 데이터
        public BMS BMS { get; set; }
        public int PlayLine { get; set; }
        public double CurrentBPM { get; set; }
        public double BPS { get; set; }
        public double SPB { get; set; }
        public double Stop { get; set; }
        public double CurrentStopPos { get; set; }
        public bool IsStopOn { get; set; }
        public double LastTiming { get; set; }

        // 전체 게임 길이 (4박 기준, 1 = 4/4)
        // BMS 데이터의 02 채널과 연관
        public double TotalLength { get; set; }

        // 인게임용
        public int TotalNotes { get; set; }

        // 전체 오브젝트 수
        public int NoteCount { get; set; }

        // 노트 오브젝트 정보
        public List<PlayNote>[] NotePlay { get; set; } // 채널 별 관리
        public List<MineNote>[] NoteMine { get; set; }
        public List<LongNote> NoteLong { get; set; }
        public List<BPMNote> NoteBPM { get; set; }
        public List<StopNote> NoteStop { get; set; }
        public List<BGANote> NoteBGA { get; set; }
        public List<BGMNote> NoteBGM { get; set; }
        public List<SplitLine> SplitLine { get; set; }

        // 노트 위치 계산을 위한 변수
        public List<double> BPMPositionFix { get; set; }
        public List<double> BPMTimingFix { get; set; }
        public List<double> LineChangePos { get; set; }
        public int BPMNum { get; set; }

        // 그래프 제작용
        public List<int> HPGraph { get; set; }

        public PlayData()
        {
            // Preference 값 가져오기
            PlayLine = Const.Playline;

            string bmsPath = Const.PlayingBMSPath;
            if (!bmsPath.Equals(""))
                BMS = new BMS(bmsPath);

            // 데이터 초기화
            NotePlay = new List<PlayNote>[8];

            for (int i = 0; i < 8; i++)
            {
                NotePlay[i] = new List<PlayNote>();
            }

            NoteMine = new List<MineNote>[8];

            for (int i = 0; i < 8; i++)
            {
                NoteMine[i] = new List<MineNote>();
            }

            NoteLong = new List<LongNote>();
            NoteBGM = new List<BGMNote>();
            NoteBGA = new List<BGANote>();
            NoteBPM = new List<BPMNote>();
            NoteStop = new List<StopNote>();
            SplitLine = new List<SplitLine>();
            BPMPositionFix = new List<double>();
            BPMTimingFix = new List<double>();
            LineChangePos = new List<double>();
            HPGraph = new List<int>();

            BPMNum = 0;
            Stop = 0;
            LastTiming = 0;
            IsStopOn = false;
        }
    }
}
