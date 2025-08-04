using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeCamera : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float effectDuration = 3f;

    public void FadeIn() 
    {
        if (fadeImage.color.a == 0f) return;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeAlpha(1f, 0f));
    }

    public void FadeOut() 
    {
        if (fadeImage.color.a == 1f) return;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeAlpha(0f, 1f));
    }
    
    // 정해진 시간에 따라서 화면전체를 덮은 이미지의 알파값을 서서히 변화 시키는 메소드
    public IEnumerator FadeAlpha(float from, float to) 
    {
        Color color = fadeImage.color;
        float time = 0f;

        while (time < effectDuration)
        {
            float alpha = Mathf.Lerp(from, to, time / effectDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, to);

        if (to == 0f)
            fadeImage.gameObject.SetActive(false);
    }
}
