using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using BMSCore;
using SimpleFileBrowser;
using System.Threading;
using TMPro;

namespace BMSPlayer
{
    public class SystemSetting : Setting
    {
        // SFX Play
        public AudioSource sfxPlay;
        public AudioClip sfxSource;
        public AudioClip sfxMove;

        // Path changer
        private List<MusicListData> musicList;
        private ListItemTree tree;

        // BMS Path
        public Text titlePath;
        public Text txtPathVar;
        public Button btnChange;
        public Button btnRefresh;
        private string bmsPath;
        private Thread refreshThread = null;
        private int encoding;
        private bool refreshed = false;
        private bool isRefreshing = false;

        // Browser
        public Text txtBrowserDesc;
        public string[] extensions;

        // Loading
        public GameObject layerLoading;
        public TextMeshProUGUI txtLoadingPath;
        private string strLoading = "";

        // Lang changer
        public Text titleLang;
        public Button btnKor;
        public Button btnJpn;
        public Button btnEng;
        public Text curLang;

        // Resolution Change
        public Text titleResol;
        public Button btn1080p;
        public Button btn900p;
        public Button btn768p;
        public Button btn720p;
        public Text txtResol;

        // Screen mode Change
        public Text titleScrMode;
        public Button btnWindowed;
        public Button btnFullScr;
        public Button btnBorderless;
        public Text txtScrMode;

        // Sync changer
        public Text titleSync;
        public Button btnAutoSync;
        public Button btnSyncUp;
        public Button btnSyncDown;
        public Text syncval;
        public Text syncdesc;

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

        // Reset All
        public Text titleReset;
        public Button btnResetAll;

        public override void Awake()
        {
            base.Awake();
            encoding = Const.Encoding;
            bmsPath = Const.BMSFolderPath;
            musicList = new List<MusicListData>();

            rows = 8;
            btn = new int[] { 2, 4, 3, 3, 3, 2, 1, 1 };

            EncolorBtn(0, 0);

            // 각 버튼 onclick 설정
            btnChange.onClick.AddListener(delegate
            {
                changePath();
            });
            btnRefresh.onClick.AddListener(pathRefresh);
            btn1080p.onClick.AddListener(delegate
            {
                Const.ScrWidth = 1920;
                Const.ScrHeight = 1080;
                changeVideoSetting();
            });
            btn900p.onClick.AddListener(delegate
            {
                Const.ScrWidth = 1600;
                Const.ScrHeight = 900;
                changeVideoSetting();
            });
            btn768p.onClick.AddListener(delegate
            {
                Const.ScrWidth = 1366;
                Const.ScrHeight = 768;
                changeVideoSetting();
            });
            btn720p.onClick.AddListener(delegate
            {
                Const.ScrWidth = 1280;
                Const.ScrHeight = 720;
                changeVideoSetting();
            });
            btnWindowed.onClick.AddListener(delegate
            {
                Const.ScreenMode = FullScreenMode.Windowed;
                changeVideoSetting();
            });
            btnFullScr.onClick.AddListener(delegate
            {
                Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                changeVideoSetting();
            });
            btnBorderless.onClick.AddListener(delegate
            {
                Const.ScreenMode = FullScreenMode.MaximizedWindow;
                changeVideoSetting();
            });
            btnKor.onClick.AddListener(delegate
            {
                changeLang(LanguageType.KO);
            });
            btnJpn.onClick.AddListener(delegate
            {
                changeLang(LanguageType.JA);
            });
            btnEng.onClick.AddListener(delegate
            {
                changeLang(LanguageType.EN);
            });
            btnAutoSync.onClick.AddListener(SetAutoSync);
            btnSyncDown.onClick.AddListener(delegate
            {
                changeSync(false);
            });
            btnSyncUp.onClick.AddListener(delegate
            {
                changeSync(true);
            });
            btn932.onClick.AddListener(delegate
            {
                changeEncoding(932);
            });
            btn949.onClick.AddListener(delegate
            {
                changeEncoding(949);
            });
            btnKeySetting.onClick.AddListener(ShowKeySetting);
            btnResetAll.onClick.AddListener(delegate
            {
                PlayerPrefs.DeleteAll();
            });
        }

