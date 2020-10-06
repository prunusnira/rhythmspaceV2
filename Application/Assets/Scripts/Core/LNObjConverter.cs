using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSCore
{
    public class LNObjConverter
    {
        // LNObj를 LNType1 형식으로 변환하는 메소드
        public void FixLongNoteLNOBJ(BMS bms)
        {
            string[] nrLane = { "11", "12", "13", "14", "15", "16", "18", "19" };
            string[] lnLane = { "51", "52", "53", "54", "55", "56", "58", "59" };
            string LNObj = bms.LNObj;
            int maxbar = bms.LastBar;

            /**
             * 최초 정의 - LINE / BAR
             * 각 라인에서 LNObj 직전에 노트가 나온 bar를 기억하기 위한 값
             */
            int[] tempBar = { -1, -1, -1, -1, -1, -1, -1, -1 };

            /**
             * 최초 정의 - note
             * 해당 LINE의 노트들, 현재 BAR는 tempBar가 관리
             */
            List<string>[] tempNote = new List<string>[8];
            for (int i = 0; i < 8; i++)
            {
                tempNote[i] = new List<string>();
            }

            /**
             * LNObj가 나왔을 때
             * 
             * 1. LNObj가 없는 bar
             * 1) 노트가 있으면 -> tempBar 갱신, tempNote 등록
             * 2) 노트가 없으면 -> 스킵
             * 
             * 1. 같은 bar에 시작 노트가 있는경우
             * 1) 같은 bar에 다수의 LNObj가 있는지 검사
             * 2) 첫 LNObj 이전에 노트가 있는지 확인
             * 3-1) 첫 LNObj 이전에 노트가 없다면 tempBar와 tempNote를 가져와서 복사
             *      복사 후 노트가 없는 부분은 모두 00으로 변환해서 5x 라인으로 이동
             * 3-2) 노트가 있다면 해당 bar 내에서 모두 해결하면 됨
             * 
             * 11 12 13 14 15 16 18 19
             */
            for (int i = 0; i < bms.LastBar; i++) // bar
            {
                for (int j = 0; j < 8; j++) // line
                {
                    if (!bms.PlayNote.ContainsKey(i)) continue;

                    string noteStr = bms.PlayNote[i][nrLane[j]];
                    List<string> currentList = GetNoteListFromString(noteStr);
                    List<int> lnPosList = new List<int>();
                    bool isLNObjExist = false;
                    bool isLNStartsFromPrev = false;
                    bool hasMoreThanTwoLNObj = false;

                    // 00이거나 노트가 없으면 넘김
                    if (currentList == null) continue;
                    
                    // list 안에서 LNObj 유무 확인
                    int lncnt = 0;

                    for (int k = 0; k < currentList.Count; k++)
                    {
                        if (currentList[k] == LNObj)
                        {
                            lncnt++;
                            isLNObjExist = true;
                        }
                    }

                    if (lncnt > 1)
                    {
                        hasMoreThanTwoLNObj = true;
                    }

                    // LN이 있는가?
                    if (isLNObjExist)
                    {
                        bool anyNoteFound = false;
                        int posBeforeLN = -1;

                        // LN이 이전 bar에서부터 시작되었는지 확인
                        for (int x = 0; x < currentList.Count; x++)
                        {
                            if (currentList[x] == "00")
                            {
                                continue;
                            }
                            else if (currentList[x] == LNObj)
                            {
                                if (!anyNoteFound)
                                {
                                    isLNStartsFromPrev = true;
                                    lnPosList.Add(x);
                                    continue;
                                }
                                else
                                {
                                    lnPosList.Add(posBeforeLN);
                                    lnPosList.Add(x);
                                    continue;
                                }
                            }
                            else
                            {
                                anyNoteFound = true;
                                posBeforeLN = x;
                            }
                        }
                    }
                    // LN이 없으면? -> 채널값에 bar와 노트 등록
                    else
                    {
                        tempBar[j] = i;
                        tempNote[j] = currentList;
                        continue;
                    }

                    // 이전 bar에서부터 노트가 이어져 온 경우
                    // 노트가 있는 이전 bar의 마지막 노트를 롱노트로 옮김
                    if (isLNStartsFromPrev)
                    {
                        int bar = tempBar[j];
                        List<string> prevstr = tempNote[j];
                        int lastidx = GetLastPosOfString(prevstr);

                        List<int> idxlist = new List<int>();
                        idxlist.Add(lastidx);

                        string lnPrevStr = bms.PlayNote[tempBar[j]][lnLane[j]];
                        List<string> lnPrevList = GetNoteListFromString(lnPrevStr);

                        string nrListReplace = GetNewNormalString(prevstr, idxlist);
                        string lnListReplace = GetNewLongNoteString(lnPrevList, prevstr, idxlist);
                        bms.PlayNote[bar][nrLane[j]] = nrListReplace;
                        bms.PlayNote[bar][lnLane[j]] = lnListReplace;
                    }

                    // 현재 노트 리스트에서 롱노트로 쓸 값들만 옮기기
                    string newNrStr = GetNewNormalString(currentList, lnPosList);
                    string newLnStr = GetNewLongNoteString(null, currentList, lnPosList);
                    bms.PlayNote[i][nrLane[j]] = newNrStr;
                    bms.PlayNote[i][lnLane[j]] = newLnStr;

                    // 롱노트를 옮긴 다음에도 단노트가 남아있으면 temp로 등록함
                    List<string> afterNoteChk = GetNoteListFromString(newNrStr);
                    if (afterNoteChk != null)
                    {
                        tempBar[j] = i;
                        tempNote[j] = afterNoteChk;
                    }
                }
            }
        }

        private List<string> GetNoteListFromString(string notestr)
        {
            if (notestr == null || notestr == "00") return null;

            List<string> notelist = new List<string>();
            for (int k = 0; k < notestr.Length / 2; k++)
            {
                notelist.Add(notestr.Substring(k * 2, 2));
            }
            return notelist;
        }

        private int GetLastPosOfString(List<string> strlist)
        {
            int lastidx = -1;
            for (int i = 0; i < strlist.Count; i++)
            {
                if (strlist[i] != "00")
                {
                    lastidx = i;
                }
            }

            return lastidx;
        }

        private string GetNewNormalString(List<string> strlist, List<int> poslist)
        {
            List<string> newStrList = strlist.ConvertAll(s => s);
            foreach (int i in poslist)
            {
                newStrList[i] = "00";
            }
            return string.Join("", newStrList);
        }

        private string GetNewLongNoteString(List<string> prevlist, List<string> strlist, List<int> poslist)
        {
            List<string> lnlist = new List<string>();
            if (prevlist == null)
            {
                // prevlist가 null이면 새로운 것만 만들어서 넘김
                for (int i = 0; i < strlist.Count; i++)
                {
                    lnlist.Add("00");
                }
            }
            else
            {
                // 기존 리스트 내용을 가져와서 거기에 업데이트를 수행
                // (기존 리스트도 사이즈는 동일하므로 거기에 그냥 업데이트만 하면 됨)
                lnlist = prevlist;
            }
            
            foreach (int i in poslist)
            {
                lnlist[i] = strlist[i];
            }
            return string.Join("", lnlist);
        }
    }
}