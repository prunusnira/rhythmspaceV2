using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BMSPlayer
{
    public class MusicDataManager
    {
        private MD5 md5;
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
            md5 = new MD5CryptoServiceProvider();
        }

        private bool isExecutorNull()
        {
            return (SQLiteMusicList.Instance == null);
        }

        public void Close()
        {
            //SQLiteMusicList.Instance.closeDB();
            md5.Clear();
        }

        public MusicListData LoadBMSFromPath(string path, int encoding)
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

                // MD5Hash 계산
                var bytehash = md5.ComputeHash(File.ReadAllBytes(path));
                string hash = BitConverter.ToString(bytehash).Replace("-", "").ToLower();

                MusicListData data = new MusicListData(
                        Data.BMS.Title,
                        Data.BMS.SubTitle,
                        Data.BMS.Artist,
                        Data.BMS.SubArtist,
                        Data.BMS.Gerne,
                        Data.BMS.BPMStart,
                        Data.BMS.BPMMin,
                        Data.BMS.BPMMax,
                        Data.BMS.FolderPath,
                        hash,
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

        public List<MusicListData> LoadBMSListWithMD5(List<string> md5list)
        {
            return SQLiteMusicList.Instance.FindBMSListWithMD5(md5list);
        }

        public List<MusicListData> LoadBMSListWithName(NameType type)
        {
            return SQLiteMusicList.Instance.FindBMSListWithName(type);
        }

        public List<string> GetPathList()
        {
            return SQLiteMusicList.Instance.GetPathList();
        }

        public void DropList()
        {
            SQLiteMusicList.Instance.DropList();
        }

        public void AddDataToDB(List<MusicListData> list, ref string strLoading)
        {
            if(list.Count > 0)
            {
                strLoading = "Registering into database";
                SQLiteMusicList.Instance.InsertBMS(list);
            }
        }

        public void RemoveDataFromDB(List<string> list, ref string strLoading, int encoding)
        {
            if (list.Count > 0)
            {
                List<KeyValuePair<string, string>> pathList = new List<KeyValuePair<string, string>>();
                // MD5 Hash를 계산해서 이 값으로 파일 삭제 처리
                for (int i = 0; i < list.Count; i++)
                {
                    // DB에 path 값이 있으면 삭제함
                    strLoading = "Gathering files to remove (" + i + "/" + list.Count + ")";

                    string dir = Path.GetDirectoryName(list[i]);
                    string file = Path.GetFileName(list[i]);

                    pathList.Add(new KeyValuePair<string, string>(dir, file));
                    // MD5Hash 계산
                    /*if(File.Exists(list[i]))
                    {
                        var bytehash = md5.ComputeHash(File.ReadAllBytes(list[i]));
                        string hash = BitConverter.ToString(bytehash).Replace("-", "").ToLower();

                        pathList.Add(hash);
                    }*/
                }
                strLoading = "Removing unnecesary files from database";
                SQLiteMusicList.Instance.DeleteBMS(pathList);
            }
        }
    }
}