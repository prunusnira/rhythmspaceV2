using System;
using System.IO;
using System.Collections.Generic;
using BMSCore;
using UnityEngine;

namespace BMSPlayer
{
    public class NoteGenerator:MonoBehaviour
    {
        public GameObject noteWhite;
        public GameObject noteBlue;
        public GameObject notePink;
        public GameObject noteMine;

        public GameObject AddNewNote(int clane, double timing, Transform parent)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(GetXPos(clane), 0, (float)timing);

            switch (clane)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                    noteObject = Instantiate(noteWhite, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    break;
                case 2:
                case 4:
                case 6:
                    noteObject = Instantiate(noteBlue, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    break;
                case 0:
                    noteObject = Instantiate(notePink, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    break;
            }

            return noteObject;
        }

        public GameObject AddNewMineNote(int clane, double timing, Transform parent)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(GetXPos(clane), 0, (float)timing);

            noteObject = Instantiate(noteMine, pos, Quaternion.identity, parent);
            noteObject.transform.localPosition = pos;

            return noteObject;
        }

        public int GetXPos(int line)
        {
            switch(line)
            {
                case 0: return -490;
                case 1: return -315;
                case 2: return -175;
                case 3: return -35;
                case 4: return 105;
                case 5: return 245;
                case 6: return 385;
                case 7: return 525;
                default: return -1000;
            }
        }

        public void AnalyzeNotes(PlayData data, int[] Layout)
        {
            for (int cbar = 0; cbar <= data.BMS.LastBar; cbar++)
            {
                // 노트 분석 및 배치
                if (data.BMS.PlayNote.ContainsKey(cbar))
                {
                    Dictionary<int, string> PlayNotesInLine = PlayNotesByBar(data.BMS.PlayNote[cbar]);
                    NoteAdderPlay(data, PlayNotesInLine, cbar, Layout);
                }

                // 지뢰노트 처리
                if (data.BMS.MineNote.ContainsKey(cbar))
                {
                    Dictionary<int, string> MineNotesInLine = MineNotesByBar(data.BMS.MineNote[cbar]);
                    NoteAdderMine(data, MineNotesInLine, cbar, Layout);
                }

                // BPM 변경: 보이지 않는 bpm 노트 오브젝트를 만들고 이 노트가 판정선 100%가 되면 bpm 값을 변경
                if (data.BMS.BPMNote.ContainsKey(cbar)) // Type1
                {
                    NoteAdderBPM(BPMNoteType.Type1, data, cbar);
                }

                if(data.BMS.BPMNoteType2.ContainsKey(cbar)) // Type2
                {
                    NoteAdderBPM(BPMNoteType.Type2, data, cbar);
                }

                // 배경음: 보이지 않는 music 노트 오브젝트를 만들고 이 노트가 판정선 100%가 되면 할당된 소리를 재생
                if (data.BMS.Music.ContainsKey(cbar))
                {
                    NoteAdderMusic(data, cbar);
                }

                // 멈춤 처리
                if (data.BMS.StopNote.ContainsKey(cbar))
                {
                    NoteAdderStop(data, cbar);
                }

                // BGA 노트 처리
                if (data.BMS.BGANote.ContainsKey(cbar))
                {
                    NoteAdderBGA(data, cbar);
                }

                if (data.BMS.BarLength.ContainsKey(cbar))
                {
                    data.TotalLength += data.BMS.BarLength[cbar];
                }
                else
                {
                    data.TotalLength += 1;
                }
            }
            LongnoteSetup(data.NotePlay, data.NoteLong);

            // 노트 오브젝트 등록 후 각 라인별로 position에 따라 정렬 수행
            // (롱노트 순서 정렬을 위해서)
            SortNote(data.NotePlay);
        }

