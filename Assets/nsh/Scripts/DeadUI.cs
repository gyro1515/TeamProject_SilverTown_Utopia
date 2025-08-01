using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadUI : MonoBehaviour
{
    public CanvasGroup fadeGroup;    // DeathFade 오브젝트의 CanvasGroup 컴포넌트
    public GameObject youDiedText;   // "You Died" 텍스트 오브젝트
    public GameObject retryButton;   // Retry 버튼 오브젝트

    public float fadeDuration = 3f;

    void Start()
    {
        // 게임 멈추기(필요하면)
        Time.timeScale = 1f;  // 페이드가 정상 작동하려면 timeScale은 1로 유지해야 함

        youDiedText.SetActive(false);
        retryButton.SetActive(false);

        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // 점점 검은색으로 덮기 (alpha 0 → 1)
            fadeGroup.alpha = t;

            yield return null;
        }

        fadeGroup.alpha = 1f;

        // 페이드 완료 후 텍스트와 버튼 활성화
        youDiedText.SetActive(true);
        retryButton.SetActive(true);
    }

    // Retry 버튼 클릭 시 호출되는 함수
    public void OnRetryButtonClicked()
    {
        // 씬 전환 시 timeScale 정상화
        Time.timeScale = 1f;

        SceneManager.LoadScene("IntroScene");
    }
}
