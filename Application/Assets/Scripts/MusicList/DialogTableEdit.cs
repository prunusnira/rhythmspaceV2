using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    class DialogTableEdit : Setting
    {
        public TextMeshProUGUI desc;
        public Button btnOK;
        public Button btnReset;
        public Button btnCancel;

        public TMP_InputField urlSt;
        public TMP_InputField urlSl;
        public TMP_InputField urlGe;

        private List<IDiffTable> diffTable;
        private List<Thread> threads;
        private bool refreshing;

        public static bool refreshFromOutside = false;

        public override void Awake()
        {
            base.Awake();

            refreshing = false;
            threads = new List<Thread>();

            desc.text = Const.DialogTableEdit[(int)Const.Language];

            btnOK.onClick.AddListener(delegate
            {
                UpdateURL();
                Refresh();
                CloseSetting();
            });

            btnReset.onClick.AddListener(ResetTable);

            btnCancel.onClick.AddListener(CloseSetting);

            diffTable = new List<IDiffTable>();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
            bool isAllDone = true;
            if(refreshing)
            {
                foreach (IDiffTable d in diffTable)
                {
                    if (!d.IsWorkDone())
                    {
                        isAllDone = false;
                        break;
                    }
                }
                if (isAllDone)
                {
                    Debug.Log("Refresh Finish");
                    threads.Clear();
                    refreshing = false;
                }
            }

            if(refreshFromOutside)
            {
                refreshFromOutside = false;
                Refresh();
            }
        }

        public override void EnableWindow()
        {
            base.EnableWindow();
            SetInputField();
        }

        public void Refresh()
        {
            UpdateURL();
            // 각각 테이블에 대해 갱신 수행
            // Thread로 돌림
            refreshing = true;
            diffTable.Add(new DiffTableStSl(Const.SatelliteURL, DiffTableMode.SATELLITE));
            diffTable.Add(new DiffTableStSl(Const.StellaURL, DiffTableMode.STELLA));
            diffTable.Add(new DiffTableGeno(Const.GenocideURL));

            foreach(IDiffTable d in diffTable)
            {
                Thread t = new Thread(new ThreadStart(d.CrawlTable));
                threads.Add(t);
                t.Start();
            }
        }

        public void ResetTable()
        {
            PlayerPrefs.DeleteKey("diff_stella");
            PlayerPrefs.DeleteKey("diff_satellite");
            PlayerPrefs.DeleteKey("diff_genocide");

            urlSt.text = Const.StellaURL;
            urlSl.text = Const.SatelliteURL;
            urlGe.text = Const.GenocideURL;
        }

        public void UpdateURL()
        {
            Const.StellaURL = urlSt.text;
            Const.SatelliteURL = urlSl.text;
            Const.GenocideURL = urlGe.text;
        }

        private void SetInputField()
        {
            urlSt.text = Const.StellaURL;
            urlSl.text = Const.SatelliteURL;
            urlGe.text = Const.GenocideURL;
        }
    }
}
