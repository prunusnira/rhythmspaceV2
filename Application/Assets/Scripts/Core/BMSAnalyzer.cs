﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BMSCore
{
    public class BMSAnalyzer
    {
        private bool isVideoExist = false;
        private bool isRandom = false;

        // 노래 선택창에서 선택한 BMS 파일의 헤더를 분석
        public void HeaderAnalyzer(BMS bms, int encoding)
        {
            FileStream bmsFileStream = File.OpenRead(bms.FilePath);
            Encoding charset = Encoding.GetEncoding(encoding);
            StreamReader bmsReader = new StreamReader(bmsFileStream, charset);
            
            // 한 줄 씩 읽으면서 분석
            string buf = null;
            while ((buf = bmsReader.ReadLine()) != null)
            {
                try
                {
                    string[] line = buf.Split(' ');
                    if (line.Length != 0 && line[0].StartsWith("#"))
                    {
                        // divide tag and other info
                        string tag = line[0].ToUpper();
                        string left = "";
                        string etc;
                        for (int i = 1; i < line.Length; i++)
                        {
                            etc = line[i];
                            if (left.Length != 0)
                            {
                                left += " ";
                            }
                            left += etc;
                        }

                        string chkWav = "";
                        if (tag.Length > 4)
                        {
                            chkWav = tag.Substring(0, 4).ToUpper();
                        }

                        // For BPM Check
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        else if (tag.ToUpper() == "#PLAYER")
                        {
                            bms.Player = int.Parse(left);
                        }
                        else if (tag.ToUpper() == "#GENRE")
                        {
                            bms.Gerne = left;
                        }
                        else if (tag.ToUpper() == "#TITLE")
                        {
                            bms.Title = left;
                        }
                        else if (tag.ToUpper() == "#SUBTITLE")
                        {
                            bms.SubTitle = left;
                        }
                        else if (tag.ToUpper() == "#ARTIST")
                        {
                            bms.Artist = left;
                        }
                        else if (tag.ToUpper() == "#SUBARTIST")
                        {
                            bms.SubArtist = left;
                        }
                        else if(tag.ToUpper() == "#DIFFICULTY")
                        {
                            int diff = 2;
                            bool parse = int.TryParse(left, out diff);
                            bms.Difficulty = diff;
                        }
                        else if (tag.ToUpper() == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.BPMStart = bpm;
                            bms.BPMMin = bpm;
                            bms.BPMMax = bpm;
                        }
                        else if (tag.ToUpper() == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.Level = level;
                        }
                        else if (tag.ToUpper() == "#RANK")
                        {
                            bms.Rank = int.Parse(left);
                        }
                        else if (tag.ToUpper() == "#TOTAL")
                        {
                            int total = 0;
                            int.TryParse(left, out total);
                            bms.TotalValue = total;
                        }
                        else if (tag.ToUpper() == "#STAGEFILE")
                        {
                            bms.StageFile = left;
                        }
                        else if (tag.ToUpper() != "#BPM" && chkWav.ToUpper() == "#BPM")
                        {
                            // 16진수로 표현 불가능 한 BPM 표현
                            double bpmC = 0d;
                            bool trial = double.TryParse(left, out bpmC);
                            GetBPMInfoNum(bpmC, ref bms);
                        }

                        // BPM CHECK
                        else if (int.TryParse(tag.Substring(1, 1), out parsedTag))
                        {
                            // 이외의 경우는 tag가 모두 노트 데이터
                            string noteBuf;
                            string[] noteTok = tag.Split(':');

                            noteBuf = noteTok[0];

                            string leftNote = "";
                            leftNote += noteTok[1];

                            int bar = int.Parse(noteBuf.Substring(1, 3));
                            string ch = noteBuf.Substring(4, 2);
                            if (ch == "03" && leftNote != "00")
                            {
                                // BPM
                                GetBPMInfo(leftNote, ref bms);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /*ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("File: " + bms.FilePath);
                    ErrorHandler.LogError("Line: " + buf);
                    ErrorHandler.LogError(e.Message+ " " +e.StackTrace);*/
                    continue;
                }
            }
            bmsReader.Close();
            bmsFileStream.Close();
            
        }

        public void FullAnalyzer(BMS bms, int encoding)
        {
            int ifCount = 0;
            int randSize = 0;

            // input stream 열기 // 기본 Default, 일본어 932, 한국어 949
            //StreamReader bmsReader = new StreamReader(bms.FilePath, System.Text.Encoding.GetEncoding(encoding));
            FileStream bmsFileStream = File.OpenRead(bms.FilePath);
            Encoding charset = Encoding.GetEncoding(encoding);
            StreamReader bmsReader = new StreamReader(bmsFileStream, charset);
            
            // 한 줄 씩 읽으면서 분석
            string buf = null;
            while ((buf = bmsReader.ReadLine()) != null)
            {
                try
                {
                    string[] line = buf.Split(' ');
                    if (line.Length != 0 && line[0].StartsWith("#"))
                    {
                        // divide tag and other info
                        string tag = line[0].ToUpper();
                        string left = "";
                        string etc;
                        for (int i = 1; i < line.Length; i++)
                        {
                            etc = line[i];
                            if (left.Length != 0)
                            {
                                left += " ";
                            }
                            left += etc;
                        }

                        string chkWav = "";
                        if(tag.Length > 4)
                        {
                            chkWav = tag.Substring(0, 4).ToUpper();
                        }
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        if (tag.ToUpper() == "#PLAYER")
                        {
                            bms.Player = int.Parse(left);
                        }
                        else if (tag.ToUpper() == "#GENRE")
                        {
                            bms.Gerne = left;
                        }
                        else if (tag.ToUpper() == "#TITLE")
                        {
                            bms.Title = left;
                        }
                        else if (tag.ToUpper() == "#SUBTITLE")
                        {
                            bms.SubTitle = left;
                        }
                        else if (tag.ToUpper() == "#ARTIST")
                        {
                            bms.Artist = left;
                        }
                        else if (tag.ToUpper() == "#SUBARTIST")
                        {
                            bms.SubArtist = left;
                        }
                        else if (tag.ToUpper() == "#DIFFICULTY")
                        {
                            bms.Difficulty = int.Parse(left);
                        }
                        else if (tag.ToUpper() == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.BPMStart = bpm;
                            bms.BPMMin = bpm;
                            bms.BPMMax = bpm;
                        }
                        else if (tag.ToUpper() == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.Level = level;
                        }
                        else if (tag.ToUpper() == "#RANK")
                        {
                            bms.Rank = int.Parse(left);
                        }
                        else if (tag.ToUpper() == "#TOTAL")
                        {
                            int total;
                            int.TryParse(left, out total);
                            bms.TotalValue = total;
                        }
                        else if (tag.ToUpper() == "#STAGEFILE")
                        {
                            bms.StageFile = left;
                        }
                        // Long Note Type Check
                        else if (tag.ToUpper() == "#LNTYPE")
                        {
                            switch (int.Parse(left))
                            {
                                case 1: bms.LNType = LNType.Type1; break;
                                case 2: bms.LNType = LNType.Type2; break;
                            }
                        }
                        else if (tag.ToUpper() == "#LNOBJ")
                        {
                            bms.LNType = LNType.Obj;
                            bms.LNObj = left;
                        }
                        else if(tag.ToUpper() == "#RANDOM")
                        {
                            isRandom = true;
                            bms.Random.Add(buf);

                            bool test = false;
                            test = int.TryParse(left, out randSize);
                        }
                        else if (tag.ToUpper() == "#IF")
                        {
                            // 랜덤 내부가 아니면 무시
                            if(isRandom)
                            {
                                bms.Random.Add(buf);
                            }
                        }
                        else if (tag.ToUpper() == "#ENDIF")
                        {
                            // 랜덤 내부가 아니면 무시
                            if (isRandom)
                            {
                                bms.Random.Add(buf);

                                ifCount++;

                                if (ifCount == randSize)
                                {
                                    isRandom = false;
                                    randSize = 0;
                                    ifCount = 0;
                                }
                            }
                        }
                        else if (tag.ToUpper() == "#ENDRANDOM")
                        {
                            isRandom = false;
                            bms.Random.Add(buf);
                        }

                        else if (chkWav.ToUpper() == "#WAV")
                        {
                            string filename = "";
                            string[] name = left.Split('.');
                            for (int i = 0; i < name.Length - 1; i++)
                            {
                                filename += name[i] + '.';
                            }

                            // 각각의 파일 확장자에 대해서 직접 검사
                            if(File.Exists(bms.FolderPath + filename + "wav"))
                            {
                                filename = filename + "wav";
                            }
                            else if (File.Exists(bms.FolderPath + filename + "WAV"))
                            {
                                filename = filename + "WAV";
                            }
                            else if(File.Exists(bms.FolderPath + filename + "ogg"))
                            {
                                filename = filename + "ogg";
                            }
                            else if (File.Exists(bms.FolderPath + filename + "OGG"))
                            {
                                filename = filename + "OGG";
                            }
                            else if (File.Exists(bms.FolderPath + filename + "mp3"))
                            {
                                filename = filename + "mp3";
                            }
                            else if (File.Exists(bms.FolderPath + filename + "MP3"))
                            {
                                filename = filename + "MP3";
                            }

                            bms.WavList.Add(tag.Substring(4, 2), filename);
                        }

                        else if (chkWav == "#BMP")
                        {
                            if (left.ToLower().EndsWith(".mpg") ||
                                left.ToLower().EndsWith(".mpeg") ||
                                left.ToLower().EndsWith(".mp4") ||
                                left.ToLower().EndsWith(".wmv") ||
                                left.ToLower().EndsWith(".avi"))
                            {
                                bms.BGAVideoFile = bms.FolderPath + left;
                                isVideoExist = true;
                            }
                            else if (left.ToLower().EndsWith(".bmp") ||
                                left.ToLower().EndsWith(".jpg") ||
                                left.ToLower().EndsWith(".jpeg") ||
                                left.ToLower().EndsWith(".png"))
                            {
                                // 파일 확장자 처리부터 수행
                                string[] filename = left.Split('.');
                                string filenameCheck = "";
                                for(int i = 0; i < filename.Length - 1; i++)
                                {
                                    filenameCheck += filename[i] + ".";
                                }

                                bool isFileExist = false;
                                if (File.Exists(bms.FolderPath + filenameCheck + "bmp"))
                                {
                                    filenameCheck += "bmp";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "BMP"))
                                {
                                    filenameCheck += "BMP";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "jpg"))
                                {
                                    filenameCheck += "jpg";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "JPG"))
                                {
                                    filenameCheck += "JPG";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "jpeg"))
                                {
                                    filenameCheck += "jpeg";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "JPEG"))
                                {
                                    filenameCheck += "JPEG";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "png"))
                                {
                                    filenameCheck += "png";
                                    isFileExist = true;
                                }
                                else if (File.Exists(bms.FolderPath + filenameCheck + "PNG"))
                                {
                                    filenameCheck += "PNG";
                                    isFileExist = true;
                                }

                                if (isFileExist)
                                    bms.BGAPaths.Add(tag.Substring(4, 2), bms.FolderPath + filenameCheck);
                            }
                        }
                        else if (tag.ToUpper() != "#BPM" && chkWav.ToUpper() == "#BPM")
                        {
                            // 16진수로 표현 불가능 한 BPM 표현
                            double bpmC = 0d;
                            bool trial = double.TryParse(left, out bpmC);
                            GetBPMInfoNum(bpmC, ref bms);

                            // 등록 필요
                            bms.BPMNum.Add(tag.Substring(4, 2), bpmC);
                        }
                        else if (chkWav.ToUpper() == "#STO")
                        {
                            int stime = 0;
                            int.TryParse(left, out stime);
                            bms.StopList.Add(tag.Substring(5, 2), stime);
                        }
                        // 문자인 경우는 제외해야 함
                        else if (int.TryParse(tag.Substring(1, 1), out parsedTag))
                        {
                            // 이외의 경우는 tag가 모두 노트 데이터
                            string noteBuf;
                            string[] noteTok = tag.Split(':');

                            noteBuf = noteTok[0];

                            string leftNote = "";
                            leftNote += noteTok[1];

                            int bar = int.Parse(noteBuf.Substring(1, 3));
                            string ch = noteBuf.Substring(4, 2);

                            if(isRandom)
                            {
                                bms.Random.Add(buf);
                            }
                            else
                            {
                                AddChannels(bms, ch, leftNote, bar);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    /*ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("File: " + bms.FilePath);
                    ErrorHandler.LogError("Line: " + buf);
                    ErrorHandler.LogError(e.Message + " " + e.StackTrace);*/
                    continue;
                }
            }
            bmsReader.Close();

            // 랜덤 처리
            AddRandomNotes(bms);

            // 롱노트 예외 처리
            // 극히 일부 곡에서 LNOBJ와 LNTYPE1을 같이 쓰는경우가 있음
            // 그냥 LNOBJ를 무시해버리자
            if (CheckIfLNExist(bms)) bms.LNType = LNType.Type1;
        }        

        public bool IsVideoExist()
        {
            return isVideoExist;
        }

        public void GetBPMInfo(string notestr, ref BMS bms)
        {
            int bpmsize = notestr.Length / 2;
            for (int n = 0; n < bpmsize; n++)
            {
                string str = notestr.Substring(n * 2, 2);
                if (str != "00")
                {
                    double bpm = Convert.ToInt32(str, 16);
                    if(bpm < bms.BPMMin)
                    {
                        bms.BPMMin = bpm;
                    }
                    if(bpm > bms.BPMMax)
                    {
                        bms.BPMMax = bpm;
                    }
                }
            }
        }

        public void GetBPMInfoNum(double bpm, ref BMS bms)
        {
            if (bpm < bms.BPMMin)
            {
                bms.BPMMin = bpm;
            }
            if (bpm > bms.BPMMax)
            {
                bms.BPMMax = bpm;
            }
        }

        // 각 채널 등록 처리
        private void AddChannels(
            BMS bms, string ch, string leftNote, int bar)
        {
            try
            {
                if (ch == "01")
                {
                    if (leftNote != "00")
                    {
                        // 베이스 음악 (MUSIC)
                        List<string> mlist;
                        if (bms.Music.ContainsKey(bar))
                        {
                            mlist = bms.Music[bar];
                            bms.Music.Remove(bar);
                        }
                        else
                        {
                            mlist = new List<string>();
                        }

                        mlist.Add(leftNote);

                        bms.Music.Add(bar, mlist);
                    }
                }
                else if (ch == "02")
                {
                    bms.BarLength.Add(bar, double.Parse(leftNote));
                }
                else if (ch == "03" && leftNote != "00")
                {   // BPM
                    bms.BPMNote.Add(bar, leftNote);
                    GetBPMInfo(leftNote, ref bms);
                }
                else if (ch == "04")
                {
                    // BGA
                    bms.BGANote.Add(bar, leftNote);
                }
                else if (ch == "06")
                {
                    // POOR IMG
                }
                else if (ch == "07")
                {
                    // LAYER IMG
                    bms.LayerNote.Add(bar, leftNote);
                }
                else if (ch == "08" && leftNote != "00")
                {
                    // BPM
                    bms.BPMNoteType2.Add(bar, leftNote);
                }
                else if (ch == "09" && leftNote != "00")
                {
                    // Stop
                    bms.StopNote.Add(bar, leftNote);
                }
                else if (ch.StartsWith("D") || ch.StartsWith("E"))
                {
                    if (bms.LastBar < bar) bms.LastBar = bar;

                    // bar 데이터 유무 체크
                    if (!bms.MineNote.ContainsKey(bar))
                        bms.MineNote[bar] = new Dictionary<string, string>();

                    // bar에 키값 있는지 여부 확인
                    if (bms.MineNote[bar].ContainsKey(ch) && bms.MineNote[bar][ch] == null)
                    {
                        bms.MineNote[bar].Remove(ch);
                        bms.MineNote[bar].Add(ch, leftNote);
                    }
                    else
                    {
                        bms.MineNote[bar].Add(ch, leftNote);
                    }

                    if (!bms.MineNote[bar].ContainsKey("D1")) bms.MineNote[bar].Add("D1", null);
                    if (!bms.MineNote[bar].ContainsKey("D2")) bms.MineNote[bar].Add("D2", null);
                    if (!bms.MineNote[bar].ContainsKey("D3")) bms.MineNote[bar].Add("D3", null);
                    if (!bms.MineNote[bar].ContainsKey("D4")) bms.MineNote[bar].Add("D4", null);
                    if (!bms.MineNote[bar].ContainsKey("D5")) bms.MineNote[bar].Add("D5", null);
                    if (!bms.MineNote[bar].ContainsKey("D6")) bms.MineNote[bar].Add("D6", null);
                    if (!bms.MineNote[bar].ContainsKey("D8")) bms.MineNote[bar].Add("D8", null);
                    if (!bms.MineNote[bar].ContainsKey("D9")) bms.MineNote[bar].Add("D9", null);

                    if (!bms.MineNote[bar].ContainsKey("E1")) bms.MineNote[bar].Add("E1", null);
                    if (!bms.MineNote[bar].ContainsKey("E2")) bms.MineNote[bar].Add("E2", null);
                    if (!bms.MineNote[bar].ContainsKey("E3")) bms.MineNote[bar].Add("E3", null);
                    if (!bms.MineNote[bar].ContainsKey("E4")) bms.MineNote[bar].Add("E4", null);
                    if (!bms.MineNote[bar].ContainsKey("E5")) bms.MineNote[bar].Add("E5", null);
                    if (!bms.MineNote[bar].ContainsKey("E6")) bms.MineNote[bar].Add("E6", null);
                    if (!bms.MineNote[bar].ContainsKey("E8")) bms.MineNote[bar].Add("E8", null);
                    if (!bms.MineNote[bar].ContainsKey("E9")) bms.MineNote[bar].Add("E9", null);
                }
                else
                {
                    if (bms.LastBar < bar) bms.LastBar = bar;

                    // bar 데이터 유무 체크
                    if (!bms.PlayNote.ContainsKey(bar))
                        bms.PlayNote[bar] = new Dictionary<string, string>();

                    // bar에 키값 있는지 여부 확인
                    if (bms.PlayNote[bar].ContainsKey(ch) && bms.PlayNote[bar][ch] == null)
                    {
                        bms.PlayNote[bar].Remove(ch);
                        bms.PlayNote[bar].Add(ch, leftNote);
                    }
                    else
                    {
                        bms.PlayNote[bar].Add(ch, leftNote);
                    }

                    // 비어있는 노트 파트에 빈 공간을 추가해서 채움
                    if (!bms.PlayNote[bar].ContainsKey("11")) bms.PlayNote[bar].Add("11", null);
                    if (!bms.PlayNote[bar].ContainsKey("12")) bms.PlayNote[bar].Add("12", null);
                    if (!bms.PlayNote[bar].ContainsKey("13")) bms.PlayNote[bar].Add("13", null);
                    if (!bms.PlayNote[bar].ContainsKey("14")) bms.PlayNote[bar].Add("14", null);
                    if (!bms.PlayNote[bar].ContainsKey("15")) bms.PlayNote[bar].Add("15", null);
                    if (!bms.PlayNote[bar].ContainsKey("16")) bms.PlayNote[bar].Add("16", null);
                    if (!bms.PlayNote[bar].ContainsKey("18")) bms.PlayNote[bar].Add("18", null);
                    if (!bms.PlayNote[bar].ContainsKey("19")) bms.PlayNote[bar].Add("19", null);
                    if (!bms.PlayNote[bar].ContainsKey("51")) bms.PlayNote[bar].Add("51", null);
                    if (!bms.PlayNote[bar].ContainsKey("52")) bms.PlayNote[bar].Add("52", null);
                    if (!bms.PlayNote[bar].ContainsKey("53")) bms.PlayNote[bar].Add("53", null);
                    if (!bms.PlayNote[bar].ContainsKey("54")) bms.PlayNote[bar].Add("54", null);
                    if (!bms.PlayNote[bar].ContainsKey("55")) bms.PlayNote[bar].Add("55", null);
                    if (!bms.PlayNote[bar].ContainsKey("56")) bms.PlayNote[bar].Add("56", null);
                    if (!bms.PlayNote[bar].ContainsKey("58")) bms.PlayNote[bar].Add("58", null);
                    if (!bms.PlayNote[bar].ContainsKey("59")) bms.PlayNote[bar].Add("59", null);
                }
            }
            catch (Exception e)
            {
                /*ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                ErrorHandler.LogError("File: " + bms.FilePath);
                ErrorHandler.LogError("Line: #" + bar+ch+":"+leftNote);
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);*/
            }
        }

        // 랜덤노트 등록 처리
        public void AddRandomNotes(BMS bms)
        {
            System.Random rand = new System.Random();

            // Nested Random, If-Else 처리를 위해 리스트로 처리
            List<int> RandomValue = new List<int>();
            List<int> IfValue = new List<int>();
            int curRand = 0;
            int randSize = 0;
            int ifCount = 0;
            bool ifable = false;

            foreach(string s in bms.Random)
            {
                string[] line = s.Split(' ');

                // divide tag and other info
                string tag = line[0].ToUpper();
                string left = "";
                string etc;
                for (int i = 1; i < line.Length; i++)
                {
                    etc = line[i];
                    if (left.Length != 0)
                    {
                        left += " ";
                    }
                    left += etc;
                }

                string chkWav = "";
                if (tag.Length > 4)
                {
                    chkWav = tag.Substring(0, 4).ToUpper();
                }
                int parsedTag = 0;

                if(tag.ToUpper() == "#RANDOM")
                {
                    bool test = false;
                    int randvalue;
                    test = int.TryParse(left, out randvalue);
                    randSize = randvalue;

                    if(test)
                    {
                        // 랜덤 진행이 가능
                        curRand = rand.Next(1, randvalue + 1);
                        RandomValue.Add(curRand);
                    }
                }
                else if(RandomValue.Count > 0)
                {
                    if(tag.ToUpper() == "#IF")
                    {
                        bool test = false;
                        int ifval;
                        test = int.TryParse(left, out ifval);

                        if(test)
                        {
                            // ifval에 따른 동작 유무 결정
                            if(ifval == curRand)
                            {
                                ifable = true;
                                IfValue.Add(ifval);
                            }
                        }
                    }
                    else if(tag.ToUpper() == "#ENDIF")
                    {
                        if(ifable)
                        {
                            ifable = false;
                            IfValue.RemoveAt(IfValue.Count - 1);
                        }

                        ifCount++;

                        if(ifCount == randSize)
                        {
                            RandomValue.RemoveAt(RandomValue.Count - 1);
                        }
                    }
                    else if (tag.ToUpper() == "#ENDRANDOM")
                    {
                        //RandomValue.RemoveAt(RandomValue.Count - 1);
                    }
                    else if(ifable) {
                        // 나머지 값을 돌면서 처리
                        if(int.TryParse(tag.Substring(1, 1), out parsedTag)) {
                            string noteBuf;
                            string[] noteTok = tag.Split(':');

                            noteBuf = noteTok[0];

                            string leftNote = "";
                            leftNote += noteTok[1];

                            int bar = int.Parse(noteBuf.Substring(1, 3));
                            string ch = noteBuf.Substring(4, 2);

                            AddChannels(bms, ch, leftNote, bar);
                        }
                    }
                }
            }
        }

        private bool CheckIfLNExist(BMS bms)
        {
            bool isLNExist = false;
            string[] lnlist = { "51", "52", "53", "54", "55", "56", "58", "59" };

            foreach(int key in bms.PlayNote.Keys)
            {
                Dictionary<string, string> noteInBar = bms.PlayNote[key];
                foreach(string k in lnlist)
                {
                    if(noteInBar[k] != null)
                    {
                        isLNExist = true;
                        break;
                    }
                }

                if (isLNExist) break;
            }

            return isLNExist;
        }
    }
}
