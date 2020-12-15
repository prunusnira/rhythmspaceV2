using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class DialogNoPattern : Setting
    {
        public Button btnOpen;
        public Button btnCloseDlg;
        public TextMeshProUGUI desc;

        public static bool LangChanged = false;

        private string URL;

        public override void Awake()
        {
            base.Awake();

            URL = "";

            btnOpen.onClick.AddListener(delegate
            {
                // 브라우저 열기
                Application.OpenURL(URL);
            });
            btnCloseDlg.onClick.AddListener(CloseSetting);

            desc.text = Const.DialogNoPattern[(int)Const.Language];
        }

        public override void Start()
        {
            base.Start();
            EncolorBtn(0, 0);
        }

        public override void Update()
        {
            base.Update();

            if (LangChanged)
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