        public override void Start()
        {
            LanguageType lang = Const.Language;
            int enc = Const.Encoding;

            switch(lang)
            {
                case LanguageType.KO:
                    curLang.text = "Korean";
                    break;
                case LanguageType.JA:
                    curLang.text = "Japanese";
                    break;
                case LanguageType.EN:
                    curLang.text = "English";
                    break;
            }
            changeLang(lang);
            
            switch(enc)
            {
                case 932:
                    curEncoding.text = "JP-Based";
                    break;
                case 949:
                    curEncoding.text = "KR-Based";
                    break;
            }
            showSync();

            if (Const.AutoSync == AutoSyncType.OFF)
            {
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto Sync OFF"; 
            }
            else
            {
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto Sync ON";
            }

            switch (Const.ScreenMode)
            {
                case FullScreenMode.Windowed:
                    txtScrMode.text = "Windowed";
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    txtScrMode.text = "Full Screen";
                    break;
                case FullScreenMode.MaximizedWindow:
                    txtScrMode.text = "Borderless";
                    break;
            }

            txtResol.text = Const.ScrWidth.ToString() + "x" + Const.ScrHeight.ToString();
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
        }

        public override void EncolorBtn(int row, int col)
        {
            // 1. 모든 버튼의 색상 리셋
            Button[] btnlist = settingAll.GetComponentsInChildren<Button>();
            foreach(Button b in btnlist)
            {
                b.GetComponent<Image>().sprite = normalBtn;
            }

            // 2. 현재 버튼 색상 변경
            switch (row)
            {
                case 0:
                    if (col == 0) ChangeSprite(btnChange);
                    else if (col == 1) ChangeSprite(btnRefresh);
                    break;
                case 1:
                    // Resolution
                    if (col == 0) ChangeSprite(btn1080p);
                    else if (col == 1) ChangeSprite(btn900p);
                    else if (col == 2) ChangeSprite(btn768p);
                    else if (col == 3) ChangeSprite(btn720p);
                    break;
                case 2:
                    // Screen Mode
                    if (col == 0) ChangeSprite(btnWindowed);
                    else if (col == 1) ChangeSprite(btnFullScr);
                    else if (col == 2) ChangeSprite(btnBorderless);
                    break;
                case 3:
                    if (col == 0) ChangeSprite(btnKor);
                    else if (col == 1) ChangeSprite(btnJpn);
                    else if (col == 2) ChangeSprite(btnEng);
                    break;
                case 4:
                    if (col == 0) ChangeSprite(btnAutoSync);
                    else if (col == 1) ChangeSprite(btnSyncDown);
                    else if (col == 2) ChangeSprite(btnSyncUp);
                    break;
                case 5:
                    if (col == 0) ChangeSprite(btn932);
                    else if (col == 1) ChangeSprite(btn949);
                    break;
                case 6:
                    ChangeSprite(btnKeySetting);
                    break;
                case 7:
                    ChangeSprite(btnResetAll);
                    break;
            }
            sfxPlay.PlayOneShot(sfxMove);
        }

        public override void ExecuteOption(int row, int col)
        {
            switch (row)
            {
                case 0:
                    switch (col)
                    {
                        case 0:
                            changePath();
                            break;
                        case 1:
                            pathRefresh();
                            break;
                    }
                    break;
                case 1:
                    switch (col)
                    {
                        case 0:
                            Const.ScrWidth = 1920;
                            Const.ScrHeight = 1080;
                            break;
                        case 1:
                            Const.ScrWidth = 1600;
                            Const.ScrHeight = 900;
                            break;
                        case 2:
                            Const.ScrWidth = 1366;
                            Const.ScrHeight = 768;
                            break;
                        case 3:
                            Const.ScrWidth = 1280;
                            Const.ScrHeight = 720;
                            break;
                    }
                    changeVideoSetting();
                    break;
                case 2:
                    switch (col)
                    {
                        case 0:
                            // Windowed
                            Const.ScreenMode = FullScreenMode.Windowed;
                            break;
                        case 1:
                            // FullScreen
                            Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                            break;
                        case 2:
                            // Borderless
                            Const.ScreenMode = FullScreenMode.MaximizedWindow;
                            break;
                    }
                    changeVideoSetting();
                    break;
                case 3:
                    changeLang((LanguageType)col);
                    break;
                case 4:
                    switch(col)
                    {
                        case 0:
                            SetAutoSync();
                            break;
                        case 1:
                            changeSync(false);
                            break;
                        case 2:
                            changeSync(true);
                            break;
                    }
                    break;
                case 5:
                    switch(col)
                    {
                        case 0:
                            changeEncoding(932);
                            break;
                        case 1:
                            changeEncoding(949);
                            break;
                    }
                    break;
                case 6:
                    ShowKeySetting();
                    break;
                case 7:
                    PlayerPrefs.DeleteAll();
                    break;
            }
        }

