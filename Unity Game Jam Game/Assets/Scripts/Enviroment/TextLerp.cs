using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextLerp : MonoBehaviour
{
    public string storyText;

    public Color fullOpacity;
    public Color noOpacity;
    public float fadeInDuration;
    public float duration;
    public GameObject textHolder;

    private bool hasBeenTriggered;
    private TextMeshProUGUI tmp;

    void Start()
    {
        tmp = textHolder.GetComponent<TextMeshProUGUI>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !hasBeenTriggered)
        {
            StartCoroutine(LerpOpacityOut());
        }
    }

    public IEnumerator LerpOpacityOut()
    {
        hasBeenTriggered = true;
        tmp.text = storyText;

        float timeElapsed = 0f;
        while (timeElapsed < fadeInDuration)
        {
            float time = timeElapsed / fadeInDuration;

            tmp.color = Color.Lerp(fullOpacity, noOpacity, time);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        tmp.color = noOpacity;

        yield return new WaitForSeconds(duration);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float secondarytimeElapsed = 0f;
        while (secondarytimeElapsed < fadeInDuration)
        {
            float time = secondarytimeElapsed / fadeInDuration;

            tmp.color = Color.Lerp(noOpacity, fullOpacity, time);

            secondarytimeElapsed += Time.deltaTime;
            yield return null;
        }
        tmp.color = fullOpacity;
    }
}
