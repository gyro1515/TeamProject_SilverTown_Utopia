using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("캐릭터 능력치")]
    [SerializeField] protected int currentHp;
    [SerializeField] protected int MaxHp;
    [SerializeField] protected int attackDamage;
    [SerializeField] float moveSpeed = 5f;

    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    protected bool isDead;

    protected SpriteRenderer sprite;
    protected Animator animator;
    protected Rigidbody2D rigidBody2D;

   

    protected Vector2 moveDirection;
    //protected List<Skill> skills;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start() // 사용 안한다면 마지막에 지우기
    {

    }
    protected virtual void Update()
    {
        MoveAnimation(moveDirection);
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void TakeDamage(int damage) 
    {
        Debug.Log("Entity " + this.gameObject.name + " Took " +damage.ToString() + "Damage");
    }

    public void ApplyDamage(Entity e, int damage = 0) 
    {
        e.TakeDamage(damage);
    }

    protected void Move()
    {
        rigidBody2D.velocity = moveDirection * MoveSpeed;
    }
    protected virtual void MoveAnimation(Vector2 direction)
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
}
