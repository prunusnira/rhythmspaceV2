using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Longnote
    {
        public int Line { get; set; }
        public Note StartNote { get; set; }
        public Note EndNote { get; set; }
        public Note MidNote { get; set; }
        public bool Used { get; set; }
        // 처리중일때의 value (Scroller.userGamePlay의 롱노트 처리 참고)
        public bool InProgress { get; set; }

        public Longnote(int lane, Note start, Note middle, Note end = null)
        {
            Line = lane;
            StartNote = start;
            MidNote = middle;
            EndNote = end;
            Used = false;
            InProgress = false;
        }
    }
}
