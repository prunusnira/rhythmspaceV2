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
        public Button btnPathRefreshChange;
        public Button btnPathRefreshReload;
        private string bmsPath;
        private Thread refreshChangeThread = null;
        private Thread refreshReloadThread = null;
        private int encoding;
        private bool refreshed = false;
        private bool isRefreshing = false;

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
        public Text titleResol;
        public Text txtResol;
        public Button btnResLeft;
        public Button btnResRight;

        // Frame Rate
        public Text titleFrame;
        public Text frameval;
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
            encoding = Const.Encoding;
            bmsPath = Const.BMSFolderPath;

            rows = 8;
            btn = new int[] { 2, 4, 3, 3, 3, 2, 1, 1 };

            //EncolorBtn(0, 0);

            // 각 버튼 onclick 설정
            btnPathChange.onClick.AddListener(delegate
            {
                changePath();
            });
            btnPathRefreshChange.onClick.AddListener(PathRefreshChanges);
            btnPathRefreshReload.onClick.AddListener(PathRefreshReload);

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
                frameval.text = inputFrame.text;
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
                changeEncoding(932);
            });
            btn949.onClick.AddListener(delegate
            {
                changeEncoding(949);
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

            changeLang(lang);

            frameval.text = Const.FrameRate.ToString();
            
            switch(enc)
            {
                case 932:
                    curEncoding.text = "Shift-JIS";
                    break;
                case 949:
                    curEncoding.text = "EUC-KR";
                    break;
            }
            showSync();

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

            if(pathFromInit)
            {
                pathFromInit = false;
                changePath();
            }

            if (langFromInit)
            {
                langFromInit = false;
                UpdateOption();
            }
        }

        public void UpdateOption()
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
            showSync();
            changeEncoding(Const.Encoding);
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
            btnPathRefreshChange.GetComponentInChildren<Text>().text = Const.settingBtnRefreshChange[(int)Const.Language];
            btnPathRefreshReload.GetComponentInChildren<Text>().text = Const.settingBtnRefreshReload[(int)Const.Language];
            btnFrame.GetComponentInChildren<Text>().text = Const.settingBtnFrameUpdate[(int)Const.Language];
            btn932.GetComponentInChildren<Text>().text = Const.settingBtnEncJP[(int)Const.Language];
            btn949.GetComponentInChildren<Text>().text = Const.settingBtnEncKR[(int)Const.Language];
            btnKeySetting.GetComponentInChildren<Text>().text = Const.settingBtnKeyChange[(int)Const.Language];
            btnTableEdit.GetComponentInChildren<Text>().text = Const.settingBtnTableEdit[(int)Const.Language];
            btnTableRefresh.GetComponentInChildren<Text>().text = Const.settingBtnTableRefresh[(int)Const.Language];
            btnResetAll.GetComponentInChildren<Text>().text = Const.settingBtnReset[(int)Const.Language];
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

        public void PathRefreshChanges()
        {
            if(!File.Exists(Const.JSONPath))
            {
                // 기존 파일이 존재하지 않아 갱신할 수 없음을 알림
            }
            else
            {
                if (!isRefreshing)
                {
                    isRefreshing = true;
                    layerLoading.SetActive(true);
                    refreshChangeThread = new Thread(new ThreadStart(RefreshChanges));
                    refreshChangeThread.Start();
                    sfxPlay.PlayOneShot(sfxSource);
                }
            }
        }

        public void PathRefreshReload()
        {
            if(Const.BMSFolderPath == "")
            {
                // 경로가 지정되어있지 않아 갱신할 수 없음을 알림
            }
            else
            {
                if (!isRefreshing)
                {
                    isRefreshing = true;
                    layerLoading.SetActive(true);
                    refreshReloadThread = new Thread(new ThreadStart(RefreshReload));
                    refreshReloadThread.Start();
                    sfxPlay.PlayOneShot(sfxSource);
                }
            }
        }

        public void pathCallback(string[] path)
        {
            Const.BMSFolderPath = path[0];
            bmsPath = path[0];
            BinaryFormatter formatter = new BinaryFormatter();
            string directory = Directory.GetDirectoryRoot(path[0]);
            txtPathVar.text = path[0];

            PathRefreshReload();
        }

        private void RefreshChanges()
        {
            /**
             * 여기서는 기존에 있는 structure.json 파일을 읽어서 json 구조체를 만들고
             * 전체 폴더 목록을 읽으면서 structure.json에 없는 폴더만
             * 새로 읽어서 추가하는 방식으로 진행해야 함
             */
            // 1. refreshAll과 동일하게 트리 생성 (단, bms 파일은 읽지 않음)
            // Generate Folder Tree
            strLoading = "Generate new file tree";
            ListItemTree tree = new ListItemTree(bmsPath);

            strLoading = "Loading file and directories";
            tree.Head = tree.CreateTree(
                tree.Head, ref strLoading, null, encoding, false);

            // 2. 기존의 json과 새로 만들어진 json을 상호비교하여
            //    추가할 파일과 삭제 할 파일을 결정
            // * 여기서 BMS 파일 목록을 다 불러와야 새로 추가할 파일을 더 정할 수 있지 않나...?
            JSONObject newJson = new JSONObject(tree.JSONStr);
            JSONObject oldJson = new JSONObject(File.ReadAllText(Const.JSONPath));

            // 비교를 편하게 하기 위해 각 JSON을 path의 list로 생성
            HashSet<string> newPaths = new HashSet<string>();
            tree.GetPathsFromJSON(newPaths, newJson);

            HashSet<string> oldPaths = new HashSet<string>();
            tree.GetPathsFromJSON(oldPaths, oldJson);

            strLoading = "Check duplication";
            HashSet<string> addSet = new HashSet<string>(newPaths);
            HashSet<string> rmSet = new HashSet<string>(oldPaths);
            addSet.ExceptWith(oldPaths);
            rmSet.ExceptWith(newPaths);

            List<MusicListData> addList = new List<MusicListData>();
            List<MusicListData> rmList = new List<MusicListData>();

            int idx = 0;
            foreach(string s in addSet)
            {
                strLoading = "Add files (" + idx + "/" + addSet.Count + "):\n" + s;
                if (!File.GetAttributes(s).HasFlag(FileAttributes.Directory))
                    addList.Add(MusicDataManager.Instance.LoadBMSFromPath(s, addList.Count, encoding));

                idx++;
            }

            idx = 0;
            foreach (string s in rmSet)
            {
                strLoading = "Remove files (" + idx + "/" + rmSet.Count + "):\n" + s;
                rmList.Add(new MusicListData
                {
                    Path = s
                });
                idx++;
            }

            // 3. 추가/삭제 항목에 대해 처리
            strLoading = "Add new BMS files";
            MusicDataManager.Instance.AddDataToDB(addList, ref strLoading);

            strLoading = "Deleted removed BMS files";
            MusicDataManager.Instance.RemoveDataFromDB(rmList, ref strLoading);

            // 4. JSON 파일은 새로 가져온 것으로 다시 저장
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

            refreshed = true;
            isRefreshing = false;
        }

        private void RefreshReload()
        {
            MusicDataManager.Instance.DropList();
            List<MusicListData> musicList = new List<MusicListData>();
            
            // Generate Folder Tree
            strLoading = "Generate new file tree";
            ListItemTree tree = new ListItemTree(bmsPath);

            strLoading = "Loading file and directories";
            tree.Head = tree.CreateTree(
                tree.Head, ref strLoading, musicList, encoding, true);

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
            MusicDataManager.Instance.AddDataToDB(musicList, ref strLoading);
            
            refreshed = true;
            isRefreshing = false;
        }

        public void SetAutoSync()
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

        public void ChangeSync(bool up)
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
                syncval.text = sync + " ms";
            }
            else
            {
                syncval.text = "+" + sync + " ms";
            }
        }

        public void changeLang(LanguageType lang)
        {
            Const.Language = lang;
            MusicListUI.isLangChanged = true;
            PlayOptionSetting.langChange = true;
            UpdateOption();
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void changeEncoding(int enc)
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

        public void UpdateVideoSetting()
        {
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode
            );
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void ChangeResolution(bool up)
        {
            Resolution[] res = Screen.resolutions;
            int curpos = 0;
            for(int i = 0; i < res.Length; i++)
            {
                if (res[i].height == Const.ScrHeight)
                {
                    curpos = i;
                    break;
                }
            }

            if(up)
            {
                if(curpos < res.Length - 1)
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

            Const.ScrWidth = res[curpos].width;
            Const.ScrHeight = res[curpos].height;

            UpdateVideoSetting();
        }

        public void ChangeScreenMode(bool up)
        {
            if(up)
            {
                switch(Const.ScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen:
                        Const.ScreenMode = FullScreenMode.Windowed;
                        break;
                    case FullScreenMode.MaximizedWindow:
                        Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                        break;
                    case FullScreenMode.Windowed:
                        Const.ScreenMode = FullScreenMode.MaximizedWindow;
                        break;
                }
            }
            else
            {
                switch (Const.ScreenMode)
                {
                    case FullScreenMode.ExclusiveFullScreen:
                        Const.ScreenMode = FullScreenMode.MaximizedWindow;
                        break;
                    case FullScreenMode.MaximizedWindow:
                        Const.ScreenMode = FullScreenMode.Windowed;
                        break;
                    case FullScreenMode.Windowed:
                        Const.ScreenMode = FullScreenMode.ExclusiveFullScreen;
                        break;
                }
            }

            UpdateVideoSetting();
        }

        public void ShowKeySetting()
        {
            MusicListUI.SetNotOnTop();
            layerKeySetting.SetActive(true);
            GetComponent<PlayKeySetting>().EnableWindow();
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void ShowTableEditor()
        {
            MusicListUI.SetNotOnTop();
            dlgTableEdit.SetActive(true);
            GetComponent<DialogTableEdit>().EnableWindow();
            sfxPlay.PlayOneShot(sfxSource);
        }

        public void ShowResetDlg()
        {
            MusicListUI.SetNotOnTop();
            dlgResetSetting.SetActive(true);
            sfxPlay.PlayOneShot(sfxSource);
        }
    }
}
