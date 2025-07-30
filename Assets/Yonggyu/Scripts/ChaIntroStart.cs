using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaIntroStart : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(8f, 0f, 0f);
    public Vector3 targetPosition = Vector3.zero;
    public float moveDuration = 0.5f;
    public float delayBeforeMove = 1.2f;

    private Renderer rend;
    private Collider col;

    private void Awake()
    {
        transform.position = startPosition;

        // 렌더러와 콜라이더 참조 (없어도 에러 안 나게 처리)
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(MoveToCenterAfterDelay());
    }

    IEnumerator MoveToCenterAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;

        float time = 0;
        Vector3 from = startPosition;
        Vector3 to = targetPosition;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            transform.position = Vector3.Lerp(from, to, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
    }
}