        // 각 라인 별 노트 데이터 저장 (Bar 1개 내)
        public Dictionary<int, string> PlayNotesByBar(Dictionary<string, string> NotesInBar) {
            Dictionary<int, string> NotesPerLine = new Dictionary<int, string>(16);

            if (NotesInBar.ContainsKey("11"))
                NotesPerLine.Add(1, NotesInBar["11"]);
            if (NotesInBar.ContainsKey("12"))
                NotesPerLine.Add(2, NotesInBar["12"]);
            if (NotesInBar.ContainsKey("13"))
                NotesPerLine.Add(3, NotesInBar["13"]);
            if (NotesInBar.ContainsKey("14"))
                NotesPerLine.Add(4, NotesInBar["14"]);
            if (NotesInBar.ContainsKey("15"))
                NotesPerLine.Add(5, NotesInBar["15"]);
            if (NotesInBar.ContainsKey("16"))
                NotesPerLine.Add(0, NotesInBar["16"]);
            if (NotesInBar.ContainsKey("18"))
                NotesPerLine.Add(6, NotesInBar["18"]);
            if (NotesInBar.ContainsKey("19"))
                NotesPerLine.Add(7, NotesInBar["19"]);
            if (NotesInBar.ContainsKey("51"))
                NotesPerLine.Add(11, NotesInBar["51"]);
            if (NotesInBar.ContainsKey("52"))
                NotesPerLine.Add(12, NotesInBar["52"]);
            if (NotesInBar.ContainsKey("53"))
                NotesPerLine.Add(13, NotesInBar["53"]);
            if (NotesInBar.ContainsKey("54"))
                NotesPerLine.Add(14, NotesInBar["54"]);
            if (NotesInBar.ContainsKey("55"))
                NotesPerLine.Add(15, NotesInBar["55"]);
            if (NotesInBar.ContainsKey("56"))
                NotesPerLine.Add(10, NotesInBar["56"]);
            if (NotesInBar.ContainsKey("58"))
                NotesPerLine.Add(16, NotesInBar["58"]);
            if (NotesInBar.ContainsKey("59"))
                NotesPerLine.Add(17, NotesInBar["59"]);

            return NotesPerLine;
        }

        public Dictionary<int, string> MineNotesByBar(Dictionary<string, string> NotesInBar)
        {
            Dictionary<int, string> NotesPerLine = new Dictionary<int, string>(16);

            if (NotesInBar.ContainsKey("D1"))
                NotesPerLine.Add(1, NotesInBar["D1"]);
            if (NotesInBar.ContainsKey("D2"))
                NotesPerLine.Add(2, NotesInBar["D2"]);
            if (NotesInBar.ContainsKey("D3"))
                NotesPerLine.Add(3, NotesInBar["D3"]);
            if (NotesInBar.ContainsKey("D4"))
                NotesPerLine.Add(4, NotesInBar["D4"]);
            if (NotesInBar.ContainsKey("D5"))
                NotesPerLine.Add(5, NotesInBar["D5"]);
            if (NotesInBar.ContainsKey("D6"))
                NotesPerLine.Add(0, NotesInBar["D6"]);
            if (NotesInBar.ContainsKey("D8"))
                NotesPerLine.Add(6, NotesInBar["D8"]);
            if (NotesInBar.ContainsKey("D9"))
                NotesPerLine.Add(7, NotesInBar["D9"]);

            return NotesPerLine;
        }

