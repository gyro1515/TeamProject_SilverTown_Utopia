using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //projectile RigidBody
    Rigidbody2D _rigidbody2D;
    //Who shoot projectile
    Entity shooter;
    //projectile damage
    int damage = 0;



    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    //temporary destroy
    private void Start()
    {
        Destroy(gameObject, 3.0f);
    }



    public void Init(Entity entity, Vector2 force, int damage)
    {
        shooter = entity;
        if (_rigidbody2D == null)
        {
            Debug.Log("RigidBody not exists in projectile");
            return;
        }
        _rigidbody2D.velocity = force;
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
            return;
        if (collision.CompareTag(shooter.tag))
            return;

        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            shooter.ApplyDamage(entity, damage);
        }
        Destroy(gameObject);
    }
}
