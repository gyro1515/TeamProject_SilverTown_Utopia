using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WireAction : MonoBehaviour
{
    public enum EWireState
    {
        None,           // 기본 상태
        CheckDistance,  // 거리 체크
        Fire,           // 와이어 발사
        Move,           // 와이어 이동
        FailFire,      // 와이어 거리 짧거나 장애물 없음
        WirePullBack,   // 와이어 이동 발사하고 와이어 당기기
        Finish          // 이동 종료 처리
    }
    EWireState wireState = EWireState.None; // 와이어 액션 상태
    public EWireState WireState { get { return wireState; } set { wireState = value; } }
    private bool bIsCoolTime = false; // 와이어 액션 쿨타임 여부
    private float coolTimeTimer = 0f; // 와이어 액션 쿨타임 타이머

    [Header("와이어 액션 설정")]
    [SerializeField] float moveSpeedMultiple = 4f; // 와이어 이동 속도를 플레이어 속도의 몇 배로 할 것인가
    [SerializeField] float wireSpeed = 10f; // 와이어 나가는 속도 -> 와이어 이동 속돌로 통일?
    [SerializeField] float wireDistance = 10f; // 와이어 액션 가능한 거리
    [SerializeField] float coolTime = 3f; // 와이어 액션 쿨타임
    [SerializeField] GameObject wireGO; // 와이어 오브젝트
    [SerializeField] LayerMask wireLayer; // 와이어를 어디에 연결할지 레이어 설정

    private Player player; // 와이어 액션을 작동할 캐릭터
    Vector2 wirePoint; // 와이어가 연결될 지점
    Vector2 startPoint; // 와이어가 시작 지점
    float moveSpeed = 0.0f; // 와이어로 이동하는 속도
    // 와이어 발사 / 회수 / 이동에 쓰이는 타이머
    float wireTime = 0.0f; // 와이어가 발사되는 총 시간 
    float wireTimer = 0.0f; // 와이어 발사 타이머

    private void Awake()
    {
        wireLayer = LayerMask.GetMask("Obstacle"); // 와이어 레이어 설정, 현재는 인스펙터창에서 설정한 값 무시
    }
    private void Start()
    {
        player = GameManager.Instance.Player; // 게임 매니저에서 플레이어 가져오기
        moveSpeed = player.MoveSpeed * moveSpeedMultiple; // 와이어 이동속도 = 플레이어 속도 * n
    }
    private void Update()
    {
        //Debug.Log(wireState);
        // 추후에는 플레이어 인풋시스템으로 옮기기
        if(Input.GetKeyDown(KeyCode.V)) // V 키를 눌렀을 때 와이어 액션 시작
        {
            // 테스트로 무한 발동
            CheckCanWireAction(); // 와이어 액션 가능 여부 체크
        }
        switch (WireState)
        {
            case EWireState.None:
                //if (bIsCoolTime) return; // 쿨타임이면 리턴
                /*WireState = EWireState.CheckDistance; // 와이어 액션 상태를 거리 체크로 변경
                CheckCanWireAction(); // 와이어 액션 가능 여부 체크*/
                break;
            case EWireState.CheckDistance:
                // 현재 빈 공간
                break;
            case EWireState.Fire:
                FireWire(); // 와이어 발사
                break;
            case EWireState.Move:
                Move(); // 와이어 이동
                break;
            case EWireState.FailFire:
                FailFireWire(); // 허공으로 와이어 발사
                break;
            case EWireState.WirePullBack:
                WirePullBack(); // 와이어 회수
                break;
            case EWireState.Finish:
                break;
            default:
                break;
        }
    }
    void CheckCanWireAction()
    {
        startPoint = player.transform.position; // 와이어 시작 지점 = 우선은 플레이어 위치, 추후 오른손이나 지정 위치로 변경 가능
        RaycastHit2D wireHit = Physics2D.Raycast(startPoint, player.LookDirection, wireDistance, wireLayer);
        if(wireHit)
        {
            Debug.Log("와이어 액션 가능");
            wirePoint = wireHit.point; // 와이어가 연결될 지점 설정
            // 디버그 와이어 그리기
            Debug.DrawLine(startPoint, wirePoint, Color.blue, 1.0f);
            WireState = EWireState.Fire; // 와이어 액션 상태를 발사로 변경
        }
        else
        {
            Debug.Log("와이어가 짧거나, 거리 내에 장애물이 없다");
            wirePoint = startPoint + player.LookDirection * wireDistance; // 와이어 최대 거리로 발사
            WireState = EWireState.FailFire; // 와이어 액션 상태를
        }
        float dist = Vector2.Distance(startPoint, wirePoint); // 와이어 시작 지점과 도달 지점 사이의 거리
        wireTime = dist / wireSpeed; // 와이어가 도달하는데 걸리는 시간 계산
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = 0;
        wireGO.transform.localScale = wireScale;
        wireGO.transform.rotation = Quaternion.FromToRotation(Vector3.right, wirePoint - startPoint); // 와이어 오브젝트의 회전 갱신
        wireGO.SetActive(true); // 와이어 오브젝트 활성화
    }
    void FireWire()
    {
        LerfFire(); // 발사된 와이어 갱신하기
    }
    void FailFireWire()
    {
        LerfFire(); // 발사된 와이어 갱신하기
    }
    void LerfFire()
    {
        //Debug.Log("와이어 발사 중");
        wireTimer += Time.deltaTime;
        // Ver1. 와이어 동작시 플레이어 이동 금지하고, 와이어 발사되기
        /*Vector2 curPoint = Vector2.Lerp(startPoint, wirePoint, wireTimer / wireTime); // 와이어의 현재 도달 지점 갱신
        float curDist = Vector2.Distance(startPoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신*/

        // Ver2. 롤 사일러스 처럼 와이어 발사하면서 움직이기
        wireGO.transform.rotation = Quaternion.FromToRotation(Vector3.right, (Vector3)wirePoint - player.transform.position); // 와이어 오브젝트의 회전 갱신
        Vector2 curPoint = Vector2.Lerp(player.transform.position, wirePoint, wireTimer / wireTime); // 와이어의 현재 도달 지점 갱신
        float curDist = Vector2.Distance(player.transform.position, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신

        if (wireTimer < wireTime) return;
        wireTimer = 0f; // 와이어 발사 타이머 초기화
        switch (WireState)
        {
            case EWireState.Fire:
                // ver1
                //wireTime = Vector2.Distance(startPoint, wirePoint) / moveSpeed; // 도착지점까지 이동하는데 걸리는 시간

                // ver2
                wireTime = Vector2.Distance(player.transform.position, wirePoint) / moveSpeed; // 도착지점까지 이동하는데 걸리는 시간
                player.MoveAnimation(wirePoint - (Vector2)player.transform.position); // 플레이어 애니메이션도 세팅하기
                player.MoveDirection = Vector2.zero; // 와이어 이동 중에는 플레이어 이동 못하도록
                player.IsWireActive = true;
                startPoint = player.transform.position; // 와이어 시작 지점 갱신

                // 
                WireState = EWireState.Move; // 발사 다 했으면 이동하기
                break;
            case EWireState.FailFire:
                WireState = EWireState.WirePullBack; // 발사 다 하고 와이어 회수하기
                break;
        }
    }
    void Move()
    {
        // Ver1
        /*wireTimer += Time.deltaTime;
        Vector2 curPoint = Vector2.Lerp(wirePoint, startPoint, 1 - (wireTimer / wireTime));
        float curDist = Vector2.Distance(wirePoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신
        player.gameObject.transform.position = curPoint; // 플레이어 위치 갱신*/

        // Ver2
        wireTimer += Time.deltaTime;
        Vector2 curPoint = Vector2.Lerp(wirePoint, startPoint, 1 - (wireTimer / wireTime));
        float curDist = Vector2.Distance(wirePoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신
        player.gameObject.transform.position = curPoint; // 플레이어 위치 갱신

        if (wireTimer < wireTime) return;
        wireTimer = 0f; // 와이어 발사 타이머 초기화
        //wireState = EWireState.Finish; // 와이어 이동 완료
        player.IsWireActive = false;
        wireState = EWireState.None; // 와이어 이동
    }
    void WirePullBack()
    {
        wireTimer += Time.deltaTime;
        Vector2 curPoint = Vector2.Lerp(wirePoint, startPoint, wireTimer / wireTime);
        float curDist = Vector2.Distance(startPoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신
        if (wireTimer < wireTime) return;
        wireTimer = 0f; // 와이어 발사 타이머 초기화
        //wireState = EWireState.Finish; // 와이어 회수 완료
        wireState = EWireState.None; // 와이어 회수 완료
    }
}
