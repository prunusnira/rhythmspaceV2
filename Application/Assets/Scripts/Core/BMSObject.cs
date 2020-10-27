namespace BMSCore
{
    public abstract class NoteObject
    {
        public int Bar { get; set; }

        // Bar 1개를 1로 단위로 하여 정의
        public double Position { get; set; }
        public double Timing { get; set; }
        public double OnScrPos { get; set; }
        public ObjectType ObjType { get; set; }
        public bool Used { get; set; }
    }

    public class VisibleNote : NoteObject
    {

        public int Line { get; set; }
        public bool OnScreen { get; set; }
        public UnityEngine.GameObject NoteObject { get; set; }
        public bool IsSRanDone { get; set; } // S-RAN 전용 변수

        public VisibleNote()
        {
            OnScreen = false;
            IsSRanDone = false;
        }
    }

    public class PlayNote : VisibleNote
    {
        public string Wav { get; set; }
        public int LNNum { get; set; }
        public NoteType PlayNoteType { get; set; }
        public bool InTimingWindow { get; set; }
        public bool IsPlayed { get; set; }
    }

    public class MineNote : VisibleNote
    {
    }

    public class SplitLine : VisibleNote
    {
    }

    public class LongNote : NoteObject
    {
        public int Line { get; set; }
        public PlayNote Start { get; set; }
        public PlayNote Mid { get; set; }
        public PlayNote End { get; set; }
        public bool Processing { get; set; }
    }

    public class BGANote : NoteObject
    {
        public UnityEngine.Sprite BGASprite { get; set; }
        public string VideoFile { get; set; }
    }

    public class LayerNote : NoteObject
    {
        public UnityEngine.Sprite BGASprite { get; set; }
    }

    public class BGMNote : NoteObject
    {
        public string Wav { get; set; }
    }

    public class TimingObject : NoteObject { }

    public class BPMNote : TimingObject
    {
        public double BPMValue { get; set; }
    }

    public class StopNote : TimingObject
    {
        // bps로 나누지 않은 값
        public double StopDuration { get; set; }
    }
}