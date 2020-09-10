using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public GameObject initialFader;
    private bool isFading = false;

    private void Awake()
    {
        initialFader.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void OnGUI()
    {
        if (initialFader.GetComponent<Renderer>().material.color.a == 1.0f)
            isFading = true;

        if(isFading)
            StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        if (initialFader.GetComponent<Renderer>().material.color.a > 0.0f)
        {
            Color c = initialFader.GetComponent<Renderer>().material.color;
            c.a -= 0.05f;
            initialFader.GetComponent<Renderer>().material.color = c;
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            isFading = false;
            initialFader.SetActive(false);
            yield return null;
        }
    }
}
