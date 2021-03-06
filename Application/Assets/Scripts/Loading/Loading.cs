﻿using BMSCore;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Async로 불러져서 데이터 로딩 중 표시
// 이 클래스는 Loading Scene을 불러오는 것을 관리하는 용도이다
namespace BMSPlayer
{
    public class Loading : MonoBehaviour
    {
        public TextMeshProUGUI gerne;
        public TextMeshProUGUI title;
        public TextMeshProUGUI subtitle;
        public TextMeshProUGUI artist;
        public TextMeshProUGUI subartist;
        public TextMeshProUGUI difficulty;
        public TextMeshProUGUI level;
        public TextMeshProUGUI bpm;

        public TextMeshProUGUI cancelMsg;

        public Image Fader;
        private bool FadeDone = false;
        private bool FadeStart = false;
        private bool FadeReady = false;
        private bool FadeReadyStart = false;
        private bool ESCPressed = false;
        private bool SceneMoveRun = false;

        private void Awake()
        {
            Keys.LoadButtonSetting();
            cancelMsg.text = Const.StageCancel[(int)Const.Language];
        }

        void Start()
        {
            MusicListData music = Const.selectedOnList.Info;
            gerne.text = music.Gerne;
            title.text = music.Title;
            subtitle.text = music.SubTitle;
            artist.text = music.Artist;
            subartist.text = music.SubArtist;
            level.text = music.Level.ToString();

            if (music.BPMmin == music.BPMmax)
            {
                bpm.text = "BPM " + music.BPMstart.ToString("0.##");
            }
            else
            {
                bpm.text = "BPM " + music.BPMmin.ToString("0.##") +
                    "~" + music.BPMmax.ToString("0.##") +
                    " (" + music.BPMstart.ToString("0.##") + " start)";
            }

            switch (music.Difficulty)
            {
                case 1:
                    level.color = new Color(119f / 255, 255f / 255, 145f / 255);
                    difficulty.text = "BEGINNER";
                    break;
                case 2:
                    level.color = new Color(92f / 255, 120f / 255, 229f / 255);
                    difficulty.text = "NORMAL";
                    break;
                case 3:
                    level.color = new Color(255f / 255, 242f / 255, 146f / 255);
                    difficulty.text = "HYPER";
                    break;
                case 4:
                    level.color = new Color(212f / 255, 95f / 255, 95f / 255);
                    difficulty.text = "ANOTHER";
                    break;
                case 5:
                    level.color = new Color(172f / 255, 46f / 255, 178f / 255);
                    difficulty.text = "INSANE";
                    break;
                default:
                    level.color = new Color(125f / 255, 125f / 255, 125f / 255);
                    difficulty.text = "UNKNOWN";
                    break;
            }

            StartCoroutine(LoadAnim(gerne, 0));
            StartCoroutine(LoadAnim(title, 1));
            StartCoroutine(LoadAnim(subtitle, 2));
            StartCoroutine(LoadAnim(artist, 3));
            StartCoroutine(LoadAnim(subartist, 4));
            StartCoroutine(LoadAnim(difficulty, 5));
            StartCoroutine(LoadAnim(level, 6));
            StartCoroutine(LoadAnim(bpm, 7));
        }

        public void Update()
        {
            // 상태에 관계없이 5초 대기
            if (!FadeReadyStart && !FadeReady)
            {
                StartCoroutine("WaitFor5Sec");
                return;
            }

            // 5초가 되기 전에 ESC를 누름
            if (!FadeReady &&
                (Input.GetKeyDown(KeyCode.Escape) ||
                    (Keys.GetBtn(2) && Keys.GetBtn(4) && Keys.GetBtn(6))
                ))
            {
                ESCPressed = true;
                return;
            }

            // ESC를 눌렀을 때 페이드 진입
            if(ESCPressed && !FadeStart)
            {
                StartCoroutine("FadeOut");
                return;
            }

            // ESC를 누르지 않고 5초가 지났을 때 페이드 진입
            if(!ESCPressed && FadeReady && !FadeStart)
            {
                StartCoroutine("FadeOut");
                return;
            }

            // ESC를 누르고 페이드가 완료되었을 때
            if(ESCPressed && FadeDone && !SceneMoveRun)
            {
                SceneMoveRun = true;
                SceneManager.LoadScene("MusicSelect");
                return;
            }

            // ESC를 누르지 않고 페이드가 완료되었을 때
            if (!ESCPressed && FadeDone && !SceneMoveRun)
            {
                SceneMoveRun = true;
                SceneManager.LoadScene("PlayScreen");
                return;
            }
        }

        IEnumerator WaitFor5Sec()
        {
            FadeReadyStart = true;
            yield return new WaitForSeconds(5f);
            FadeReady = true;
        }

        IEnumerator LoadAnim(TextMeshProUGUI mesh, int idx)
        {
            yield return new WaitForSeconds(0.05f * idx);
            Animator anim = mesh.GetComponent<Animator>();
            anim.Play("SlideRtoL");
        }

        IEnumerator FadeOut()
        {
            FadeStart = true;
            Fader.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            for (float time = 0; time <= 1f; time += 1f / 6)
            {
                Fader.color = new Color(0f, 0f, 0f, time);
                yield return new WaitForSeconds(0.03f);
            }

            FadeDone = true;
        }
    }
}