        public void NoteAdderPlay(
            PlayData data,
            Dictionary<int, string> line,
            int cbar,
            int[] Layout)
        {
            int longcnt = 0;
            System.Random rand = new System.Random();

            // 라인별 노트 추가
            for (int cline = 0; cline < 18; cline++)
            {
                if (line.ContainsKey(cline))
                {
                    if (line[cline] != null)
                    {
                        int size = line[cline].Length / 2;

                        for (int n = 0; n < size; n++)
                        {
                            string noteStr = line[cline].Substring(n * 2, 2);
                            if (noteStr != "00")
                            {
                                double position = (double)n / size;

                                if (data.BMS.BarLength.ContainsKey(cbar))
                                {
                                    position *= data.BMS.BarLength[cbar];
                                }

                                double realpos = (data.TotalLength + position);

                                PlayNote note = new PlayNote
                                {
                                    Position = realpos,
                                    OnScrPos = realpos,
                                    Wav = noteStr,
                                    Bar = cbar,
                                    ObjType = ObjectType.PLAYABLE
                                };
                                switch (cline)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                        note.PlayNoteType = NoteType.SINGLE;
                                        data.TotalNotes++;
                                        data.NoteCount++;
                                        break;
                                    case 10:
                                    case 11:
                                    case 12:
                                    case 13:
                                    case 14:
                                    case 15:
                                    case 16:
                                    case 17:
                                        note.PlayNoteType = NoteType.LNTEMP;
                                        longcnt++;
                                        break;
                                }

                                // 라인 설정
                                switch (cline)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                        // SRAN이 아니라면
                                        if(Layout != null)
                                        {
                                            note.Line = Layout[cline - 1];
                                        }
                                        else
                                        {
                                            note.Line = rand.Next(1, 8);
                                        }
                                        break;
                                    case 11:
                                    case 12:
                                    case 13:
                                    case 14:
                                    case 15:
                                    case 16:
                                    case 17:
                                        // SRAN이 아니라면
                                        if (Layout != null)
                                        {
                                            note.Line = Layout[cline - 11];
                                        }
                                        else
                                        {
                                            note.Line = rand.Next(1, 8);
                                        }
                                        break;
                                }
                                data.NotePlay[note.Line].Add(note);
                            }
                        }
                    }
                }
            }
            data.NoteCount += longcnt / 2;
            data.TotalNotes += longcnt / 2;
        }

        public void LongnoteSetup(List<PlayNote>[] notePlay, List<LongNote> lnlist)
        {
            bool[] LNAdding = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                LNAdding[i] = false;
            }

            foreach (List<PlayNote> notes in notePlay)
            {
                foreach (PlayNote n in notes)
                {
                    if(n.PlayNoteType == NoteType.LNTEMP)
                    {
                        int cline = n.Line;
                        if (LNAdding[cline])
                        {
                            // 이미 롱노트가 추가중인 상태이면 현재 라인의 lnlist를 갱신하고 노트 표시 추가
                            for (int i = 0; i < lnlist.Count; i++)
                            {
                                if (lnlist[i].Line == cline && lnlist[i].End == null)
                                {
                                    n.PlayNoteType = NoteType.LNEND;
                                    n.LNNum = i;
                                    lnlist[i].End = n;
                                    lnlist[i].Mid.Position = 
                                        (lnlist[i].Start.Position + n.Position) / 2;
                                    LNAdding[cline] = false;
                                }
                            }
                        }
                        else
                        {
                            // 시작노트 처리
                            n.PlayNoteType = NoteType.LNSTART;
                            n.LNNum = lnlist.Count;

                            // 가운데 노트 처리
                            PlayNote MidNote = new PlayNote
                            {
                                Position = n.Position,
                                Wav = "L#",
                                Bar = n.Bar,
                                Line = n.Line,
                                LNNum = lnlist.Count,
                                PlayNoteType = NoteType.LNMID,
                                ObjType = ObjectType.PLAYABLE
                            };

                            // 롱노트 오브젝트 생성
                            LongNote ln = new LongNote
                            {
                                Line = cline,
                                Start = n,
                                Mid = MidNote
                            };

                            // 롱노트 등록
                            lnlist.Add(ln);
                            LNAdding[cline] = true;
                        }
                    }
                }
            }
        }

        public void NoteAdderMine(
            PlayData data,
            Dictionary<int, string> line,
            int cbar,
            int[] Layout)
        {
            int longcnt = 0;

            // 라인별 노트 추가
            for (int cline = 0; cline < 8; cline++)
            {
                if (line.ContainsKey(cline))
                {
                    if (line[cline] != null)
                    {
                        int size = line[cline].Length / 2;

                        for (int n = 0; n < size; n++)
                        {
                            string noteStr = line[cline].Substring(n * 2, 2);
                            if (noteStr != "00")
                            {
                                double position = (double)n / size;

                                if (data.BMS.BarLength.ContainsKey(cbar))
                                {
                                    position *= data.BMS.BarLength[cbar];
                                }

                                double realpos = (data.TotalLength + position);

                                MineNote note = new MineNote
                                {
                                    Position = realpos,
                                    OnScrPos = realpos,
                                    Bar = cbar,
                                    ObjType = ObjectType.MINE
                                };

                                note.Line = cline;
                                data.NoteCount++;
                                data.NoteMine[note.Line].Add(note);
                            }
                        }
                    }
                }
            }
            data.NoteCount += longcnt / 2;
            data.TotalNotes += longcnt / 2;
        }

        public void NoteAdderBPM(BPMNoteType type, PlayData data, int cbar)
        {
            string bpmline;

            if(type == BPMNoteType.Type1)
            {
                bpmline = data.BMS.BPMNote[cbar];
            }
            else
            {
                bpmline = data.BMS.BPMNoteType2[cbar];
            }

            if (bpmline.Length > 0)
            {
                int bpmsize = bpmline.Length / 2;
                for (int n = 0; n < bpmsize; n++)
                {
                    string noteStr = bpmline.Substring(n * 2, 2);
                    if (noteStr != "00")
                    {
                        double position = (double)n / bpmsize;
                        if (data.BMS.BarLength.ContainsKey(cbar))
                        {
                            position *= data.BMS.BarLength[cbar];
                        }

                        double realpos = (data.TotalLength + position);

                        BPMNote note = new BPMNote
                        {
                            Position = realpos,
                            Bar = cbar,
                            ObjType = ObjectType.BPM
                        };

                        if (type == BPMNoteType.Type1)
                        {
                            note.BPMValue = Convert.ToInt32(noteStr, 16);
                        }   
                        else if(type == BPMNoteType.Type2)
                        {
                            note.BPMValue = data.BMS.BPMNum[noteStr];
                        }

                        data.NoteBPM.Add(note);
                        data.NoteCount++;
                        data.BPMPositionFix.Add(realpos);
                    }
                }
            }
        }

        public void NoteAdderMusic(PlayData data, int cbar)
        {
            for (int num = 0; num < data.BMS.Music[cbar].Count; num++)
            {
                string mnote = data.BMS.Music[cbar][num];
                int size = mnote.Length / 2;
                for (int n = 0; n < size; n++)
                {
                    string noteStr = data.BMS.Music[cbar][num].Substring(n * 2, 2);
                    if (noteStr != "00")
                    {
                        double position = (double)n / size;
                        if (data.BMS.BarLength.ContainsKey(cbar))
                        {
                            position *= data.BMS.BarLength[cbar];
                        }

                        double realpos = (data.TotalLength + position);

                        BGMNote note = new BGMNote
                        {
                            Position = realpos,
                            Wav = noteStr,
                            Bar = cbar,
                            ObjType = ObjectType.MUSIC
                        };

                        data.NoteBGM.Add(note);
                        data.NoteCount++;
                    }
                }
            }
        }

        public void NoteAdderBGA(PlayData data, int cbar)
        {
            int size = data.BMS.BGANote[cbar].Length / 2;

            for (int n = 0; n < size; n++)
            {
                string noteStr = data.BMS.BGANote[cbar].Substring(n * 2, 2);
                if (noteStr != "00")
                {
                    double position = (double)n / size;

                    if (data.BMS.BarLength.ContainsKey(cbar))
                    {
                        position *= data.BMS.BarLength[cbar];
                    }

                    double realpos = (data.TotalLength + position);

                    BGANote note = new BGANote
                    {
                        Position = realpos,
                        Bar = cbar,
                        ObjType = ObjectType.BGA
                    };

                    if (data.BMS.BGAImages.Count == 0)
                    {
                        note.VideoFile = data.BMS.BGAVideoFile;
                    }
                    else
                    {
                        if(data.BMS.BGAImages.ContainsKey(noteStr))
                        {
                            note.BGASprite = data.BMS.BGAImages[noteStr];
                        }
                    }

                    data.NoteBGA.Add(note);
                    data.NoteCount++;
                }
            }
        }

        public void NoteAdderStop(PlayData data, int cbar)
        {
            int size = data.BMS.StopNote[cbar].Length / 2;

            for (int n = 0; n < size; n++)
            {
                string noteStr = data.BMS.StopNote[cbar].Substring(n * 2, 2);
                if (noteStr != "00")
                {
                    double position = (double)n / size;

                    if (data.BMS.BarLength.ContainsKey(cbar))
                    {
                        position *= data.BMS.BarLength[cbar];
                    }

                    double realpos = (data.TotalLength + position);

                    StopNote note = new StopNote
                    {
                        Position = realpos,
                        Bar = cbar,
                        ObjType = ObjectType.STOP,
                        StopDuration = (double)data.BMS.StopList[noteStr] / 192
                        // 나중에 실시간으로 멈출 때 bps로 나누어야 함
                    };
                    data.NoteStop.Add(note);
                    data.NoteCount++;
                }
            }
        }

        private double CalculateTotalLength(BMS bms)
        {
            double length = 0;
            for (int i = 0; i < bms.LastBar; i++)
            {
                if (bms.BarLength.ContainsKey(i))
                    length += bms.BarLength[i];
                else length += 1;
            }
            return length;
        }

        private void SortNote(List<PlayNote>[] notePlay)
        {
            foreach(List<PlayNote> nl in notePlay)
            {
                nl.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            }
        }

        public void PositionToTiming(PlayData data)
        {
            Dictionary<int, List<BPMNote>> BPMChangeCheck =
                new Dictionary<int, List<BPMNote>>();

            Dictionary<int, List<StopNote>> StopChangeCheck =
                new Dictionary<int, List<StopNote>>();

            // 작업 진행 전에 bar마다 bpm/stop 체크
            foreach(BPMNote n in data.NoteBPM)
            {
                List<BPMNote> bpmBar;
                if (BPMChangeCheck.ContainsKey(n.Bar))
                {
                    bpmBar = BPMChangeCheck[n.Bar];
                    bpmBar.Add(n);
                    BPMChangeCheck[n.Bar] = bpmBar;
                }
                else
                {
                    bpmBar = new List<BPMNote>();
                    bpmBar.Add(n);
                    BPMChangeCheck.Add(n.Bar, bpmBar);
                }
            }

            foreach(StopNote n in data.NoteStop)
            {
                List<StopNote> stopBar;
                if (StopChangeCheck.ContainsKey(n.Bar))
                {
                    stopBar = StopChangeCheck[n.Bar];
                    stopBar.Add(n);
                    StopChangeCheck[n.Bar] = stopBar;
                }
                else
                {
                    stopBar = new List<StopNote>();
                    stopBar.Add(n);
                    StopChangeCheck.Add(n.Bar, stopBar);
                }
            }

            double prevTime = 0;
            double prevBarPos = 0;
            double prevChangePos = 0;
            double bps = data.BMS.BPMStart / 240;
            double nextbps = bps;

            double stopPos = 0;
            double stopTime = 0;
            bool isStop = false;

            // Bar는 000부터 시작하므로 시간 계산을 0부터 진행
            for (int bar = 0; bar < data.BMS.LastBar + 1; bar++)
            {
                double barLength = 1;
                if(data.BMS.BarLength.ContainsKey(bar))
                {
                    barLength = data.BMS.BarLength[bar];
                }
                
                // 모든 노트에서 bpm이 변경되거나 시간이 멈추는 부분에 대해
                // 범위를 나누고 각각 시간 계산을 별도로 진행
                List<NoteObject> timeChangeInBar = new List<NoteObject>();

                if (BPMChangeCheck.ContainsKey(bar) ||
                    StopChangeCheck.ContainsKey(bar))
                {

                    if(BPMChangeCheck.Count > 0 && BPMChangeCheck.ContainsKey(bar))
                    {
                        List<BPMNote> bpmChangeInBar = BPMChangeCheck[bar];
                        bpmChangeInBar.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));

                        foreach (BPMNote n in BPMChangeCheck[bar])
                        {
                            timeChangeInBar.Add(n);
                        }
                    }

                    if (StopChangeCheck.Count > 0 && StopChangeCheck.ContainsKey(bar))
                    {
                        List<StopNote> stopChangeInBar = StopChangeCheck[bar];
                        stopChangeInBar.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));

                        foreach (StopNote n in StopChangeCheck[bar])
                        {
                            timeChangeInBar.Add(n);
                        }
                    }

                    timeChangeInBar.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));

                    // 각 Position을 기준으로 같은바의 해당 Position보다 아래에 있으면
                    // 노트 타임을 설정함
                    for (int i = 0; i < timeChangeInBar.Count+1; i++)
                    {
                        double PosStart = 0;
                        double PosEnd = 0;
                        if(bar == 101)
                        {
                            int a = 1 + 1;
                        }
                        bps = nextbps;

                        // i == 0이면 bar 시작위치에서 bpm 변경 노트까지를 판단
                        if(i == 0)
                        {
                            PosStart = prevBarPos;
                            PosEnd = timeChangeInBar[i].Position;
                        }
                        // i가 마지막 인덱스이면 이전 위치에서 barlength까지
                        else if (i == timeChangeInBar.Count)
                        {
                            PosStart = timeChangeInBar[i - 1].Position;
                            PosEnd = prevBarPos + barLength;
                        }
                        // 나머지는 이전것부터 현재것까지
                        else
                        {
                            PosStart = timeChangeInBar[i - 1].Position;
                            PosEnd = timeChangeInBar[i].Position;
                        }

                        // 각 노트에 대해 Position 설정
                        for (int j = 0; j < 8; j++)
                        {
                            foreach (PlayNote n in data.NotePlay[j])
                            {
                                // Stop 발생 시 해당위치에 있는 노트들은 처리를 수행함
                                if(isStop && n.Position == stopPos)
                                {
                                    CalculateTiming(n, bps, stopTime, PosStart);
                                }
                                // 현재 시간 변경점 이내에 있는 경우
                                else if (n.Bar == bar &&
                                        n.Position >= PosStart &&
                                        n.Position < PosEnd
                                    )
                                {
                                    CalculateTiming(n, bps, prevTime, PosStart);
                                }
                            }
                        }

                        for (int j = 0; j < 8; j++)
                        {
                            foreach (MineNote n in data.NoteMine[j])
                            {
                                if (isStop && n.Position == stopPos)
                                {
                                    CalculateTiming(n, bps, stopTime, PosStart);
                                }
                                else if (n.Bar == bar &&
                                        n.Position >= PosStart &&
                                        n.Position < PosEnd
                                    )
                                {
                                    CalculateTiming(n, bps, prevTime, PosStart);
                                }
                            }
                        }

                        foreach (BPMNote n in data.NoteBPM)
                        {
                            if (isStop && n.Position == stopPos)
                            {
                                CalculateTiming(n, bps, stopTime, PosStart);
                                data.BPMTimingFix.Add(n.Timing);
                            }
                            else if (n.Bar == bar &&
                                    n.Position >= PosStart &&
                                    n.Position < PosEnd
                                )
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                                data.BPMTimingFix.Add(n.Timing);
                            }
                        }

                        foreach (BGANote n in data.NoteBGA)
                        {
                            if (isStop && n.Position == stopPos)
                            {
                                CalculateTiming(n, bps, stopTime, PosStart);
                            }
                            else if (n.Bar == bar &&
                                    n.Position >= PosStart &&
                                    n.Position < PosEnd
                                )
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                            }
                        }

                        foreach (BGMNote n in data.NoteBGM)
                        {
                            if (isStop && n.Position == stopPos)
                            {
                                CalculateTiming(n, bps, stopTime, PosStart);
                            }
                            else if (n.Bar == bar &&
                                    n.Position >= PosStart &&
                                    n.Position < PosEnd
                                )
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                            }
                        }

                        foreach (StopNote n in data.NoteStop)
                        {
                            if (isStop && n.Position == stopPos)
                            {
                                CalculateTiming(n, bps, stopTime, PosStart);
                            }
                            else if (n.Bar == bar &&
                                    n.Position >= PosStart &&
                                    n.Position < PosEnd
                                )
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                            }
                        }

                        if(isStop)
                        {
                            isStop = false;
                        }

                        if (i != timeChangeInBar.Count)
                        {
                            // 현재 선택된 노트가 BPM 노트이면 bps 값을 변경
                            if (timeChangeInBar[i] is BPMNote)
                            {
                                BPMNote bpm = timeChangeInBar[i] as BPMNote;
                                nextbps = bpm.BPMValue / 240;
                            }

                            if(timeChangeInBar[i] is StopNote)
                            {
                                StopNote stop = timeChangeInBar[i] as StopNote;
                                stopPos = stop.Position;
                                stopTime = prevTime + (stopPos - prevChangePos) / bps * 10;
                                isStop = true;
                                prevTime += stop.StopDuration / bps * 10;
                            }

                            prevTime += (timeChangeInBar[i].Position - prevChangePos) / bps * 10;
                            prevChangePos = timeChangeInBar[i].Position;
                        }
                        else
                        {
                            prevBarPos += barLength;
                            prevTime += (prevBarPos - prevChangePos) / bps * 10;
                            prevChangePos = prevBarPos;
                        }
                    }
                }
                else
                {
                    double PosStart = prevBarPos;

                    // 모든 노트에 대해 타이밍 계산을 진행
                    for (int i = 0; i < 8; i++)
                    {
                        foreach (PlayNote n in data.NotePlay[i])
                        {
                            if(n.Bar == bar)
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                            }
                        }
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        foreach (MineNote n in data.NoteMine[i])
                        {
                            if (n.Bar == bar)
                            {
                                CalculateTiming(n, bps, prevTime, PosStart);
                            }
                        }
                    }

                    foreach(BGANote n in data.NoteBGA)
                    {
                        if (n.Bar == bar)
                        {
                            CalculateTiming(n, bps, prevTime, PosStart);
                        }
                    }

                    foreach(BGMNote n in data.NoteBGM)
                    {
                        if (n.Bar == bar)
                        {
                            CalculateTiming(n, bps, prevTime, PosStart);
                        }
                    }

                    if (bar == 48)
                    {
                        int a = 1 + 1;
                    }

                    prevBarPos += barLength;
                    prevChangePos = prevBarPos;
                    prevTime += barLength / bps * 10;
                }
            }
        }

        private void CalculateTiming(NoteObject n, double bps, double prevTime, double prevPos)
        {
            double time = (n.Position - prevPos) / bps * 10;
            n.Timing = prevTime + time;
        }

        public void SortAllNotes(PlayData data)
        {
            data.NoteBGA.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            data.NoteBGM.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            data.NoteBPM.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            data.NoteStop.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            for(int i = 0; i < 8; i++)
            {
                data.NoteMine[i].Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
                data.NotePlay[i].Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            }
        }
    }
}
