using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

namespace BMSPlayer
{
    public class PlayKeySetting : Setting
    {
        public Button dfkbd;
        public Button dfds4;
        public Button dfxb;

        public Button key1k;
        public Button key1c;
        public Button key2k;
        public Button key2c;
        public Button key3k;
        public Button key3c;
        public Button key4k;
        public Button key4c;
        public Button key5k;
        public Button key5c;
        public Button key6k;
        public Button key6c;
        public Button key7k;
        public Button key7c;
        public Button key8k;
        public Button key8c;

        public GameObject presetLoad;
        public GameObject presetSave;
        public GameObject presetSaveInput;
        public GameObject keyPopup;
        public GameObject ctrlPopup;

        public InputField presetSaveInputField;
        public GameObject presetSaveInputError;

        public override void Awake()
        {
            base.Awake();
            Keys.LoadKBSetting();
            Keys.LoadSOSetting();
            loadCurrent();

            rows = 4;
            btn = new int[] { 3, 4, 6, 6 };
            EncolorBtn(0, 0);
        }

        public override void Update()
        {
            base.Update();
            setupSetting();

            if (Const.isKeyChanged)
            {
                setupButtons();
                Const.isKeyChanged = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit hit;
                if (settingAll.activeSelf && Physics.Raycast(touchPos, Vector3.forward, out hit))
                {
                    switch (hit.collider.name)
                    {
                        case "k51":
                        case "k52":
                        case "k53":
                        case "k54":
                        case "k55":
                        case "k56":
                        case "k87":
                        case "k88":
                            changeKey(hit.collider.name);
                            break;
                        case "c51":
                        case "c52":
                        case "c53":
                        case "c54":
                        case "c55":
                        case "c56":
                        case "c87":
                        case "c88":
                            changeCtrl(hit.collider.name);
                            break;
                        case "loadpreset":
                            presetLoad.SetActive(true);
                            loadPreset();
                            break;
                        case "savepreset":
                            presetSave.SetActive(true);
                            presetSaveInput.SetActive(true);
                            break;
                        case "defaultxb":
                            Keys.setXBDefault();
                            setupButtons();
                            break;
                        case "defaultp4":
                            Keys.setDS4Default();
                            setupButtons();
                            break;
                        case "closeSetting":
                            closeSetting();
                            break;
                        case "closeSave":
                            closeSave();
                            break;
                        case "closeDiff":
                            closeDiff();
                            break;
                    }
                }
            }
        }

        public override void EncolorBtn(int row, int col)
        {
            // 1. 모든 버튼의 색상 리셋
            Button[] btnlist = settingAll.GetComponentsInChildren<Button>();
            foreach (Button b in btnlist)
            {
                b.GetComponent<Image>().sprite = normalBtn;
            }

            // 2. 현재 버튼 색상 변경
            switch (row)
            {
                case 0:
                    if (col == 0) ChangeSprite(dfkbd);
                    else if (col == 1) ChangeSprite(dfds4);
                    else if (col == 2) ChangeSprite(dfxb);
                    break;
                case 1:
                    if (col == 0) ChangeSprite(key7k);
                    else if (col == 1) ChangeSprite(key7c);
                    else if (col == 2) ChangeSprite(key8k);
                    else if (col == 3) ChangeSprite(key8c);
                    break;
                case 2:
                    if (col == 0) ChangeSprite(key1k);
                    else if (col == 1) ChangeSprite(key2k);
                    else if (col == 2) ChangeSprite(key3k);
                    else if (col == 3) ChangeSprite(key4k);
                    else if (col == 4) ChangeSprite(key5k);
                    else if (col == 5) ChangeSprite(key6k);
                    break;
                case 3:
                    if (col == 0) ChangeSprite(key1c);
                    else if (col == 1) ChangeSprite(key2c);
                    else if (col == 2) ChangeSprite(key3c);
                    else if (col == 3) ChangeSprite(key4c);
                    else if (col == 4) ChangeSprite(key5c);
                    else if (col == 5) ChangeSprite(key6c);
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
                        Keys.setKBDefault();
                        setupButtons();
                    }
                    else if (col == 1)
                    {
                        Keys.setDS4Default();
                        setupButtons();
                    }
                    else if (col == 2)
                    {
                        Keys.setXBDefault();
                        setupButtons();
                    }
                    break;
                case 1:
                    if (col == 0) changeKey("7k");
                    else if (col == 1) changeKey("7c");
                    else if (col == 2) changeKey("8k");
                    else if (col == 3) changeKey("8c");
                    break;
                case 2:
                    if (col == 0) changeKey("1k");
                    else if (col == 1) changeKey("2k");
                    else if (col == 2) changeKey("3k");
                    else if (col == 3) changeKey("4k");
                    else if (col == 4) changeKey("5k");
                    else if (col == 5) changeKey("6k");
                    break;
                case 3:
                    if (col == 0) changeKey("1c");
                    else if (col == 1) changeKey("2c");
                    else if (col == 2) changeKey("3c");
                    else if (col == 3) changeKey("4c");
                    else if (col == 4) changeKey("5c");
                    else if (col == 5) changeKey("6c");
                    break;
            }
        }

