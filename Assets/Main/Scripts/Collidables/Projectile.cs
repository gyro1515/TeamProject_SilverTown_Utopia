using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //projectile RigidBody
    Rigidbody2D projectileRigidbody;
    //Who shoot projectile
    Entity shooter;
    //projectile damage
    int damage = 0;
    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
    }

    //At generation
    public void Init(Entity entity, Vector2 force, int damage)
    {
        shooter = entity;
        if (projectileRigidbody == null)
        {
            Debug.Log("RigidBody not exists in projectile");
            return;
        }
        //set projectile movement
        projectileRigidbody.velocity = force;
        this.damage = damage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
            return;
        //if Shooter Collider, skip
        if (collision.gameObject.CompareTag(shooter.tag))
            return;
        //if Entity, Attack
        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            shooter.ApplyDamage(entity, damage);
        }
        Destroy(gameObject);
    }
}
