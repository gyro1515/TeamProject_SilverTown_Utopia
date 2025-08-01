using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Skill Data", menuName = "Scriptable Object/Skill Data/ProjectileSkill")]
public class ProjectileSkill : Skill
{
    [SerializeField] Projectile projectilePrefab;

    //number of projectiile Generated
    [SerializeField] int projectileQuantity = 0;
    //Radius how to shoow projectile
    [SerializeField] float spread = 0.0f;
    //Each Projectile Speed
    [SerializeField] float projectileSpeed = 0.0f;
    [SerializeField] public Vector2 scale = Vector2.one;

    public override void UseSkill(Entity entity, Vector2 dir)
    {
        base.UseSkill(entity, dir);
        if (projectileQuantity <= 0)
            return;
        float startangle;
        if (projectileQuantity == 1)
            startangle = 0;
        else
            startangle = (-spread / 2.0f);

        int count = 0;
        do
        {
            Projectile projectile = Instantiate(projectilePrefab, positionCenter, Quaternion.identity);
            projectile.transform.localScale = scale;
            projectile.Init(shooter, RotateVector2(direction * projectileSpeed, startangle), skillDamage);
            startangle += (spread / 2.0f) / (projectileQuantity / 2);
            count++;
        } while (count < projectileQuantity);
    }

    private Vector2 RotateVector2(Vector2 vec, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * vec ;
    }


}
