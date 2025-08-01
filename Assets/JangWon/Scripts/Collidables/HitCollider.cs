using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    //HitCollider where damage will applied
    [SerializeField] Collider2D hitCollider;
    //Sprite of Warning Red Zone
    [SerializeField] SpriteRenderer warningZoneSprite;
    //Sprite of OutLine
    [SerializeField] SpriteRenderer outlineSprite;
    //xAngle Value of HitCollider -> To show Attack is stick to Map
    [SerializeField] float xAngle = 0.0f;
    //if Enable, show Hit Collider after warning delay -> usually for Floor Testing
    //if Disable, don't show Hit Collider after warning delay
    [SerializeField] bool visualizeFloor = false;
    //if Enable, Can avoid with jump
    //if not, Can't Avoid with jump
    [SerializeField] bool isJumpAvoidable = false;
    //Entity who use this skill
    Entity shooter;
    //Checker of Durations
    float start = 0.0f;
    //Duration of Warning
    float endDuration = 0.0f;
    //Duration of attack collider enable -> Time where damage will be applied
    float attackRemain = 0.0f;
    //Warning Size Increment gap for each FixedDeltaTime
    Vector3 incremental = Vector3.zero;
    //Damage of hitCollider itself
    int damage = 0;


    public void Init(Entity entity, Vector2 pos, float end, float remain, float attackAngle, int damage)
    {
        //During Warning, disable Collider
        hitCollider.enabled = false;
        //Init
        shooter = entity;
        transform.localPosition = pos;
        transform.rotation = Quaternion.Euler(xAngle, 0.0f, attackAngle);
        endDuration = end;
        attackRemain = remain;
        //Set incremental based on endDuration
        if (endDuration != 0.0f)
        {
            Vector3 originalScale = warningZoneSprite.transform.localScale;
            incremental = new Vector3(
                (1 - originalScale.x) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.y) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.z) * Time.fixedDeltaTime / (endDuration)
                );
        }
        this.damage = damage;
        //Start show Warning
        StartCoroutine(showWarning());
    }

    //Show warning zone
    public IEnumerator showWarning()
    {
        while (endDuration != 0.0f && endDuration > start)
        {
            warningZoneSprite.transform.localScale += incremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if ((endDuration == 0.0f) || (start >= endDuration))
        {
            warningZoneSprite.transform.localScale = Vector3.one;
            if (visualizeFloor)
            {
                warningZoneSprite.color = new Color(255, 0, 255, 255);
                outlineSprite.color = new Color(255, 255, 255, 255);
            }
            else
            {
                warningZoneSprite.color = new Color(255, 0, 0, 0);
                outlineSprite.color = new Color(255, 255, 255, 0);
            }

            SkillEntry enemy = shooter.GetComponent<SkillEntry>();
            //Enable Collider and Start applying damage
            ApplyDamage();
            
        }
    }

    void ApplyDamage()
    {
        hitCollider.enabled = true;
        Destroy(gameObject, attackRemain + 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if Attack Collider, skip
        if (collision.gameObject.CompareTag("Attack"))
            return;
        //if Shooter Collider, skip
        if (collision.gameObject.CompareTag(shooter.tag))
            return;
        //if Entity, Attack
        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            shooter.ApplyDamage(entity, damage, isJumpAvoidable);
        }
    }
}
