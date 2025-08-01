using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntroUIController : MonoBehaviour
{
    [Header("��ư ������Ʈ��")]
    public GameObject startButton;
    public GameObject settingButton;
    public GameObject rankingButton;
    public GameObject exitButton;

    [Header("UI RectTransform")]
    public RectTransform startButtonRect;
    public RectTransform titleRect;

    [Header("UI �̵� ����")]
    public CanvasMoveController moveController; // UIMove ������Ʈ�� �ִ� ��Ʈ�ѷ�
    public float moveDuration = 0.5f;
    public float scaleAmount = 1.3f; // ��ư Ȯ�� ����

    // Start ��ư Ŭ�� �� ����
    public void OnStartButtonClicked()
    {
        StartCoroutine(FadeOutAndDisable(settingButton));
        StartCoroutine(FadeOutAndDisable(rankingButton));
        StartCoroutine(FadeOutAndDisable(exitButton));
        StartCoroutine(Sequence());
    }

    // Exit ��ư Ŭ�� �� ����
    public void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Start ��ư �̵� & Ȯ�� �� 1�� ��� �� �����̵� ����
    IEnumerator Sequence()
    {
        yield return StartCoroutine(MoveAndScaleStartButton());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SlideAndTransition());
    }

    // ��ư �����ϰ� ����� ��Ȱ��ȭ
    IEnumerator FadeOutAndDisable(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = obj.AddComponent<CanvasGroup>();
        }

        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        obj.SetActive(false);
    }

    // Start ��ư �߾����� �̵� + Ȯ��
    IEnumerator MoveAndScaleStartButton()
    {
        Vector2 startPos = startButtonRect.anchoredPosition;
        Vector2 targetPos = Vector2.zero;

        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * scaleAmount;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            startButtonRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            startButtonRect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
    }

    // Ÿ��Ʋ�� Start ��ư�� �������� �����̵� �� ĵ������ ���� �̵�
    IEnumerator SlideAndTransition()
    {
        float elapsed = 0f;
        float slowedDuration = moveDuration * 1.3f;

        Vector2 titleStart = titleRect.anchoredPosition;
        Vector2 startStart = startButtonRect.anchoredPosition;

        // ȭ�� ������ ������ �������� ����� ū �Ÿ� ����
        float slideDistance = 2000f;

        Vector2 titleTarget = titleStart + new Vector2(-slideDistance, 0);
        Vector2 startTarget = startStart + new Vector2(-slideDistance, 0);

        while (elapsed < slowedDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slowedDuration);

            titleRect.anchoredPosition = Vector2.Lerp(titleStart, titleTarget, t);
            startButtonRect.anchoredPosition = Vector2.Lerp(startStart, startTarget, t);

            yield return null;
        }

        // ������ ��ġ ������ ����
        titleRect.anchoredPosition = titleTarget;
        startButtonRect.anchoredPosition = startTarget;

        // ĵ���� ��ü �̵� (�׷� �̵�)
        moveController.SlideLeft();
    }

}
