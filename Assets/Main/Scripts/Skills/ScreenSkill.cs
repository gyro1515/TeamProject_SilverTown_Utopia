using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenSkill : MonoBehaviour
{
    [Header("스크린 스킬 설정")]
    [SerializeField] GameObject animPrefab;
    [SerializeField] Vector2 screenStartOffset = new Vector2(0, -14f); // 시작점
    [SerializeField] Vector2 screenEndOffset = new Vector2(0, -8f); // 카메라에 캐릭터가 있을 위치
    //[SerializeField] Enemy forTestEnemy; // 테스트 용
    [SerializeField] float flyTime = 1.0f;
    [SerializeField] float moveToCamTime = 1.0f;
    [SerializeField] float attackTime = 1.7f;
    [SerializeField] float attackInterval = 0.05f;
    [SerializeField] float rayDist = 1.0f; // 플레이어와 장애물이 얼마나 떨어져있을 수 있는가
    GameObject skillObject = null;
    Camera cam;
    float timer = 0.0f;
    float attackTimer = 0.0f;
    float animLength = -1.0f;
    Vector2 startFlyPos = Vector2.zero;
    Vector2 landingStartPos = Vector2.zero;

    public enum EScreenSKillState
    {
        None, Fly, MoveToCamera, UseSkill, AwayFromCamera, Landing, Finish
    }
    EScreenSKillState skillState = EScreenSKillState.None;
    Enemy enemy = null;
    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        // 테스트
        /*if(Input.GetKeyDown(KeyCode.Y) && skillState == EScreenSKillState.None)
        {
            UseScreenSkill();
        }*/

        switch (skillState)
        {
            case EScreenSKillState.Fly: //  위로 날아가기
                Fly();
                break;
            case EScreenSKillState.MoveToCamera: // 카메라 아래에서 올라오기
                MoveToCamera();
                break;
            case EScreenSKillState.UseSkill: // 스킬 시전 = 애니메이션 재생
                UseSkill();
                break;
            case EScreenSKillState.AwayFromCamera: // 카메로 아래로 내려가기
                AwayFromCamera();
                break;
            case EScreenSKillState.Landing: // 다시 착지까지
                Landing();
                break;
        }
    }
    public void Init(Enemy _enemy)
    {
        enemy = _enemy;
    }
    public void UseScreenSkill()
    {
        skillState = EScreenSKillState.Fly;
        // 애니메이션 세팅
        enemy.MoveAnimation(Vector2.up);
        enemy.MoveAnimation(Vector2.zero);
        enemy.IsScreenPattern = false;
        startFlyPos = enemy.gameObject.transform.position;
        //enabled = true; 추후 이 스킬이 사용될때만 업데이트 활성화 하기, 현재는 업데이트에서 테스트 키입력 받음
    }
    void Fly()
    {
        timer += Time.deltaTime;
        Vector2 camPos = cam.transform.position;
        camPos.x = enemy.transform.position.x;
        camPos.y += 15f;

        Vector2 lerpPos = Vector2.Lerp(enemy.transform.position, camPos, timer / flyTime);
        enemy.gameObject.transform.position = lerpPos;
        if (timer < flyTime) return;
        timer = 0.0f;
        skillState = EScreenSKillState.MoveToCamera;
    }
    void MoveToCamera()
    {
        timer += Time.deltaTime;
        Vector2 camStartPos = (Vector2)cam.transform.position + screenStartOffset;
        Vector2 camEndPos = (Vector2)cam.transform.position + screenEndOffset;

        Vector2 lerpPos = Vector2.Lerp(camStartPos, camEndPos, timer / moveToCamTime);
        enemy.gameObject.transform.position = lerpPos;
        if (timer < flyTime) return;
        timer = 0.0f;
        skillState = EScreenSKillState.UseSkill;

        // 메인 UI 잠깐 끄기
        UIManager.Instance.SetMainUIActive(false);
        Vector3 animPos = cam.transform.position;
        animPos.z = 0;
        skillObject = Instantiate(animPrefab, animPos, Quaternion.Euler(0, 0, 180f), this.transform);
        enemy.MoveAnimation(Vector2.up);
        enemy.MoveAnimation(Vector2.zero);
        AnimatorStateInfo stateInfo = skillObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        animLength = stateInfo.length; // 애니메이션 재생 길이
        StartCoroutine(DestroySkillObject(animLength + 0.05f)); // 초 후에 애니메이션 오브젝트 파괴 후, 메인UI 활성화
    }
    void UseSkill()
    {
        // 위치 고정
        Vector2 camEndPos = (Vector2)cam.transform.position + screenEndOffset;
        enemy.gameObject.transform.position = camEndPos;

        timer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackTime)
        {
            attackTimer = 0.0f;
            timer = 0.0f;
            skillState = EScreenSKillState.AwayFromCamera; // 카메라부터 다시 멀어지기
            return;
        }
        if (timer < attackInterval) return;
        timer -= attackInterval;
        ApplyDamage();
    }
    void ApplyDamage()
    {

        RaycastHit2D hit = Physics2D.Raycast(enemy.target.transform.position, Vector2.down, rayDist, LayerMask.GetMask("Obstacle")); // 플레이어 아래 방향에 오브젝트가 존재하는가
        Debug.DrawLine(enemy.target.transform.position, enemy.target.transform.position + Vector3.down * rayDist);
        if(!hit)
        {
            enemy.ApplyDamage(enemy.target, 9999, false, false);
        }
    }
    void AwayFromCamera()
    {
        timer += Time.deltaTime;
        Vector2 camStartPos = (Vector2)cam.transform.position + screenStartOffset;
        Vector2 camEndPos = (Vector2)cam.transform.position + screenEndOffset;

        Vector2 lerpPos = Vector2.Lerp(camEndPos, camStartPos, timer / moveToCamTime);
        enemy.gameObject.transform.position = lerpPos;
        if (timer < flyTime) return;
        timer = 0.0f;
        skillState = EScreenSKillState.Landing;
        landingStartPos = (Vector2)cam.transform.position;
        landingStartPos.x = startFlyPos.x;
        landingStartPos.y += 15f;
        enemy.MoveAnimation(Vector2.down);
        enemy.MoveAnimation(Vector2.zero);
        UIManager.Instance.SetMainUIActive(true);

    }
    void Landing()
    {
        timer += Time.deltaTime;
        Vector2 lerpPos = Vector2.Lerp(landingStartPos, startFlyPos, timer / flyTime);
        enemy.gameObject.transform.position = lerpPos;
        if (timer < flyTime) return;
        timer = 0.0f;
        skillState = EScreenSKillState.None; // 스킬 종료
        ShakeCamera shake = Camera.main.GetComponent<ShakeCamera>();
        StartCoroutine(shake?.ShakeEffectCamera()); // 착지 흔들림 추가
        enemy.isPatternEnd = true;
        enemy.IsScreenPattern = true;

    }
    IEnumerator DestroySkillObject(float timeLength)
    {
        yield return new WaitForSeconds(timeLength);
        Destroy(skillObject); // 애니메이션 파괴
        //enabled = false; // 테스트 끝나면 업데이트 안되게 하기
    }
}
