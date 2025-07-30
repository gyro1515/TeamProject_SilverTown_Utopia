using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill : ScriptableObject
{
    //Entity who cast skill
    public Entity shooter;

    //Delay of the skill itself
    protected float _coolTime;
    public float coolTime { get => _coolTime; }

    //damage value of skill
    [SerializeField] protected int _skillDamage;
    public int skillDamage { get => _skillDamage; }

    SpriteRenderer _spriteRenderer;


    public virtual void UseSkill(Entity entity)
    {
        this.shooter = entity;
    }
}
