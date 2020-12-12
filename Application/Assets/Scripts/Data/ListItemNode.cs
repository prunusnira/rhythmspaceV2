using BMSCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BMSPlayer
{
    public class ListItemNode
    {
        public List<int> Parent { get; set; }
        public List<ListItemNode> Children { get; set; }

        // BMS or Folder or Table
        public ItemType Type { get; set; }

        public string Path { get; set; }
        public string Display { get; set; }

        public bool HaveBMS { get; set; }
        public MusicListData Info { get; set; }

        // Table Data
        public bool IsFromTable { get; set; }
        public string Url { get; set; }
        public bool Exist { get; set; }

        // Distinguisher
        public ClearType Clear { get; set; }
    }
}
