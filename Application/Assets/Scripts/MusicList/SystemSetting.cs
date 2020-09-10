using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GracesGames.SimpleFileBrowser.Scripts;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace BMSPlayer
{
    public class SystemSetting : Setting
    {
        // Path changer
        public Text txtPathTitle;
        public Text txtPathVar;
        public GameObject objBrowser;
        public GameObject browserBg;
        public TextMesh txtBrowserDesc;
        public string[] extensions;
        public Button btnChange;
        public Button btnRefresh;
        public GameObject layerLoading;
        private bool isRefresh;
        private bool isRefreshRun;

        // Lang changer
        public Button btnKor;
        public Button btnJpn;
        public Button btnEng;
        public Text curLang;

        // Audio
        public Button btnUnitySound;
        public Button btnFmod;
        public Text curSound;
        public Text soundDesc;

        // Sync changer
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

        public Text debugText;

        public override void Awake()
        {
            base.Awake();

            isRefresh = false;

            isRefreshRun = false;
            
            rows = 6;
            btn = new int[] { 2, 3, 2, 2, 2, 1 };

            EncolorBtn(0, 0);
        }

        public override void Start()
        {
            int lang = Const.GetLang();
            int enc = Const.GetEncoding();
            int sound = Const.GetAudio();
            int sync = Const.GetSync();

            switch(lang)
            {
                case 0:
                    curLang.text = "Korean";
                    break;
                case 1:
                    curLang.text = "Japanese";
                    break;
                case 2:
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

            switch(sound)
            {
                case 0:
                    curSound.text = "UNITY Audio";
                    break;
                case 1:
                    curSound.text = "FMOD Audio";
                    break;
            }

            showSync();
        }

        public override void Update()
        {
            base.Update();

            // refresh
            if(isRefresh)
            {
                StartCoroutine("refresh");
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
                    if (col == 0) ChangeSprite(btnKor);
                    else if (col == 1) ChangeSprite(btnJpn);
                    else if (col == 2) ChangeSprite(btnEng);
                    break;
                case 2:
                    if (col == 0) ChangeSprite(btnSyncDown);
                    else if (col == 1) ChangeSprite(btnSyncUp);
                    break;
                case 3:
                    if (col == 0) ChangeSprite(btn932);
                    else if (col == 1) ChangeSprite(btn949);
                    break;
                case 4:
                    if (col == 0) ChangeSprite(btnUnitySound);
                    else if (col == 1) ChangeSprite(btnFmod);
                    break;
                case 5:
                    ChangeSprite(btnKeySetting);
                    break;
            }
        }

        public override void ExecuteOption(int row, int col)
        {
            switch (row)
            {
                case 0:
                    if (col == 0)
                    {
                        changePath();
                    }
                    else if (col == 1)
                    {
                        setRefresh();
                    }
                    break;
                case 1:
                    changeLang(col);
                    break;
                case 2:
                    if (col == 0)
                    {
                        changeSync(false);
                    }
                    else if (col == 1)
                    {
                        changeSync(true);
                    }
                    break;
                case 3:
                    if (col == 0)
                    {
                        changeEncoding(932);
                    }
                    else if (col == 1)
                    {
                        changeEncoding(949);
                    }
                    break;
                case 4:
                    if (col == 0)
                    {
                        changeAudio(false);
                    }
                    else if (col == 1)
                    {
                        changeAudio(true);
                    }
                    break;
                case 5:
                    ShowKeySetting();
                    break;
            }
        }

        public void UpdateOption()
        {
            int lang = Const.GetLang();
            txtPathVar.text = Const.GetBMSFolderPath();
            settingDesc.text = Const.settingDesc[lang];
            showSync();
            changeEncoding(Const.GetEncoding());
            syncdesc.text = Const.settingSyncDesc[lang];
            encodingWarn.text = Const.settingChangeWarning[lang];
        }

        public void changePath()
        {
            browserBg.SetActive(true);
            txtBrowserDesc.text = Const.browserDesc[Const.GetLang()];
            /* https://github.com/GracesGames/SimpleFileBrowser */
            GameObject browser = Instantiate(objBrowser, browserBg.transform);
            browser.name = "BMS Top Path Select";

            FileBrowser browserScr = browser.GetComponent<FileBrowser>();
            browserScr.SetupFileBrowser(ViewMode.Landscape);
            browserScr.OpenFilePanel(extensions);
            browserScr.OnFileSelect += pathCallback;
            browserScr.OnFileBrowserClose += pathCancelCallback;
        }

        public void setRefresh()
        {
            layerLoading.SetActive(true);
            isRefresh = true;
        }

        public void pathCallback(string path)
        {
            Const.SetBMSFolderPath(path);
            browserBg.SetActive(false);
            BinaryFormatter formatter = new BinaryFormatter();
            string directory = Directory.GetDirectoryRoot(path);
            txtPathVar.text = path;

            layerLoading.SetActive(true);
            isRefresh = true;
        }

        public void pathCancelCallback()
        {
            browserBg.SetActive(false);
        }

        IEnumerator refresh()
        {
            if(!isRefreshRun)
            {
                isRefreshRun = true;
                Debug.Log("Refresh Start");
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                isRefreshRun = false;
                isRefresh = false;
                Debug.Log("Refreshing...");
                int rtn = MusicDataRegister.AddBMSFromPath();
                switch(rtn)
                {
                    case 0:
                        Debug.Log("Refresh Complete");
                        break;
                    case 1:
                        Debug.Log("No bms on path");
                        break;
                    case 2:
                        Debug.Log("path is empty");
                        break;
                }
                layerLoading.SetActive(false);
                Const.isRefreshDone = true;
            }
        }

        public void changeSync(bool up)
        {
            int sync = Const.GetSync();
            if (up)
            {
                Const.SetSync(sync + 1);
            }
            else
            {
                Const.SetSync(sync - 1);
            }
            showSync();
        }

        public void showSync()
        {
            int sync = Const.GetSync();
            if (sync < 0)
            {
                syncval.text = sync.ToString();
            }
            else
            {
                syncval.text = "+" + sync;
            }
        }

        public void changeLang(int lang)
        {
            Const.SetLang(lang);
            switch (lang)
            {
                case 0:
                    curLang.text = "Korean";
                    break;
                case 1:
                    curLang.text = "Japanese";
                    break;
                case 2:
                    curLang.text = "English";
                    break;
            }
            Const.SetLang(lang);
            UpdateOption();
        }

        public void changeEncoding(int enc)
        {
            Const.SetEncoding(enc);
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

        public void changeAudio(bool fmod)
        {
            if(fmod)
            {
                Const.SetAudio(1);
                curSound.text = "FMOD Audio";
            }
            else
            {
                Const.SetAudio(0);
                curSound.text = "UNITY Audio";
            }
        }

        public void ShowKeySetting()
        {
            CloseSetting();
            MusicListUI.SetNotOnTop();
            layerKeySetting.SetActive(true);
            GetComponent<PlayKeySetting>().EnableWindow();
        }
    }
}
