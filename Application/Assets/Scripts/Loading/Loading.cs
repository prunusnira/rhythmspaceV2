using BMSPlayer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Async로 불러져서 데이터 로딩 중 표시
// 이 클래스는 Loading Scene을 불러오는 것을 관리하는 용도이다
public class Loading : MonoBehaviour {
    [SerializeField]
    public Image progressBar;
    private static string next;

    public TextMeshPro gerne;
    public TextMeshPro title;
    public TextMeshPro subtitle;
    public TextMeshPro artist;
    public TextMeshPro subartist;
    public TextMeshPro difficulty;
    public TextMeshPro level;
    public TextMeshPro bpm;

    void Start () {
        gerne.text = Const.selectedMusic.Gerne;
        title.text = Const.selectedMusic.Title;
        subtitle.text = Const.selectedMusic.SubTitle;
        artist.text = Const.selectedMusic.Artist;
        subartist.text = Const.selectedMusic.SubArtist;
        level.text = Const.selectedMusic.Level.ToString();

        if (Const.selectedMusic.BPMmin == Const.selectedMusic.BPMmax)
        {
            bpm.text = "BPM " + Const.selectedMusic.BPMstart;
        }
        else
        {
            bpm.text = "BPM " + Const.selectedMusic.BPMmin +
                "~" + Const.selectedMusic.BPMmax +
                " (" + Const.selectedMusic.BPMstart + " start)";
        }

        switch (Const.selectedMusic.Difficulty)
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
        StartCoroutine("LoadPlayScr");
    }

    IEnumerator LoadPlayScr()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("PlayScreen");
    }

    IEnumerator LoadAnim(TextMeshPro mesh, int idx)
    {
        yield return new WaitForSeconds(0.05f * idx);
        Animator anim = mesh.GetComponent<Animator>();
        anim.Play("SlideRtoL");
    }

    /*public void Update()
    {
        StartCoroutine(LoadScene());
    }

    public static void StartLoading(string scene)
    {
        next = scene;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation nextScene = SceneManager.LoadSceneAsync(next);
        nextScene.allowSceneActivation = false;

        float timer = 0f;
        while(!nextScene.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if(nextScene.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                
                if (progressBar.fillAmount == 1f)
                {
                    nextScene.allowSceneActivation = true;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount,
                    nextScene.progress, timer);

                Debug.Log("Loading :" + progressBar.fillAmount);

                if (progressBar.fillAmount >= nextScene.progress)
                    timer = 0f;
            }
        }
    }*/
}
