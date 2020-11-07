using BMSCore;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Const
    {
        public static string DBPath = "";
        public static string JSONPath = "";
        static Const()
        {
#if(UNITY_EDITOR)
            DBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\db.sqlite";
            JSONPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\structure.json";
#elif(UNITY_STANDALONE)
            DBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\db.sqlite";
            JSONPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\structure.json";
#endif
        }
        public static int CHANNEL = 320;
        public static int SPEEDMULTIPLIER = 5;
        public static int DEFAULTPOS = 0;
        public static int LINE = 8;
        public static float NoteSizeCalc = 7.5f;

        public static List<RecordData> record = new List<RecordData>();
        public static int musiclistIdx = 0;

        public static ListItemTree BMSTree;
        public static ListItemNode selectedOnList = null;
        public static List<int> ListDepth = new List<int>();
        public static int ListPos = 0;
        public static bool ChangeLayout = true;

        public static int MyBestScore = 0;
        public static string MyBestRank = "";
        public static RecordData MyBestPrev;
        public static List<int> ResultGraph;

        public static int NoteStartPos = 1100;

        public static int[] CurrentLayout = { 1, 2, 3, 4, 5, 6, 7 };

        // Language
        #region Language
        public static string[] settingTitlePath = new string[3]
        {
            "BMS 경로",
            "BMS Path",
            "BMS Path"
        };
        public static string[] settingTitleScrRes = new string[3]
        {
            "화면 해상도",
            "画面解像度",
            "Resolution"
        };
        public static string[] settingTitleScrMode = new string[3]
        {
            "화면 모드",
            "画面モード",
            "Screen Mode"
        };
        public static string[] settingTitleLang = new string[3]
        {
            "언어",
            "言語",
            "Language"
        };
        public static string[] settingTitleSync = new string[3]
        {
            "싱크 조절",
            "シンクロ調整",
            "Sync"
        };
        public static string[] settingTitleEncoding = new string[3]
        {
            "인코딩",
            "インコーディング",
            "Encoding"
        };
        public static string[] settingTitleKey = new string[3]
        {
            "조작 설정",
            "操作設定",
            "Control Setting"
        };
        public static string[] settingTitleReset = new string[3]
        {
            "설정 초기화",
            "設定リセット",
            "Reset All Setting"
        };


        public static string[] settingDesc = new string[3]
        {
            "게임 설정을 변경합니다",
            "ゲームの設定を変更します",
            "Change your game settings"
        };

        public static string[] settingPathTitle = new string[3]
        {
            "BMS 경로 변경",
            "BMS経路更新",
            "BMS path"
        };

        public static string[] settingPathCurrent = new string[3]
        {
            "현재 경로",
            "現在経路",
            "Current path"
        };

        public static string[] settingPathSelector = new string[3]
        {
            "BMS 파일이 있는 경로를 선택해주세요",
            "BMSの経路を選択してください",
            "Select folder where BMS files exist"
        };

        public static string[] settingNoPathFound = new string[3]
        {
            "경로가 설정되어 있지 않습니다",
            "経路が設定してございません",
            "Can not find the path"
        };

        public static string[] settingEncodingDesc = new string[3]
        {
            "BMS 파일을 읽어올 때 적용할 인코딩을 선택합니다",
            "BMSファイルを読む時適用するインコーディングを設定します",
            "Set encoding for parsing BMS"
        };

        public static string[] settingSyncDesc = new string[3]
        {
            "1 변동시 마다 1ms씩 판정 위치가 변경됩니다.",
            "１変更について1msの判定変化があります",
            "Judge timing changes by 1ms per 1"
        };

        public static string[] settingChangeWarning = new string[3]
        {
            "경로 혹은 인코딩 변경 시 모든 BMS 파일을 새로 읽어들이므로\n로딩에 시간이 소요됩니다",
            "経路やインコーディングの更新は全てのBMSファイルを\n読み直すことになりますので時間が掛ります",
            "If you change path or encoding, it will load all BMS files from scratch\nwhich takes lots of time"
        };

        public static string[] videoCodecMsg = new string[3]
        {
            "Microsoft Store에서 MPEG-2 Video Extension을 설치해주세요\n" +
            "설치 후에도 일부 BGA는 나오지 않을 수 있습니다\n\n" +
            "p.s. 현재 VLC-Unity 라이브러리에 에러가 있어 영상 표시에 문제가 있습니다.\n" +
            "문제가 해결되면 영상 재생 문제는 해결됩니다",
            "Microsoft StoreからMPEG-2 Video Extensionをインストールしてください\n" +
            "インストール後も一部のBGAは表示されません\n\n" +
            "p.s. 現在VLC-Unityライブラリのエラーがあり、映像表示に問題があります。\n" +
            "エラーが解決すると再生問題も解決になります",
            "Please install MPEG-2 Video Extension from Microsoft Store\n" +
            "Some BGAs will not displayed after installation.\n\n" +
            "p.s. Currently, VLC-Unity library has error for now.\n" +
            "It will be fixed after VLC-Unity library is fixed"
        };

        public static string[] listSelect = new string[3]
        {
            "곡 선택",
            "曲を選択",
            "SELECT"
        };

        public static string[] listSystemOp = new string[3]
        {
            "시스템 옵션",
            "システム設定",
            "SYSTEM OPTION"
        };

        public static string[] listPlayEnter = new string[3]
        {
            "플레이 / 폴더 진입",
            "プレイ / フォルダーに入る",
            "PLAY / INTO FOLDER"
        };

        public static string[] listPlayClick = new string[3]
        {
            "폴더클릭: 폴더 진입, 곡 1번 클릭: 곡 정보 표시, 곡 2번 클릭: 플레이",
            "フォルダー選択：フォルダーに入る, 曲1回選択：曲選択, 曲2回選択：プレイ",
            "CLICK FOLDER: INTO FOLDER, CLICK MUSIC: SHOW INFO, CLICK AGAIN: PLAY"
        };

        public static string[] listUpper = new string[3]
        {
            "상위 폴더로 이동 / 검색 취소",
            "上位フォルダーへ移動 / 検索キャンセル",
            "UPPER FOLDER / CANCEL SEARCH"
        };

        public static string[] playOpPage = new string[3]
        {
            "페이지 전환",
            "ページ変更",
            "PAGE SWAP"
        };

        public static string[] listTip = new string[3]
        {
            "각 항목에 할당된 키보드 버튼을 누르거나 마우스로 클릭하세요",
            "各項目のキーをキーボードから押すかあるいはマウスでクリックしてください",
            "Press button on keyboard or click with mouse"
        };

        public static string[] RandomDescNR = new string[3]
        {
            "정규 배치",
            "正規譜面",
            "Normal Type"
        };

        public static string[] RandomDescRD = new string[3]
        {
            "라인 무작위 배치",
            "ラインランダム",
            "Line Random"
        };

        public static string[] RandomDescMR = new string[3]
        {
            "라인 순서 반대",
            "ラインが逆順",
            "Line Reversed"
        };

        public static string[] RandomDescSR = new string[3]
        {
            "노트 무작위 배치",
            "各オブジェクトがランダム",
            "Random for Each Note"
        };

        public static string[] RandomDescCR = new string[3]
        {
            "색상별 라인 무작위",
            "ラインカラーランダム",
            "Line Random by Color"
        };

        public static string[] RandomDescCU = new string[3]
        {
            "유저 지정 랜덤 배치",
            "カスタム配置",
            "User custom random"
        };

        public static string[] KeyChangeDescription = new string[3]
        {
            "변경할 키를 눌러주세요\n(ESC로 취소)",
            "キーを選択してください\n(ESCでキャンセル)",
            "Press key\n(Cancel with ESC)"
        };

        public static string[] CustomRandomError = new string[3]
        {
            "1~7의 모든 숫자가 들어오도록 재설정하세요",
            "１から７のすべての数字が入るようにしてください",
            "All the 1 to 7 numbers are needed"
        };

        public static string[] CustomRandomDesc = new string[3]
        {
            "마우스로 각 칸을 선택하여 숫자를 입력하세요",
            "マウスで各ボタンをクリックしてから数字を入力してください",
            "Click on each input field to fill the numbers"
        };

        public static string[] keySettingDesc = new string[3]
        {
            "변경할 버튼을 선택해주세요",
            "更新するボタンを選択してください",
            "Please select button to be changed"
        };

        public static string[,] LayoutDesc = new string[2, 3]
        {
            {
                "기어-BGA-그래프 (2P 반대)",
                "ギアー・BGA・グラフ(２Pは逆)",
                "Gear-BGA-Graph (Opposite on 2P)"
            },
            {
                "기어-그래프-BGA (2P 반대)",
                "ギアー・グラフ・BGA(２Pは逆)",
                "Gear-Graph-BGA (Opposite on 2P)"
            }
        };

        public static string[,] FSDiffDisplayDesc = new string[3, 3]
        {
            { "", "", "" },
            {
                "콤보 위",
                "コンボ上",
                "With combo"
            },
            {
                "사이드",
                "サイド",
                "Gear side"
            }
        };

        public static string[] ResultTxtRetry = new string[3]
        {
            "재시도",
            "リトライ",
            "Retry"
        };

        public static string[] ResultTxtRetrySame = new string[3]
        {
            "재시도(동일패턴)",
            "リトライ(同じ譜面)",
            "Retry(Same Pattern)"
        };

        public static string[] ResultTxtNext = new string[3]
        {
            "곡 선택으로",
            "曲選択に移動",
            "Go Music Select"
        };

        public static string[] ResultTxtRetrySameDesc = new string[3]
        {
            "* RAMDOM에서만 동일 패턴이 적용됩니다",
            "* RANDOMのみ同じ譜面になります",
            "* Same pattern in RAMDOM only"
        };
        #endregion

        // System Setting Values
        #region System Setting
        public static string BMSFolderPath
        {
            get
            {
                return PlayerPrefs.GetString("bmspath", settingNoPathFound[(int)Language]);
            }
            set
            {
                PlayerPrefs.SetString("bmspath", value);
            }
        }

        public static string PlayingBMSPath
        {
            get
            {
                return PlayerPrefs.GetString("playbms", "");
            }
            set
            {
                PlayerPrefs.SetString("playbms", value);
            }
        }

        public static int Encoding
        {
            get
            {
                return PlayerPrefs.GetInt("encoding", 932);
            }
            set
            {
                PlayerPrefs.SetInt("encoding", value);
            }
        }

        public static LanguageType Language
        {
            get
            {
                return (LanguageType)PlayerPrefs.GetInt("lang", (int)LanguageType.EN);
            }
            set
            {
                PlayerPrefs.SetInt("lang", (int)value);
            }
        }

        public static AutoSyncType AutoSync
        {
            get
            {
                return (AutoSyncType)PlayerPrefs.GetInt("autosync", (int)AutoSyncType.OFF);
            }
            set
            {
                PlayerPrefs.SetInt("autosync", (int)value);
            }
        }

        public static int Sync
        {
            get
            {
                return PlayerPrefs.GetInt("sync", 0);
            }
            set
            {
                PlayerPrefs.SetInt("sync", value);
            }
        }

        public static int KeyToChange
        {
            get
            {
                return PlayerPrefs.GetInt("keyToChange", 0);
            }
            set
            {
                PlayerPrefs.SetInt("keyToChange", value);
            }
        }

        public static int ScrWidth
        {
            get
            {
                return PlayerPrefs.GetInt("scrWidth", 1920);
            }
            set
            {
                PlayerPrefs.SetInt("scrWidth", value);
            }
        }
        public static int ScrHeight
        {
            get
            {
                return PlayerPrefs.GetInt("scrHeight", 1080);
            }
            set
            {
                PlayerPrefs.SetInt("scrHeight", value);
            }
        }

        public static FullScreenMode ScreenMode
        {
            get
            {
                return (FullScreenMode)PlayerPrefs.GetInt("scrMode", (int)FullScreenMode.MaximizedWindow);
            }
            set
            {
                PlayerPrefs.SetInt("scrMode", (int)value);
            }
        }
        #endregion

        // Play Setting
        #region Play Setting
        public static int SpeedStd
        {
            get
            {
                return PlayerPrefs.GetInt("speed", 300);
            }
            set
            {
                PlayerPrefs.SetInt("speed", value);
            }
        }

        public static int SpeedCon
        {
            get
            {
                return PlayerPrefs.GetInt("speedfl", 400);
            }
            set
            {
                PlayerPrefs.SetInt("speedfl", value);
            }
        }

        public static SpdType SpdType
        {
            get
            {
                return (SpdType)PlayerPrefs.GetInt("spdtype", (int)SpdType.CONSTANT);
            }
            set
            {
                PlayerPrefs.SetInt("spdtype", (int)value);
            }
        }

        public static AutoPlayType Auto
        {
            get
            {
                return (AutoPlayType)PlayerPrefs.GetInt("auto", (int)AutoPlayType.OFF);
            }
            set
            {
                PlayerPrefs.SetInt("auto", (int)value);
            }
        }

        public static int DisplayJudge
        {
            get
            {
                return PlayerPrefs.GetInt("pjudge", 1);
            }
            set
            {
                PlayerPrefs.SetInt("pjudge", value);
            }
        }

        public static GaugeType GaugeType
        {
            get
            {
                return (GaugeType)PlayerPrefs.GetInt("gaugeType", (int)GaugeType.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("gaugeType", (int)value);
            }
        }

        public static GraphTargetType GraphTarget
        {
            get
            {
                return (GraphTargetType)PlayerPrefs.GetInt("graphTargetType", (int)GraphTargetType.OFF);
            }
            set
            {
                PlayerPrefs.SetInt("graphTargetType", (int)value);
            }
        }

        public static NoteLayout NoteLayout
        {
            get
            {
                return (NoteLayout)PlayerPrefs.GetInt("noteLayout", (int)NoteLayout.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("noteLayout", (int)value);
            }
        }

        public static JudgeType JudgeType
        {
            get
            {
                return (JudgeType)PlayerPrefs.GetInt("judgeType", (int)JudgeType.ARCADE);
            }
            set
            {
                PlayerPrefs.SetInt("judgeType", (int)value);
            }
        }

        public static int CoverSudPos
        {
            get
            {
                return PlayerPrefs.GetInt("suddenPos", 725);
            }
            set
            {
                PlayerPrefs.SetInt("suddenPos", value);
            }
        }

        public static int CoverHidPos
        {
            get
            {
                return PlayerPrefs.GetInt("hiddenPos", 725);
            }
            set
            {
                PlayerPrefs.SetInt("hiddenPos", value);
            }
        }

        public static int AreaLiftPos
        {
            get
            {
                return PlayerPrefs.GetInt("areaPos", 0);
            }
            set
            {
                PlayerPrefs.SetInt("areaPos", value);
            }
        }
        #endregion

        // Judge Timing
        #region Judge Timing
        public static double[] JudgeArcade = new double[5]
        {
            10d / 60, 30d / 60, 80d / 60, 100d / 60, 120d / 60
        };

        public static double[] JudgeOriginEasy = new double[5]
        {
            0.2, 0.6, 1.2, 2.0, 4.0
        };

        public static double[] JudgeOriginNormal = new double[5]
        {
            0.15, 0.45, 0.9, 1.5, 4.0
        };

        public static double[] JudgeOriginHard = new double[5]
        {
            0.125, 0.375, 0.75, 1.25, 4.0
        };

        public static double[] JudgeOriginVeryHard = new double[5]
        {
            0.1, 0.3, 0.6, 1.0, 4.0
        };

        public static double[] JudgeLR2Easy = new double[5]
        {
            0.21, 0.6, 1.2, 2.0, 10.0
        };

        public static double[] JudgeLR2Normal = new double[5]
        {
            0.18, 0.4, 1.0, 2.0, 10.0
        };

        public static double[] JudgeLR2Hard = new double[5]
        {
            0.15, 0.3, 0.6, 2.0, 10.0
        };

        public static double[] JudgeLR2VeryHard = new double[5]
        {
            0.08, 0.24, 0.4, 2.0, 10.0
        };

        public static double[] JudgeBREasy = new double[5]
        {
            0.2, 0.6, 1.5, 2.2, 5.0
        };

        public static double[] JudgeBRNormal = new double[5]
        {
            0.15, 0.45, 1.125, 2.75, 5.0
        };

        public static double[] JudgeBRHard = new double[5]
        {
            0.1, 0.3, 0.75, 3.3, 5.0
        };

        public static double[] JudgeBRVeryHard = new double[5]
        {
            0.05, 0.15, 0.375, 3.85, 5.0
        };
        #endregion

        // Play Status
        #region Play Status
        public static ClearType Clear
        {
            get
            {
                return (ClearType)PlayerPrefs.GetInt("clear", (int)ClearType.FAIL);
            }
            set
            {
                PlayerPrefs.SetInt("clear", (int)value);
            }
        }

        public static int ResultPerfect
        {
            get
            {
                return PlayerPrefs.GetInt("rst_p", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_p", value);
            }
        }

        public static int ResultGreat
        {
            get
            {
                return PlayerPrefs.GetInt("rst_gr", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_gr", value);
            }
        }

        public static int ResultGood
        {
            get
            {
                return PlayerPrefs.GetInt("rst_gd", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_gd", value);
            }
        }

        public static int ResultOk
        {
            get
            {
                return PlayerPrefs.GetInt("rst_o", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_o", value);
            }
        }

        public static int ResultMiss
        {
            get
            {
                return PlayerPrefs.GetInt("rst_m", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_m", value);
            }
        }

        public static int ResultComboBreak
        {
            get
            {
                return PlayerPrefs.GetInt("rst_cb", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_cb", value);
            }
        }

        public static int ResultScore
        {
            get
            {
                return PlayerPrefs.GetInt("rst_score", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_score", value);
            }
        }

        public static float ResultAvgRate
        {
            get
            {
                return PlayerPrefs.GetFloat("rst_rate", 0);
            }
            set
            {
                PlayerPrefs.SetFloat("rst_rate", value);
            }
        }

        public static int ResultMaxCombo
        {
            get
            {
                return PlayerPrefs.GetInt("rst_combo", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_combo", value);
            }
        }

        public static float ResultTimeDiff
        {
            get
            {
                return PlayerPrefs.GetFloat("rst_timediff", 0f);
            }
            set
            {
                PlayerPrefs.SetFloat("rst_timediff", value);
            }
        }

        public static string ResultRank
        {
            get
            {
                return PlayerPrefs.GetString("rst_rank", "f");
            }
            set
            {
                PlayerPrefs.SetString("rst_rank", value);
            }
        }

        public static int ResultFast
        {
            get
            {
                return PlayerPrefs.GetInt("rst_fast", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_fast", value);
            }
        }

        public static int ResultSlow
        {
            get
            {
                return PlayerPrefs.GetInt("rst_slow", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_slow", value);
            }
        }

        public static int ResultTarget
        {
            get
            {
                return PlayerPrefs.GetInt("rst_target", 0);
            }
            set
            {
                PlayerPrefs.SetInt("rst_target", value);
            }
        }
        #endregion

        // Play Skin Setting
        #region Play Skin
        public static SkinType GearSkin
        {
            get
            {
                return (SkinType)PlayerPrefs.GetInt("gearskin", (int)SkinType.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("gearskin", (int)value);
            }
        }

        public static SkinSize GearSize
        {
            get
            {
                return (SkinSize)PlayerPrefs.GetInt("gearsize", (int)SkinSize.STANDARD);
            }
            set
            {
                PlayerPrefs.SetInt("gearsize", (int)value);
            }
        }

        public static NoteSkin NoteSkin
        {
            get
            {
                return (NoteSkin)PlayerPrefs.GetInt("noteskin", (int)NoteSkin.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("noteskin", (int)value);
            }
        }

        public static NoteSize NoteSize
        {
            get
            {
                return (NoteSize)PlayerPrefs.GetInt("notesize", (int)NoteSize.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("notesize", (int)value);
            }
        }

        public static GraphType GraphType
        {
            get
            {
                return (GraphType)PlayerPrefs.GetInt("graphType", (int)GraphType.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("graphType", (int)value);
            }
        }

        public static UILayoutType LayoutType
        {
            get
            {
                return (UILayoutType)PlayerPrefs.GetInt("layoutType", (int)UILayoutType.TYPEA);
            }
            set
            {
                PlayerPrefs.SetInt("layoutType", (int)value);
            }
        }

        public static int BGAOnOff
        {
            get
            {
                return PlayerPrefs.GetInt("bgaonoff", 1);
            }
            set
            {
                PlayerPrefs.SetInt("bgaonoff", value);
            }
        }

        public static int PlayerSide
        {
            get
            {
                return PlayerPrefs.GetInt("playerside", 0);
            }
            set
            {
                PlayerPrefs.SetInt("playerside", value);
            }
        }

        public static DisplayPosType FastSlow
        {
            get
            {
                return (DisplayPosType)PlayerPrefs.GetInt("fastslow", (int)DisplayPosType.TYPEA);
            }
            set
            {
                PlayerPrefs.SetInt("fastslow", (int)value);
            }
        }

        public static DisplayPosType TargetDiff
        {
            get
            {
                return (DisplayPosType)PlayerPrefs.GetInt("targetdiff", (int)DisplayPosType.TYPEB);
            }
            set
            {
                PlayerPrefs.SetInt("targetdiff", (int)value);
            }
        }

        public static DisplayPosType RateDiff
        {
            get
            {
                return (DisplayPosType)PlayerPrefs.GetInt("ratediff", (int)DisplayPosType.TYPEB);
            }
            set
            {
                PlayerPrefs.SetInt("ratediff", (int)value);
            }
        }
        #endregion

        // Position, Width, Height
        #region Skin Detail
        // 1P + GraphBig
        public static int[] BGA_1P_GraphBig_Left_PosX = new int[3] {
            -453, -352, -244
        };
        public static int[] BGA_1P_GraphBig_Left_PosY = new int[3] {
            -250, -174, -93
        };

        public static int[] BGA_1P_GraphBig_Right_PosX = new int[3] {
            48, 151, 258
        };
        public static int[] BGA_1P_GraphBig_Right_PosY = new int[3] {
            -250, -174, -93
        };

        public static int[] BGA_1P_GraphSm_Left_PosX = new int[3] {
            -448, -352, -245
        };
        public static int[] BGA_1P_GraphSm_Left_PosY = new int[3] {
            -344, -278, -198
        };

        public static int[] BGA_1P_GraphSm_Right_PosX = new int[3] {
            -84, 12, 118
        };
        public static int[] BGA_1P_GraphSm_Right_PosY = new int[3] {
            -344, -278, -198
        };

        public static int[] BGA_1P_GraphMini_PosX = new int[3] {
            -140, -90, 8
        };
        public static int[] BGA_1P_GraphMini_PosY = new int[3] {
            -344, -344, -282
        };

        public static int[] BGA_1P_GraphOff_PosX = new int[3] {
            -240, -200, -160
        };
        public static int[] BGA_1P_GraphOff_PosY = new int[3] {
            -344, -344, -344
        };

        public static int[] BGA_2P_GraphBig_Left_PosX = new int[3] {
            -958, -961, -960
        };
        public static int[] BGA_2P_GraphBig_Left_PosY = new int[3] {
            -250, -174, -93
        };

        public static int[] BGA_2P_GraphBig_Right_PosX = new int[3] {
            -453, -457, -456
        };
        public static int[] BGA_2P_GraphBig_Right_PosY = new int[3] {
            -250, -174, -93
        };

        public static int[] BGA_2P_GraphSm_Left_PosX = new int[3] {
            -949, -961, -960
        };
        public static int[] BGA_2P_GraphSm_Left_PosY = new int[3] {
            -344, -278, -198
        };

        public static int[] BGA_2P_GraphSm_Right_PosX = new int[3] {
            -590, -597, -596
        };
        public static int[] BGA_2P_GraphSm_Right_PosY = new int[3] {
            -344, -278, -198
        };

        public static int[] BGA_2P_GraphMini_PosX = new int[3] {
            -885, -946, -960
        };
        public static int[] BGA_2P_GraphMini_PosY = new int[3] {
            -344, -344, -282
        };

        public static int[] BGA_2P_GraphOff_PosX = new int[3] {
            -765, -800, -870
        };
        public static int[] BGA_2P_GraphOff_PosY = new int[3] {
            -344, -344, -344
        };

        // Off Gear: 가운데 고정
        public static int[] BGA_GraphOffGear_PosX = new int[3] {
            -453, -351, -244
        };
        public static int[] BGA_GraphOffGear_PosY = new int[3] {
            -250, -93, 66
        };
        public static int[] BGA_GraphOffGear_Wid = new int[3] {
            910, 701, 489
        };
        public static int[] BGA_GraphOffGear_Hei = new int[3] {
            683, 525, 366
        };

        // BGA Size (Width, Height)
        public static int[] BGA_GraphBig_Wid = new int[3] {
            910, 809, 701
        };
        public static int[] BGA_GraphBig_Hei = new int[3] {
            683, 607, 525
        };
        public static int[] BGA_GraphSm_Wid = new int[3] {
            1035, 948, 841
        };
        public static int[] BGA_GraphSm_Hei = new int[3] {
            777, 711, 630
        };
        public static int[] BGA_GraphMini_Wid = new int[3] {
            1035, 1035, 952
        };
        public static int[] BGA_GraphMini_Hei = new int[3] {
            777, 777, 714
        };
        public static int[] BGA_GraphOff_Wid = new int[3] {
            1035, 1035, 1035
        };
        public static int[] BGA_GraphOff_Hei = new int[3] {
            777, 777, 777
        };

        // 1P Right = 0
        public static int[] Graph_1P_Big_Left_PosX = new int[3] {
            -913, -809, -703
        };
        public static int[] Graph_2P_Big_Left_PosX = new int[3] {
            -1417, -1417, -1417
        };
        public static int[] Graph_2P_Big_Right_PosX = new int[3] {
            -503, -610, -716
        };

        // 1P Right = 0
        public static int[] Graph_1P_Small_Left_PosX = new int[3] {
            -1052, -949, -841
        };
        public static int[] Graph_2P_Small_Left_PosX = new int[3] {
            -1556, -1556, -1556
        };
        public static int[] Graph_2P_Small_Right_PosX = new int[3] {
            -503, -609, -714
        };

        public static int[] Graph_1P_Mini_PosX = new int[3] {
            -1038, -935, -828
        };
        public static int[] Graph_2P_Mini_PosX = new int[3] {
            -379, -485, -590
        };

        // Text Position
        public static int[] HPTextPosX = new int[3]
        {
            -539, -470, -343
        };
        public static int[] HPTextPosY = new int[3]
        {
            -317, -317, -413
        };
        public static int[] GaugeTypeTextPosX = new int[3]
        {
            -694, -613, -679
        };
        public static int[] GaugeTypeTextPosY = new int[3]
        {
            -330, -330, -382
        };
        public static int[] DiffTextPosX = new int[3]
        {
            -856, -816, -853
        };
        public static int[] DiffTextPosY = new int[3]
        {
            -446, -446, -489
        };
        public static int[] LvTextPosX = new int[3]
        {
            -747, -707, -744
        };
        public static int[] LvTextPosY = new int[3]
        {
            -446, -446, -489
        };
        public static int[] SpdStdTextPosX = new int[3]
        {
            -517, -450, -527
        };
        public static int[] SpdStdTextPosY = new int[3]
        {
            -449, -449, -489
        };
        public static int[] SpdConTextPosX = new int[3]
        {
            -516, -449, -306
        };
        public static int[] SpdConTextPosY = new int[3]
        {
            -486, -486, -489
        };

        // 2P Big Type B
        public static int[] UpperLeftPosX = new int[3]
        {
            -500, -500, -500
        };
        public static int[] LowerLeftPosX = new int[3]
        {
            -300, -400, -500
        };
        // 1P Big Type B
        public static int[] UpperRightPosX = new int[3]
        {
            500, 600, 710
        };
        public static int[] LowerRightPosX = new int[3]
        {
            300, 400, 500
        };
        // Others
        public static int[] UpperCenter_1PPosX = new int[3]
        {
            0, 100, 200
        };
        public static int[] LowerCenter_1PPosX = new int[3]
        {
            -200, -100, 0
        };
        public static int[] UpperCenter_2PPosX = new int[3]
        {
            0, -100, -200
        };
        public static int[] LowerCenter_2PPosX = new int[3]
        {
            200, 100, 0
        };
        #endregion

        // Custom Key
        #region Custom Key
        public static int Custom1
        {
            get
            {
                return PlayerPrefs.GetInt("customKey1", 1);
            }
            set
            {
                PlayerPrefs.SetInt("customKey1", value);
            }
        }

        public static int Custom2
        {
            get
            {
                return PlayerPrefs.GetInt("customKey2", 2);
            }
            set
            {
                PlayerPrefs.SetInt("customKey2", value);
            }
        }

        public static int Custom3
        {
            get
            {
                return PlayerPrefs.GetInt("customKey3", 3);
            }
            set
            {
                PlayerPrefs.SetInt("customKey3", value);
            }
        }

        public static int Custom4
        {
            get
            {
                return PlayerPrefs.GetInt("customKey4", 4);
            }
            set
            {
                PlayerPrefs.SetInt("customKey4", value);
            }
        }

        public static int Custom5
        {
            get
            {
                return PlayerPrefs.GetInt("customKey5", 5);
            }
            set
            {
                PlayerPrefs.SetInt("customKey5", value);
            }
        }

        public static int Custom6
        {
            get
            {
                return PlayerPrefs.GetInt("customKey6", 6);
            }
            set
            {
                PlayerPrefs.SetInt("customKey6", value);
            }
        }

        public static int Custom7
        {
            get
            {
                return PlayerPrefs.GetInt("customKey7", 7);
            }
            set
            {
                PlayerPrefs.SetInt("customKey7", value);
            }
        }
        #endregion
    }
}
