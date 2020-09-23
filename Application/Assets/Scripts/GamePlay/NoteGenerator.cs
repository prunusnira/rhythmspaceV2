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

        public GameObject AddNewNote(int clane, double zpos, Transform parent)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(GetXPos(clane), 1, (float)zpos);

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

        public void AnalyzeNotes(
            BMS bms,
            List<Note>[] notePlay, List<Note> noteBGA, List<Note> noteBGM,
            List<Note> noteBPM, List<Longnote> lnlist, List<Note> noteStop,
            int bars, double spb, ref double totalLen, ref int noteCount,
            ref int totalNotes, ref double totalTime) {
            int lines = Const.Playline;
            //int longcnt = 0;
            for (int cbar = 0; cbar <= bars; cbar++)
            {
                // 노트 분석 및 배치
                if (bms.Note.ContainsKey(cbar))
                {
                    Dictionary<int, string> lane = LaneMapGenerator(bms.Note[cbar]);
                    NoteAdder(notePlay, lane, bms.BarLength, cbar, lines, ref totalLen, ref totalNotes, ref noteCount);
                }

                // BPM 변경: 보이지 않는 bpm 노트 오브젝트를 만들고 이 노트가 판정선 100%가 되면 bpm 값을 변경
                if (bms.BPMNote.ContainsKey(cbar)) // Type1
                {
                    NoteAdderBPM(noteBPM, bms.BPMNote[cbar], bms.BarLength,
                        BPMNoteType.Type1, cbar, ref totalLen, ref noteCount);
                }

                if(bms.BPMNoteType2.ContainsKey(cbar)) // Type2
                {
                    NoteAdderBPM(noteBPM, bms.BPMNoteType2[cbar], bms.BarLength,
                        BPMNoteType.Type2, cbar, ref totalLen, ref noteCount);
                }

                // 배경음: 보이지 않는 music 노트 오브젝트를 만들고 이 노트가 판정선 100%가 되면 할당된 소리를 재생
                if (bms.Music.ContainsKey(cbar))
                {
                    NoteAdderMusic(noteBGM, bms.Music[cbar], bms.BarLength, cbar, ref totalLen, ref noteCount);
                }

                // 멈춤 처리
                if (bms.StopNote.ContainsKey(cbar))
                {
                    NoteAdderStop(noteStop, bms.StopNote[cbar], bms.BarLength,
                        cbar, ref totalLen, ref noteCount);
                }

                // BGA 노트 처리
                if (bms.BGANote.ContainsKey(cbar))
                {
                    NoteAdderBGA(noteBGA, bms.BGANote[cbar], bms.BarLength, cbar, ref totalLen, ref noteCount);
                }

                if (bms.BarLength.ContainsKey(cbar))
                {
                    totalLen += bms.BarLength[cbar];
                    totalTime += spb * bms.BarLength[cbar];
                }
                else
                {
                    totalLen += 1;
                    totalTime += spb;
                }
            }
            LongnoteSetup(notePlay, lnlist);

            // 노트 오브젝트 등록 후 각 라인별로 position에 따라 정렬 수행
            // (롱노트 순서 정렬을 위해서)
            SortNote(notePlay);

        }

        // 각 라인 별 노트 데이터 저장 (Bar 1개 내)
        public Dictionary<int, string> LaneMapGenerator(Dictionary<string, string> bar) {
            Dictionary<int, string> lane = new Dictionary<int, string>(16);

            if (bar.ContainsKey("11"))
                lane.Add(1, bar["11"]);
            if (bar.ContainsKey("12"))
                lane.Add(2, bar["12"]);
            if (bar.ContainsKey("13"))
                lane.Add(3, bar["13"]);
            if (bar.ContainsKey("14"))
                lane.Add(4, bar["14"]);
            if (bar.ContainsKey("15"))
                lane.Add(5, bar["15"]);
            if (bar.ContainsKey("16"))
                lane.Add(0, bar["16"]);
            if (bar.ContainsKey("18"))
                lane.Add(6, bar["18"]);
            if (bar.ContainsKey("19"))
                lane.Add(7, bar["19"]);
            if (bar.ContainsKey("51"))
                lane.Add(11, bar["51"]);
            if (bar.ContainsKey("52"))
                lane.Add(12, bar["52"]);
            if (bar.ContainsKey("53"))
                lane.Add(13, bar["53"]);
            if (bar.ContainsKey("54"))
                lane.Add(14, bar["54"]);
            if (bar.ContainsKey("55"))
                lane.Add(15, bar["55"]);
            if (bar.ContainsKey("56"))
                lane.Add(10, bar["56"]);
            if (bar.ContainsKey("58"))
                lane.Add(16, bar["58"]);
            if (bar.ContainsKey("59"))
                lane.Add(17, bar["59"]);

            return lane;
        }

        public void NoteAdder(List<Note>[] notes, Dictionary<int, string> lane,
            Dictionary<int, double> barLength, int cbar, int lines,
            ref double totalLen, ref int totalNotes, ref int noteCount) {

            int longcnt = 0;

            // 라인별 노트 추가
            for (int cline = 0; cline < 18; cline++)
            {
                if (lane.ContainsKey(cline))
                {
                    if (lane[cline] != null)
                    {
                        int size = lane[cline].Length / 2;

                        for (int n = 0; n < size; n++)
                        {
                            string noteStr = lane[cline].Substring(n * 2, 2);
                            if (noteStr != "00")
                            {
                                double position = (double)n / size;

                                if (barLength.ContainsKey(cbar))
                                {
                                    position *= barLength[cbar];
                                }

                                double realpos = (totalLen + position) * Const.FRAMEMULTIPLIER + Const.FIXEDSTARTHEIGHT;
                                
                                switch (cline)
                                {
                                    case 0:
                                        notes[0].Add(new Note(realpos, noteStr, cbar, 0));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 1:
                                        notes[1].Add(new Note(realpos, noteStr, cbar, 1));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 2:
                                        notes[2].Add(new Note(realpos, noteStr, cbar, 2));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 3:
                                        notes[3].Add(new Note(realpos, noteStr, cbar, 3));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 4:
                                        notes[4].Add(new Note(realpos, noteStr, cbar, 4));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 5:
                                        notes[5].Add(new Note(realpos, noteStr, cbar, 5));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 6:
                                        notes[6].Add(new Note(realpos, noteStr, cbar, 6));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 7:
                                        notes[7].Add(new Note(realpos, noteStr, cbar, 7));
                                        totalNotes++;
                                        noteCount++;
                                        break;
                                    case 10:
                                        notes[0].Add(new Note(realpos, noteStr, cbar, 0, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 11:
                                        notes[1].Add(new Note(realpos, noteStr, cbar, 1, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 12:
                                        notes[2].Add(new Note(realpos, noteStr, cbar, 2, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 13:
                                        notes[3].Add(new Note(realpos, noteStr, cbar, 3, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 14:
                                        notes[4].Add(new Note(realpos, noteStr, cbar, 4, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 15:
                                        notes[5].Add(new Note(realpos, noteStr, cbar, 5, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 16:
                                        notes[6].Add(new Note(realpos, noteStr, cbar, 6, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                    case 17:
                                        notes[7].Add(new Note(realpos, noteStr, cbar, 7, ObjectType.PLAYABLE, true));
                                        longcnt++;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            noteCount += longcnt / 2;
            totalNotes += longcnt / 2;
        }

        public void LongnoteSetup(List<Note>[] notePlay, List<Longnote> lnlist)
        {
            bool[] lnadd = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                lnadd[i] = false;
            }

            foreach (List<Note> notes in notePlay)
            {
                foreach (Note n in notes)
                {
                    if(n.IsLongnote)
                    {
                        int cline = n.Line;
                        if (lnadd[cline])
                        {
                            // 이미 롱노트가 추가중인 상태이면 현재 라인의 lnlist를 갱신하고 노트 표시 추가
                            for (int i = 0; i < lnlist.Count; i++)
                            {
                                if (lnlist[i].Line == cline && lnlist[i].EndNote == null)
                                {
                                    n.LNEnd = true;
                                    n.LNNum = i;
                                    lnlist[i].EndNote = n;
                                    lnlist[i].MidNote.Position = 
                                        (lnlist[i].StartNote.Position + n.Position) / 2;
                                    lnadd[cline] = false;
                                }
                            }
                        }
                        else
                        {
                            n.LNStart = true;
                            n.LNNum = lnlist.Count;
                            Note lnNote = new Note(n.Position + 1, "L#", n.Bar, n.Line, ObjectType.PLAYABLE, true);
                            lnNote.LNMid = true;
                            lnlist.Add(new Longnote(cline, n, lnNote));
                            lnadd[cline] = true;
                        }
                    }
                }
            }
        }

        public void NoteAdderBPM(List<Note> notes, string bpmline,
            Dictionary<int, double> barLength, BPMNoteType type,
            int cbar, ref double totalLen, ref int noteCount) {
            if (bpmline.Length > 0)
            {
                int bpmsize = bpmline.Length / 2;
                for (int n = 0; n < bpmsize; n++)
                {
                    string noteStr = bpmline.Substring(n * 2, 2);
                    if (noteStr != "00")
                    {
                        double position = (double)n / bpmsize;
                        if (barLength.ContainsKey(cbar))
                        {
                            position *= barLength[cbar];
                        }

                        double realpos = (totalLen + position) * Const.FRAMEMULTIPLIER + Const.FIXEDSTARTHEIGHT;

                        if(type == BPMNoteType.Type1)
                        {
                            notes.Add(new Note(realpos, noteStr, cbar, 8, ObjectType.BPM));
                        }   
                        else if(type == BPMNoteType.Type2)
                        {
                            notes.Add(new Note(realpos, noteStr, cbar, 8, ObjectType.BPMT2));
                        }

                        noteCount++;
                    }
                }
            }
        }

        public void NoteAdderMusic(List<Note> notes, List<string> musicnote,
            Dictionary<int, double> barLength, int cbar,
            ref double totalLen, ref int noteCount)
        {
            for (int num = 0; num < musicnote.Count; num++)
            {
                string mnote = musicnote[num];
                int size = mnote.Length / 2;
                for (int n = 0; n < size; n++)
                {
                    string noteStr = musicnote[num].Substring(n * 2, 2);
                    if (noteStr != "00")
                    {
                        double position = (double)n / size;
                        if (barLength.ContainsKey(cbar))
                        {
                            position *= barLength[cbar];
                        }

                        double realpos = (totalLen + position) * Const.FRAMEMULTIPLIER + Const.FIXEDSTARTHEIGHT;

                        notes.Add(new Note(realpos, noteStr, cbar, num + 9, ObjectType.MUSIC));
                        noteCount++;
                    }
                }
            }
        }

        public void NoteAdderBGA(List<Note> notes, string bgaNotes,
            Dictionary<int, double> barLength, int cbar,
            ref double totalLen, ref int noteCount)
        {
            int size = bgaNotes.Length / 2;

            for (int n = 0; n < size; n++)
            {
                string noteStr = bgaNotes.Substring(n * 2, 2);
                if (noteStr != "00")
                {
                    double position = (double)n / size;

                    if (barLength.ContainsKey(cbar))
                    {
                        position *= barLength[cbar];
                    }

                    double realpos = (totalLen + position) * Const.FRAMEMULTIPLIER + Const.FIXEDSTARTHEIGHT;

                    notes.Add(new Note(realpos, noteStr, cbar, 8, ObjectType.BGA));
                    noteCount++;
                }
            }
        }

        public void NoteAdderStop(List<Note> notes, string stopNotes,
            Dictionary<int, double> barLength, int cbar,
            ref double totalLen, ref int noteCount)
        {
            int size = stopNotes.Length / 2;

            for (int n = 0; n < size; n++)
            {
                string noteStr = stopNotes.Substring(n * 2, 2);
                if (noteStr != "00")
                {
                    double position = (double)n / size;

                    if (barLength.ContainsKey(cbar))
                    {
                        position *= barLength[cbar];
                    }

                    double realpos = (totalLen + position) * Const.FRAMEMULTIPLIER + Const.FIXEDSTARTHEIGHT;

                    notes.Add(new Note(realpos, noteStr, cbar, 8, ObjectType.STOP));
                    noteCount++;
                }
            }
        }

        private double calculateTotalLength(BMS bms)
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

        private void SortNote(List<Note>[] notePlay)
        {
            foreach(List<Note> nl in notePlay)
            {
                nl.Sort((x1, x2) => x1.Position.CompareTo(x2.Position));
            }
        }
    }
}
