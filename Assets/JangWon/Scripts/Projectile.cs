using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //projectile RigidBody
    Rigidbody2D rigidbody2D;
    //Who shoot projectile
    string ShooterTag;
    //projectile damage
    int damage = 0;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    //temporary destroy
    private void Start()
    {
        Destroy(gameObject, 3.0f);
    }



    public void Init(/*Entity e,*/ Vector2 force, int damage)
    {
        if (rigidbody2D == null)
        {
            Debug.Log("RigidBody not exists in projectile");
            return;
        }
        rigidbody2D.velocity = force;
        this.damage = damage;

        //ShooterTag = e.gameObject.tag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ShooterTag || collision.CompareTag("Projectile"))
            return;

        //if entity, give damage
        /*
        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            entity.TakeDamage(damage)
        }
        */
        Destroy(collision.gameObject);
    }
}
