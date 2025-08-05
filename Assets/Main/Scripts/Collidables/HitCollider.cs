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
    //Bind Animation Rotation
    [SerializeField] bool isAnimRotationFixed = false;
    float angle;
    //Entity who use this skill
    Entity shooter;
    //Checker of Durations
    float start = 0.0f;
    //Duration of Warning
    float warningDuration = 0.0f;
    //Duration of attack collider enable -> Time where damage will be applied
    float attackRemain = 0.0f;
    //Warning Size Increment gap for each FixedDeltaTime
    Vector3 incremental = Vector3.zero;
    //Damage of hitCollider itself
    int damage = 0;
    //For Sleeping rigidbody mode
    List<Rigidbody2D> rigidbody2Ds = new List<Rigidbody2D>();
    // 스킬 게임오브젝트 임시 저장용
    GameObject skillObject = null;
    // 쿼터써클은 스킬 이펙트 소환 위치를 다르게
    public enum HitColliderShape
    {
        Cone, Other
    }

    public void Init(Entity entity, Vector2 pos, float warningEnd, float remain, float attackAngle, int damage, GameObject animPrefab, bool isanimfixed = false)
    {
        //During Warning, disable Collider
        hitCollider.enabled = false;
        //Init
        shooter = entity;
        transform.localPosition = pos;
        angle = attackAngle;
        transform.rotation = Quaternion.Euler(xAngle, 0.0f, attackAngle);
        warningDuration = warningEnd;
        attackRemain = remain;
        //Set incremental based on endDuration
        if (warningDuration != 0.0f)
        {
            Vector3 originalScale = warningZoneSprite.transform.localScale;
            incremental = new Vector3(
                (1 - originalScale.x) * Time.fixedDeltaTime / (warningDuration),
                (1 - originalScale.y) * Time.fixedDeltaTime / (warningDuration),
                (1 - originalScale.z) * Time.fixedDeltaTime / (warningDuration)
                );
        }
        this.damage = damage;
        //Start show Warning
        StartCoroutine(showWarning(animPrefab));
        isAnimRotationFixed = isanimfixed;
    }

    //Show warning zone
    public IEnumerator showWarning(GameObject animPrefab)
    {
        while (warningDuration != 0.0f && warningDuration > start)
        {
            warningZoneSprite.transform.localScale += incremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if ((warningDuration == 0.0f) || (start >= warningDuration))
        {
            warningZoneSprite.transform.localScale = Vector3.one;
            if (!visualizeFloor)
            {
                warningZoneSprite.color = new Color(255, 0, 0, 0);
                outlineSprite.color = new Color(255, 255, 255, 0);
            }

            Enemy enemy = shooter.GetComponent<Enemy>();
            
            if (animPrefab != null)
            {
                //Debug.Log(animPrefab.name);
                skillObject = Instantiate(animPrefab);
                if (!isAnimRotationFixed)
                    skillObject.gameObject.transform.rotation = transform.rotation;
                else
                    skillObject.transform.Rotate(Vector3.forward, angle);
                skillObject.gameObject.transform.position = transform.position;

                /*Vector3 rot = transform.rotation.eulerAngles;
                rot.z += 90.0f;
                
                skillObject = Instantiate(animPrefab, transform.position, Quaternion.Euler(rot));*/

                /*if (hitCollider.GetComponent<RectTransform>() == null)
                {
                    skillObject = Instantiate(animPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.Log($"{hitCollider.GetComponent<RectTransform>().anchoredPosition} / {hitCollider.GetComponent<RectTransform>().localPosition} / {transform.position} / {transform.localPosition}");
                    skillObject = Instantiate(animPrefab, hitCollider.GetComponent<RectTransform>().anchoredPosition, Quaternion.identity);
                }*/
            }
            //Enable Collider and Start applying damage
            ApplyDamage();
        }
    }

    void ApplyDamage()
    {
        hitCollider.enabled = true;
        Destroy(gameObject, attackRemain + 0.1f);
        if (skillObject != null)
        {
            AnimatorStateInfo stateInfo = skillObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float animLength = stateInfo.length;
            Destroy(skillObject, (attackRemain > animLength ? attackRemain : animLength) + 0.1f);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if Shooter Collider, skip
        if (collision.gameObject.CompareTag(shooter.tag))
            return;
        //if Entity, Attack
        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>().sleepMode != RigidbodySleepMode2D.NeverSleep)
            {
                rigidbody2Ds.Add(collision.gameObject.GetComponent<Rigidbody2D>());
                collision.gameObject.GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
            }
            Entity entity = collision.gameObject.GetComponent<Entity>();
            shooter.ApplyDamage(entity, damage,isJumpAvoidable);
        }
    }

    private void OnDestroy()
    {
        foreach (Rigidbody2D rigidbody in rigidbody2Ds)
        {
            rigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
        }
    }
}
