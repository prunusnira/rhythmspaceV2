using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class NoteObjectAdder : MonoBehaviour
    {
        // Display Note
        private NoteGenerator generator;
        public GameObject[] noteParentObj;

        private void Awake()
        {
            // 노트 표시용 설정
            generator = GetComponent<NoteGenerator>();
        }

        public void DisplaySplitLine(SplitLine line)
        {
            GameObject noteObj = generator.AddNewSplitLine(line.Timing, noteParentObj[Const.PlayerSide].transform);
            noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
            line.OnScreen = true;
            line.NoteObject = noteObj;
        }

        public void DisplayMineNote(MineNote note)
        {
            GameObject noteObj = generator.AddNewMineNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
            noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
            note.OnScreen = true;
            note.NoteObject = noteObj;
        }

        public void DisplayPlayNote(PlayNote note, List<LongNote> lnlist)
        {
            if (note.PlayNoteType == NoteType.SINGLE)
            {
                GameObject noteObj = generator.AddNewNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
                noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                note.OnScreen = true;
                note.NoteObject = noteObj;
            }
            else
            {
                // 아직 롱노트가 없으면 일반 노트를 하나 추가하고
                // 롱노트를 이 노트의 위치와 시작 위치 사이에 추가할 수 있도록 함
                // 이 동작은 Scroller.moveNotes()에서 isLong()을 확인해서 표기한다

                // 시작노트
                if (note.PlayNoteType == NoteType.LNSTART)
                {
                    GameObject noteObj = generator.AddNewNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
                    noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                    note.OnScreen = true;
                    note.NoteObject = noteObj;

                    // 끝노트도 같이 추가한다
                    for (int i = 0; i < lnlist.Count; i++)
                    {
                        if (lnlist[i].Start == note)
                        {
                            // 가운데노트
                            PlayNote lnNote = lnlist[i].Mid;
                            GameObject lnObj = generator.AddNewNote(lnNote.Line, lnNote.Timing, noteParentObj[Const.PlayerSide].transform);
                            lnObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                            lnNote.OnScreen = true;
                            lnNote.NoteObject = lnObj;

                            // 끝노트
                            PlayNote endNote = lnlist[i].End;
                            GameObject endObj = generator.AddNewNote(endNote.Line, endNote.Timing, noteParentObj[Const.PlayerSide].transform);
                            endObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                            endNote.OnScreen = true;
                            endNote.NoteObject = endObj;

                            lnlist[i].Mid.Position = (note.Position + endNote.Position) / 2;
                        }
                    }
                }
            }
        }
    }
}

