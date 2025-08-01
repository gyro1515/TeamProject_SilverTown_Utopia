using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntroUIController : MonoBehaviour
{
    [Header("버튼 오브젝트들")]
    public GameObject startButton;
    public GameObject settingButton;
    public GameObject rankingButton;
    public GameObject exitButton;

    [Header("UI RectTransform")]
    public RectTransform startButtonRect;
    public RectTransform titleRect;

    [Header("UI 이동 관련")]
    public CanvasMoveController moveController; // UIMove 오브젝트에 있는 컨트롤러
    public float moveDuration = 0.5f;
    public float scaleAmount = 1.3f; // 버튼 확대 비율

    // Start 버튼 클릭 시 실행
    public void OnStartButtonClicked()
    {
        StartCoroutine(FadeOutAndDisable(settingButton));
        StartCoroutine(FadeOutAndDisable(rankingButton));
        StartCoroutine(FadeOutAndDisable(exitButton));
        StartCoroutine(Sequence());
    }

    // Exit 버튼 클릭 시 실행
    public void OnExitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Start 버튼 이동 & 확대 → 1초 대기 → 슬라이드 실행
    IEnumerator Sequence()
    {
        yield return StartCoroutine(MoveAndScaleStartButton());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SlideAndTransition());
    }

    // 버튼 투명하게 만들고 비활성화
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

    // Start 버튼 중앙으로 이동 + 확대
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

    // 타이틀과 Start 버튼을 왼쪽으로 슬라이드 → 캔버스도 같이 이동
    IEnumerator SlideAndTransition()
    {
        float elapsed = 0f;
        float slowedDuration = moveDuration * 1.3f;

        Vector2 titleStart = titleRect.anchoredPosition;
        Vector2 startStart = startButtonRect.anchoredPosition;

        // 화면 밖으로 완전히 나가도록 충분히 큰 거리 설정
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

        // 마지막 위치 강제로 보정
        titleRect.anchoredPosition = titleTarget;
        startButtonRect.anchoredPosition = startTarget;

        // 캔버스 전체 이동 (그룹 이동)
        moveController.SlideLeft();
    }

}
