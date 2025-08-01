using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadUI : MonoBehaviour
{
    public CanvasGroup fadeGroup;    // DeathFade ������Ʈ�� CanvasGroup ������Ʈ
    public GameObject youDiedText;   // "You Died" �ؽ�Ʈ ������Ʈ
    public GameObject retryButton;   // Retry ��ư ������Ʈ

    public float fadeDuration = 3f;

    void Start()
    {
        // ���� ���߱�(�ʿ��ϸ�)
        Time.timeScale = 1f;  // ���̵尡 ���� �۵��Ϸ��� timeScale�� 1�� �����ؾ� ��

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

            // ���� ���������� ���� (alpha 0 �� 1)
            fadeGroup.alpha = t;

            yield return null;
        }

        fadeGroup.alpha = 1f;

        // ���̵� �Ϸ� �� �ؽ�Ʈ�� ��ư Ȱ��ȭ
        youDiedText.SetActive(true);
        retryButton.SetActive(true);
    }

    // Retry ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    public void OnRetryButtonClicked()
    {
        // �� ��ȯ �� timeScale ����ȭ
        Time.timeScale = 1f;

        SceneManager.LoadScene("IntroScene");
    }
}
