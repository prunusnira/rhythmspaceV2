using BMSCore;
using System.Collections.Generic;

namespace BMSPlayer
{
    public class ListItemNode
    {
        public List<int> Parent { get; set; }
        public List<ListItemNode> Children { get; set; }

        // BMS or Folder
        public ItemType Type { get; set; }

        public string Path { get; set; }
        public string Display { get; set; }

        public bool HasBMS { get; set; }
        public MusicListData Info { get; set; }
    }
}
