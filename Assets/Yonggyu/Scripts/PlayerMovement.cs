using UnityEngine;

public class PlayerController : Entity
{
    // 문제 없으면 지울 예정**************



    /*private Rigidbody2D rigidBody2D;
    //private Animator animator;
    private Vector2 direction;

    //Parring
    [SerializeField] private const float parringDelay = 1.0f;
    [SerializeField] private const float invincibleDelay = 0.25f;
    public float parringStartTime = 0;

    //Attack
    [SerializeField] Skill baseAttack= null;


    protected override void TakeDamage(int damage)
    {
        if (Time.fixedTime - parringStartTime <= invincibleDelay)
        {
            Debug.Log("Parring success");
            return;
        }
        base.TakeDamage(damage);
    }
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        baseAttack = Instantiate(baseAttack);
        parringStartTime -= parringDelay;
    }

    private void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(direction.x, direction.y).normalized;
        MoveAnimation(direction);

        if (Input.GetMouseButtonDown(1))
            Parring();

        if (Input.GetMouseButtonDown(0))
            BaseAttack((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position);

    }

    private void FixedUpdate()
    {
        rigidBody2D.velocity = direction * MoveSpeed;
    }

    private void MoveAnimation(Vector2 direction) 
    {
        if(direction == Vector2.zero)
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

    private void Parring()
    {
        if (parringStartTime + parringDelay >= Time.fixedTime)
            return;
        parringStartTime = Time.fixedTime;

        Debug.Log("Parring tried at : " + parringStartTime.ToString());
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
    }*/
}
