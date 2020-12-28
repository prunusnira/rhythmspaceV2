using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class NoteObjectAdder : MonoBehaviour
    {
        [Header("Split line")]
        public GameObject[] split;

        [Header("Note Prefab")]
        public GameObject noteWhite;
        public GameObject noteWhiteLn;
        public GameObject noteBlue;
        public GameObject noteBlueLn;
        public GameObject noteTurn;
        public GameObject noteTurnLn;
        public GameObject noteTurnAuto;
        public GameObject noteMine;
        private GameObject splitLine;

        [Header("Parent Objects")]
        public GameObject[] noteParentObjNr;
        public GameObject[] noteParentObjW125;
        public GameObject[] noteParentObjW150;
        private GameObject noteParent;

        public void Awake()
        {
            // 노트 종류 변경
            splitLine = split[(int)Const.GearSize];

            // 노트 사이즈 변경
            float widthScaleTT = 73;
            float widthScaleW = 50;
            float widthScaleB = 50;
            float heightScale = 1;
            switch (Const.GearSize)
            {
                case SkinSize.STANDARD:
                    noteParent = noteParentObjNr[Const.PlayerSide];
                    break;
                case SkinSize.WIDE125:
                    noteParent = noteParentObjW125[Const.PlayerSide];
                    widthScaleTT = 91f / 73;
                    widthScaleW = 63f / 50;
                    widthScaleB = 62f / 50;
                    break;
                case SkinSize.WIDE150:
                    noteParent = noteParentObjW150[Const.PlayerSide];
                    widthScaleTT = 110f / 73;
                    widthScaleW = 75f / 50;
                    widthScaleB = 75f / 50;
                    break;
            }

            switch(Const.NoteSize)
            {
                case NoteSize.SUPERSLIM:
                    heightScale = 0.3f;
                    break;
                case NoteSize.SLIM:
                    heightScale = 0.6f;
                    break;
                case NoteSize.NORMAL:
                    break;
                case NoteSize.THICK:
                    heightScale = 1.3f;
                    break;
                case NoteSize.SUPERTHICK:
                    heightScale = 1.6f;
                    break;
            }
            Const.SplitLineSizeCalc = heightScale * 15;
            Const.LnHeightCalc = 15f;

            noteWhite.transform.localScale = new Vector3(widthScaleW, heightScale, 1);
            noteWhiteLn.transform.localScale = new Vector3(widthScaleW, heightScale, 1);
            noteBlue.transform.localScale = new Vector3(widthScaleB, heightScale, 1);
            noteBlueLn.transform.localScale = new Vector3(widthScaleB, heightScale, 1);
            noteTurn.transform.localScale = new Vector3(widthScaleTT, heightScale, 1);
            noteTurnLn.transform.localScale = new Vector3(widthScaleTT, heightScale, 1);
            noteTurnAuto.transform.localScale = new Vector3(widthScaleTT, heightScale, 1);
            noteMine.transform.localScale = new Vector3(widthScaleB, heightScale, 1);
        }

        public void DisplaySplitLine(SplitLine line)
        {
            GameObject noteObj = AddNewSplitLine(line.Timing, noteParent.transform);
            noteObj.transform.SetParent(noteParent.transform, false);
            line.OnScreen = true;
            line.NoteObject = noteObj;
        }

        public void DisplayMineNote(MineNote note)
        {
            GameObject noteObj = AddNewMineNote(note.Line, note.Timing, noteParent.transform);
            noteObj.transform.SetParent(noteParent.transform, false);
            note.OnScreen = true;
            note.NoteObject = noteObj;
        }

        public void DisplayPlayNote(PlayNote note, List<LongNote> lnlist)
        {
            if (note.PlayNoteType == NoteType.SINGLE)
            {
                GameObject noteObj = AddNewNote(note.Line, note.Timing, noteParent.transform, note.PlayNoteType);
                noteObj.transform.SetParent(noteParent.transform, false);
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
                    GameObject noteObj = AddNewNote(note.Line, note.Timing, noteParent.transform, note.PlayNoteType);
                    noteObj.transform.SetParent(noteParent.transform, false);
                    note.OnScreen = true;
                    note.NoteObject = noteObj;

                    // 끝노트도 같이 추가한다
                    for (int i = 0; i < lnlist.Count; i++)
                    {
                        if (lnlist[i].Start == note)
                        {
                            // 가운데노트
                            PlayNote lnNote = lnlist[i].Mid;
                            GameObject lnObj = AddNewNote(lnNote.Line, lnNote.Timing, noteParent.transform, note.PlayNoteType);
                            lnObj.transform.SetParent(noteParent.transform, false);
                            lnNote.OnScreen = true;
                            lnNote.NoteObject = lnObj;

                            // 끝노트
                            PlayNote endNote = lnlist[i].End;
                            GameObject endObj = AddNewNote(endNote.Line, endNote.Timing, noteParent.transform, note.PlayNoteType);
                            endObj.transform.SetParent(noteParent.transform, false);
                            endNote.OnScreen = true;
                            endNote.NoteObject = endObj;

                            lnlist[i].Mid.Position = (note.Position + endNote.Position) / 2;
                        }
                    }
                }
            }
        }

        public GameObject AddNewNote(int clane, double timing, Transform parent, NoteType noteType)
        {
            /**
             * lanes 중 clane에 노트 오브젝트르 생성함
             * 노트 배치: 흰파흰흰파흰
             * 
             * 노트를 추가하는 과정에서는 bpm을 전혀 고려하지 않고 있는 그대로 노트를 배치해야 변속 시 스크롤 속도를 조절할 수 있다.
             */
            GameObject noteObject = null;

            Vector3 pos = new Vector3(GetXPos(clane), (float)timing, -1);

            switch (clane)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                    if (noteType == NoteType.LNSTART || noteType == NoteType.LNEND || noteType == NoteType.LNMID)
                        noteObject = Instantiate(noteWhiteLn, pos, Quaternion.identity, parent);
                    else
                        noteObject = Instantiate(noteWhite, pos, Quaternion.identity, parent);

                    noteObject.transform.localPosition = pos;
                    break;
                case 2:
                case 4:
                case 6:
                    if (noteType == NoteType.LNSTART || noteType == NoteType.LNEND || noteType == NoteType.LNMID)
                        noteObject = Instantiate(noteBlueLn, pos, Quaternion.identity, parent);
                    else
                        noteObject = Instantiate(noteBlue, pos, Quaternion.identity, parent);

                    noteObject.transform.localPosition = pos;
                    break;
                case 0:
                    if (Const.Auto == AutoPlayType.TURNTABLE)
                    {
                        noteObject = Instantiate(noteTurnAuto, pos, Quaternion.identity, parent);
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteScrAuto;
                    }
                    else {
                        if (noteType == NoteType.LNSTART || noteType == NoteType.LNEND || noteType == NoteType.LNMID)
                        {
                            noteObject = Instantiate(noteTurnLn, pos, Quaternion.identity, parent);
                            noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteScrLn;
                        }
                        else
                        {
                            noteObject = Instantiate(noteTurn, pos, Quaternion.identity, parent);
                            noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteScr;
                        }
                    }
                    noteObject.transform.localPosition = pos;
                    break;
            }

            switch (clane)
            {
                case 1:
                    if(noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn1;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn1Ln;
                    }
                    break;
                case 2:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn2;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn2Ln;
                    }
                    break;
                case 3:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn3;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn3Ln;
                    }
                    break;
                case 4:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn4;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn4Ln;
                    }
                    break;
                case 5:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn5;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn5Ln;
                    }
                    break;
                case 6:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn6;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn6Ln;
                    }
                    break;
                case 7:
                    if (noteType == NoteType.SINGLE)
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn7;
                    }
                    else
                    {
                        noteObject.GetComponent<SpriteRenderer>().sprite = SkinSetting.NoteBtn7Ln;
                    }
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

            noteObject = Instantiate(noteMine, pos, Quaternion.identity, parent);
            noteObject.transform.localPosition = pos;

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

            noteObject = Instantiate(splitLine, pos, Quaternion.identity, parent);
            noteObject.transform.localPosition = pos;

            return noteObject;
        }

        public float GetXPos(int line)
        {
            if (Const.PlayerSide == 0)
            {
                switch(Const.GearSize)
                {
                    case SkinSize.STANDARD:
                        switch (line)
                        {
                            case 0: return -175f;
                            case 1: return -113.5f;
                            case 2: return -63.5f;
                            case 3: return -13.5f;
                            case 4: return 36.5f;
                            case 5: return 86.5f;
                            case 6: return 136.5f;
                            case 7: return 186.5f;
                            default: return 10000f;
                        }
                    case SkinSize.WIDE125:
                        switch (line)
                        {
                            case 0: return -219;
                            case 1: return -142f;
                            case 2: return -79.5f;
                            case 3: return -17f;
                            case 4: return 45.5f;
                            case 5: return 108f;
                            case 6: return 170.5f;
                            case 7: return 233f;
                            default: return 10000f;
                        }
                    case SkinSize.WIDE150:
                        switch (line)
                        {
                            case 0: return -262.5f;
                            case 1: return -170f;
                            case 2: return -95f;
                            case 3: return -20f;
                            case 4: return 55f;
                            case 5: return 130f;
                            case 6: return 205f;
                            case 7: return 280f;
                            default: return 10000f;
                        }
                }
            }
            else
            {
                switch (Const.GearSize)
                {
                    case SkinSize.STANDARD:
                        switch (line)
                        {
                            case 0: return 175f;
                            case 1: return -186.5f;
                            case 2: return -136.5f;
                            case 3: return -86.5f;
                            case 4: return -36.5f;
                            case 5: return 13.5f;
                            case 6: return 63.5f;
                            case 7: return 113.5f;
                            default: return 10000f;
                        }
                    case SkinSize.WIDE125:
                        switch (line)
                        {
                            case 0: return 219f;
                            case 1: return -233f;
                            case 2: return -170.5f;
                            case 3: return -108f;
                            case 4: return -45.5f;
                            case 5: return 17f;
                            case 6: return 79.5f;
                            case 7: return 142f;
                            default: return 10000f;
                        }
                    case SkinSize.WIDE150:
                        switch (line)
                        {
                            case 0: return 262.5f;
                            case 1: return -280f;
                            case 2: return -205f;
                            case 3: return -130f;
                            case 4: return -55f;
                            case 5: return 20f;
                            case 6: return 95f;
                            case 7: return 170f;
                            default: return 10000f;
                        }
                }
            }
            return 10000f;
        }
    }
}

