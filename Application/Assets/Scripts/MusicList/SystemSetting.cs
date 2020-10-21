using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GracesGames.SimpleFileBrowser.Scripts;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using BMSCore;

namespace BMSPlayer
{
    public class SystemSetting : Setting
    {
        // Path changer
        private MusicListManager mlm;
        private List<MusicListData> musicList;
        private ListItemTree tree;

        public Text txtPathTitle;
        public Text txtPathVar;
        public GameObject objBrowser;
        public GameObject browserBg;
        public TextMesh txtBrowserDesc;
        public string[] extensions;
        public Button btnChange;
        public Button btnRefresh;
        public GameObject layerLoading;
        public TextMesh txtLoadingPath;

        // Lang changer
        public Button btnKor;
        public Button btnJpn;
        public Button btnEng;
        public Text curLang;

        // Resolution Change
        public Button btn1080p;
        public Button btn900p;
        public Button btn768p;
        public Button btn720p;
        public Text txtResol;

        // Screen mode Change
        public Button btnWindowed;
        public Button btnFullScr;
        public Button btnBorderless;
        public Text txtScrMode;

        // Sync changer
        public Button btnAutoSync;
        public Button btnSyncUp;
        public Button btnSyncDown;
        public Text syncval;
        public Text syncdesc;

        // Encoding
        public Button btn932;
        public Button btn949;
        public Text curEncoding;
        public Text encodingWarn;

        // Key Change
        public GameObject layerKeySetting;
        public Button btnKeySetting;

        // Reset All
        public Button btnResetAll;

        public Text debugText;

        public override void Awake()
        {
            base.Awake();

            mlm = new MusicListManager();
            musicList = new List<MusicListData>();
            
            rows = 8;
            btn = new int[] { 2, 4, 3, 3, 3, 2, 1, 1 };

            EncolorBtn(0, 0);
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
        }

        public override void Update()
        {
            base.Update();
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
        }

        public void changePath()
        {
            browserBg.SetActive(true);
            txtBrowserDesc.text = Const.browserDesc[(int)Const.Language];
            /* https://github.com/GracesGames/SimpleFileBrowser */
            GameObject browser = Instantiate(objBrowser, browserBg.transform);
            browser.name = "BMS Top Path Select";

            FileBrowser browserScr = browser.GetComponent<FileBrowser>();
            browserScr.SetupFileBrowser(ViewMode.Landscape);
            browserScr.OpenFilePanel(extensions);
            browserScr.OnFileSelect += pathCallback;
            browserScr.OnFileBrowserClose += pathCancelCallback;
        }

        public void pathRefresh()
        {
            layerLoading.SetActive(true);
            StartCoroutine("refresh");
        }

        public void pathCallback(string path)
        {
            Const.BMSFolderPath = path;
            browserBg.SetActive(false);
            BinaryFormatter formatter = new BinaryFormatter();
            string directory = Directory.GetDirectoryRoot(path);
            txtPathVar.text = path;

            layerLoading.SetActive(true);
            StartCoroutine("refresh");
        }

        public void pathCancelCallback()
        {
            browserBg.SetActive(false);
        }

        IEnumerator refresh()
        {
            musicList.RemoveRange(0, musicList.Count);

            // Generate Folder Tree
            txtLoadingPath.text = "Generate new file tree";
            yield return new WaitForSeconds(0.0001f);
            tree = new ListItemTree(Const.BMSFolderPath);

            txtLoadingPath.text = "Loading file and directories";
            yield return new WaitForSeconds(0.0001f);
            tree.Head = tree.CreateTree(tree.Head);

            // Save as JSON file
            txtLoadingPath.text = "Saving JSON File";
            yield return new WaitForSeconds(0.0001f);
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

            // 실제 파일을 DB에 등록
            // Load each file
            int index = 0;
            foreach(string file in tree.FileList)
            {
                // 각 파일별로 돌면서 파일을 등록함
                txtLoadingPath.text = "Loading " + file;
                yield return new WaitForSeconds(0.0001f);
                MusicListData bmsdata = mlm.LoadBMSFromFolder(file, index);
                if(bmsdata != null) musicList.Add(bmsdata);
                index++;
            }
            yield return new WaitForSeconds(0.0001f);

            // 수집한 BMS 데이터를 DB에 등록
            txtLoadingPath.text = "Registering into database";
            yield return new WaitForSeconds(0.0001f);
            mlm = new MusicListManager();
            mlm.AddDataToDB(musicList);
            mlm.Close();

            layerLoading.SetActive(false);
            Const.isRefreshDone = true;
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
        }

        public void changeVideoSetting()
        {
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode,
                Const.ScrRefresh
            );
        }

        public void ShowKeySetting()
        {
            CloseSetting();
            MusicListUI.SetNotOnTop();
            layerKeySetting.SetActive(true);
            GetComponent<PlayKeySetting>().EnableWindow();
        }

        IEnumerator LoadFile()
        {

            yield return null;
        }
    }
}
