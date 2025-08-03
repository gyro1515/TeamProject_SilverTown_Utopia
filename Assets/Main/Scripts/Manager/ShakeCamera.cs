using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // 카메라가 쫓아가야될 오브젝트의 트렌스폼

    public float duration = 0.2f; // 흔들림 시간
    public float magnitude = 0.3f; // 흔들림 크기(세기)

    // 카메라를 흔드는 효과 
    public IEnumerator ShakeEffectCamera()
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

}
