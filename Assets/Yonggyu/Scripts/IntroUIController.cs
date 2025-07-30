using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroUIController : MonoBehaviour
{
    public RectTransform title;
    public RectTransform startButton;
    public RectTransform settingsButton;
    public RectTransform rankingButton;
    public RectTransform exitButton;

    public float moveDuration = 0.5f;
    public Vector2 centerUIPos = Vector2.zero;

    public void OnStartClicked()
    {
        // 1. Settings / Ranking / Exit ��ư ��Ȱ��ȭ
        settingsButton.gameObject.SetActive(false);
        rankingButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        // 2. Start ��ư �߾����� �̵� �� Ȯ��
        StartCoroutine(MoveAndScale(startButton, centerUIPos, new Vector3(1.5f, 1.5f, 1f)));

        // 3. Title �� Start ��ư �������� ����
        StartCoroutine(MoveOutLeft(title, moveDuration, 0.6f));
        StartCoroutine(MoveOutLeft(startButton, moveDuration, 0.6f));
    }

    IEnumerator MoveAndScale(RectTransform target, Vector2 targetPos, Vector3 targetScale)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector3 startScale = target.localScale;
        float time = 0;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            target.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = targetPos;
        target.localScale = targetScale;
    }

    IEnumerator MoveOutLeft(RectTransform target, float duration, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(-800f, 0);
        float time = 0;

        while (time < duration)
        {
            float t = time / duration;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = endPos;
    }
}
