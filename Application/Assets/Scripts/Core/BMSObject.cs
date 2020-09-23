namespace BMSCore
{
    public abstract class NoteObject
    {
        public int Bar { get; set; }
        public double Position { get; set; }
        public ObjectType ObjType { get; set; }
        public bool Used { get; set; }

        // Position on screen - deprecated
        public double ScrPos { get; set; }
    }

    public class PlayNote : NoteObject
    {
        public int Line { get; set; }
        public string Wav { get; set; }
        public int LNNum { get; set; }
        public NoteType PlayNoteType { get; set; }
    }

    public class LongNote : NoteObject
    {
        public PlayNote Start { get; set; }
        public PlayNote End { get; set; }
    }

    public class BPMNote : NoteObject
    {
        public double BPMValue { get; set; }
    }

    public class BGANote : NoteObject
    {
        public string FilePath { get; set; }
    }

    public class StopNote : NoteObject
    {
        public long StopDurationMS { get; set; }
    }

    public class BGMNote : NoteObject
    {
        public string Wav { get; set; }
    }

    public class MineNote : NoteObject
    {
        public int Line { get; set; }
        public int LNNum { get; set; }
    }
}