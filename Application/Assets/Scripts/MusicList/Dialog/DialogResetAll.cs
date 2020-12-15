using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class DialogResetAll : Setting
    {
        public Button btnOK;
        public Button btnCancel;
        public TextMeshProUGUI desc;

        public override void Awake()
        {
            base.Awake();

            btnOK.onClick.AddListener(delegate
            {
                PlayerPrefs.DeleteAll();
                Application.Quit();
            });
            btnCancel.onClick.AddListener(CloseSetting);

            desc.text = Const.DialogResetAll[(int)Const.Language];
        }

        public override void Start()
        {
            base.Start();
            EncolorBtn(0, 0);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
