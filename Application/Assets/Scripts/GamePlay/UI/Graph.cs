using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Graph : MonoBehaviour
    {
        public GameObject GraphBig;
        public GameObject GraphSmall;
        public GameObject GraphMini;

        public SpriteRenderer GraphBigBody;
        public SpriteRenderer GraphSmallBody;
        public SpriteRenderer GraphMiniBody;
        public Sprite GraphBigDark;
        public Sprite GraphSmallDark;
        public Sprite GraphMiniDark;

        public TextMesh[] valCur;
        public TextMesh[] valBest;
        public TextMesh[] valBestDiff;
        public TextMesh[] valTar;
        public TextMesh[] valTarDiff;

        public SpriteRenderer[] graSprCur;
        public SpriteRenderer[] graSprBest;
        public SpriteRenderer[] graSprBestBase;
        public SpriteRenderer[] graSprTar;
        public SpriteRenderer[] graSprTarBase;

        private void Awake()
        {
            switch (Const.GraphType)
            {
                case GraphType.NORMAL:
                    GraphBig.SetActive(true);
                    break;
                case GraphType.SMALL:
                    GraphSmall.SetActive(true);
                    break;
                case GraphType.MINI:
                    GraphMini.SetActive(true);
                    break;
            }

            if(Const.GearSkin == "dark")
            {
                GraphBigBody.sprite = GraphBigDark;
                GraphSmallBody.sprite = GraphSmallDark;
                GraphMiniBody.sprite = GraphMiniDark;
            }

            ObjectPositionSetup();
        }

        public void SetInitialGraph(int totalNotes)
        {
            graSprCur[(int)Const.GraphType].material.SetFloat("_Progress", 0f);
            graSprBest[(int)Const.GraphType].material.SetFloat("_Progress", 0f);
            graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", 0f);

            graSprBestBase[(int)Const.GraphType].material.SetFloat("_Progress", (float)Const.MyBestScore / totalNotes / 2);

            switch (Const.GraphTarget)
            {
                case GraphTargetType.A:
                    graSprTarBase[(int)Const.GraphType].material.SetFloat("_Progress", 6f / 9);
                    break;
                case GraphTargetType.AA:
                    graSprTarBase[(int)Const.GraphType].material.SetFloat("_Progress", 7f / 9);
                    break;
                case GraphTargetType.AAA:
                    graSprTarBase[(int)Const.GraphType].material.SetFloat("_Progress", 8f / 9);
                    break;
                case GraphTargetType.MAX:
                    graSprTarBase[(int)Const.GraphType].material.SetFloat("_Progress", 1f);
                    break;
                default:
                    graSprTarBase[(int)Const.GraphType].material.SetFloat("_Progress", 0f);
                    break;
            }
        }

        public void UpdateGraph(int ex, int procNotes, int totalNotes)
        {
            graSprCur[(int)Const.GraphType].material.SetFloat("_Progress", ((float)ex) / (totalNotes * 2));
            valCur[(int)Const.GraphType].text = ex.ToString();

            // 타겟 그래프 상승
            switch (Const.GraphTarget)
            {
                case GraphTargetType.A:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 6 / 9);
                    valTar[(int)Const.GraphType].text = (procNotes * 2 * 6 / 9).ToString();
                    valTarDiff[(int)Const.GraphType].text = (ex - procNotes * 2 * 6 / 9).ToString();
                    break;
                case GraphTargetType.AA:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 7 / 9);
                    valTar[(int)Const.GraphType].text = (procNotes * 2 * 7 / 9).ToString();
                    valTarDiff[(int)Const.GraphType].text = (ex - procNotes * 2 * 7 / 9).ToString();
                    break;
                case GraphTargetType.AAA:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 8 / 9);
                    valTar[(int)Const.GraphType].text = (procNotes * 2 * 8 / 9).ToString();
                    valTarDiff[(int)Const.GraphType].text = (ex - procNotes * 2 * 8 / 9).ToString();
                    break;
                case GraphTargetType.MAX:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes);
                    valTar[(int)Const.GraphType].text = (procNotes * 2).ToString();
                    valTarDiff[(int)Const.GraphType].text = (ex - procNotes * 2).ToString();
                    break;
            }

            graSprBest[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * Const.MyBestScore / totalNotes / 2);
            valBest[(int)Const.GraphType].text = (Const.MyBestScore * procNotes / totalNotes).ToString();

            valBestDiff[(int)Const.GraphType].text = (ex - (Const.MyBestScore * procNotes / totalNotes)).ToString();
        }

        private void ObjectPositionSetup()
        {
            bool isTypeA = true;
            switch (Const.LayoutType)
            {
                case UILayoutType.TYPEA:
                    isTypeA = true;
                    break;
                case UILayoutType.TYPEB:
                    isTypeA = false;
                    break;
            }

            int PlaySide = Const.PlayerSide;

            switch (Const.GraphType)
            {
                case GraphType.SMALL:
                    if (isTypeA)
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Left BGA - 기본 그래프 위치
                        }
                        else
                        {
                            // 2P Right BGA
                            GraphSmall.transform.localPosition =
                                new Vector2(
                                    Const.Graph_2P_Small_Left_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            GraphSmall.transform.localPosition =
                                new Vector2(
                                    Const.Graph_1P_Small_Left_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                        else
                        {
                            // 2P Left BGA
                            GraphSmall.transform.localPosition =
                                new Vector2(
                                    Const.Graph_2P_Small_Right_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                    }
                    break;
                case GraphType.MINI:
                    // Mini는 타입 상관없이 고정
                    if (PlaySide == 0)
                    {
                        // 1P BGA
                        GraphMini.transform.localPosition =
                            new Vector2(
                                Const.Graph_1P_Mini_PosX,
                                GraphSmall.transform.localPosition.y
                            );
                    }
                    else
                    {
                        // 2P BGA
                        GraphMini.transform.localPosition =
                            new Vector2(
                                Const.Graph_2P_Mini_PosX,
                                GraphSmall.transform.localPosition.y
                            );
                    }
                    break;
                case GraphType.OFFBGA:
                    break;
                case GraphType.NORMAL:
                case GraphType.OFFGEAR:
                default:
                    // Type A일때는 가운데임
                    if (isTypeA)
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            // 기본 그래프 위치
                        }
                        else
                        {
                            // 2P Left BGA
                            GraphBig.transform.localPosition =
                                new Vector2(
                                    Const.Graph_2P_Big_Left_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            GraphBig.transform.localPosition =
                                new Vector2(
                                    Const.Graph_1P_Big_Left_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                        else
                        {
                            // 2P Left BGA
                            GraphBig.transform.localPosition =
                                new Vector2(
                                    Const.Graph_2P_Big_Right_PosX,
                                    GraphSmall.transform.localPosition.y
                                );
                        }
                    }
                    break;
            }
        }
    }
}
