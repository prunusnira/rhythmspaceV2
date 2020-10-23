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
        public static int SPEEDMULTIPLIER = 10;
        public static int DEFAULTPOS = 0;
        public static int LINE = 8;

        public static bool DEBUG = true;
        public static bool isKeyChanged = false;
        public static bool isRefreshDone = false;

        public static List<RecordData> record = new List<RecordData>();
        public static int musiclistIdx = 0;

        public static ListItemTree BMSTree;
        public static ListItemNode selectedOnList = null;
        public static List<int> ListDepth = new List<int>();
        public static int ListPos = 0;
        public static bool ChangeLayout = false;

        public static int MyBestScore = 0;
        public static string MyBestRank = "";
        public static RecordData MyBestPrev;
        public static List<int> ResultGraph;

        public static int[] CustomRandom =
        {
            Custom1,
            Custom2,
            Custom3,
            Custom4,
            Custom5,
            Custom6,
            Custom7,
        };

        public static int[] CurrentLayout = { 1, 2, 3, 4, 5, 6, 7 };

        // Language
        #region Language
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

        public static string[] browserDesc = new string[3]
        {
            "파일 브라우저 사용법\n" +
            "1) 상단의 ↑ 버튼과 탐색창을 사용해 폴더 이동\n" +
            "2) 오른쪽 아래의 SELECT 버튼으로 적용",
            "ファイルブラウザの利用について\n" +
            "1) 上段の↑ボタンと探索画面を利用してフォルダーへ移動\n" +
            "2) 右下のSELECTボタンで確認",
            "How to use file browser\n" +
            "1) Move to BMS folder with ↑ button and click on browser\n" +
            "2) Click the button on bottom-right 'SELECT' to apply"
        };

        public static string[] videoCodecMsg = new string[3]
        {
            "Microsoft Store에서 MPEG-2 Video Extension을 설치해주세요",
            "Microsoft StoreからMPEG-2 Video Extensionをインストールしてください",
            "Please install MPEG-2 Video Extension from Microsoft Store"
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
            "ラインが逆番",
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
            "커스텀 라인 (F11으로 변경)",
            "カスタムライン (F11で変更)",
            "User custom (F11 to change)"
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
            "マウスで各ボタンの数字を入力してください",
            "Click on each input field to fill the numbers"
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
        public static int ScrRefresh
        {
            get
            {
                return PlayerPrefs.GetInt("scrRefresh", 144);
            }
            set
            {
                PlayerPrefs.SetInt("scrRefresh", value);
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
        public static int SpeedFixed
        {
            get
            {
                return PlayerPrefs.GetInt("speed", 200);
            }
            set
            {
                PlayerPrefs.SetInt("speed", value);
            }
        }

        public static int SpeedFluid
        {
            get
            {
                return PlayerPrefs.GetInt("speedfl", 200);
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
                return (SpdType)PlayerPrefs.GetInt("spdtype", (int)SpdType.FIXED);
            }
            set
            {
                PlayerPrefs.SetInt("spdtype", (int)value);
            }
        }

        public static int Playline
        {
            get
            {
                return PlayerPrefs.GetInt("playline", 8);
            }
            set
            {
                PlayerPrefs.SetInt("playline", value);
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

        public static JudgeUIType JudgeUIType
        {
            get
            {
                return (JudgeUIType)PlayerPrefs.GetInt("judgeUIType", 0);
            }
            set
            {
                PlayerPrefs.SetInt("judgeUIType", (int)value);
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

        public static string Skin
        {
            get
            {
                return PlayerPrefs.GetString("skin", "");
            }
            set
            {
                PlayerPrefs.SetString("skin", value);
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
