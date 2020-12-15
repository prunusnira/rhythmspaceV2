using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class DialogNoPath : Setting
    {
        public Button btnCloseDlg;
        public TextMeshProUGUI desc;

        public static bool LangChanged = false;

        private string URL;

        public override void Awake()
        {
            base.Awake();
            btnCloseDlg.onClick.AddListener(CloseSetting);

            desc.text = Const.DialogNoPath[(int)Const.Language];
        }

        public override void Start()
        {
            base.Start();
            EncolorBtn(0, 0);
        }

        public override void Update()
        {
            base.Update();

            if(LangChanged)
            {
                LangChanged = false;
                desc.text = Const.DialogNoPath[(int)Const.Language];
            }
        }

        public void SetURL(string url)
        {
            URL = url;
        }
    }
}
