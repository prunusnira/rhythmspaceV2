using BMSCore;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BMSPlayer
{
    class DiffTableGeno: IDiffTable
    {
        private string Url;
        private string JSONBody;
        private bool isWorkDone;
        private string StrLoading;

        public DiffTableGeno(string fromUrl)
        {
            Url = fromUrl;
            isWorkDone = false;
        }

        public async void CrawlTable()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage msg = await client.GetAsync(Url);
            Stream stream = await msg.Content.ReadAsStreamAsync();
            TextReader reader = new StreamReader(stream, Encoding.GetEncoding(932), true) as TextReader;
            string document = await reader.ReadToEndAsync();

            // 각 라인별로 읽으면서 json만 가져오기
            string[] lines = document.Split(new string[] { "\r", "\n" }, StringSplitOptions.None);
            string arrstr = "";
            bool inRead = false;

            foreach(string s in lines)
            {
                if(inRead)
                {
                    if(s == "];")
                    {
                        arrstr += "]";
                        break;
                    }
                    else
                    {
                        arrstr += s + "\n";
                    }
                }
                else
                {
                    if(s.Contains("mname ="))
                    {
                        inRead = true;
                        arrstr += "[\n";
                    }
                }
            }

            // 읽은 내용을 List로 만들기
            List<DiffTableData> list = new List<DiffTableData>();
            JSONObject json = new JSONObject(arrstr);
            for(int i = 0; i < json.list.Count; i++)
            {
                JSONObject obj = json.list[i];
                string lvstr = obj.list[1].str;
                lvstr = lvstr.Replace("\"", "").Replace("★", "");
                int lv = 0;
                if(lvstr == "???")
                {
                    lv = 99;
                }
                else
                {
                    lv = Convert.ToInt32(lvstr);
                }

                string title = obj.list[2].str;

                string artistTag = obj.list[4].str;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(artistTag);
                HtmlNode node = doc.DocumentNode.Element("a");

                string url = node.GetAttributeValue("href", string.Empty);
                string artist = node.InnerText;

                // 각각을 Node로 만들어서 추가
                list.Add(new DiffTableData(title, artist, lv, url, ""));
            }
            TableDataManager.Instance.AddDataToDB(list, null, ref StrLoading);
            isWorkDone = true;
        }

        public bool IsWorkDone()
        {
            return isWorkDone;
        }
    }
}
