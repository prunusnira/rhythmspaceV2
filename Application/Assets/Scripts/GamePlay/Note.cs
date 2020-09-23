using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Note
    {
        public double Position { get; set; }
        public double ScrPos { get; set; }
        public string Wav { get; set; }
        public ObjectType Notetype;
        public int Bar { get; set; }
        public int Line { get; set; }
        // 0: 턴테이블, 1~7 각버튼 대응
        public bool Released { get; set; } // 노트가 표시되었는지 유무
        public bool Used { get; set; } // 노트 처리 유무
        public GameObject Noteobj { get; set; }

        // 롱노트 관련 처리
        public bool IsLongnote { get; set; }
        public int LNNum { get; set; }
        public bool LNStart { get; set; }
        public bool LNMid { get; set; }
        public bool LNEnd { get; set; }

        // 타이밍 윈도우 체크
        public bool TimingWindowChk { get; set; }

        public Note(double pos, string wav, int bar, int lane, ObjectType notetype = ObjectType.PLAYABLE, bool longnote = false)
        {
            Position = pos;
            this.Wav = wav;
            this.Notetype = notetype;
            this.Bar = bar;
            this.Line = lane;
            Released = false;
            Used = false;
            Noteobj = null;
            this.IsLongnote = longnote;
            ScrPos = 4000;
            LNNum = -1;
            LNStart = false;
            LNMid = false;
            LNEnd = false;
            TimingWindowChk = false;
        }
    }
}
