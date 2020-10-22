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

        public Button[] btnKey;
        public Button[] btnCon;

        public GameObject presetLoad;
        public GameObject presetSave;
        public GameObject presetSaveInput;
        public GameObject keyPopup;

        public InputField presetSaveInputField;
        public GameObject presetSaveInputError;

        public override void Awake()
        {
            base.Awake();
            Keys.LoadButtonSetting();

            rows = 10;
            btn = new int[] { 3, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            EncolorBtn(0, 0);
        }

        public override void Start()
        {
            UpdateButton();
        }

        public override void Update()
        {
            base.Update();

            if (Const.isKeyChanged)
            {
                UpdateButton();
                Const.isKeyChanged = false;
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
                    if (col == 0) ChangeSprite(btnKey[0]);
                    else if (col == 1) ChangeSprite(btnCon[0]);
                    break;
                case 2:
                    if (col == 0) ChangeSprite(btnKey[8]);
                    else if (col == 1) ChangeSprite(btnCon[8]);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    if (col == 0) ChangeSprite(btnKey[row - 2]);
                    else if (col == 1) ChangeSprite(btnCon[row - 2]);
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
                        UpdateButton();
                    }
                    else if (col == 1)
                    {
                        Keys.setDS4Default();
                        UpdateButton();
                    }
                    else if (col == 2)
                    {
                        Keys.setXBDefault();
                        UpdateButton();
                    }
                    break;
                case 1:
                    if (col == 0) ChangeKey(10);
                    else if (col == 1) ChangeKey(20);
                    break;
                case 2:
                    if (col == 0) ChangeKey(18);
                    else if (col == 1) ChangeKey(28);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    if (col == 0) ChangeKey(10+row-2);
                    else if (col == 1) ChangeKey(20+row-2);
                    break;
            }
        }

        public void UpdateButton()
        {
            for(int i = 0; i < 9; i++)
            {
                UpdateButtonText(btnKey[i], Keys.btnSet1[i]);
                UpdateButtonText(btnCon[i], Keys.btnSet2[i]);
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
