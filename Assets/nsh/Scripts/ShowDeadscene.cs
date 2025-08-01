using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowDeadscene : MonoBehaviour
{
    public Image fadeImage;           // FadePanel의 Image 컴포넌트
    public GameObject Youdied;    // "You died" 텍스트 오브젝트
    private float fadeDuration = 2f;

    void Start()
    {
        // 카메라 위치를 GameScene에서 가져온 위치로 이동
        Camera.main.transform.position = PlayerDeathKey.lastCameraPosition;

        Youdied.SetActive(false); // 시작 시 숨김
        StartCoroutine(FadeInAndShowText());
    }

    IEnumerator FadeInAndShowText()
    {
        // Fade In
        float time = 0f;
        Color color = fadeImage.color;
        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;

        // 텍스트 출력
        Youdied.SetActive(true);

        // 5초 대기 후 IntroScene으로 이동
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("IntroScene");
    }
}
