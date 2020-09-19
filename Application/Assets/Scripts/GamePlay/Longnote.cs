using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Longnote
    {
        private int lane;
        private double startpos;
        private double endpos;
        private Note start;
        private Note end;
        private Note middle;
        private bool used = false;
        // 처리중일때의 value (Scroller.userGamePlay의 롱노트 처리 참고)
        private bool inProgress = false;
        private double processTime = 0;

        public Longnote(int lane, Note start, double startpos, Note middle, double endpos = -1, Note end = null)
        {
            this.lane = lane;
            this.start = start;
            this.startpos = startpos;
            this.endpos = endpos;
            this.middle = middle;
            this.end = end;
        }

        public bool isEnded()
        {
            if (end == null) return false;
            else return true;
        }

        public int getLane() { return lane; }
        public double getStartPos() { return startpos; }
        public double getEndPos() { return endpos; }
        public Note getStart() { return start; }
        public Note getMiddle() { return middle; }
        public Note getEnd() { return end; }
        public bool isUsed() { return used; }
        public bool isInProgress() { return inProgress; }
        public double getProcTime() { return processTime; }

        public void setStartPos(double pos) { this.startpos = pos; }
        public void setEnd(Note end) { this.end = end; }
        public void setEndPos(double endpos) { this.endpos = endpos; }
        public void setUsed(bool used) { this.used = used; }
        public void setInProgress(bool inProgress) { this.inProgress = inProgress; }
        public void setProcTime(double processTime) { this.processTime = processTime; }
    }
}
