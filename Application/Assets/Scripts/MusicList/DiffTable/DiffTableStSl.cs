using BMSCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

namespace BMSPlayer
{
    class DiffTableStSl: IDiffTable
    {
        private DiffTableMode Mode;
        private string Url;
        private string JSONBody;
        private bool isWorkDone;

        public DiffTableStSl(string fromUrl, DiffTableMode mode)
        {
            Url = fromUrl;
            Mode = mode;
            isWorkDone = false;
        }

        public async void CrawlTable()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage res = await client.GetAsync(Url);
            res.EnsureSuccessStatusCode();
            JSONBody = await res.Content.ReadAsStringAsync();

            List<DiffTableData> list = new List<DiffTableData>();

            JSONObject obj = new JSONObject(JSONBody);
            if(!obj.IsNull)
            {
                List<JSONObject> json = obj.list;
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
                if (list.Count > 0) TableDataManager.Instance.AddDataToDB(list, Mode);
            }
            isWorkDone = true;
            Debug.Log("DONE " + Mode);
        }

        public bool IsWorkDone()
        {
            return isWorkDone;
        }
    }
}
