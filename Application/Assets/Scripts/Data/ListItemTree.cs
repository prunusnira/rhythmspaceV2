using BMSCore;
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
            ListItemNode current, ref string strLoading,
            List<MusicListData> musicList, int encoding)
        {
            JSONStr += "{\n\"Path\":\"" + current.Path.Replace("\\", "/") + "\",\n" +
                "\"BMS\":\"" +
                HasBMS(current, ref strLoading, musicList, encoding).ToString() +
                "\",\n" +
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
                            GenerateFolderNode(dirs[i], cdepth), ref strLoading,
                            musicList, encoding
                        )
                    );

                    if (i == dirs.Length - 1)
                    {
                        JSONStr += "\n";
                    }
                    else
                    {
                        JSONStr += ",\n";
                    }
                }
            }

            JSONStr += "]\n}";
            return current;
        }

        // JSON Control
        public ListItemNode JSONtoFolderNode(JSONObject json, List<int> depth)
        {
            ListItemNode node = new ListItemNode();
            node.Children = new List<ListItemNode>();

            node.HasBMS = bool.Parse(json.GetField("BMS").str);
            node.Path = json.GetField("Path").str;
            node.Parent = depth;
            JSONObject children = json.GetField("Children");

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
            return node;
        }

        // FolderNode 컨트롤 용 메소드
        public ListItemNode GenerateFolderNode(string path, List<int> depth)
        {
            ListItemNode node = new ListItemNode();
            node.Children = new List<ListItemNode>();
            node.Path = path;
            node.HasBMS = false;
            node.Parent = depth;
            return node;
        }

        public bool HasChildren(ListItemNode node)
        {
            return node.Children.Count > 0 ? true : false;
        }

        public bool HasBMS(
            ListItemNode node, ref string strLoading,
            List<MusicListData> musicList, int encoding)
        {
            string[] bmsfiles = Directory.GetFiles(node.Path, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.ToLower().EndsWith(".bms") || s.ToLower().EndsWith(".bme")
                        || s.ToLower().EndsWith(".bml")).ToArray();

            if (bmsfiles.Length > 0)
            {
                foreach (string bms in bmsfiles)
                {
                    //FileList.Add(bms);
                    strLoading = "Loading " + bms;
                    MusicListData bmsdata = MusicListManager.Instance.LoadBMSFromPath(bms, musicList.Count, encoding);
                    if (bmsdata != null) musicList.Add(bmsdata);
                }
                return true;
            }
            else
            {
                return false;
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
    }
}
