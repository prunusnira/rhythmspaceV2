using BMSPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BMSCore
{
    public class BMSAnalyzer
    {
        private bool isWavExist = false;
        private bool isOggExist = false;
        private bool isVideoExist = false;

        // 노래 선택창에서 선택한 BMS 파일의 헤더를 분석
        public void HeaderAnalyzer(BMS bms)
        {
            // input stream 열기 // 기본 Default, 일본어 932, 한국어 949
            int encoding = Const.Encoding;

            StreamReader bmsReader = new StreamReader(
                bms.FilePath, System.Text.Encoding.GetEncoding(encoding)
            );

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

                        string chkWav = tag.Substring(0, 4).ToUpper();

                        // For BPM Check
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        else if (tag == "#PLAYER")
                        {
                            bms.Player = int.Parse(left);
                        }
                        else if (tag == "#GENRE")
                        {
                            bms.Gerne = left;
                        }
                        else if (tag == "#TITLE")
                        {
                            bms.Title = left;
                        }
                        else if (tag == "#SUBTITLE")
                        {
                            bms.SubTitle = left;
                        }
                        else if (tag == "#ARTIST")
                        {
                            bms.Artist = left;
                        }
                        else if (tag == "#SUBARTIST")
                        {
                            bms.SubArtist = left;
                        }
                        else if(tag == "#DIFFICULTY")
                        {
                            bms.Difficulty = int.Parse(left);
                        }
                        else if (tag == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.BPMStart = bpm;
                            bms.BPMMin = bpm;
                            bms.BPMMax = bpm;
                        }
                        else if (tag == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.Level = level;
                        }
                        else if (tag == "#RANK")
                        {
                            bms.Rank = int.Parse(left);
                        }
                        else if (tag == "#TOTAL")
                        {
                            int note;
                            int.TryParse(left, out note);
                            bms.TotalNotes = note;
                        }
                        else if (tag == "#STAGEFILE")
                        {
                            bms.StageFile = left;
                        }
                        else if (tag != "#BPM" && chkWav == "#BPM")
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
                            int ch = int.Parse(noteBuf.Substring(4, 2));
                            if (ch == 3 && leftNote != "00")
                            {
                                // BPM
                                GetBPMInfo(leftNote, ref bms);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.LogError("===File Error===");
                    ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("from: " + bms.FilePath);
                    continue;
                }
            }
            bmsReader.Close();
            
        }

        public void FullAnalyzer(BMS bms)
        {
            // wav ogg 체크
            string[] wavfiles = Directory.GetFiles(bms.FolderPath, "*.wav");
            string[] oggfiles = Directory.GetFiles(bms.FolderPath, "*.ogg");

            if (wavfiles.Length > 0)
                isWavExist = true;
            if (oggfiles.Length > 0)
                isOggExist = true;

            // input stream 열기 // 기본 Default, 일본어 932, 한국어 949
            int encoding = Const.Encoding;
            StreamReader bmsReader = new StreamReader(bms.FilePath, System.Text.Encoding.GetEncoding(encoding));

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

                        string chkWav = tag.Substring(0, 4).ToUpper();
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        if (tag == "#PLAYER")
                        {
                            bms.Player = int.Parse(left);
                        }
                        else if (tag == "#GENRE")
                        {
                            bms.Gerne = left;
                        }
                        else if (tag == "#TITLE")
                        {
                            bms.Title = left;
                        }
                        else if (tag == "#SUBTITLE")
                        {
                            bms.SubTitle = left;
                        }
                        else if (tag == "#ARTIST")
                        {
                            bms.Artist = left;
                        }
                        else if (tag == "#SUBARTIST")
                        {
                            bms.SubArtist = left;
                        }
                        else if (tag == "#DIFFICULTY")
                        {
                            bms.Difficulty = int.Parse(left);
                        }
                        else if (tag == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.BPMStart = bpm;
                            bms.BPMMin = bpm;
                            bms.BPMMax = bpm;
                        }
                        else if (tag == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.Level = level;
                        }
                        else if (tag == "#RANK")
                        {
                            bms.Rank = int.Parse(left);
                        }
                        else if (tag == "#TOTAL")
                        {
                            int note;
                            int.TryParse(left, out note);
                            bms.TotalNotes = note;
                        }
                        else if (tag == "#STAGEFILE")
                        {
                            bms.StageFile = left;
                        }
                        // Long Note Type Check
                        else if (tag == "#LNTYPE")
                        {
                            switch (int.Parse(left))
                            {
                                case 1: bms.LNType = LNType.Type1; break;
                                case 2: bms.LNType = LNType.Type2; break;
                            }
                        }
                        else if (tag == "#LNOBJ")
                        {
                            bms.LNType = LNType.Obj;
                            bms.LNObj = left;
                        }

                        else if (chkWav == "#WAV")
                        {
                            string filename = "";
                            string[] name = left.Split('.');
                            for (int i = 0; i < name.Length - 1; i++)
                            {
                                filename += name[i] + '.';
                            }
                            if (isWavExist && !isOggExist)
                                filename += "wav";
                            else if (!isWavExist && isOggExist)
                                filename += "ogg";
                            else
                            {
                                // 혼돈의 카오스...
                            }

                            bms.WavList.Add(tag.Substring(4, 2), filename);
                        }

                        else if (chkWav == "#BMP")
                        {
                            if (left.EndsWith(".mpg") || left.EndsWith(".mp4"))
                            {
                                bms.BGAVideoFile = bms.FolderPath + left;
                                isVideoExist = true;
                            }
                            else if (left.EndsWith(".bmp") || left.EndsWith(".jpg")
                                || left.EndsWith(".jpeg") || left.EndsWith(".png"))
                            {
                                // 파일 확장자 처리부터 수행
                                string[] filename = left.Split('.');
                                string filenameCheck = "";
                                for(int i = 0; i < filename.Length - 1; i++)
                                {
                                    filenameCheck += filename[i] + ".";
                                }

                                if (File.Exists(bms.FolderPath + filenameCheck + "bmp")) filenameCheck += "bmp";
                                else if (File.Exists(bms.FolderPath + filenameCheck + "jpg")) filenameCheck += "jpg";
                                else if (File.Exists(bms.FolderPath + filenameCheck + "jpeg")) filenameCheck += "jpeg";
                                else if (File.Exists(bms.FolderPath + filenameCheck + "png")) filenameCheck += "png";

                                bms.BGAImages.Add(tag.Substring(4, 2), Tools.createSpriteFromFile(bms.FolderPath + filenameCheck));
                            }
                        }
                        else if (tag != "#BPM" && chkWav == "#BPM")
                        {
                            // 16진수로 표현 불가능 한 BPM 표현
                            double bpmC = 0d;
                            bool trial = double.TryParse(left, out bpmC);
                            GetBPMInfoNum(bpmC, ref bms);

                            // 등록 필요
                            bms.BPMNum.Add(tag.Substring(4, 2), bpmC);
                        }
                        else if (chkWav == "#STO")
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
                                // LAYER IMG
                            }
                            else if (ch == "07")
                            {
                                // POOR IMG
                            }
                            else if(ch == "08" && leftNote != "00")
                            {
                                // BPM
                                bms.BPMNoteType2.Add(bar, leftNote);
                            }
                            else if(ch == "09" && leftNote != "00")
                            {
                                // Stop
                                bms.StopNote.Add(bar, leftNote);
                            }
                            else if(ch.StartsWith("D"))
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

                            //bms->mNote.insert(std::pair<int, Bar>(bar, mbar));
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.LogError("===File Error===");
                    ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("from: " + bms.FilePath);
                    continue;
                }
            }
            bmsReader.Close();
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
    }
}
