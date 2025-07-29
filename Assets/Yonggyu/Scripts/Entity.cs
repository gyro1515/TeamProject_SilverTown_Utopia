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


    protected void TakeDamage() 
    {
    }

    protected void ApplyDamage(Entity e) 
    {
    }

    protected void Move()
    {

    }
}
