using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Async로 불러져서 데이터 로딩 중 표시
// 이 클래스는 Loading Scene을 불러오는 것을 관리하는 용도이다
public class Loading : MonoBehaviour {
    [SerializeField]
    public Image progressBar;
    private static string next;

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadScene());
	}
	
	// Update is called once per frame
	void Update () {
		
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

                Debug.Log("Loading :" + progressBar.fillAmount);
                
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
    }
}
