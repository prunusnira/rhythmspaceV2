using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public TextMeshPro[] sudPosNr;
        public TextMeshPro[] sudPosW125;
        public TextMeshPro[] sudPosW150;
        public TextMeshPro[] hidPosNr;
        public TextMeshPro[] hidPosW125;
        public TextMeshPro[] hidPosW150;
        public TextMeshPro[] liftPosNr;
        public TextMeshPro[] liftPosW125;
        public TextMeshPro[] liftPosW150;
        private TextMeshPro sudPos;
        private TextMeshPro hidPos;
        private TextMeshPro liftPos;

        private int max;

        // Start is called before the first frame update
        void Awake()
        {
            coverSudPos = Const.CoverSudPos;
            coverHidPos = Const.CoverHidPos;
            playAreaPos = Const.AreaLiftPos;

            switch(Const.GearSize)
            {
                case SkinSize.STANDARD:
                    sudPos = sudPosNr[Const.PlayerSide];
                    hidPos = hidPosNr[Const.PlayerSide];
                    liftPos = liftPosNr[Const.PlayerSide];
                    max = 725;
                    break;
                case SkinSize.WIDE125:
                    sudPos = sudPosW125[Const.PlayerSide];
                    hidPos = hidPosW125[Const.PlayerSide];
                    liftPos = liftPosW125[Const.PlayerSide];
                    max = 725;
                    break;
                case SkinSize.WIDE150:
                    sudPos = sudPosW150[Const.PlayerSide];
                    hidPos = hidPosW150[Const.PlayerSide];
                    liftPos = liftPosW150[Const.PlayerSide];
                    max = 890;
                    break;
            }
        }

        private void Start()
        {
            StartCoroutine(AutoRemove(sudPos));
            StartCoroutine(AutoRemove(hidPos));
            StartCoroutine(AutoRemove(liftPos));
        }

        public void CoverSuddenDown()
        {
            float posy = 0;
            coverSudPos += 1;
            if (Const.GearSize != SkinSize.WIDE150)
            {
                if (coverSudPos > 725)
                {
                    coverSudPos = 725;
                }

                posy = Const.SHUTTER - coverSudPos;
            }
            else
            {
                if (coverSudPos > 890)
                {
                    coverSudPos = 890;
                }

                posy = Const.SHUTTERL - coverSudPos;
            }

            if (Const.PlayerSide == 0)
            {
                float posx = coverSud1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud1P[(int)Const.GearSize].transform.localPosition.z;

                coverSud1P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, posy, posz);
            }
            else
            {
                float posx = coverSud2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud2P[(int)Const.GearSize].transform.localPosition.z;

                coverSud2P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, posy, posz);
            }

            sudPos.text = "SUD " + coverSudPos + " / " + max;
            sudPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(sudPos));

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverSuddenUp()
        {
            float posy = 0;
            coverSudPos -= 1;
            if (coverSudPos < 0)
            {
                coverSudPos = 0;
            }
            if (Const.GearSize != SkinSize.WIDE150)
            {
                posy = Const.SHUTTER - coverSudPos;
            }
            else
            {
                posy = Const.SHUTTERL - coverSudPos;
            }

            if (Const.PlayerSide == 0)
            {
                float posx = coverSud1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud1P[(int)Const.GearSize].transform.localPosition.z;

                coverSud1P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, posy, posz);
            }
            else
            {
                float posx = coverSud2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverSud2P[(int)Const.GearSize].transform.localPosition.z;

                coverSud2P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, posy, posz);
            }

            sudPos.text = "SUD " + coverSudPos + " / " + max;
            sudPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(sudPos));

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverHiddenDown()
        {
            float posy = 0;
            coverHidPos -= 1;
            if (coverHidPos < 0)
            {
                coverHidPos = 0;
            }
            if (Const.GearSize != SkinSize.WIDE150)
            {
                posy = Const.SHUTTER - coverHidPos;
            }
            else
            {
                posy = Const.SHUTTERL - coverHidPos;
            }

            if (Const.PlayerSide == 0)
            {
                float posx = coverHid1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid1P[(int)Const.GearSize].transform.localPosition.z;

                coverHid1P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, -1f * posy, posz);
            }
            else
            {
                float posx = coverHid2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid2P[(int)Const.GearSize].transform.localPosition.z;

                coverHid2P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, -1f * posy, posz);
            }

            hidPos.text = "HID " + coverHidPos + " / " + max;
            hidPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(hidPos));

            Const.CoverHidPos = coverHidPos;
        }

        public void CoverHiddenUp()
        {
            float posy = 0;
            coverHidPos += 1;
            if (Const.GearSize != SkinSize.WIDE150)
            {
                if (coverHidPos > 725)
                {
                    coverHidPos = 725;
                }

                posy = Const.SHUTTER - coverHidPos;
            }
            else
            {
                if (coverHidPos > 890)
                {
                    coverHidPos = 890;
                }

                posy = Const.SHUTTERL - coverHidPos;
            }

            if (Const.PlayerSide == 0)
            {
                float posx = coverHid1P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid1P[(int)Const.GearSize].transform.localPosition.z;

                coverHid1P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, -1f * posy, posz);
            }
            else
            {
                float posx = coverHid2P[(int)Const.GearSize].transform.localPosition.x;
                float posz = coverHid2P[(int)Const.GearSize].transform.localPosition.z;

                coverHid2P[(int)Const.GearSize].transform.localPosition
                    = new Vector3(posx, -1f * posy, posz);
            }

            hidPos.text = "HID " + coverHidPos + " / " + max;
            hidPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(hidPos));

            Const.CoverHidPos = coverHidPos;
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

            liftPos.text = "LIFT " + playAreaPos + " / " + max;
            liftPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(liftPos));
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

            liftPos.text = "LIFT " + playAreaPos + " / " + max;
            liftPos.gameObject.SetActive(true);
            StartCoroutine(AutoRemove(liftPos));
        }

        private IEnumerator AutoRemove(TextMeshPro txt)
        {
            yield return new WaitForSeconds(0.5f);

            txt.gameObject.SetActive(false);

            yield return null;
        }
    }
}
