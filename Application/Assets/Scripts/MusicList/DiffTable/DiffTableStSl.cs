using BMSCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace BMSPlayer
{
    public enum DiffTableMode
    {
        STELLA = 0,
        SATELLITE = 1
    }

    class DiffTableStSl: IDiffTable
    {
        private DiffTableMode mode;
        private string Url;
        private string JSONBody;
        private bool isWorkDone;
        private string StrLoading;

        public DiffTableStSl(string fromUrl, DiffTableMode mode)
        {
            Url = fromUrl;
            this.mode = mode;
            isWorkDone = false;
        }

        public async void CrawlTable()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(Url);
            res.EnsureSuccessStatusCode();
            JSONBody = await res.Content.ReadAsStringAsync();

            List<DiffTableData> list = new List<DiffTableData>();

            List<JSONObject> json = new JSONObject(JSONBody).list;
            for (int i = 0; i < json.Count; i++)
            {
                // 가져올 내용
                // title, artist, level, md5, url
                string title = json[i].GetField("title").str;
                string artist = json[i].GetField("artist").str;
                int level = Convert.ToInt32(json[i].GetField("level").str);
                string md5 = json[i].GetField("md5").str;
                string url = json[i].GetField("url").str;

                list.Add(new DiffTableData(title, artist, level, url, md5));
            }
            TableDataManager.Instance.AddDataToDB(list, mode, ref StrLoading);
            isWorkDone = true;
        }

        public bool IsWorkDone()
        {
            return isWorkDone;
        }
    }
}
