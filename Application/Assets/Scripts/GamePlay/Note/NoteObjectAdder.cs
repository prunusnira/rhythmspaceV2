using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class NoteObjectAdder : MonoBehaviour
    {
        // Display Note

        public GameObject noteWhite;
        public GameObject noteBlue;
        public GameObject noteTurn;
        public GameObject noteTurnAuto;
        public GameObject noteMine;
        public GameObject splitLine;

        public GameObject[] noteParentObj;

        public void DisplaySplitLine(SplitLine line)
        {
            GameObject noteObj = AddNewSplitLine(line.Timing, noteParentObj[Const.PlayerSide].transform);
            noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
            line.OnScreen = true;
            line.NoteObject = noteObj;
        }

        public void DisplayMineNote(MineNote note)
        {
            GameObject noteObj = AddNewMineNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
            noteObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
            note.OnScreen = true;
            note.NoteObject = noteObj;
        }

        public void DisplayPlayNote(PlayNote note, List<LongNote> lnlist)
        {
            if (note.PlayNoteType == NoteType.SINGLE)
            {
                GameObject noteObj = AddNewNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
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
                    GameObject noteObj = AddNewNote(note.Line, note.Timing, noteParentObj[Const.PlayerSide].transform);
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
                            GameObject lnObj = AddNewNote(lnNote.Line, lnNote.Timing, noteParentObj[Const.PlayerSide].transform);
                            lnObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                            lnNote.OnScreen = true;
                            lnNote.NoteObject = lnObj;

                            // 끝노트
                            PlayNote endNote = lnlist[i].End;
                            GameObject endObj = AddNewNote(endNote.Line, endNote.Timing, noteParentObj[Const.PlayerSide].transform);
                            endObj.transform.SetParent(noteParentObj[Const.PlayerSide].transform, false);
                            endNote.OnScreen = true;
                            endNote.NoteObject = endObj;

                            lnlist[i].Mid.Position = (note.Position + endNote.Position) / 2;
                        }
                    }
                }
            }
        }

        public GameObject AddNewNote(int clane, double timing, Transform parent)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(GetXPos(clane), (float)timing, -1);
            Vector3 deg90 = new Vector3(90f, 0f);

            switch (clane)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                    noteObject = Instantiate(noteWhite, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    noteObject.transform.localRotation = Quaternion.Euler(deg90);
                    break;
                case 2:
                case 4:
                case 6:
                    noteObject = Instantiate(noteBlue, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    noteObject.transform.localRotation = Quaternion.Euler(deg90);
                    break;
                case 0:
                    if (Const.Auto == AutoPlayType.TURNTABLE)
                        noteObject = Instantiate(noteTurnAuto, pos, Quaternion.identity, parent);
                    else
                        noteObject = Instantiate(noteTurn, pos, Quaternion.identity, parent);
                    noteObject.transform.localPosition = pos;
                    noteObject.transform.localRotation = Quaternion.Euler(deg90);
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

            Vector3 pos = new Vector3(GetXPos(clane), (float)timing, -1);
            Vector3 deg90 = new Vector3(90f, 0f);

            noteObject = Instantiate(noteMine, pos, Quaternion.identity, parent);
            noteObject.transform.localPosition = pos;
            noteObject.transform.localRotation = Quaternion.Euler(deg90);

            return noteObject;
        }

        public GameObject AddNewSplitLine(double timing, Transform parent)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(0, (float)timing, 0);
            Vector3 deg90 = new Vector3(90f, 0f);

            noteObject = Instantiate(splitLine, pos, Quaternion.identity, parent);
            noteObject.transform.localPosition = pos;
            noteObject.transform.localRotation = Quaternion.Euler(deg90);

            return noteObject;
        }

        public float GetXPos(int line)
        {
            if (Const.PlayerSide == 0)
            {
                switch (line)
                {
                    case 0: return -175f;
                    case 1: return -113.5f;
                    case 2: return -65.5f;
                    case 3: return -13.5f;
                    case 4: return 36.5f;
                    case 5: return 86.5f;
                    case 6: return 136.5f;
                    case 7: return 186.5f;
                    default: return -1000f;
                }
            }
            else
            {
                switch (line)
                {
                    case 0: return 175f;
                    case 1: return -186.5f;
                    case 2: return -136.5f;
                    case 3: return -86.5f;
                    case 4: return -36.5f;
                    case 5: return 13.5f;
                    case 6: return 65.5f;
                    case 7: return 113.5f;
                    default: return -1000f;
                }
            }
        }
    }
}

