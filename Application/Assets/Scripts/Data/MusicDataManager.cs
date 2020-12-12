using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return (SQLiteMusicList.Instance == null);
        }

        public void Close()
        {
            SQLiteMusicList.Instance.closeDB();
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

                bool ln = Data.NoteLong.Count > 0 ? true : false;

                // 폴더 내에 preview 이름을 가진 파일 찾기
                string[] files = Directory.EnumerateFiles(
                    Data.BMS.FolderPath, "*preview*.*", SearchOption.TopDirectoryOnly).ToArray();

                string previewFile = "";
                if (files.Length > 0)
                {
                    foreach(string f in files)
                    {
                        if (f.ToLower().EndsWith("ogg") || f.ToLower().EndsWith("wav"))
                        {
                            previewFile = f;
                            break;
                        }
                    }
                }

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
                        Data.BMS.Rank,
                        ln,
                        previewFile
                    );

                return data;
            }
            return null;
        }

        public List<MusicListData> LoadBMSFromDBOverall()
        {
            return SQLiteMusicList.Instance.SelectMusicList();
        }

        public List<MusicListData> LoadBMSFromFolder(string path)
        {
            return SQLiteMusicList.Instance.SelectMusicList(path);
        }

        public List<MusicListData> LoadBMSWithName(string text)
        {
            return SQLiteMusicList.Instance.FindBMSWithTitle(text);
        }

        public MusicListData LoadBMSWithPath(string path)
        {
            return SQLiteMusicList.Instance.FindBMSWithPath(path);
        }

        public List<MusicListData> LoadBMSListWithMD5(List<string> md5list)
        {
            return SQLiteMusicList.Instance.FindBMSListWithMD5(md5list);
        }

        public List<MusicListData> LoadBMSListWithName(NameType type)
        {
            return SQLiteMusicList.Instance.FindBMSListWithName(type);
        }

        public void DropList()
        {
            SQLiteMusicList.Instance.DropList();
        }

        public void AddDataToDB(List<MusicListData> list, ref string strLoading)
        {
            if (list.Count != 0)
            {
                List<MusicListData> dataList = new List<MusicListData>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                for (int i = 0; i < list.Count; i++)
                {
                    strLoading = "[DB Work] Gathering data for database (" + (i + 1) + "/" + list.Count + ")";
                    MusicListData d = list[i];
                    strLoading = "[DB Work] Opening file stream (" + (i + 1) + "/" + list.Count + ")";
                    FileStream fstream = File.OpenRead(d.Path + d.FileName);
                    strLoading = "[DB Work] Computing MD5 Hash (" + (i + 1) + "/" + list.Count + ")";
                    var bytehash = md5.ComputeHash(fstream);

                    strLoading = "[DB Work] Get hash string (" + (i + 1) + "/" + list.Count + ")";
                    string hash = BitConverter.ToString(bytehash).Replace("-", "").ToLower();
                    d.MD5Hash = hash;
                    dataList.Add(d);
                    fstream.Close();
                }
                strLoading = "[DB Work] Registering into database";
                SQLiteMusicList.Instance.InsertBMS(dataList);
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
                    MusicListData d = LoadBMSWithPath(list[i].Path);
                    if (d != null) dataList.Add(d.Path);
                }
                strLoading = "[DB Work] Removing unnecesary files from database";
                SQLiteMusicList.Instance.DeleteBMS(dataList);
            }
        }
    }
}