        public void setupSetting()
        {
            settingDesc.text = "Current Preset: " + PlayerPrefs.GetString("keypreset", "defaultp4");
        }

        public void closeSetting()
        {
            settingAll.SetActive(false);
        }

        public void closeSave()
        {
            presetSave.SetActive(false);
            presetSaveInput.SetActive(false);
        }

        public void closeDiff()
        {
            presetLoad.SetActive(false);
        }

        public void loadDefaultXB()
        {
            Keys.setXBDefault();
            PlayerPrefs.SetString("keypreset", "defaultxb");
        }

        public void loadDefaultP4()
        {
            Keys.setDS4Default();
            PlayerPrefs.SetString("keypreset", "defaultp4");
        }

        public void loadCurrent()
        {
            if (Keys.checkDefaultFileExist())
            {
                updateKey(PlayerPrefs.GetString("keypreset", "defaultp4"));
            }
            else
            {
                loadDefaultP4();
            }

            setupButtons();
        }

        public void updateKey(string name)
        {
            Keys.loadPreset(name);
        }

        public void setupButtons()
        {
            ChangeButton(key1k, Keys.btnkb[0]);
            ChangeButton(key2k, Keys.btnkb[1]);
            ChangeButton(key3k, Keys.btnkb[2]);
            ChangeButton(key4k, Keys.btnkb[3]);
            ChangeButton(key5k, Keys.btnkb[4]);
            ChangeButton(key6k, Keys.btnkb[5]);
            ChangeButton(key7k, Keys.btnkb[6]);
            ChangeButton(key8k, Keys.btnkb[7]);

            ChangeButton(key1c, Keys.btnct[0]);
            ChangeButton(key2c, Keys.btnct[1]);
            ChangeButton(key3c, Keys.btnct[2]);
            ChangeButton(key4c, Keys.btnct[3]);
            ChangeButton(key5c, Keys.btnct[4]);
            ChangeButton(key6c, Keys.btnct[5]);
            ChangeButton(key7c, Keys.btnct[6]);
            ChangeButton(key8c, Keys.btnct[7]);
        }

        public void ChangeButton(Button btn, string key)
        {
            btn.GetComponentInChildren<Text>().text = key;
        }

        public void changeKey(string name)
        {
            Const.SetKeyChange(name);
            keyPopup.SetActive(true);
        }

        public void changeCtrl(string name)
        {
            Const.SetKeyChange(name);
            ctrlPopup.SetActive(true);
        }

        public void loadPreset()
        {
            // 파일 목록 가져오기
            if (!Directory.Exists(Application.dataPath + "/KeySetting/Preset/"))
            {
                Directory.CreateDirectory(Application.dataPath + "/KeySetting/Preset/");
            }

            string[] files = Directory.GetFiles(Application.dataPath + "/KeySetting/Preset/");
            foreach (string path in files)
            {
                if (!path.EndsWith(".meta"))
                {
                    string[] spliter = new string[2] { "/", "\\" };
                    string[] pathsplited = path.Split(spliter, StringSplitOptions.None);
                    string filename = pathsplited[pathsplited.Length - 1];
                    string presetname = pathsplited[pathsplited.Length - 1].Split('.')[0];

                    /*GameObject item = Instantiate(prefabSelector);
                    Text[] mesh = item.transform.GetComponentsInChildren<Text>();
                    mesh[0].text = presetname;
                    mesh[1].text = "";
                    mesh[2].text = "";
                    item.transform.SetParent(scrollView.transform, false);

                    item.GetComponent<Button>().onClick.AddListener(
                        delegate { KeySettings.loadPreset(presetname); setupButtons(); closeDiff(); PlayerPrefs.SetString("keypreset", presetname); }
                    );*/
                }
            }
        }

        public void savePreset()
        {
            string name = presetSaveInputField.text;
            if (name != "")
            {
                Keys.savePreset(name);
                presetSaveInput.SetActive(false);
                presetSave.SetActive(false);
            }
            else
                presetSaveInputError.SetActive(true);
        }
    }
}
