using UnityEngine;

public class PlayerController : Entity
{
    private Rigidbody2D rigidBody2D;
    //private Animator animator;
    [SerializeField] public float MoveSpeed { get; private set; } = 5f;
    private Vector2 direction;

    //Parring
    [SerializeField] private const float parringDelay = 1.0f;
    [SerializeField] private const float invincibleDelay = 0.25f;
    public float parringStartTime = 0;


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
        parringStartTime -= parringDelay;
    }

    private void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(direction.x, direction.y).normalized;
        MoveAnimation(direction);

        if (Input.GetMouseButtonDown(0))
            Parring();

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
}
