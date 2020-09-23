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
        public List<Note>[] NotePlay { get; set; }
        public List<Note> NoteBGA { get; set; }
        public List<Note> NoteBPM { get; set; }
        public List<Note> NoteBGM { get; set; }
        public List<Note> NoteStop { get; set; }
        public List<Longnote> LongNotes { get; set; }
        public double PrevBPM { get; set; }
        public double CurrentBPM { get; set; }
        public double BPS { get; set; }
        public double SPB { get; set; }

        public PlayData()
        {
            // Preference 값 가져오기
            PlayLine = Const.Playline;

            string bmsPath = Const.PlayingBMSPath;
            if (!bmsPath.Equals(""))
                BMS = new BMS(bmsPath);

            // 데이터 초기화
            NotePlay = new List<Note>[8];
            LongNotes = new List<Longnote>();

            for(int i = 0; i < 8; i++)
            {
                NotePlay[i] = new List<Note>();
            }

            NoteBGM = new List<Note>();
            NoteBGA = new List<Note>();
            NoteBPM = new List<Note>();
            NoteStop = new List<Note>();
        }
	}
}
