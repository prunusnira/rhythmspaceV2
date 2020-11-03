using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BMSPlayer
{
    public class MusicListManager
    {
        MD5 md5;

        private static MusicListManager instance;
        public static MusicListManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new MusicListManager();
                }
                if(instance.isExecutorNull())
                {
                    instance.Close();
                    instance = new MusicListManager();
                }
                return instance;
            }
        }

        private MusicListManager()
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
                        Convert.ToInt32(Data.LastTiming)
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
            return SQLiteExecutor.Instance.FindMusicList(text);
        }

        public void AddDataToDB(List<MusicListData> list)
        {
            if (list.Count != 0)
            {
                SQLiteExecutor.Instance.DropList();
                List<MusicListData> dataList = new List<MusicListData>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                foreach (MusicListData d in list)
                {
                    FileStream fstream = File.OpenRead(d.Path + d.FileName);
                    var bytehash = md5.ComputeHash(fstream);
                    fstream.Close();

                    string hash = BitConverter.ToString(bytehash);
                    d.MD5Hash = hash;
                    dataList.Add(d);
                    fstream.Close();
                }
                SQLiteExecutor.Instance.InsertBMS(dataList);
            }
        }
    }
}