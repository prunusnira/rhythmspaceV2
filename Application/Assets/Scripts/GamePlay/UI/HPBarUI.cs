using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class HPBarUI : MonoBehaviour
    {
        private HPController hpController;
        public TextMesh GaugeType;
        public TextMesh HP;

        public SpriteRenderer[] hpBarNR;
        public SpriteRenderer[] hpBarW125;
        public SpriteRenderer[] hpBarW150;
        private SpriteRenderer hpBar;

        public Sprite hpBarAssisted;
        public Sprite hpBarEasy;
        public Sprite hpBarNormal;
        public Sprite hpBarHard;
        public Sprite hpBarExHard;

        // Start is called before the first frame update
        void Awake()
        {
            try
            {
                switch(Const.GearSize)
                {
                    case SkinSize.STANDARD:
                        hpBar = hpBarNR[Const.PlayerSide];
                        break;
                    case SkinSize.WIDE125:
                        hpBar = hpBarW125[Const.PlayerSide];
                        break;
                    case SkinSize.WIDE150:
                        hpBar = hpBarW150[Const.PlayerSide];
                        break;
                }

                hpController = HPController.Instance;
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }
        private void Start()
        {
            SetInitialHPBar();
        }

        private void SetInitialHPBar()
        {
            switch (Const.GaugeType)
            {
                case BMSPlayer.GaugeType.ASSISTED:
                    GaugeType.text = "ASSISTED";
                    GaugeType.color = new Color(206f / 255, 159f / 255, 1f);
                    hpBar.sprite = SkinSetting.GaugeAssisted;
                    break;
                case BMSPlayer.GaugeType.EASY:
                    GaugeType.text = "EASY";
                    GaugeType.color = new Color(159f / 255, 1f, 180f / 255);
                    hpBar.sprite = SkinSetting.GaugeEasy;
                    break;
                case BMSPlayer.GaugeType.NORMAL:
                    GaugeType.text = "NORMAL";
                    GaugeType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar.sprite = SkinSetting.GaugeNormal;
                    break;
                case BMSPlayer.GaugeType.HARD:
                    GaugeType.text = "HARD";
                    GaugeType.color = new Color(1f, 159f / 255, 159f / 255);
                    hpBar.sprite = SkinSetting.GaugeHard;
                    break;
                case BMSPlayer.GaugeType.EXHARD:
                    GaugeType.text = "EX-HARD";
                    GaugeType.color = new Color(246f / 255, 1f, 159f / 255);
                    hpBar.sprite = SkinSetting.GaugeEXHard;
                    break;
                default:
                    GaugeType.text = "NORMAL";
                    GaugeType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar.sprite = SkinSetting.GaugeNormal;
                    break;
            }
        }

        public void UpdateHP(int hp)
        {
            float chp = (float)hp / hpController.HPMax;
            hpBar.material.SetFloat("_Progress", chp);
            HP.text = (chp * 100).ToString("0.00") + "%";
        }
    }
}
