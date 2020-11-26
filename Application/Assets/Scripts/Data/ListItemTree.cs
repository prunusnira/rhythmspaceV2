using BMSCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BMSPlayer
{
    public class ListItemTree
    {
        public ListItemNode Head { get; set; }
        public List<string> DirList { get; set; }
        public List<string> FileList { get; set; }
        public string JSONStr { get; set; }

        public ListItemTree(string topPath)
        {
            Head = GenerateFolderNode(topPath, null);
            DirList = new List<string>();
            DirList.Add(topPath);
            FileList = new List<string>();
            JSONStr = "";
        }

        public ListItemTree(ListItemNode topNode)
        {
            Head = topNode;
        }

        public ListItemTree(JSONObject json)
        {
            Head = JSONtoFolderNode(json, new List<int>());
        }

        public ListItemNode CreateTree(
            ListItemNode current,
            ref string strLoading,
            List<MusicListData> musicList,
            int encoding,
            bool readBMS)
        {
            strLoading = current.Path;

            JSONStr += "{\n\"Path\":\"" + current.Path.Replace("\\", "/") + "\",\n" +
                "\"Type\":\"folder\",\n" +
                "\"Children\":[\n";

            string[] dirs = GetSubFolders(current);

            // Depth 가져오기
            List<int> depth;
            if (current.Parent == null)
            {
                depth = new List<int>();
            }
            else
            {
                depth = current.Parent;
            }

            // 서브 폴더에 대하여 depth의 선택 번호를 추가하여 등록
            if(dirs.Length > 0)
            {
                for(int i = 0; i < dirs.Length; i++)
                {
                    // dir path 추가
                    DirList.Add(dirs[i]);

                    // Depth 추가
                    List<int> cdepth = DepthClone(depth);
                    cdepth.Add(i);
                    AddChild(
                        current,
                        CreateTree(
                            GenerateFolderNode(dirs[i], cdepth),
                            ref strLoading,
                            musicList,
                            encoding,
                            readBMS
                        )
                    );

                    if (i != dirs.Length - 1)
                    {
                        JSONStr += ",\n";
                    }
                }
            }

            addBMSinFolder(dirs.Length, current, ref strLoading, musicList, encoding, readBMS);

            JSONStr += "]\n}";
            return current;
        }

        // JSON Control
        public ListItemNode JSONtoFolderNode(JSONObject json, List<int> depth)
        {
            ListItemNode node = new ListItemNode();
            node.Children = new List<ListItemNode>();

            string type = json.GetField("Type").str;
            switch(type)
            {
                case "folder": node.Type = ItemType.DIRECTORY; break;
                case "bms": node.Type = ItemType.BMS; break;
            }
            node.Path = json.GetField("Path").str;
            node.Parent = depth;
            JSONObject children = json.GetField("Children");

            if(children != null)
            {
                for (int i = 0; i < children.list.Count; i++)
                {
                    // Depth 가져오기
                    List<int> cdepth;
                    if (depth == null)
                    {
                        cdepth = new List<int>();
                    }
                    else
                    {
                        cdepth = DepthClone(depth);
                    }
                    cdepth.Add(i);

                    node.Children.Add(JSONtoFolderNode(children.list[i], cdepth));
                }
            }
            return node;
        }

        // FolderNode 컨트롤 용 메소드
        public ListItemNode GenerateFolderNode(string path, List<int> depth)
        {
            ListItemNode node = new ListItemNode();
            node.Children = new List<ListItemNode>();
            node.Path = path;
            node.Type = ItemType.DIRECTORY;
            node.Parent = depth;
            return node;
        }

        public bool HasChildren(ListItemNode node)
        {
            return node.Children.Count > 0 ? true : false;
        }

        public void addBMSinFolder(
            int dirsize,
            ListItemNode node,
            ref string strLoading,
            List<MusicListData> musicList,
            int encoding,
            bool readBMS)
        {
            string[] bmsfiles = Directory.GetFiles(node.Path, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.ToLower().EndsWith(".bms") || s.ToLower().EndsWith(".bme")
                        || s.ToLower().EndsWith(".bml")).ToArray();

            if (bmsfiles.Length > 0)
            {
                if(dirsize > 0) JSONStr += ",\n";
                foreach (string bms in bmsfiles)
                {
                    if (readBMS)
                    {
                        strLoading = bms;
                        MusicListData bmsdata = MusicDataManager.Instance.LoadBMSFromPath(bms, musicList.Count, encoding);
                        if (bmsdata != null) musicList.Add(bmsdata);
                    }
                    JSONStr +=
                        "{\n\"Path\":\"" + bms.Replace("\\", "/") + "\",\n" +
                        "\"Type\":\"bms\"\n}";

                    if(bmsfiles[bmsfiles.Length-1] != bms)
                    {
                        JSONStr += ",\n";
                    }
                    else
                    {
                        JSONStr += "\n";
                    }
                }
            }
            else
            {
                JSONStr += "\n";
            }
        }

        public void AddChild(ListItemNode current, ListItemNode child)
        {
            current.Children.Add(child);
        }

        // Coroutine UI 업데이트를 위해 작업 분리
        public string[] GetSubFolders(ListItemNode node)
        {
            string[] dirs = Directory.GetDirectories(node.Path, "*", SearchOption.TopDirectoryOnly);
            return dirs;
        }

        // Deep Copy Parent
        public List<int> DepthClone(List<int> orig)
        {
            List<int> clone = new List<int>();
            foreach(int i in orig)
            {
                clone.Add(i);
            }
            return clone;
        }

        public void GetPathsFromJSON(HashSet<string> list, JSONObject json)
        {
            // 현재 노드에서 추가
            list.Add(json.GetField("Path").str);

            JSONObject children = json.GetField("Children");
            if (children != null)
            {
                if (children.list.Count > 0)
                {
                    foreach (JSONObject o in json.GetField("Children").list)
                    {
                        GetPathsFromJSON(list, o);
                    }
                }
            }
        }
    }
}
