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
        public static string FMOD_CHANNEL_GROUP = "bmsChnGroup";
        public static int CHANNEL = 320;

        public static int frameMultiplier = 1000;

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
        "1 변동시 마다 3px씩 판정 위치가 변경됩니다",
        "１変更について３pxの判定変化があります",
        "Judge timing changes by 3px per 1"
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
        public static void SetBMSFolderPath(string path)
        {
            PlayerPrefs.SetString("bmspath", path);
        }

        public static string GetBMSFolderPath()
        {
            return PlayerPrefs.GetString("bmspath", settingNoPathFound[GetLang()]);
        }

        public static void SetPlayingBMSPath(string path)
        {
            PlayerPrefs.SetString("playbms", path);
        }

        public static string GetPlayingBMSPath()
        {
            return PlayerPrefs.GetString("playbms", "");
        }

        public static void SetEncoding(int enc)
        {
            PlayerPrefs.SetInt("encoding", enc);
        }

        public static int GetEncoding()
        {
            return PlayerPrefs.GetInt("encoding", 932);
        }

        public static void SetAudio(int audio)
        {
            PlayerPrefs.SetInt("audio", audio);
        }

        public static int GetAudio()
        {
            return PlayerPrefs.GetInt("audio", 0);
        }

        public static void SetLang(int lang)
        {
            PlayerPrefs.SetInt("lang", lang);
        }

        public static int GetLang()
        {
            return PlayerPrefs.GetInt("lang", 2);
        }

        public static void SetSync(int sync)
        {
            PlayerPrefs.SetInt("sync", sync);
        }

        public static int GetSync()
        {
            return PlayerPrefs.GetInt("sync", 0);
        }

        public static void SetKeyChange(string key)
        {
            PlayerPrefs.SetString("keyToChange", key);
        }

        public static string GetKeyChange()
        {
            return PlayerPrefs.GetString("keyToChange", "");
        }
        #endregion

        // Play Setting
        #region Play Setting
        public static void SetSpeedFixed(int speed)
        {
            PlayerPrefs.SetInt("speed", speed);
        }

        public static int GetSpeedFixed()
        {
            return PlayerPrefs.GetInt("speed", 200);
        }

        public static void SetSpeedFluid(int speed)
        {
            PlayerPrefs.SetInt("speedfl", speed);
        }

        public static int GetSpeedFluid()
        {
            return PlayerPrefs.GetInt("speedfl", 200);
        }

        public static void SetSpdType(SpdType type)
        {
            PlayerPrefs.SetInt("spdtype", (int)type);
        }

        public static SpdType GetSpdType()
        {
            return (SpdType)PlayerPrefs.GetInt("spdtype", (int)SpdType.FIXED);
        }

        public static void SetPlayline(int line)
        {
            PlayerPrefs.SetInt("playline", line);
        }

        public static int GetPlayline()
        {
            return PlayerPrefs.GetInt("playline", 8);
        }

        public static void SetAuto(int auto)
        {
            PlayerPrefs.SetInt("auto", auto);
        }

        public static int GetAuto()
        {
            return PlayerPrefs.GetInt("auto", 0);
        }

        public static void SetPJudge(int pjudge)
        {
            PlayerPrefs.SetInt("pjudge", pjudge);
        }

        public static int GetPJudge()
        {
            return PlayerPrefs.GetInt("pjudge", 1);
        }

        public static void SetJudgeType(JudgeType type)
        {
            PlayerPrefs.SetInt("judgeType", (int)type);
        }

        public static JudgeType GetJudgeType()
        {
            return (JudgeType)PlayerPrefs.GetInt("judgeType", (int)JudgeType.NORMAL);
        }

        public static void SetJudgeUIType(JudgeUIType type)
        {
            PlayerPrefs.SetInt("judgeUIType", (int)type);
        }

        public static JudgeUIType GetJudgeUIType()
        {
            return (JudgeUIType)PlayerPrefs.GetInt("judgeUIType", 0);
        }

        public static void SetGraphTarget(GraphTargetType type)
        {
            PlayerPrefs.SetInt("graphTargetType", (int)type);
        }

        public static GraphTargetType GetGraphTarget()
        {
            return (GraphTargetType)PlayerPrefs.GetInt("graphTargetType", (int)GraphTargetType.OFF);
        }
        #endregion

        // Play Status
        #region Play Status
        public static void SetClear(int clear)
        {
            PlayerPrefs.SetInt("clear", clear);
        }

        public static int GetClear()
        {
            return PlayerPrefs.GetInt("clear", 0);
        }

        public static void SetResultPerfect(int pfc)
        {
            PlayerPrefs.SetInt("rst_p", pfc);
        }

        public static int GetResultPerfect()
        {
            return PlayerPrefs.GetInt("rst_p", 0);
        }

        public static void SetResultGreat(int gr)
        {
            PlayerPrefs.SetInt("rst_gr", gr);
        }

        public static int GetResultGreat()
        {
            return PlayerPrefs.GetInt("rst_gr", 0);
        }

        public static void SetResultGood(int gd)
        {
            PlayerPrefs.SetInt("rst_gd", gd);
        }

        public static int GetResultGood()
        {
            return PlayerPrefs.GetInt("rst_gd", 0);
        }

        public static void SetResultOk(int ok)
        {
            PlayerPrefs.SetInt("rst_o", ok);
        }

        public static int GetResultOk()
        {
            return PlayerPrefs.GetInt("rst_o", 0);
        }

        public static void SetResultMiss(int m)
        {
            PlayerPrefs.SetInt("rst_m", m);
        }

        public static int GetResultMiss()
        {
            return PlayerPrefs.GetInt("rst_m", 0);
        }

        public static void SetResultComboBreak(int v)
        {
            PlayerPrefs.SetInt("rst_cb", v);
        }

        public static int GetResultComboBreak()
        {
            return PlayerPrefs.GetInt("rst_cb", 0);
        }

        public static void SetResultExScore(int score)
        {
            PlayerPrefs.SetInt("rst_exscore", score);
        }

        public static int GetResultExScore()
        {
            return PlayerPrefs.GetInt("rst_exscore", 0);
        }

        public static void SetResultAvgRate(float rate)
        {
            PlayerPrefs.SetFloat("rst_rate", rate);
        }

        public static float GetResultAvgRate()
        {
            return PlayerPrefs.GetFloat("rst_rate", 0);
        }

        public static void SetResultMaxCombo(int combo)
        {
            PlayerPrefs.SetInt("rst_combo", combo);
        }

        public static int GetResultMaxCombo()
        {
            return PlayerPrefs.GetInt("rst_combo", 0);
        }

        public static void SetResultTimeDiff(float timediff)
        {
            PlayerPrefs.SetFloat("rst_timediff", timediff);
        }

        public static float GetResultTimeDiff()
        {
            return PlayerPrefs.GetFloat("rst_timediff", 0f);
        }
        #endregion
    }
}
