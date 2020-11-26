using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class DialogQuit : Setting
    {
        public Button btnOK;
        public Button btnCancel;
        public TextMeshProUGUI desc;

        public override void Awake()
        {
            base.Awake();

            btnOK.onClick.AddListener(delegate
            {
                Application.Quit();
            });
            btnCancel.onClick.AddListener(CloseSetting);

            desc.text = Const.DialogExitMsg[(int)Const.Language];
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
