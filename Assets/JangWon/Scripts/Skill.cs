using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill : ScriptableObject
{
    //Entity who cast skill
    //public Entity entity;


    //Delay of the skill itself
    private float _coolTime;
    public float coolTime { get => _coolTime; }

    //damage value of skill
    private int _skillDamage;
    public float skillDamage { get => _skillDamage; }

    SpriteRenderer _spriteRenderer;

    protected virtual void Init(/*Entity entity*/)
    {
        //this.entity = entity;
    }

    public abstract void UseSkill();
}
