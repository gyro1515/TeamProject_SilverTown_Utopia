using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMoveController : MonoBehaviour
{
    public float slideDistance = 800f;
    public float slideDuration = 1f;

    public void SlideLeft()
    {
        StartCoroutine(SlideLeftCoroutine());
    }

    IEnumerator SlideLeftCoroutine()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = startPos + Vector3.left * slideDistance;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }
}
