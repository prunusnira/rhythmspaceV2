using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class CoverLiftControl : MonoBehaviour
    {
        public GameObject[] coverSud1P;
        public GameObject[] coverHid1P;
        public GameObject[] areaLift1P;
        public GameObject[] coverSud2P;
        public GameObject[] coverHid2P;
        public GameObject[] areaLift2P;
        private int coverSudPos;
        private int coverHidPos;
        private int playAreaPos;

        // Start is called before the first frame update
        void Awake()
        {
            coverSudPos = Const.CoverSudPos;
            coverHidPos = Const.CoverHidPos;
            playAreaPos = Const.AreaLiftPos;
        }

        public void CoverSuddenDown()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = coverSud1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud1P[(int)Const.GearSize].transform.localPosition.z;

                coverSudPos -= 1;
                if (coverSudPos < 0)
                {
                    coverSudPos = 0;
                }

                coverSud1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, coverSudPos, posz);

                Const.CoverSudPos = coverSudPos;
            }
            else
            {
                float posx = coverSud2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud2P[(int)Const.GearSize].transform.localPosition.z;

                coverSudPos -= 1;
                if (coverSudPos < 0)
                {
                    coverSudPos = 0;
                }

                coverSud2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, coverSudPos, posz);

                Const.CoverSudPos = coverSudPos;
            }
        }

        public void CoverSuddenUp()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = coverSud1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud1P[(int)Const.GearSize].transform.localPosition.z;

                coverSudPos += 1;
                if (Const.GearSize != SkinSize.WIDE150)
                {
                    if (coverSudPos > 725)
                    {
                        coverSudPos = 725;
                    }
                }
                else
                {
                    if (coverSudPos > 890)
                    {
                        coverSudPos = 890;
                    }
                }

                coverSud1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, coverSudPos, posz);

                Const.CoverSudPos = coverSudPos;
            }
            else
            {
                float posx = coverSud2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud2P[(int)Const.GearSize].transform.localPosition.z;

                coverSudPos += 1;
                if (Const.GearSize != SkinSize.WIDE150)
                {
                    if (coverSudPos > 725)
                    {
                        coverSudPos = 725;
                    }
                }
                else
                {
                    if (coverSudPos > 890)
                    {
                        coverSudPos = 890;
                    }
                }

                coverSud2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, coverSudPos, posz);

                Const.CoverSudPos = coverSudPos;
            }
        }

        public void CoverHiddenDown()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = coverHid1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid1P[(int)Const.GearSize].transform.localPosition.z;

                coverHidPos += 1;
                if (Const.GearSize != SkinSize.WIDE150)
                {
                    if (coverHidPos > 725)
                    {
                        coverHidPos = 725;
                    }
                }
                else
                {
                    if (coverHidPos > 890)
                    {
                        coverHidPos = 890;
                    }
                }

                coverHid1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

                Const.CoverHidPos = coverHidPos;
            }
            else
            {
                float posx = coverHid2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid2P[(int)Const.GearSize].transform.localPosition.z;

                coverHidPos += 1;
                if (Const.GearSize != SkinSize.WIDE150)
                {
                    if (coverHidPos > 725)
                    {
                        coverHidPos = 725;
                    }
                }
                else
                {
                    if (coverHidPos > 890)
                    {
                        coverHidPos = 890;
                    }
                }

                coverHid2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

                Const.CoverHidPos = coverHidPos;
            }
        }

        public void CoverHiddenUp()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = coverHid1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid1P[(int)Const.GearSize].transform.localPosition.z;

                coverHidPos -= 1;
                if (coverHidPos < 0)
                {
                    coverHidPos = 0;
                }

                coverHid1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

                Const.CoverHidPos = coverHidPos;
            }
            else
            {
                float posx = coverHid2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid2P[(int)Const.GearSize].transform.localPosition.z;

                coverHidPos -= 1;
                if (coverHidPos < 0)
                {
                    coverHidPos = 0;
                }

                coverHid2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

                Const.CoverHidPos = coverHidPos;
            }
        }

        public void CoverLiftDown()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = areaLift1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = areaLift1P[(int)Const.GearSize].transform.localPosition.z;

                playAreaPos -= 1;
                if (playAreaPos < 0)
                {
                    playAreaPos = 0;
                }

                areaLift1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, playAreaPos, posz);

                Const.AreaLiftPos = playAreaPos;
            }
            else
            {
                float posx = areaLift2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = areaLift2P[(int)Const.GearSize].transform.localPosition.z;

                playAreaPos -= 1;
                if (playAreaPos < 0)
                {
                    playAreaPos = 0;
                }

                areaLift2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, playAreaPos, posz);

                Const.AreaLiftPos = playAreaPos;
            }
        }

        public void CoverLiftUp()
        {
            if (Const.PlayerSide == 0)
            {
                float posx = areaLift1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = areaLift1P[(int)Const.GearSize].transform.localPosition.z;

                playAreaPos += 1;
                if(Const.GearSize != SkinSize.WIDE150)
                {
                    if (playAreaPos > 725)
                    {
                        playAreaPos = 725;
                    }
                }
                else
                {
                    if (playAreaPos > 890)
                    {
                        playAreaPos = 890;
                    }
                }

                areaLift1P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, playAreaPos, posz);

                Const.AreaLiftPos = playAreaPos;
            }
            else
            {
                float posx = areaLift2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = areaLift2P[(int)Const.GearSize].transform.localPosition.z;

                playAreaPos += 1;
                if (Const.GearSize != SkinSize.WIDE150)
                {
                    if (playAreaPos > 725)
                    {
                        playAreaPos = 725;
                    }
                }
                else
                {
                    if (playAreaPos > 890)
                    {
                        playAreaPos = 890;
                    }
                }

                areaLift2P[(int)Const.GearSize].transform.localPosition = new Vector3(posx, playAreaPos, posz);

                Const.AreaLiftPos = playAreaPos;
            }
        }
    }
}
