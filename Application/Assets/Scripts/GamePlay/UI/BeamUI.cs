using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class BeamUI : MonoBehaviour
    {
        public GameObject[] beamNr1P;
        public GameObject[] beamNr2P;
        public GameObject[] beamW1251P;
        public GameObject[] beamW1252P;
        public GameObject[] beamW1501P;
        public GameObject[] beamW1502P;
        private GameObject[] beam1P;
        private GameObject[] beam2P;

        public GameObject[] BtnPressNr1P;
        public GameObject[] BtnPressNr2P;
        public GameObject[] BtnPressW1251P;
        public GameObject[] BtnPressW1252P;
        public GameObject[] BtnPressW1501P;
        public GameObject[] BtnPressW1502P;
        private GameObject[] BtnPress1P;
        private GameObject[] BtnPress2P;

        // Start is called before the first frame update
        void Awake()
        {
            switch(Const.GearSize)
            {
                case SkinSize.STANDARD:
                    beam1P = beamNr1P;
                    beam2P = beamNr2P;
                    BtnPress1P = BtnPressNr1P;
                    BtnPress2P = BtnPressNr2P;
                    break;
                case SkinSize.WIDE125:
                    beam1P = beamW1251P;
                    beam2P = beamW1252P;
                    BtnPress1P = BtnPressW1251P;
                    BtnPress2P = BtnPressW1252P;
                    break;
                case SkinSize.WIDE150:
                    beam1P = beamW1501P;
                    beam2P = beamW1502P;
                    BtnPress1P = BtnPressW1501P;
                    BtnPress2P = BtnPressW1502P;
                    break;
            }
        }

        public void ShowAndHideBeam(int line, bool onoff)
        {
            if (onoff)
            {
                if (Const.PlayerSide == 0)
                {
                    beam1P[line].SetActive(true);
                    BtnPress1P[line].SetActive(true);
                }
                else
                {
                    beam2P[line].SetActive(true);
                    BtnPress2P[line].SetActive(true);
                }
            }
            else
            {
                if (Const.PlayerSide == 0)
                {
                    beam1P[line].SetActive(false);
                    BtnPress1P[line].SetActive(false);
                }
                else
                {
                    beam2P[line].SetActive(false);
                    BtnPress2P[line].SetActive(false);
                }
            }
        }
    }
}
