using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowDeadscene : MonoBehaviour
{
    public Image fadeImage;           // FadePanel�� Image ������Ʈ
    public GameObject Youdied;    // "You died" �ؽ�Ʈ ������Ʈ
    private float fadeDuration = 2f;

    void Start()
    {
        // ī�޶� ��ġ�� GameScene���� ������ ��ġ�� �̵�
        Camera.main.transform.position = PlayerDeathKey.lastCameraPosition;

        Youdied.SetActive(false); // ���� �� ����
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

        // �ؽ�Ʈ ���
        Youdied.SetActive(true);

        // 5�� ��� �� IntroScene���� �̵�
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("IntroScene");
    }
}
