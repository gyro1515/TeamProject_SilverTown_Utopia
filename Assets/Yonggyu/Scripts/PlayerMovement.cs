using UnityEngine;

public class PlayerController : Entity
{
    Rigidbody2D rigidBody2D;
    Animator animator;
    [SerializeField] public float MoveSpeed { get; private set; }
    private Vector2 direction;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        
    }
}
