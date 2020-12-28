using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BMSPlayer
{
    class SkinLoader
    {
        private static string SkinPath = Application.streamingAssetsPath + "/Skin";
        private static string GearPath = Application.streamingAssetsPath + "/Gear";
        private static string NotePath = Application.streamingAssetsPath + "/Note";

        public static Dictionary<string, string> SkinList = new Dictionary<string, string>();
        public static Dictionary<string, string> SkinData = new Dictionary<string, string>();

        public static Dictionary<string, string> GearList = new Dictionary<string, string>();
        public static Dictionary<string, string> GearData = new Dictionary<string, string>();

        public static Dictionary<string, string> NoteList = new Dictionary<string, string>();
        public static Dictionary<string, string> NoteData = new Dictionary<string, string>();

        // 게임이 처음 켜질 때 수행함
        public static void CollectSkinList()
        {
            // 판단 방법
            // Streaming Assets의 Skin 폴더 하위의 각 폴더 목록을 가져와서
            // 최상위 폴더의 skindata.cnf 파일을 읽어서
            // SkinSetName 값이 있는지 확인
            SkinList.Clear();
            GearList.Clear();
            NoteList.Clear();

            foreach(string p in Directory.EnumerateDirectories(SkinPath, "*", SearchOption.TopDirectoryOnly))
            {
                string path = p.Replace("\\", "/");
                string cnfPath = path + "/skindata.cnf";

                FileStream stream = File.OpenRead(cnfPath);
                StreamReader reader = new StreamReader(stream);
                while(true)
                {
                    string line = reader.ReadLine();
                    if(line == null)
                    {
                        break;
                    }
                    else
                    {
                        if(line.StartsWith("SkinSetName"))
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            if(pair.Length == 2)
                            {
                                SkinList.Add(pair[1], path);
                            }
                            break;
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }

            foreach (string p in Directory.EnumerateDirectories(GearPath, "*", SearchOption.TopDirectoryOnly))
            {
                string path = p.Replace("\\", "/");
                string cnfPath = path + "/geardata.cnf";

                FileStream stream = File.OpenRead(cnfPath);
                StreamReader reader = new StreamReader(stream);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if (line.StartsWith("GearSetName"))
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            if (pair.Length == 2)
                            {
                                GearList.Add(pair[1], path);
                            }
                            break;
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }

            foreach (string p in Directory.EnumerateDirectories(NotePath, "*", SearchOption.TopDirectoryOnly))
            {
                string path = p.Replace("\\", "/");
                string cnfPath = path + "/notedata.cnf";

                FileStream stream = File.OpenRead(cnfPath);
                StreamReader reader = new StreamReader(stream);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if (line.StartsWith("NoteSetName"))
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            if (pair.Length == 2)
                            {
                                NoteList.Add(pair[1], path);
                            }
                            break;
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }
        }

        // 경로 불러오기
        public static void LoadSkinPath()
        {
            // 정해진 스킨 이름에 해당하는 파일로 열기
            if(SkinList.ContainsKey(Const.SkinName))
            {
                SkinData.Clear();
                string path = SkinList[Const.SkinName];

                SkinSetting.SkinSetMainPath = path;

                // 파일을 열어서 각 항목 불러오기
                FileStream stream = File.OpenRead(path + "/skindata.cnf");
                StreamReader reader = new StreamReader(stream);
                while(true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if(!line.StartsWith("#") && line.Length > 0)
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            SkinData.Add(pair[0], pair[1]);
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }

            if (GearList.ContainsKey(Const.GearName))
            {
                GearData.Clear();
                string path = GearList[Const.GearName];

                SkinSetting.GearSetMainPath = path;

                // 파일을 열어서 각 항목 불러오기
                FileStream stream = File.OpenRead(path + "/geardata.cnf");
                StreamReader reader = new StreamReader(stream);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if (!line.StartsWith("#") && line.Length > 0)
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            GearData.Add(pair[0], pair[1]);
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }

            if (NoteList.ContainsKey(Const.NoteName))
            {
                NoteData.Clear();
                string path = NoteList[Const.NoteName];

                SkinSetting.NoteSetMainPath = path;

                // 파일을 열어서 각 항목 불러오기
                FileStream stream = File.OpenRead(path + "/notedata.cnf");
                StreamReader reader = new StreamReader(stream);
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if (!line.StartsWith("#") && line.Length > 0)
                        {
                            string[] pair = line.Split(new string[] { "=" }, StringSplitOptions.None);
                            NoteData.Add(pair[0], pair[1]);
                        }
                    }
                }
                reader.Close();
                stream.Close();
            }

            SkinSetting.IsSkinLoaded = true;
        }
    }
}
