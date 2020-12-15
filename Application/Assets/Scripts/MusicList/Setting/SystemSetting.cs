using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using BMSCore;
using SimpleFileBrowser;
using System.Threading;
using TMPro;
using System;
using System.Linq;

namespace BMSPlayer
{
    public class SystemSetting : Setting
    {
        // SFX Play
        public AudioSource sfxPlay;
        public AudioClip sfxSource;
        public AudioClip sfxMove;

        // BMS Path
        public Text titlePath;
        public Text txtPathVar;
        public Button btnPathChange;
        public Button btnPathRefresh;
        private string bmsPath;
        private Thread refreshThread = null;
        private int encoding;
        private bool refreshed = false;
        private bool isRefreshing = false;
        public GameObject dlgNoPath;

        // Browser
        public Text txtBrowserDesc;

        // Loading
        public GameObject layerLoading;
        public TextMeshProUGUI txtLoadingPath;
        private string strLoading = "";

        // Lang changer
        public Text titleLang;
        public Button btnKor;
        public Button btnJpn;
        public Button btnEng;

        // Resolution Change
        private Resolutions res;
        public Text titleResol;
        public Text txtResol;
        public Button btnResLeft;
        public Button btnResRight;

        // Frame Rate
        public Text titleFrame;
        public InputField inputFrame;
        public Button btnFrame;

        // Screen mode Change
        public Text titleScrMode;
        public Text txtScrMode;
        public Button btnWinLeft;
        public Button btnWinRight;

        // Sync changer
        public Text titleSync;
        public Button btnAutoSync;
        public Button btnSyncLeft;
        public Button btnSyncRight;
        public Text syncval;

        // Encoding
        public Text titleEncoding;
        public Button btn932;
        public Button btn949;
        public Text curEncoding;
        public Text encdesc;

        // Key Change
        public Text titleKey;
        public GameObject layerKeySetting;
        public Button btnKeySetting;

        // Table Editor
        public Text titleTable;
        public Button btnTableEdit;
        public Button btnTableRefresh;
        public GameObject dlgTableEdit;

        // Reset All
        public Text titleReset;
        public Button btnResetAll;
        public GameObject dlgResetSetting;
        public TextMeshProUGUI txtResetMsg;

        public static bool pathFromInit = false;
        public static bool langFromInit = false;

        public override void Awake()
        {
            base.Awake();
            res = new Resolutions();

            encoding = Const.Encoding;
            bmsPath = Const.BMSFolderPath;

            rows = 8;
            btn = new int[] { 2, 4, 3, 3, 3, 2, 1, 1 };

            //EncolorBtn(0, 0);

            // 각 버튼 onclick 설정
            btnPathChange.onClick.AddListener(delegate
            {
                ChangePath();
            });
            btnPathRefresh.onClick.AddListener(PathRefresh);

            // 해상도 변경
            btnResLeft.onClick.AddListener(delegate
            {
                ChangeResolution(false);
            });
            btnResRight.onClick.AddListener(delegate
            {
                ChangeResolution(true);
            });

            // 프레임
            btnFrame.onClick.AddListener(delegate
            {
                int frame = Convert.ToInt32(inputFrame.text);
                if (frame < 300) frame = 300;
                else if (frame > 5000) frame = 5000;
                Const.FrameRate = frame;
                Application.targetFrameRate = frame;
            });

            btnWinLeft.onClick.AddListener(delegate
            {
                ChangeScreenMode(false);
            });
            btnWinRight.onClick.AddListener(delegate
            {
                ChangeScreenMode(true);
            });

            btnKor.onClick.AddListener(delegate
            {
                ChangeLang(LanguageType.KO);
            });
            btnJpn.onClick.AddListener(delegate
            {
                ChangeLang(LanguageType.JA);
            });
            btnEng.onClick.AddListener(delegate
            {
                ChangeLang(LanguageType.EN);
            });
            btnAutoSync.onClick.AddListener(SetAutoSync);
            btnSyncLeft.onClick.AddListener(delegate
            {
                ChangeSync(false);
            });
            btnSyncRight.onClick.AddListener(delegate
            {
                ChangeSync(true);
            });
            btn932.onClick.AddListener(delegate
            {
                ChangeEncoding(932);
            });
            btn949.onClick.AddListener(delegate
            {
                ChangeEncoding(949);
            });
            btnKeySetting.onClick.AddListener(ShowKeySetting);
            btnTableEdit.onClick.AddListener(ShowTableEditor);
            btnTableRefresh.onClick.AddListener(delegate {
                DialogTableEdit.refreshFromOutside = true;
            });
            btnResetAll.onClick.AddListener(delegate
            {
                ShowResetDlg();
            });
        }

