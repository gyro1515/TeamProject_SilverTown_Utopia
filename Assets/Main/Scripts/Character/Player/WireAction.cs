using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class WireAction : MonoBehaviour
{
    public enum EWireState
    {
        None,            // 기본 상태
        //CheckDistance, // 거리 체크
        Fire,            // 와이어 발사
        Move,            // 와이어 이동
        FailFire,        // 와이어 거리 짧거나 장애물 없음
        WirePullBack,    // 와이어 발사하고 와이어 당기기
        //Finish         // 이동 종료 처리
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
    private Rigidbody2D playerRb; // 플레이어 리기드바디
    private CapsuleCollider2D playerCol;
    Vector2 wirePoint; // 와이어가 연결될 지점
    Vector2 startPoint; // 와이어가 시작 지점
    float moveSpeed = 0.0f; // 와이어로 이동하는 속도
    // 와이어 발사 / 회수 / 이동에 쓰이는 타이머
    float wireTime = 0.0f; // 와이어가 발사되는 총 시간 
    float wireTimer = 0.0f; // 와이어 발사 타이머
    GameObject colGO = null; // 와이어와 충돌한 오브젝트 -> 플레이어 충돌에서 사용

    private void Awake()
    {
        wireLayer = LayerMask.GetMask("Obstacle"); // 와이어 레이어 설정, 현재는 인스펙터창에서 설정한 값 무시
    }
    private void Start()
    {
        player = GameManager.Instance.Player; // 게임 매니저에서 플레이어 가져오기
        playerRb = player.GetComponent<Rigidbody2D>(); // 이동 제어용
        playerCol = player.GetComponent<CapsuleCollider2D>(); // 이동 제어용
        moveSpeed = player.MoveSpeed * moveSpeedMultiple; // 와이어 이동속도 = 플레이어 속도 * n
    }
    private void Update() // 자연스러움위해 픽스드로 이동?
    {
        switch (WireState)
        {
           
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
            
        }
    }
    /*private void FixedUpdate() // 충돌 부자연스러움 해결위해
    {
        switch (WireState)
        {
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
            default:
                break;
        }
        
    }*/
    
    public void CheckCanWireAction()
    {
        ReSetWire(); // 테스트로 누를때마다 초기화

        startPoint = player.transform.position; // 와이어 시작 지점 = 우선은 플레이어 위치, 추후 오른손이나 지정 위치로 변경 가능
        RaycastHit2D wireHit = Physics2D.Raycast(startPoint, player.LookDirection, wireDistance, wireLayer);
        if(wireHit)
        {
            /*Debug.Log($"전: {wireHit.point} / {wireHit.distance}");
            wireHit = Physics2D.CapsuleCast(startPoint, playerCol.size, playerCol.direction, 0, player.LookDirection, wireDistance, wireLayer);
            Debug.Log($"후: {wireHit.point} / {wireHit.distance}");*/

            wirePoint = wireHit.point; // 와이어가 연결될 지점 설정
            WireState = EWireState.Fire; // 와이어 액션 상태를 발사로 변경
            colGO = wireHit.collider.gameObject; // 와이어에 닿은 장애물 게임오브젝트 ->  와이어 이동 중, 장애물 충돌에 사용

        }
        else
        {
            wirePoint = startPoint + player.LookDirection * wireDistance; // 와이어 최대 거리로 발사
            WireState = EWireState.FailFire; // 와이어 액션 상태를 실패 발사로
            colGO = null; // 닿은 장애물 없음 판정

        }
        // 디버그 와이어 그리기
        Debug.DrawLine(startPoint, wirePoint, UnityEngine.Color.blue, 1.0f);

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
        //wireTimer += Time.fixedDeltaTime;

        // Ver1. 와이어 동작시 플레이어 이동 금지하고, 와이어 발사되기
        /*Vector2 curPoint = Vector2.Lerp(startPoint, wirePoint, wireTimer / wireTime); // 와이어의 현재 도달 지점 갱신
        float curDist = Vector2.Distance(startPoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신*/

        // Ver2. 롤 사일러스 처럼 와이어 발사하면서 움직이기
        Vector2 playerPos = player.transform.position;
        wireGO.transform.rotation = Quaternion.FromToRotation(Vector3.right, wirePoint - playerPos); // 와이어 오브젝트의 회전 갱신
        Vector2 curPoint = Vector2.Lerp(playerPos, wirePoint, wireTimer / wireTime); // 와이어의 현재 도달 지점 갱신
        float curDist = Vector2.Distance(playerPos, curPoint); // 현재 와이어의 거리
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
                wireTime = Vector2.Distance(playerPos, wirePoint) / moveSpeed; // 도착지점까지 이동하는데 걸리는 시간
                player.MoveAnimation(wirePoint - playerPos); // 플레이어 애니메이션도 세팅하기
                player.MoveDirection = Vector2.zero; // 와이어 이동 중에는 플레이어 이동 못하도록
                player.IsWireActive = true;
                startPoint = playerPos; // 와이어 시작 지점 갱신
                playerCol.isTrigger = true; // 이동 중 물리 충돌끄기
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
        //wireTimer += Time.fixedDeltaTime;
        wireTimer += Time.deltaTime;
        Vector2 curPoint = Vector2.Lerp(wirePoint, startPoint, 1 - (wireTimer / wireTime));
        float curDist = Vector2.Distance(wirePoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신

        // 플레이어 위치 갱신의 2가지 방식, 둘 다 문제 있음
        //player.gameObject.transform.position = curPoint; // 플레이어 위치 갱신
        //playerRb.MovePosition(curPoint); // 플레이어 위치 갱신을 리기드 바디로 -> 이러면 장애물 사이에 막힘

        // 또 다른 방식: 자연스러운 이동위해 움직일때마다 충돌 
        Collider2D hit = Physics2D.OverlapCapsule(curPoint, playerCol.size, playerCol.direction, 0f, wireLayer);
        //if (hit == null || hit.gameObject != colGO)
        if (hit == null)
        {
            //Debug.Log("이동");
            player.gameObject.transform.position = curPoint;
        }
        else
        {
            // 이동 중지
            //Debug.Log("이동 중지");
            ReSetWire();
        }


        if (wireTimer < wireTime) return;
        ReSetWire();
    }
    void WirePullBack()
    {
        // ver1
        /*wireTimer += Time.deltaTime;
        Vector2 curPoint = Vector2.Lerp(wirePoint, startPoint, wireTimer / wireTime);
        float curDist = Vector2.Distance(startPoint, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신
        if (wireTimer < wireTime) return;
        wireTimer = 0f; // 와이어 발사 타이머 초기화
        wireState = EWireState.None; // 와이어 회수 완료*/

        // ver2
        wireTimer += Time.deltaTime;
        //wireTimer += Time.fixedDeltaTime;
        Vector2 playerPos = player.transform.position;
        wireGO.transform.rotation = Quaternion.FromToRotation(Vector3.right, wirePoint - playerPos); // 와이어 오브젝트의 회전 갱신
        Vector2 curPoint = Vector2.Lerp(wirePoint, playerPos, wireTimer / wireTime);
        float curDist = Vector2.Distance(playerPos, curPoint); // 현재 와이어의 거리
        Vector3 wireScale = wireGO.transform.localScale; // 와이어 오브젝트의 스케일 가져오기
        wireScale.x = curDist;
        wireGO.transform.localScale = wireScale; // 와이어 오브젝트의 스케일 갱신
        if (wireTimer < wireTime) return;
        wireTimer = 0f; // 와이어 발사 타이머 초기화
        wireState = EWireState.None; // 와이어 회수 완료
    }
    void ReSetWire() // 현재는 테스트에만 쓰이나, 플레이어와 장애플 물리 충돌 부자연스러움때문에, Player가 오브젝트 충돌시에만 호출할 수 있음
    {
        wirePoint = Vector2.zero;
        startPoint = Vector2.zero;
        wireTime = 0f;
        wireTimer = 0f;
        wireState = EWireState.None;
        player.IsWireActive = false;
        colGO = null;
        playerCol.isTrigger = false;
        wireGO.SetActive(false);
    }
   /* public bool CheckIsWiredObstacle(GameObject col)
    {
        if (col == colGO) 
        { 
            Debug.Log("같은 장애물");
            ReSetWire();
            return true; 
        }
        Debug.Log("다른 장애물");
        return false;
    }*/
}
