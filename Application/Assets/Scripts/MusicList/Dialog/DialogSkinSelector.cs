using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    class DialogSkinSelector : Setting
    {
        public TextMeshProUGUI desc;
        public Button btnOK;
        public Button btnCancel;

        public GameObject prefabSkin;

        public GameObject parentSkin;
        public GameObject parentGear;
        public GameObject parentNote;

        public TextMeshProUGUI currentSkin;
        public TextMeshProUGUI currentGear;
        public TextMeshProUGUI currentNote;

        private bool isChanged = false;

        public override void Awake()
        {
            base.Awake();

            desc.text = Const.DialogSkin[(int)Const.Language];

            btnOK.onClick.AddListener(delegate
            {
                SkinSetting.IsSkinLoaded = false;
                CloseSetting();
            });

            btnCancel.onClick.AddListener(CloseSetting);

            currentSkin.text = Const.SkinName;
            currentGear.text = Const.GearName;
            currentNote.text = Const.NoteName;
        }

        public override void Start()
        {
            base.Start();

            UpdateSelection();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void EnableWindow()
        {
            base.EnableWindow();
        }

        // Awake에서 Init된 스킨 값들(SkinListInit 참고)을 사용하여
        // 스킨 목록을 채움
        private void UpdateSelection()
        {
            foreach (string k in SkinLoader.SkinList.Keys)
            {
                GameObject obj = Instantiate(prefabSkin, parentSkin.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = k;

                obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    currentSkin.text = k;
                    Const.SkinName = k;
                    isChanged = true;
                });
            }

            foreach (string k in SkinLoader.GearList.Keys)
            {
                GameObject obj = Instantiate(prefabSkin, parentGear.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = k;

                obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    currentGear.text = k;
                    Const.GearName = k;
                    isChanged = true;
                });
            }

            foreach (string k in SkinLoader.NoteList.Keys)
            {
                GameObject obj = Instantiate(prefabSkin, parentNote.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = k;

                obj.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    currentNote.text = k;
                    Const.NoteName = k;
                    isChanged = true;
                });
            }
        }
    }
}
