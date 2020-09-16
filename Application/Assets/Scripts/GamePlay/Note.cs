using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Note
    {
        public enum NOTETYPE { PLAYABLE, MUSIC, BPM, BPMT2, BGA };

        private double position { get; set; }
        private double startPos { get; set; }
        private double scrpos { get; set; }
        private string wav { get; set; }
        private NOTETYPE notetype;
        private int bar { get; set; }
        private int lane { get; set; }
        // 0: 턴테이블, 1~7 각버튼 대응
        private bool released { get; set; } // 노트가 표시되었는지 유무
        private bool used { get; set; } // 노트 처리 유무
        public GameObject noteobj { get; set; }

        // 롱노트 관련 처리
        private bool longnote { get; set; }
        private int lnnum { get; set; }
        private bool lnStart { get; set; }
        private bool lnMid { get; set; }
        private bool lnEnd { get; set; }

        public Note(double pos, string wav, int bar, int lane, NOTETYPE notetype = NOTETYPE.PLAYABLE, bool longnote = false)
        {
            position = pos;
            startPos = pos;
            this.wav = wav;
            this.notetype = notetype;
            this.bar = bar;
            this.lane = lane;
            released = false;
            used = false;
            noteobj = null;
            this.longnote = longnote;
            scrpos = 4000;
            lnnum = -1;
            lnStart = false;
            lnMid = false;
            lnEnd = false;
        }

        public double getPosition() { return position; }
        public double getStartPosition() { return startPos; }
        public double getScrPos() { return scrpos; }
        public NOTETYPE getNotetype() { return notetype; }
        public string getWav() { return wav; }
        public int getBar() { return bar; }
        public int getLane() { return lane; }
        public bool isUsed() { return used; }
        public bool isReleased() { return released; }
        public bool isLong() { return longnote; }
        public int getLnNum() { return lnnum; }
        public bool isLnStart() { return lnStart; }
        public bool isLnMid() { return lnMid; }
        public bool isLnEnd() { return lnEnd; }

        public void setUsed(bool used) { this.used = used; }
        public void setReleased(bool released) { this.released = released; }
        public void setNote(GameObject obj) { this.noteobj = obj; released = true; }
        public void setPosition(double position) { this.position = position; }
        public void setScrPos(double scrpos) { this.scrpos = scrpos; }
        public void changeToMusic() { notetype = NOTETYPE.MUSIC; longnote = false; }
        public void setLnNum(int lnnum) { this.lnnum = lnnum; }
        public void setLnStart(bool lnStart) { this.lnStart = lnStart; }
        public void setLnMid(bool lnMid) { this.lnMid = lnMid; }
        public void setLnEnd(bool lnEnd) { this.lnEnd = lnEnd; }
    }
}
