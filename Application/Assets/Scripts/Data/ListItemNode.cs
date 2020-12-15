using BMSCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BMSPlayer
{
    public class ListItem
    {
        // BMS or Folder or Table
        public ItemType Type { get; set; }

        public string Path { get; set; }
        public string Display { get; set; }
        public MusicListData Info { get; set; }

        // Table Data
        public bool IsFromTable { get; set; }
        public string Url { get; set; }
        public bool Exist { get; set; }

        // Distinguisher
        public ClearType Clear { get; set; }

        public ListItem()
        {
            Clear = ClearType.NOPLAY;
        }
    }
}
