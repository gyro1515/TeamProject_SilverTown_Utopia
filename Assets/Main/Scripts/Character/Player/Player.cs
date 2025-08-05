using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [SerializeField] const float actionDelay = 1.0f;
    private float actionTime = 0.0f;
    [Header("추가능력치")]
    [SerializeField] public float playerDamageMultiplier = 1.0f;
    [SerializeField] public int playerExtraHealth = 0;
    // Parring
    [Header("패링")]
    [SerializeField] private const float damageDelay = 1.0f;
    [SerializeField] private const float invincibleDelay = 0.25f;
    private float parringStartTime = 0;
    private float damageStartTime = 0;
    public int killCount = 0;

    // Jump
    [Header("점프")]
    bool isJumping = false;

    //Attack
    [SerializeField] public Skill baseAttack;
    [SerializeField] public List<Skill> playerSkills;
    [SerializeField] public List<float> skillCooldown;
    [SerializeField] public List<float> activateTime;
    public Enemy closestEnemy = null;

    // 방향용 Cam
    Camera cam;
    Vector2 lookDirection;
    public Vector2 LookDirection { get { return lookDirection; } }
    float length = 0f;
    // 와이어 액션
    bool isWireActive = false;
    public bool IsWireActive { get { return isWireActive; } set { isWireActive = value; } }
    WireAction wireaction = null;

    protected override void Awake()
    {
        base.Awake();
        attackDamage = 10;
        if(baseAttack != null)
            baseAttack = Instantiate(baseAttack,transform);

        if (playerSkills.Count != skillCooldown.Count)
        {
            Debug.Log("Unmatched Skill count and Skill Cooldown Count in player");
            return;
        }
        
        for (int i = 0; i < skillCooldown.Count; i++)
        {
            playerSkills[i] = Instantiate(playerSkills[i],transform);
            activateTime[i] = -skillCooldown[i];
        }

        parringStartTime -= actionDelay;
        damageStartTime -= damageDelay;
        actionTime -= actionDelay;
        cam = Camera.main;
        wireaction = GetComponent<WireAction>();

        UIManager.Instance.SetHpBar((float)currentHp / GetMaxHP()); // 체력바 세팅
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
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isWireActive) wireaction.CheckIsWiredObstacle(collision.gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isWireActive) wireaction.CheckIsWiredObstacle(collision.gameObject);

    }*/
    
    /*protected override void FixedUpdate()
    {
        base.FixedUpdate(); // 무브는 부모에서
    }*/

    private void Parring()
    {
        if (parringStartTime + actionDelay >= Time.fixedTime)
            return;
        parringStartTime = Time.fixedTime;
        actionTime = Time.fixedTime;

    }
    protected override void TakeDamage(int damage, bool isJumpAvoidable = false, bool canParring = true)
    {
        if (canParring)
        {
            // 패링이 된다면 데미지를 받지 않도록
            if (Time.fixedTime - parringStartTime <= invincibleDelay)
            {
                damageStartTime = Time.fixedTime;
                return;
            }
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

        // 데미지를 입었을때 카메라 흔들고 데미지 애니매이션 재생
        ShakeCamera shake = Camera.main.GetComponent<ShakeCamera>();
        if (shake != null)
            StartCoroutine(shake.ShakeEffectCamera());
        gameObject.GetComponentInChildren<Animator>().Play("TakeDamage");
        UIManager.Instance.SetHpBar((float)currentHp / GetMaxHP());
    }
    private void BaseAttack()
    {
        if (baseAttack == null)
        {
            Debug.Log("Player BaseAttack is null");
            return;
        }
        actionTime = Time.fixedTime;
        if ((lookDirection.magnitude) < 0.9f)
            baseAttack.UseSkill(this as Entity, Vector2.zero);
        else
            baseAttack.UseSkill(this as Entity, lookDirection.normalized);
    }

    private void SkillAttack(int index)
    {
        if (index >= playerSkills.Count || playerSkills[index] == null)
        {
            Debug.Log("Player Skill is null");
            return;
        }
        activateTime[index] = Time.fixedTime;
        actionTime = Time.fixedTime;
        playerSkills[index].UseSkill(this as Entity, lookDirection.normalized);
    }



    // 아래는 Player Input Component에서 불러와줌
    void OnMove(InputValue inputValue)
    {
        if(isWireActive) return; // 와이어 액션 중에는 이동 불가
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
        if (isWireActive) return; // 와이어 액션 중에는 점프 불가
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

        //lookDirection 다시 계산하는 이유: 마우스 움직임이 없다면 OnLook() 호출 안됨...
        // 따라서 lookDirection따라 스킬을 사용하는 것들은 발사할때 lookDirection갱신 필요
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);
        lookDirection = lookDirection.normalized;
        wireaction.CheckCanWireAction();

    }
    void OnSkill1(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (isWireActive) return; // 와이어 액션 중에는 공격 불가
        if (Time.fixedTime - actionTime < actionDelay)
            return;

        if (0 >= playerSkills.Count || Time.fixedTime - activateTime[0] < skillCooldown[0])
            return;
        SkillAttack(0);
    }
    void OnSkill2(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (isWireActive) return; // 와이어 액션 중에는 공격 불가
        if (Time.fixedTime - actionTime < actionDelay)
            return;

        if (1 >= playerSkills.Count || Time.fixedTime - activateTime[1] < skillCooldown[1])
            return;
        SkillAttack(1);
    }
    void OnSkill3(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (isWireActive) return; // 와이어 액션 중에는 공격 불가
        if (Time.fixedTime - actionTime < actionDelay)
            return;

        if (2 >= playerSkills.Count || Time.fixedTime - activateTime[2] < skillCooldown[2])
            return;
        SkillAttack(2);
    }



    void OnBaseAttack(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        if (isWireActive) return; // 와이어 액션 중에는 공격 불가
        if (Time.fixedTime - actionTime < actionDelay)
            return;
        BaseAttack();
    }


    //Animation Event
    public void JumpEnd()
    {
        isJumping = false;
        Debug.Log("Player Jump End");
    }

    public void UpgradeHP(int hp)
    {
        this.playerExtraHealth += hp;
        this.currentHp = Mathf.Clamp(currentHp + hp, 0, GetMaxHP());
    }

    public void SetDamageMultiplier(float f)
    {
        this.playerDamageMultiplier = f;
    }

    public override int GetAttackDamage()
    {
        return (int)(attackDamage * playerDamageMultiplier);
    }

    protected override int GetMaxHP()
    {
        return base.GetMaxHP() + playerExtraHealth;
    }

    public void Levelup()
    {
        MaxHp = (int)(100 + 20 * Mathf.Sqrt(killCount));
        int HPoffset =  MaxHp - (int)(100 + 20 * Mathf.Sqrt(killCount - 1));
        this.currentHp = Mathf.Clamp(currentHp + HPoffset, 0, GetMaxHP());

        attackDamage = (int)(10 + 5 * Mathf.Sqrt(killCount));
    }
    public void SetCurHp(int addHp)
    {
        currentHp = Mathf.Clamp(currentHp + addHp, 0, GetMaxHP());
        UIManager.Instance.SetHpBar((float)currentHp / GetMaxHP());
    }
    protected override void OnDead()
    {
        base.OnDead();
        // 플레이어 죽음처리
        Debug.Log("플레이어 죽음");
    }
}
