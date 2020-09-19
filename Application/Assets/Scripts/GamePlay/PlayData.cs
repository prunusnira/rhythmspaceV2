using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
	// 게임플레이시 필요한 모든 실시간 데이터를 담고있는 클래스
	// Static 형태로 데이터를 관리하며 다른 클래스에서 참조함
	public class PlayData
    {
		public PlayData()
        {
            // Preference 값 가져오기
            playline = Const.GetPlayline();

            string bmsPath = Const.GetPlayingBMSPath();
            if (!bmsPath.Equals(""))
                bms = new BMS(bmsPath);

            // 데이터 초기화
            notePlay = new List<Note>[8];
            longnotes = new List<Longnote>();

            for(int i = 0; i < 8; i++)
            {
                notePlay[i] = new List<Note>();
            }

            noteBGM = new List<Note>();
            noteBGA = new List<Note>();
            noteBPM = new List<Note>();
        }

		// BMS 관련 데이터
		private BMS bms;
		public BMS BMS
        {
			get { return bms; }
			set { bms = value; }
		}

		private int playline;
		public int PlayLine
        {
			get { return playline; }
			set { playline = value; }
		}

        private List<Note>[] notePlay;
        public List<Note>[] NotePlay
        {
            get { return notePlay; }
            set { notePlay = value; }
        }


        private List<Note> noteBGA;
        public List<Note> NoteBGA
        {
            get { return noteBGA; }
            set { noteBGA = value; }
        }

        private List<Note> noteBPM;
        public List<Note> NoteBPM
        {
            get { return noteBPM; }
            set { noteBPM = value; }
        }

        private List<Note> noteBGM;
        public List<Note> NoteBGM
        {
            get { return noteBGM; }
            set { noteBGM = value; }
        }


        /*private List<Note> notes;
		public List<Note> Notes
        {
			get { return notes; }
			set { notes = value; }
		}*/

        private List<Longnote> longnotes;
		public List<Longnote> LongNotes
        {
			get { return longnotes; }
			set { longnotes = value; }
		}

		// BPM
		private double prevBPM;
		public double PrevBPM
        {
			get { return prevBPM; }
			set { prevBPM = value; }
		}

		private double currentBPM;
		public double CurrentBPM
        {
			get { return currentBPM; }
			set { currentBPM = value; }
		}

		private double bps;
		public double BPS
        {
			get { return bps; }
			set { bps = value; }
		}

		private double spb;
		public double SPB
        {
			get { return spb; }
			set { spb = value; }
		}
	}
}
