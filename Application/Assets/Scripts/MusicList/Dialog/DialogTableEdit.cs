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
        public TMP_InputField urlGeNM;
        public TMP_InputField urlGeINS;

        private List<IDiffTable> diffTable;
        private List<Thread> threads;
        private bool refreshing;

        // Loading
        public GameObject layerLoading;
        public TextMeshProUGUI txtLoadingPath;
        private string strLoading = "";

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
            SetInputField();
        }

        public override void Update()
        {
            base.Update();

            txtLoadingPath.text = strLoading;

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
        }

        public void Refresh()
        {
            strLoading = "Start refreshing...";
            layerLoading.SetActive(true);

            UpdateURL();
            // 각각 테이블에 대해 갱신 수행
            // Thread로 돌림
            refreshing = true;
            diffTable.Add(new DiffTableStSl(Const.SatelliteURL, DiffTableMode.SATELLITE, ref strLoading));
            diffTable.Add(new DiffTableStSl(Const.StellaURL, DiffTableMode.STELLA, ref strLoading));
            diffTable.Add(new DiffTableGeno(Const.GenocideNormalURL, DiffTableMode.GENONM, ref strLoading));
            diffTable.Add(new DiffTableGeno(Const.GenocideInsaneURL, DiffTableMode.GENOINS, ref strLoading));

            foreach(IDiffTable d in diffTable)
            {
                Thread t = new Thread(new ThreadStart(d.CrawlTable));
                threads.Add(t);
                t.Start();
            }

            StartCoroutine(CheckThreadWork());
        }

        public void ResetTable()
        {
            PlayerPrefs.DeleteKey("diff_stella");
            PlayerPrefs.DeleteKey("diff_satellite");
            PlayerPrefs.DeleteKey("diff_genonm");
            PlayerPrefs.DeleteKey("diff_genoins");

            urlSt.text = Const.StellaURL;
            urlSl.text = Const.SatelliteURL;
            urlGeNM.text = Const.GenocideNormalURL;
            urlGeINS.text = Const.GenocideInsaneURL;
        }

        public void UpdateURL()
        {
            Const.StellaURL = urlSt.text;
            Const.SatelliteURL = urlSl.text;
            Const.GenocideNormalURL = urlGeNM.text;
            Const.GenocideInsaneURL = urlGeINS.text;
        }

        private void SetInputField()
        {
            urlSt.text = Const.StellaURL;
            urlSl.text = Const.SatelliteURL;
            urlGeNM.text = Const.GenocideNormalURL;
            urlGeINS.text = Const.GenocideInsaneURL;
        }

        System.Collections.IEnumerator CheckThreadWork()
        {
            yield return null;
            while (true)
            {
                bool cont = false;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped)
                    {
                        cont = true;
                        break;
                    }
                }
                yield return null;

                if (!cont)
                {
                    layerLoading.SetActive(false);
                    break;
                }
            }
        }
    }
}
