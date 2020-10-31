using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class CustomPattern : Setting
    {
        public InputField button1;
        public InputField button2;
        public InputField button3;
        public InputField button4;
        public InputField button5;
        public InputField button6;
        public InputField button7;

        public Button btnOK;
        public Button btnCancel;

        public TextMeshProUGUI desc;
        public TextMeshProUGUI msg;

        // Start is called before the first frame update
        public override void Awake()
        {
            base.Awake();

            rows = 1;
            btn = new int[] { 2 };
            
            button1.onValueChanged.AddListener(CheckValue);
            button2.onValueChanged.AddListener(CheckValue);
            button3.onValueChanged.AddListener(CheckValue);
            button4.onValueChanged.AddListener(CheckValue);
            button5.onValueChanged.AddListener(CheckValue);
            button6.onValueChanged.AddListener(CheckValue);
            button7.onValueChanged.AddListener(CheckValue);

            button1.text = "1";
            button2.text = "2";
            button3.text = "3";
            button4.text = "4";
            button5.text = "5";
            button6.text = "6";
            button7.text = "7";

            btnOK.onClick.AddListener(delegate
            {
                ExecuteOption(0, 0);
            });
            btnCancel.onClick.AddListener(delegate
            {
                ExecuteOption(0, 1);
            });
        }

        public override void Start()
        {
            base.Start();
            EncolorBtn(0, 0);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
        public override void EncolorBtn(int row, int col)
        {
            // 1. 모든 버튼의 색상 리셋
            Button[] btnlist = settingAll.GetComponentsInChildren<Button>();
            foreach (Button b in btnlist)
            {
                b.GetComponent<Image>().sprite = normalBtn;
            }

            if (col == 0) ChangeSprite(btnOK);
            else if (col == 1) ChangeSprite(btnCancel);

        }

        public override void ExecuteOption(int row, int col)
        {
            if(col == 0)
            {
                // 숫자가 정확히 나열되었는지 검사
                if(CheckAllValue())
                {
                    // 저장하고 종료
                    Const.Custom1 = int.Parse(button1.text);
                    Const.Custom2 = int.Parse(button2.text);
                    Const.Custom3 = int.Parse(button3.text);
                    Const.Custom4 = int.Parse(button4.text);
                    Const.Custom5 = int.Parse(button5.text);
                    Const.Custom6 = int.Parse(button6.text);
                    Const.Custom7 = int.Parse(button7.text);
                    MusicListUI.isCustomRandom = true;
                    CloseSetting();
                    msg.text = "";
                }
                else
                {
                    // 에러 메시지 표기
                    msg.text = Const.CustomRandomError[(int)Const.Language];
                }
            }
            else if(col == 1)
            {
                // 취소하고 나감
                CloseSetting();
                msg.text = "";
            }
        }

        // 인풋 필드에 1~7만 있는지 검사
        public void CheckValue(string value)
        {
            int num;
            bool isNum = int.TryParse(value, out num);
            if (isNum)
            {
                if (num > 7 || num < 1)
                {
                    value = "";
                }
            }
            else
            {
                value = "";
            }
        }

        private bool CheckAllValue()
        {
            List<int> all = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            int[] values = new int[]
            {
                int.Parse(button1.text),
                int.Parse(button2.text),
                int.Parse(button3.text),
                int.Parse(button4.text),
                int.Parse(button5.text),
                int.Parse(button6.text),
                int.Parse(button7.text)
            };

            foreach(int i in values)
            {
                if (all.Contains(i))
                {
                    all.Remove(i);
                }
                else break;
            }

            if(all.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void EnableWindow()
        {
            base.EnableWindow();
            desc.text = Const.CustomRandomDesc[(int)Const.Language];

            button1.text = Const.Custom1.ToString();
            button2.text = Const.Custom2.ToString();
            button3.text = Const.Custom3.ToString();
            button4.text = Const.Custom4.ToString();
            button5.text = Const.Custom5.ToString();
            button6.text = Const.Custom6.ToString();
            button7.text = Const.Custom7.ToString();
        }
    }
}
