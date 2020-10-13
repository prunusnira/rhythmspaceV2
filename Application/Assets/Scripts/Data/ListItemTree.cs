using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BMSPlayer
{
    public class ListItemTree
    {
        public ListItemNode Head { get; set; }

        public ListItemTree(string topPath)
        {
            Head = GenerateFolderNode(topPath, null);
        }

        public ListItemTree(ListItemNode topNode)
        {
            Head = topNode;
        }

        public ListItemTree(JSONObject json)
        {
            Head = JSONtoFolderNode(json, new List<int>());
        }

        public ListItemNode CreateTree(ListItemNode current)
        {
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
                    // Depth 추가
                    List<int> cdepth = DepthClone(depth);
                    cdepth.Add(i);
                    AddChild(current, CreateTree(GenerateFolderNode(dirs[i], cdepth)));
                }
            }
            return current;
        }

        public string CreateTreeJSON(ListItemNode current)
        {
            string jsonStr = "{\n\"Path\":\"" + current.Path.Replace("\\", "/") + "\",\n" +
                "\"BMS\":\"" + HasBMS(current).ToString() + "\",\n" +
                "\"Children\":[\n";
            for (int i = 0; i < current.Children.Count; i++)
            {
                ListItemNode node = current.Children[i];
                jsonStr += "\t" + CreateTreeJSON(node);
                if (i == current.Children.Count - 1)
                {
                    jsonStr += "\n";
                }
                else
                {
                    jsonStr += ",\n";
                }
            }
            jsonStr += "]\n}";

            return jsonStr;
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

        public bool HasBMS(ListItemNode node)
        {
            string[] bmsfiles = Directory.GetFiles(node.Path, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.ToLower().EndsWith(".bms") || s.ToLower().EndsWith(".bme")).ToArray();

            if (bmsfiles.Length > 0) return true;
            else return false;
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
