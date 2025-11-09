using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBackground : MonoBehaviour
{
    public Color fullOpacity;
    public Color noOpacity;
    public float duration = 1.0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        fullOpacity = GetComponent<SpriteRenderer>().color;
    }

    public void LerpOpacity(float timeShiftDuration)
    {
        StartCoroutine(LerpOpacityOut(timeShiftDuration));
    }

    public IEnumerator LerpOpacityOut(float timeShiftDuration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float time = timeElapsed / duration;

            spriteRenderer.color = Color.Lerp(fullOpacity, noOpacity, time);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = noOpacity;

        yield return new WaitForSeconds(timeShiftDuration - (duration * 2));

        float secondarytimeElapsed = 0f;
        while (secondarytimeElapsed < duration)
        {
            float time = secondarytimeElapsed / duration;

            spriteRenderer.color = Color.Lerp(noOpacity, fullOpacity, time);

            secondarytimeElapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = fullOpacity;
    }
}
