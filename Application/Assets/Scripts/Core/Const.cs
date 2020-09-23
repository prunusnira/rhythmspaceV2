using BMSCore;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class Const
    {
        public static string DBPath = "";
        static Const()
        {
#if(UNITY_EDITOR)
            DBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\db.sqlite";
#elif(UNITY_STANDALONE)
            DBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\rhythmspace\\config\\db1.sqlite";
#endif
        }
        public static int CHANNEL = 320;
        public static int FRAMEMULTIPLIER = 1000;
        public static int FIXEDSTARTHEIGHT = 3000;
        public static int LINE = 8;

        public static bool DEBUG = true;
        public static bool isKeyChanged = false;
        public static bool isRefreshDone = false;

        public static List<MusicListData> list = new List<MusicListData>();
        public static List<RecordData> record = new List<RecordData>();
        public static MusicListData selectedMusic = null;
        public static int musiclistIdx = 0;

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
            "1) 탐색기로 폴더를 탐색\n" +
            "2)Path가 맞는지 확인\n" +
            "3) 오른쪽 아래의 버튼으로 적용",
            "1) エキスプローラーでフォルダーを探索\n" +
            "2) Pathを確認\n" +
            "3) 右下のボタンを押す",
            "1) Search path with explorer\n" +
            "2) Check Path on the top\n" +
            "3) Click the button on bottom-right"
        };

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

        public static string KeyChange
        {
            get
            {
                return PlayerPrefs.GetString("keyToChange", "");
            }
            set
            {
                PlayerPrefs.SetString("keyToChange", value);
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

        public static JudgeType JudgeType
        {
            get
            {
                return (JudgeType)PlayerPrefs.GetInt("judgeType", (int)JudgeType.NORMAL);
            }
            set
            {
                PlayerPrefs.SetInt("judgeType", (int)value);
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
        #endregion
    }
}
