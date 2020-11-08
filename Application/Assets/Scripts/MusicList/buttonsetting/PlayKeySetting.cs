using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using BMSCore;

namespace BMSPlayer
{
    public class PlayKeySetting : Setting
    {
        public Button dfkbd;
        public Button dfds4;
        public Button dfxb;
        public Button dfpitch;
        public Button dflr2;

        public Button[] btnKeySet1;
        public Button[] btnKeySet2;

        public GameObject keyPopup;

        public Text keySettingDesc;

        public static bool KeySettingON = false;

        public override void Awake()
        {
            base.Awake();
            Keys.LoadButtonSetting();

            rows = 10;
            btn = new int[] { 4, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            //EncolorBtn(0, 0);

            // 각 버튼 onclick 설정
            dfkbd.onClick.AddListener(delegate
            {
                Keys.setKBDefault();
                UpdateButton();
            });
            dfxb.onClick.AddListener(delegate
            {
                Keys.setXBDefault();
                UpdateButton();
            });
            dfds4.onClick.AddListener(delegate
            {
                Keys.setDS4Default();
                UpdateButton();
            });
            dfpitch.onClick.AddListener(delegate
            {
                Keys.setPITCHDefault();
                UpdateButton();
            });
            dflr2.onClick.AddListener(delegate
            {
                Keys.setLR2Default();
                UpdateButton();
            });

            btnKeySet1[0].onClick.AddListener(delegate
            {
                ChangeKey(10);
            });
            btnKeySet1[1].onClick.AddListener(delegate
            {
                ChangeKey(11);
            });
            btnKeySet1[2].onClick.AddListener(delegate
            {
                ChangeKey(12);
            });
            btnKeySet1[3].onClick.AddListener(delegate
            {
                ChangeKey(13);
            });
            btnKeySet1[4].onClick.AddListener(delegate
            {
                ChangeKey(14);
            });
            btnKeySet1[5].onClick.AddListener(delegate
            {
                ChangeKey(15);
            });
            btnKeySet1[6].onClick.AddListener(delegate
            {
                ChangeKey(16);
            });
            btnKeySet1[7].onClick.AddListener(delegate
            {
                ChangeKey(17);
            });
            btnKeySet1[8].onClick.AddListener(delegate
            {
                ChangeKey(18);
            });
            btnKeySet1[9].onClick.AddListener(delegate
            {
                ChangeKey(19);
            });
            btnKeySet1[10].onClick.AddListener(delegate
            {
                ChangeKey(20);
            });

            btnKeySet2[0].onClick.AddListener(delegate
            {
                ChangeKey(30);
            });
            btnKeySet2[1].onClick.AddListener(delegate
            {
                ChangeKey(31);
            });
            btnKeySet2[2].onClick.AddListener(delegate
            {
                ChangeKey(32);
            });
            btnKeySet2[3].onClick.AddListener(delegate
            {
                ChangeKey(33);
            });
            btnKeySet2[4].onClick.AddListener(delegate
            {
                ChangeKey(34);
            });
            btnKeySet2[5].onClick.AddListener(delegate
            {
                ChangeKey(35);
            });
            btnKeySet2[6].onClick.AddListener(delegate
            {
                ChangeKey(36);
            });
            btnKeySet2[7].onClick.AddListener(delegate
            {
                ChangeKey(37);
            });
            btnKeySet2[8].onClick.AddListener(delegate
            {
                ChangeKey(38);
            });
            btnKeySet2[9].onClick.AddListener(delegate
            {
                ChangeKey(39);
            });
            btnKeySet2[10].onClick.AddListener(delegate
            {
                ChangeKey(40);
            });

            keySettingDesc.text = Const.keySettingDesc[(int)Const.Language];
        }

        public override void Start()
        {
            UpdateButton();
        }

        public override void Update()
        {
            base.Update();

            if (MusicListUI.isKeyChanged)
            {
                UpdateButton();
                MusicListUI.isKeyChanged = false;
            }

            if(KeySettingON)
            {
                ErrorHandler.LogError("Axis Y Value: "+Input.GetAxis("axisY"));
            }
        }

        public override void CloseSetting()
        {
            base.CloseSetting();
            KeySettingON = false;
        }

        public void UpdateButton()
        {
            for(int i = 0; i < 11; i++)
            {
                UpdateButtonText(btnKeySet1[i], Keys.btnSet1[i]);
                UpdateButtonText(btnKeySet2[i], Keys.btnSet2[i]);
            }
        }

        public void UpdateButtonText(Button btn, string key)
        {
            btn.GetComponentInChildren<Text>().text = key;
        }

        public void ChangeKey(int keypos)
        {
            KeyMapping.keyAssigned = false;
            Const.KeyToChange = keypos;
            keyPopup.SetActive(true);
        }
    }
}