        public void UpdateOption()
        {
            LanguageType lang = Const.Language;
            txtPathVar.text = Const.BMSFolderPath;
            settingDesc.text = Const.settingDesc[(int)lang];
            showSync();
            changeEncoding(Const.Encoding);
            syncdesc.text = Const.settingSyncDesc[(int)lang];
            encdesc.text = Const.settingEncodingDesc[(int)lang];

            titlePath.text = Const.settingTitlePath[(int)lang];
            titleResol.text = Const.settingTitleScrRes[(int)lang];
            titleScrMode.text = Const.settingTitleScrMode[(int)lang];
            titleLang.text = Const.settingTitleLang[(int)lang];
            titleSync.text = Const.settingTitleSync[(int)lang];
            titleEncoding.text = Const.settingTitleEncoding[(int)lang];
            titleKey.text = Const.settingTitleKey[(int)lang];
            titleReset.text = Const.settingTitleReset[(int)lang];
        }

        public void changePath()
        {
            if(!isRefreshing)
            {
                FileBrowser.ShowLoadDialog(
                    (path) => pathCallback(path),
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

        public void pathRefresh()
        {
            if (!isRefreshing)
            {
                isRefreshing = true;
                musicList.RemoveRange(0, musicList.Count);
                layerLoading.SetActive(true);
                refreshThread = new Thread(new ThreadStart(refresh));
                refreshThread.Start();
                sfxPlay.PlayOneShot(sfxSource);
            }
        }

        public void pathCallback(string[] path)
        {
            Const.BMSFolderPath = path[0];
            bmsPath = path[0];
            BinaryFormatter formatter = new BinaryFormatter();
            string directory = Directory.GetDirectoryRoot(path[0]);
            txtPathVar.text = path[0];

            pathRefresh();
        }

        private void refresh()
        {
            // Generate Folder Tree
            strLoading = "Generate new file tree";
            tree = new ListItemTree(bmsPath);

            strLoading = "Loading file and directories";
            tree.Head = tree.CreateTree(
                tree.Head, ref strLoading, musicList, encoding);

            // Save as JSON file
            strLoading = "Saving JSON File";
            if (!File.Exists(Const.JSONPath))
            {
                Directory.CreateDirectory(Directory.GetParent(Const.JSONPath).FullName);
            }
            else
            {
                File.Delete(Const.JSONPath);
            }
            File.Create(Const.JSONPath).Close();
            File.WriteAllText(Const.JSONPath, tree.JSONStr);

            // 수집한 BMS 데이터를 DB에 등록
            strLoading = "Database process...";
            MusicListManager.Instance.AddDataToDB(musicList, ref strLoading);
            
            refreshed = true;
            isRefreshing = false;
        }

        public void SetAutoSync()
        {
            if (Const.AutoSync == AutoSyncType.OFF)
            {
                Const.AutoSync = AutoSyncType.ON;
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto Sync ON";
            }
            else
            {
                Const.AutoSync = AutoSyncType.OFF;
                btnAutoSync.GetComponentInChildren<Text>().text = "Auto Sync OFF";
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void changeSync(bool up)
        {
            if (up)
            {
                Const.Sync++;
            }
            else
            {
                Const.Sync--;
            }
            showSync();
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void showSync()
        {
            int sync = Const.Sync;
            if (sync < 0)
            {
                syncval.text = sync.ToString();
            }
            else
            {
                syncval.text = "+" + sync;
            }
        }

        public void changeLang(LanguageType lang)
        {
            Const.Language = lang;
            MusicListUI.isLangChanged = true;
            switch (lang)
            {
                case LanguageType.KO:
                    curLang.text = "Korean";
                    break;
                case LanguageType.JA:
                    curLang.text = "Japanese";
                    break;
                case LanguageType.EN:
                    curLang.text = "English";
                    break;
            }
            Const.Language = lang;
            UpdateOption();
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void changeEncoding(int enc)
        {
            Const.Encoding = enc;
            switch (enc)
            {
                case 932:
                    curEncoding.text = "JP-Based";
                    break;
                case 949:
                    curEncoding.text = "KR-Based";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void changeVideoSetting()
        {
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode
            );
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void ShowKeySetting()
        {
            PlayKeySetting.KeySettingON = true;
            CloseSetting();
            MusicListUI.SetNotOnTop();
            layerKeySetting.SetActive(true);
            GetComponent<PlayKeySetting>().EnableWindow();
            sfxPlay.PlayOneShot(sfxSource);
        }
    }
}
