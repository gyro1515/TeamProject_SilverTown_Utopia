using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Skill : ScriptableObject
{
    //Entity who cast skill
    public Entity shooter;

    //Delay of the skill itself
    [SerializeField]protected float _coolTime;
    public float coolTime { get => _coolTime; }

    //damage value of skill
    [SerializeField] protected int _skillDamage;
    public int skillDamage { get => _skillDamage; }

    //direction to attack
    public Vector2 direction;

    [SerializeField] protected bool isLocal = true;

    [SerializeField] public Vector3 PositionCenter = Vector3.zero;

    public virtual void UseSkill(Entity entity, Vector2 dir)
    {
        this.shooter = entity;
        this.direction = dir.normalized;
    }
}
