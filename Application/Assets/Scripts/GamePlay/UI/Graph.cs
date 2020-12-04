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

            if(Const.GearSkin == SkinType.DARK)
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
                case GraphTargetType.RECORD:
                    Const.ResultTarget = Const.MyBestScore * procNotes / totalNotes;
                    break;
                case GraphTargetType.A:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 6 / 9);
                    Const.ResultTarget = procNotes * 2 * 6 / 9;
                    break;
                case GraphTargetType.AA:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 7 / 9);
                    Const.ResultTarget = procNotes * 2 * 7 / 9;
                    break;
                case GraphTargetType.AAA:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 8 / 9);
                    Const.ResultTarget = procNotes * 2 * 8 / 9;
                    break;
                case GraphTargetType.MAX:
                    graSprTar[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes);
                    Const.ResultTarget = procNotes * 2;
                    break;
            }

            graSprBest[(int)Const.GraphType].material.SetFloat("_Progress", ((float)procNotes) / totalNotes * Const.MyBestScore / totalNotes / 2);
            valBest[(int)Const.GraphType].text = (Const.MyBestScore * procNotes / totalNotes).ToString();

            valBestDiff[(int)Const.GraphType].text = (ex - (Const.MyBestScore * procNotes / totalNotes)).ToString();
            valTar[(int)Const.GraphType].text = Const.ResultTarget.ToString();
            valTarDiff[(int)Const.GraphType].text = (ex - Const.ResultTarget).ToString();
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
                                new Vector3(
                                    Const.Graph_2P_Small_Left_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
                                );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            GraphSmall.transform.localPosition =
                                new Vector3(
                                    Const.Graph_1P_Small_Left_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
                                );
                        }
                        else
                        {
                            // 2P Left BGA
                            GraphSmall.transform.localPosition =
                                new Vector3(
                                    Const.Graph_2P_Small_Right_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
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
                            new Vector3(
                                Const.Graph_1P_Mini_PosX[(int)Const.GearSize],
                                GraphSmall.transform.localPosition.y,
                                GraphSmall.transform.localPosition.z
                            );
                    }
                    else
                    {
                        // 2P BGA
                        GraphMini.transform.localPosition =
                            new Vector3(
                                Const.Graph_2P_Mini_PosX[(int)Const.GearSize],
                                GraphSmall.transform.localPosition.y,
                                GraphSmall.transform.localPosition.z
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
                                new Vector3(
                                    Const.Graph_2P_Big_Left_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
                                );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            GraphBig.transform.localPosition =
                                new Vector3(
                                    Const.Graph_1P_Big_Left_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
                                );
                        }
                        else
                        {
                            // 2P Left BGA
                            GraphBig.transform.localPosition =
                                new Vector3(
                                    Const.Graph_2P_Big_Right_PosX[(int)Const.GearSize],
                                    GraphSmall.transform.localPosition.y,
                                    GraphSmall.transform.localPosition.z
                                );
                        }
                    }
                    break;
            }
        }
    }
}
