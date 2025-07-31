using UnityEngine;

public class MoveTest : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    [SerializeField] public float MoveSpeed { get; private set; } = 5f;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 direction;
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(direction.x, direction.y).normalized;
        MoveAnimation(direction);
        rigidBody2D.velocity = direction * MoveSpeed;
    }

    private void FixedUpdate()
    {

    }

    private void MoveAnimation(Vector2 direction)
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