        public override void Start()
        {
            LanguageType lang = Const.Language;
            int enc = Const.Encoding;

            ChangeLang(lang);

            inputFrame.text = Const.FrameRate.ToString();
            
            switch(enc)
            {
                case 932:
                    curEncoding.text = "Shift-JIS";
                    break;
                case 949:
                    curEncoding.text = "EUC-KR";
                    break;
            }
            ShowSync();

            if (Const.AutoSync == AutoSyncType.OFF)
            {
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto OFF"; 
            }
            else
            {
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto ON";
            }

            switch (Const.ScreenMode)
            {
                case FullScreenMode.Windowed:
                    txtScrMode.text = Const.settingBtnScrModeWin[(int)Const.Language];
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    txtScrMode.text = Const.settingBtnScrModeFull[(int)Const.Language];
                    break;
                case FullScreenMode.MaximizedWindow:
                    txtScrMode.text = Const.settingBtnScrModeBorder[(int)Const.Language];
                    break;
            }

            ShowCurrentResolution();
        }

        public override void Update()
        {
            base.Update();

            // Refresh 상황에 따라 메시지 변경
            txtLoadingPath.text = strLoading;

            if(layerLoading.activeSelf && refreshed)
            {
                MusicListUI.isRefreshDone = true;
                layerLoading.SetActive(false);
                refreshed = false;
            }

            if(pathFromInit)
            {
                pathFromInit = false;
                ChangePath();
            }

            if (langFromInit)
            {
                langFromInit = false;
                ChangeLang(Const.Language);
            }
        }

        private void UpdateOption()
        {
            LanguageType lang = Const.Language;
            if(Const.BMSFolderPath == "")
            {
                txtPathVar.text = Const.settingNoPathFound[(int)lang];
            }
            else
            {
                txtPathVar.text = Const.BMSFolderPath;
            }
            settingDesc.text = Const.settingDesc[(int)lang];
            ShowSync();
            ChangeEncoding(Const.Encoding);
            encdesc.text = Const.settingEncodingDesc[(int)lang];

            titlePath.text = Const.settingTitlePath[(int)lang];
            titleResol.text = Const.settingTitleScrRes[(int)lang];
            titleFrame.text = Const.settingTitleFrameRate[(int)lang];
            titleScrMode.text = Const.settingTitleScrMode[(int)lang];
            titleLang.text = Const.settingTitleLang[(int)lang];
            titleSync.text = Const.settingTitleSync[(int)lang];
            titleEncoding.text = Const.settingTitleEncoding[(int)lang];
            titleKey.text = Const.settingTitleKey[(int)lang];
            titleTable.text = Const.settingTitleDiffTable[(int)lang];
            titleReset.text = Const.settingTitleReset[(int)lang];
            txtResetMsg.text = Const.DialogResetAll[(int)lang];

            // 버튼 언어 변경
            btnPathChange.GetComponentInChildren<Text>().text = Const.settingBtnBMSOpen[(int)Const.Language];
            btnPathRefresh.GetComponentInChildren<Text>().text = Const.settingBtnRefresh[(int)Const.Language];
            btnFrame.GetComponentInChildren<Text>().text = Const.settingBtnFrameUpdate[(int)Const.Language];
            btn932.GetComponentInChildren<Text>().text = Const.settingBtnEncJP[(int)Const.Language];
            btn949.GetComponentInChildren<Text>().text = Const.settingBtnEncKR[(int)Const.Language];
            btnKeySetting.GetComponentInChildren<Text>().text = Const.settingBtnKeyChange[(int)Const.Language];
            btnTableEdit.GetComponentInChildren<Text>().text = Const.settingBtnTableEdit[(int)Const.Language];
            btnTableRefresh.GetComponentInChildren<Text>().text = Const.settingBtnTableRefresh[(int)Const.Language];
            btnResetAll.GetComponentInChildren<Text>().text = Const.settingBtnReset[(int)Const.Language];
        }

