using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace BMSPlayer
{
    public class MusicDataManager
    {
        MD5 md5;

        private static MusicDataManager instance;
        public static MusicDataManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new MusicDataManager();
                }
                if(instance.isExecutorNull())
                {
                    instance.Close();
                    instance = new MusicDataManager();
                }
                return instance;
            }
        }

        private MusicDataManager()
        {
            md5 = MD5.Create();
        }

        private bool isExecutorNull()
        {
            return (SQLiteExecutor.Instance == null);
        }

        public void Close()
        {
            SQLiteExecutor.Instance.closeDB();
            md5.Clear();
        }

        public MusicListData LoadBMSFromPath(string path, int index, int encoding)
        {
            if (path != "")
            {
                PlayData Data = new PlayData(path);
                BMSAnalyzer analyzer = new BMSAnalyzer();
                analyzer.FullAnalyzer(Data.BMS, encoding);
                if (Data.BMS.Player != 1) return null;

                if (Data.BMS.LNType == LNType.Obj)
                {
                    LNObjConverter LNConverter = new LNObjConverter();
                    LNConverter.FixLongNoteLNOBJ(Data.BMS);
                }

                NoteGenerator generator = new NoteGenerator();
                generator.AnalyzeNotes(Data, new int[] { 1, 2, 3, 4, 5, 6, 7 });
                generator.PositionToTiming(Data);

                MusicListData data = new MusicListData(
                        index,
                        Data.BMS.Title,
                        Data.BMS.SubTitle,
                        Data.BMS.Artist,
                        Data.BMS.SubArtist,
                        Data.BMS.Gerne,
                        Data.BMS.BPMStart,
                        Data.BMS.BPMMin,
                        Data.BMS.BPMMax,
                        Data.BMS.FolderPath,
                        Data.BMS.Level,
                        Data.BMS.Difficulty,
                        Data.BMS.FileName,
                        Data.BMS.StageFile,
                        Data.TotalNotes,
                        (int)Data.LastTiming,
                        Data.BMS.Rank
                    );

                return data;
            }
            return null;
        }

        public List<MusicListData> LoadBMSFromDBOverall()
        {
            return SQLiteExecutor.Instance.SelectMusicList();
        }

        public List<MusicListData> LoadBMSFromFolder(string path)
        {
            return SQLiteExecutor.Instance.SelectMusicList(path);
        }

        public List<MusicListData> FindBMSWithName(string text)
        {
            return SQLiteExecutor.Instance.FindBMSWithTitle(text);
        }

        public MusicListData FindBMSWithPath(string path)
        {
            return SQLiteExecutor.Instance.FindBMSWithPath(path);
        }

        public void DropList()
        {
            SQLiteExecutor.Instance.DropList();
        }

        public void AddDataToDB(List<MusicListData> list, ref string strLoading)
        {
            if (list.Count != 0)
            {
                List<MusicListData> dataList = new List<MusicListData>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                for (int i = 0; i < list.Count; i++)
                {
                    strLoading = "Gathering data to register to database (" + (i + 1) + "/" + list.Count + ")";
                    MusicListData d = list[i];
                    FileStream fstream = File.OpenRead(d.Path + d.FileName);
                    var bytehash = md5.ComputeHash(fstream);
                    fstream.Close();

                    string hash = BitConverter.ToString(bytehash).Replace("-", "").ToLower();
                    d.MD5Hash = hash;
                    dataList.Add(d);
                    fstream.Close();
                }
                strLoading = "Registering into database";
                SQLiteExecutor.Instance.InsertBMS(dataList);
            }
        }

        public void RemoveDataFromDB(List<MusicListData> list, ref string strLoading)
        {
            if (list.Count != 0)
            {
                List<string> dataList = new List<string>();
                // MD5 Hash를 계산해서 이 값으로 파일 삭제 처리
                for (int i = 0; i < list.Count; i++)
                {
                    // DB에 path 값이 있으면 삭제함
                    MusicListData d = FindBMSWithPath(list[i].Path);
                    if (d != null) dataList.Add(d.Path);
                }
                strLoading = "Removing unnecesary files from database";
                SQLiteExecutor.Instance.DeleteBMS(dataList);
            }
        }
    }
}