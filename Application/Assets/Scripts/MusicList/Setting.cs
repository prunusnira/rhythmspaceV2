using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class Setting : MonoBehaviour
    {
        // Overall
        public GameObject settingAll;
        public Text settingDesc;
        public Sprite normalBtn;
        public Sprite selectedBtn;

        protected int row;
        protected int col;
        protected int rows;
        protected int[] btn;

        public virtual void Awake()
        {
            row = 0;
            col = 0;
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {
            try
            {
                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseSetting();
                }

                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.UpArrow))
                {
                    // 옵션 항목 이동
                    if (row == 0) row = rows - 1;
                    else row--;
                    col = 0;
                    EncolorBtn(row, col);
                }

                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.DownArrow))
                {
                    // 옵션 항목 이동
                    if (row == rows - 1) row = 0;
                    else row++;
                    col = 0;
                    EncolorBtn(row, col);
                }

                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    // 옵션 데이터 이동
                    if (col == 0) col = btn[row] - 1;
                    else col--;
                    EncolorBtn(row, col);
                }

                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.RightArrow))
                {
                    // 옵션 데이터 이동
                    if (col == btn[row] - 1) col = 0;
                    else col++;
                    EncolorBtn(row, col);
                }

                if (settingAll.activeSelf && Input.GetKeyDown(KeyCode.Return))
                {
                    // 옵션 버튼 클릭
                    ExecuteOption(row, col);
                }
            }
            catch(Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        public virtual void EnableWindow()
        {
        }

        public void ChangeSprite(Button b)
        {
            b.GetComponent<Image>().sprite = selectedBtn;
        }

        public virtual void EncolorBtn(int row, int col)
        {

        }

        public virtual void ExecuteOption(int row, int col)
        {

        }

        public void CloseSetting()
        {
            settingAll.SetActive(false);
            MusicListUI.SetOnTop();
        }
    }
}