        private void ChangePath()
        {
            if(!isRefreshing)
            {
                FileBrowser.ShowLoadDialog(
                    (path) => PathCallback(path),
                    null,
                    true,
                    false,
                    Const.BMSFolderPath,
                    "Select BMS Folder",
                    "Select"
                );
                sfxPlay.PlayOneShot(sfxSource);
            }
        }

        private void PathRefresh()
        {
            if(bmsPath == "")
            {
                // 경로가 지정되지 않음
                dlgNoPath.SetActive(true);
                return;
            }

            if (!isRefreshing)
            {
                strLoading = "Starting refresh...";
                isRefreshing = true;
                layerLoading.SetActive(true);
                refreshThread = new Thread(new ThreadStart(Refresh));
                refreshThread.Start();
                sfxPlay.PlayOneShot(sfxSource);
            }
        }

        private void PathCallback(string[] path)
        {
            Const.BMSFolderPath = path[0];
            bmsPath = path[0];
            BinaryFormatter formatter = new BinaryFormatter();
            string directory = Directory.GetDirectoryRoot(path[0]);
            txtPathVar.text = path[0];

            PathRefresh();
        }

        private void Refresh()
        {
            // PATH에서 하위 모든 파일 목록을 가져옴
            strLoading = "Gathering file list from disk";
            List<string> filesDir = Directory.EnumerateFiles(bmsPath, "*.*", SearchOption.AllDirectories).Where(s =>
                    s.ToLower().EndsWith(".bms") ||
                    s.ToLower().EndsWith(".bme") ||
                    s.ToLower().EndsWith(".bml")).ToList();

            strLoading = "Gathering file list from DB";
            List<string> filesDB = MusicDataManager.Instance.GetPathList();

            strLoading = "Processing list";
            HashSet<string> newFiles = new HashSet<string>(filesDir);
            newFiles.ExceptWith(filesDB);

            HashSet<string> delFiles = new HashSet<string>(filesDB);
            delFiles.ExceptWith(filesDir);

            // Add new files
            List<MusicListData> addList = new List<MusicListData>();
            int idx = 0;
            foreach (string s in newFiles)
            {
                strLoading = "Analyzing BMS files (" + idx + "/" + newFiles.Count + "):\n" + s;
                if (!File.GetAttributes(s).HasFlag(FileAttributes.Directory))
                {
                    MusicListData d = MusicDataManager.Instance.LoadBMSFromPath(s, encoding);
                    if (d != null) addList.Add(d);
                }

                idx++;
            }

            // 삭제 항목에 대해 처리
            strLoading = "Deleted removed BMS files";
            MusicDataManager.Instance.RemoveDataFromDB(new List<string>(delFiles), ref strLoading, encoding);

            // 추가 항목에 대해 처리
            strLoading = "Add new BMS files";
            MusicDataManager.Instance.AddDataToDB(addList, ref strLoading);

            refreshed = true;
            isRefreshing = false;
        }

