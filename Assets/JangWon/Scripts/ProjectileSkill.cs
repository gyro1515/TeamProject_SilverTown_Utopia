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
    //Entity who shoot Projectile

    //direction to shoot projectiles
    public Vector2 direction;

    public override void UseSkill()
    {
        //Temp Code
        direction = direction.normalized;
        //TempCode End
        if (projectileQuantity <= 0)
            return;
        float startangle;
        if (projectileQuantity == 1)
            startangle = 0;
        else
            startangle = (-spread / 2.0f);

        Projectile firstProjectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);

        firstProjectile.Init(RotateVector2(direction * projectileSpeed, startangle), skillDamage);
        firstProjectile.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        for (int count = 1; count < projectileQuantity; count++)
        {
            startangle += (spread / 2.0f) / (projectileQuantity / 2);
            Projectile projectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);

            projectile.Init(RotateVector2(direction * projectileSpeed, startangle), skillDamage);
        }
    }

    private Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v ;
    }


}
