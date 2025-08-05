using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("캐릭터 능력치")]
    [SerializeField] protected int currentHp;
    [SerializeField] protected int MaxHp;
    [SerializeField] public int attackDamage { get; protected set; }
    [SerializeField] float moveSpeed = 5f;

    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    protected bool isDead;

    protected SpriteRenderer sprite;
    protected Animator animator;
    protected Rigidbody2D rigidBody2D;

   

    protected Vector2 moveDirection;
    public Vector2 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }
    //protected List<Skill> skills;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start() // 사용 안한다면 마지막에 지우기
    {
        SetHp();
    }
    protected virtual void Update()
    {
        MoveAnimation(moveDirection);
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void TakeDamage(int damage, bool isJumpAvoidable = false, bool canParring = true) 
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, GetMaxHP());
        if (currentHp <= 0)
        {
            Invoke("OnDead", 0);
        }
    }

    public void ApplyDamage(Entity e, int damage = 0, bool isJumpAvoidable = false, bool canParring = true) 
    {
        e.TakeDamage(damage, isJumpAvoidable, canParring);
    }

    protected void Move()
    {
        rigidBody2D.velocity = moveDirection * MoveSpeed;
    }
    public virtual void MoveAnimation(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            animator.SetBool("IsMove", false);
        }
        else
        {
            animator.SetBool("IsMove", true);
            animator.SetFloat("XInput", direction.x);
            animator.SetFloat("YInput", direction.y);
        }
    }

    public virtual int GetAttackDamage()
    {
        return attackDamage;
    }

    protected virtual void OnDead()
    {
        isDead = true;
    }

    protected virtual void SetHp()
    {
        currentHp = MaxHp;
    }

    protected virtual int GetMaxHP()
    {
        return MaxHp;
    }
    

}
