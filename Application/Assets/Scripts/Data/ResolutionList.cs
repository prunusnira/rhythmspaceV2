using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BMSPlayer
{
    class ResolutionList
    {
        public static int[] HD = { 1280, 720 };
        public static int[] FWXGA = { 1366, 768 };
        public static int[] HDPlus = { 1600, 900 };
        public static int[] FHD = { 1920, 1080 };
        public static int[] QHD = { 2560, 1440 };
        public static int[] UHD = { 3840, 2160 };
    }

    class Resolutions
    {
        public List<int[]> reslist = new List<int[]>();

        public Resolutions() {
            foreach(Resolution r in Screen.resolutions)
            {
                if(r.width == ResolutionList.HD[0] &&
                    r.height == ResolutionList.HD[1] &&
                    !reslist.Contains(ResolutionList.HD))
                {
                    reslist.Add(ResolutionList.HD);
                }
                else if (r.width == ResolutionList.FWXGA[0] &&
                    r.height == ResolutionList.FWXGA[1] &&
                    !reslist.Contains(ResolutionList.FWXGA))
                {
                    reslist.Add(ResolutionList.FWXGA);
                }
                else if (r.width == ResolutionList.HDPlus[0] &&
                    r.height == ResolutionList.HDPlus[1] &&
                    !reslist.Contains(ResolutionList.HDPlus))
                {
                    reslist.Add(ResolutionList.HDPlus);
                }
                else if (r.width == ResolutionList.FHD[0] &&
                    r.height == ResolutionList.FHD[1] &&
                    !reslist.Contains(ResolutionList.FHD))
                {
                    reslist.Add(ResolutionList.FHD);
                }
                else if (r.width == ResolutionList.QHD[0] &&
                    r.height == ResolutionList.QHD[1] &&
                    !reslist.Contains(ResolutionList.QHD))
                {
                    reslist.Add(ResolutionList.QHD);
                }
                else if (r.width == ResolutionList.UHD[0] &&
                    r.height == ResolutionList.UHD[1] &&
                    !reslist.Contains(ResolutionList.UHD))
                {
                    reslist.Add(ResolutionList.UHD);
                }
            }
        }
    }
}
