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

    //SkillDamage
    public int skillDamage;
    float skillMultiplier = 1.0f;

    //direction to attack
    public Vector2 direction;

    //Position where attack will be held
    [SerializeField] public Vector3 positionCenter = Vector3.zero;
    [SerializeField] public GameObject animPrefab;


    public virtual void UseSkill(Entity entity, Vector2 dir)
    {
        this.shooter = entity;
        if(shooter.CompareTag("Player"))
            positionCenter = entity.transform.position + Vector3.down * 0.5f;
        this.direction = dir;
        skillDamage = entity.attackDamage + skillDamage;
        skillDamage = (int)(skillDamage * skillMultiplier);
        if (animPrefab == null) return;

    }

    public void UpgradeMultiplier(float multiscale)
    {
        if (multiscale < 0)
            return;
        skillMultiplier = multiscale;
    }
}
