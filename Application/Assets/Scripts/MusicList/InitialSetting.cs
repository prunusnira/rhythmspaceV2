using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class InitialSetting : MonoBehaviour
    {
        public GameObject dlgStep1;
        public Button langEn;
        public Button langKo;
        public Button langJa;

        public GameObject dlgStep2;
        public TextMeshProUGUI descStep2;
        public Button skinNr;
        public Button skin125;
        public Button skin150;

        public GameObject dlgStep3;
        public TextMeshProUGUI descStep3;
        public Button judgeArcade;
        public Button judgeOriginal;
        public Button judgeLR2;
        public Button judgeBeatoraja;

        public GameObject dlgStep4;
        public TextMeshProUGUI descStep4;
        public Button tableRefrsh;
        public Button tableSkip;

        public GameObject dlgStep5;
        public TextMeshProUGUI descStep5;
        public Button bmsSelect;
        public Button bmsSkip;

        public void Awake()
        {
            langEn.onClick.AddListener(delegate
            {
                Const.Language = LanguageType.EN;
                MusicListUI.isLangChanged = true;
                UpdateText();
                ShowNextStep(2);
            });
            langKo.onClick.AddListener(delegate
            {
                Const.Language = LanguageType.KO;
                MusicListUI.isLangChanged = true;
                UpdateText();
                ShowNextStep(2);
            });
            langJa.onClick.AddListener(delegate
            {
                Const.Language = LanguageType.JA;
                MusicListUI.isLangChanged = true;
                UpdateText();
                ShowNextStep(2);
            });

            skinNr.onClick.AddListener(delegate
            {
                Const.GearSize = SkinSize.STANDARD;
                PlayOptionSetting.gearSizeInit = true;
                ShowNextStep(3);
            });
            skin125.onClick.AddListener(delegate
            {
                Const.GearSize = SkinSize.WIDE125;
                PlayOptionSetting.gearSizeInit = true;
                ShowNextStep(3);
            });
            skin150.onClick.AddListener(delegate
            {
                Const.GearSize = SkinSize.WIDE150;
                PlayOptionSetting.gearSizeInit = true;
                ShowNextStep(3);
            });

            judgeArcade.onClick.AddListener(delegate
            {
                Const.JudgeType = JudgeType.ARCADE;
                PlayOptionSetting.judgeInit = true;
                ShowNextStep(4);
            });
            judgeOriginal.onClick.AddListener(delegate
            {
                Const.JudgeType = JudgeType.ORIGINAL;
                PlayOptionSetting.judgeInit = true;
                ShowNextStep(4);
            });
            judgeLR2.onClick.AddListener(delegate
            {
                Const.JudgeType = JudgeType.LR2;
                PlayOptionSetting.judgeInit = true;
                ShowNextStep(4);
            });
            judgeBeatoraja.onClick.AddListener(delegate
            {
                Const.JudgeType = JudgeType.BEATORAJA;
                PlayOptionSetting.judgeInit = true;
                ShowNextStep(4);
            });

            tableRefrsh.onClick.AddListener(delegate
            {
                DialogTableEdit.refreshFromOutside = true;
                ShowNextStep(5);
            });
            tableSkip.onClick.AddListener(delegate
            {
                ShowNextStep(5);
            });

            bmsSelect.onClick.AddListener(delegate
            {
                SystemSetting.pathFromInit = true;
                ShowNextStep(0);
            });
            bmsSkip.onClick.AddListener(delegate
            {
                ShowNextStep(0);
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            if(!Const.InitialSetting)
            {
                Const.InitialSetting = true;
                // 표시하기
                dlgStep1.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void UpdateText()
        {
            descStep2.text = Const.InitSettingSkin[(int)Const.Language];
            descStep3.text = Const.InitSettingJudgeTiming[(int)Const.Language];
            descStep4.text = Const.InitSettingTable[(int)Const.Language];
            descStep5.text = Const.InitSettingBMSPath[(int)Const.Language];
        }

        private void ShowNextStep(int step)
        {
            switch(step)
            {
                case 0:
                    dlgStep5.SetActive(false);
                    break;
                case 2:
                    dlgStep1.SetActive(false);
                    dlgStep2.SetActive(true);
                    break;
                case 3:
                    dlgStep2.SetActive(false);
                    dlgStep3.SetActive(true);
                    break;
                case 4:
                    dlgStep3.SetActive(false);
                    dlgStep4.SetActive(true);
                    break;
                case 5:
                    dlgStep4.SetActive(false);
                    dlgStep5.SetActive(true);
                    break;
            }
        }
    }
}
