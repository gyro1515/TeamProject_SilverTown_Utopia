using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected int currentHp;
    protected int MaxHp;

    protected SpriteRenderer sprite;
    protected Animator animator;

    protected int attackDamage;
    protected bool isDead;

    protected Vector2 moveDirection;
    //protected List<Skill> skills;


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

    }
}