        private void SetAutoSync()
        {
            if (Const.AutoSync == AutoSyncType.OFF)
            {
                Const.AutoSync = AutoSyncType.ON;
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto ON";
            }
            else
            {
                Const.AutoSync = AutoSyncType.OFF;
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto OFF";
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ChangeSync(bool up)
        {
            if (up)
            {
                Const.Sync++;
            }
            else
            {
                Const.Sync--;
            }
            ShowSync();
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ShowSync()
        {
            int sync = Const.Sync;
            if (sync < 0)
            {
                syncval.text = sync + " ms";
            }
            else
            {
                syncval.text = "+" + sync + " ms";
            }
        }

        private void ChangeLang(LanguageType lang)
        {
            Const.Language = lang;
            MusicListUI.isLangChanged = true;
            PlayOptionSetting.langChange = true;
            DialogNoPath.LangChanged = true;
            DialogNoPattern.LangChanged = true;
            UpdateOption();
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ChangeEncoding(int enc)
        {
            Const.Encoding = enc;
            switch (enc)
            {
                case 932:
                    curEncoding.text = "Shift-JIS";
                    break;
                case 949:
                    curEncoding.text = "EUC-KR";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void UpdateVideoSetting()
        {
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode
            );
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ChangeResolution(bool up)
        {
            int curpos = 0;
            for(int i = 0; i < res.reslist.Count; i++)
            {
                if (res.reslist[i][0] == Const.ScrWidth &&
                    res.reslist[i][1] == Const.ScrHeight)
                {
                    curpos = i;
                    break;
                }
            }

            if(up)
            {
                if(curpos < res.reslist.Count - 1)
                {
                    curpos++;
                }
            }
            else
            {
                if (curpos > 0)
                {
                    curpos--;
                }
            }

            Const.ScrWidth = res.reslist[curpos][0];
            Const.ScrHeight = res.reslist[curpos][1];
            txtResol.text = Const.ScrWidth.ToString() + "x" + Const.ScrHeight.ToString() +
                "("+(curpos+1)+"/"+res.reslist.Count+")";

            UpdateVideoSetting();
        }

        private void ChangeScreenMode(bool up)
        {
            if(up)
            {
                switch(Const.ScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen:
                        Const.ScreenMode = FullScreenMode.Windowed;
                        txtScrMode.text = Const.settingBtnScrModeWin[(int)Const.Language];
                        break;
                    case FullScreenMode.MaximizedWindow:
                        Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                        txtScrMode.text = Const.settingBtnScrModeFull[(int)Const.Language];
                        break;
                    case FullScreenMode.Windowed:
                        Const.ScreenMode = FullScreenMode.MaximizedWindow;
                        txtScrMode.text = Const.settingBtnScrModeBorder[(int)Const.Language];
                        break;
                }
            }
            else
            {
                switch (Const.ScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen:
                        Const.ScreenMode = FullScreenMode.MaximizedWindow;
                        txtScrMode.text = Const.settingBtnScrModeBorder[(int)Const.Language];
                        break;
                    case FullScreenMode.MaximizedWindow:
                        Const.ScreenMode = FullScreenMode.Windowed;
                        txtScrMode.text = Const.settingBtnScrModeWin[(int)Const.Language];
                        break;
                    case FullScreenMode.Windowed:
                        Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                        txtScrMode.text = Const.settingBtnScrModeFull[(int)Const.Language];
                        break;
                }
            }

            UpdateVideoSetting();
        }

        private void ShowCurrentResolution()
        {
            int curpos = 0;
            for (int i = 0; i < res.reslist.Count; i++)
            {
                if (res.reslist[i][0] == Const.ScrWidth &&
                    res.reslist[i][1] == Const.ScrHeight)
                {
                    curpos = i;
                    break;
                }
            }

            txtResol.text = Const.ScrWidth.ToString() + "x" + Const.ScrHeight.ToString() +
                "(" + (curpos + 1) + "/" + res.reslist.Count + ")";
        }

        private void ShowKeySetting()
        {
            MusicListUI.SetNotOnTop();
            layerKeySetting.SetActive(true);
            GetComponent<PlayKeySetting>().EnableWindow();
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ShowTableEditor()
        {
            MusicListUI.SetNotOnTop();
            dlgTableEdit.SetActive(true);
            GetComponent<DialogTableEdit>().EnableWindow();
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void ShowResetDlg()
        {
            MusicListUI.SetNotOnTop();
            dlgResetSetting.SetActive(true);
            sfxPlay.PlayOneShot(sfxSource);
        }
    }
}
