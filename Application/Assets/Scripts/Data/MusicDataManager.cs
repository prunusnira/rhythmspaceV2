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
        SQLiteExecutor executor;
        MD5 md5;

        public MusicListManager()
        {
            executor = SQLiteExecutor.getInstance();
            md5 = MD5.Create();
        }

        public void Close()
        {
            executor.closeDB();
            md5.Clear();
        }

        public MusicListData LoadBMSFromFolder(string path, int index)
        {
            if (path != "")
            {
                BMS bms = new BMS(path);
                BMSAnalyzer analyzer = new BMSAnalyzer();
                analyzer.HeaderAnalyzer(bms);
                if (bms.Player != 1) return null;

                MusicListData data = new MusicListData(
                        index, bms.Title, bms.SubTitle,
                        bms.Artist, bms.SubArtist, bms.Gerne,
                        bms.BPMStart, bms.BPMMin, bms.BPMMax,
                        bms.FolderPath, bms.Level, bms.Difficulty,
                        bms.FileName, bms.StageFile
                    );

                return data;
            }
            return null;
        }

        public List<MusicListData> LoadBMSFromDBOverall()
        {
            return executor.SelectMusicList();
        }

        public List<MusicListData> LoadBMSFromFolder(string path)
        {
            return executor.SelectMusicList(path);
        }

        public void AddDataToDB(List<MusicListData> list)
        {
            if (list.Count != 0)
            {
                executor.DropList();
                List<string[]> paramList = new List<string[]>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                foreach (MusicListData d in list)
                {
                    FileStream fstream = File.OpenRead(d.Path + d.FileName);
                    var bytehash = md5.ComputeHash(fstream);
                    fstream.Close();

                    string hash = BitConverter.ToString(bytehash);

                    string[] param =
                    {
                        d.Title, d.SubTitle, d.Artist, d.SubArtist, d.Gerne,
                        d.BPMstart.ToString(), d.BPMmin.ToString(), d.BPMmax.ToString(),
                        d.Path, hash, d.Level.ToString(), d.Difficulty.ToString(),
                        d.FileName, d.Jacket
                    };
                    paramList.Add(param);
                    fstream.Close();
                }
                executor.InsertBMS(paramList);
            }
        }
    }
}