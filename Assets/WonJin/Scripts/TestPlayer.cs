using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float speed = 10.0f;
    Rigidbody2D _rig2d;
    Vector2 moveDir = Vector2.zero;
    private void Start()
    {
        _rig2d = GetComponent<Rigidbody2D>();
        
    }
    private void Update()
    {
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir = (Vector3)Vector2.up ;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir = (Vector3)Vector2.down ;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector2.right;
        }

        moveDir = moveDir.normalized;


    }
    private void FixedUpdate()
    {
        _rig2d.velocity = moveDir * speed;
        
    }
}
