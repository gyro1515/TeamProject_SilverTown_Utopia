using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; // 카메라가 쫓아가야될 오브젝트의 트렌스폼

    [field: SerializeField] public float CameraSpeed { get; private set; } = 5.0f; // 카메라의 이동속도
    private float minBound = -5f;   // 카메라 이동제한의 최소값
    private float maxBound = 5f;    // 카메라 이동제한의 최대값

    // 카메라의 이동은 FixedUpate에서의 이동처리가 끝난후에 처리해야 하기 때문에 LateUpdate에서 처리
    private void LateUpdate()
    {
        CameraMove();   // 카메라의 움직임을 제어
    }

    // 카메라의 이동을 처리하는 메소드
    private void CameraMove()
    {
        if (targetTransform == null) return;    // 이동할 타겟오브젝트가 없다면 리턴
        Vector3 cameraPosition = targetTransform.position;  // 받아온 타겟오브젝트의 포지션값에서 세부조정을 하기 위해 변수에 담아둠
        cameraPosition.z = -10f;    // 2D의 경우 Z축으로 -10만큼 떨어져 있어야 화면이 표시된다

        // 카메라가 이동하는 위치에 제한을 둔다
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minBound, maxBound);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minBound, maxBound);

        // 카메라의 포지션값을 타겟오브젝트의 포지션값으로 대입(Lerp를 이용해서 카메라가 한발늦게 따라오는 효과를 준다)
        this.transform.position = Vector3.Lerp(
            this.transform.position,
            cameraPosition,
            Time.deltaTime * CameraSpeed);
    }

    //*************** 개인 도전과제 ***************
    // 카메라를 흔드는 효과 
    private void ShakeEffectCamera() { }

    // 카메라 줌인 줌아웃 효과
    private void ZoomEffectCamera() { }

    // 카메라 페이드 인아웃 효과
    private void FadeEffectCamera() { }
    //*************** 개인 도전과제 ***************
}
