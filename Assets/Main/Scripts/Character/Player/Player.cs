using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    // Parring
    [Header("패링")]
    [SerializeField] private const float damageDelay = 1.0f;
    [SerializeField] private const float parringDelay = 1.0f;
    [SerializeField] private const float invincibleDelay = 0.25f;
    private float parringStartTime = 0;
    private float damageStartTime = 0;
    public int killCount = 0;

    // Jump
    [Header("점프")]
    bool isJumping = false;

    //Attack
    [SerializeField] Skill baseAttack = null;
    [SerializeField] private const float attackDelay = 1.0f;
    private float attackTime = 0.0f;

    // 방향용 Cam
    Camera cam;
    Vector2 lookDirection;
    float length = 0f;
    protected override void Awake()
    {
        base.Awake();
        if(baseAttack != null)
        baseAttack = Instantiate(baseAttack);

        parringStartTime -= parringDelay;
        damageStartTime -= damageDelay;
        attackTime -= attackDelay;
        cam = Camera.main;

        UIManager.Instance.SetHpBar((float)currentHp / MaxHp); // 체력바 세팅
    }

    /*protected override void Start()// 사용 안한다면 마지막에 지우기
    {
        base.Start();
        // Awake로 이동
        *//*baseAttack = Instantiate(baseAttack);
        parringStartTime -= parringDelay;*//*
    }*/

    protected override void Update()
    {
       /* direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(direction.x, direction.y).normalized;*/
        // 방향을 정하고 애니메이션이 실행되도록
        base.Update();

        // UI 테스트용도
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.SetSelectCardUIActive();
        }

    }

    /*protected override void FixedUpdate()
    {
        base.FixedUpdate(); // 무브는 부모에서
    }*/

    private void Parring()
    {
        if (parringStartTime + parringDelay >= Time.fixedTime)
            return;
        parringStartTime = Time.fixedTime;

        Debug.Log("Parring tried at : " + parringStartTime.ToString());
    }
    protected override void TakeDamage(int damage, bool isJumpAvoidable = false)
    {
        // 패링이 된다면 데미지를 받지 않도록
        if (Time.fixedTime - parringStartTime <= invincibleDelay)
        {
            damageStartTime = Time.fixedTime;
            return;
        }
        //점프로 공격 피하기 가능하다면, 피하기
        if (isJumping && isJumpAvoidable)
        {
            return;
        }
        if (Time.fixedTime - damageStartTime <= damageDelay)
        {
            return;
        }

        damageStartTime = Time.fixedTime;
        base.TakeDamage(damage);
        Debug.Log($"{currentHp} / {MaxHp}");
        UIManager.Instance.SetHpBar((float)currentHp / MaxHp);
    }
    private void BaseAttack(Vector2 mousepos)
    {
        if (baseAttack == null)
        {
            Debug.Log("Player BaseAttack is null");
            return;
        }
        if ((mousepos.magnitude) < 0.9f)
            baseAttack.UseSkill(this as Entity, Vector2.zero);
        else
            baseAttack.UseSkill(this as Entity, mousepos.normalized);
    }

    // 아래는 Player Input Component에서 불러와줌
    void OnMove(InputValue inputValue)
    {
        moveDirection = inputValue.Get<Vector2>();
        moveDirection = moveDirection.normalized; // 아마 자동 노멀라이즈
    }
    void OnLook(InputValue inputValue)
    {
        Vector2 mousePosition = inputValue.Get<Vector2>();
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);
        lookDirection = lookDirection.normalized;
        length = (worldPos - (Vector2)transform.position).magnitude;
    }
    void OnJump(InputValue inputValue)
    {
        // inputValue.isPressed를 안하면 키 다운, 키 업 두 번 호출 됨
        if (!inputValue.isPressed) return;
        if (isJumping)
            return;
        isJumping = true;
        Debug.Log("Player is Jumping");
        // Animation 처리
        // 반드시 애니메이션 종료 시점에 이벤트 JumpEnd() 넣어줄 것

        //<=================== Need to be removed Later ===================>
        Invoke("JumpEnd", 1.0f);
    }
    void OnWire(InputValue inputValue)
    {
        // inputValue.isPressed를 안하면 키 다운, 키 업 두 번 호출 됨
        if (!inputValue.isPressed) return;

    }
    void OnSkill1(InputValue inputValue)
    {
        // inputValue.isPressed를 안하면 키 다운, 키 업 두 번 호출 됨

        // 현재 좌클릭

        // 3초 지속
        Debug.DrawLine(transform.position, transform.position + (Vector3)lookDirection * length, Color.blue, 3.0f);
    }

    void OnBaseAttack(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (Time.fixedTime - attackTime < attackDelay)
            return;
        baseAttack.UseSkill(this, lookDirection);
    }


    //Animation Event
    public void JumpEnd()
    {
        isJumping = false;
        Debug.Log("Player Jump End");
    }
}
