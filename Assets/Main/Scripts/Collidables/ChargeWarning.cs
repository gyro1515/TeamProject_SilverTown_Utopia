using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChargeWarning : MonoBehaviour
{
    //Sprite of Warning Red Zone
    [SerializeField] SpriteRenderer warningZoneSprite;
    //Sprite of OutLine
    [SerializeField] SpriteRenderer outlineSprite;
    //Entity who use this skill
    [SerializeField] Enemy shooter;
    //Starting position of charge (usually entity position)
    [SerializeField] Vector2 startPos;
    //Ending position of charge (usually current player position at active)
    [SerializeField] Vector2 endPos;
    //Damage of charge itself -> Can be removed
    [SerializeField] int damage;

    //Duration of Warning
    float endDuration = 0.0f;
    //Duration of Charge moving
    float chargeDuration = 0.0f;
    //Checker of Durations
    float start = 0.0f;
    //Warning Size Increment gap for each FixedDeltaTime
    Vector3 warningIncremental = Vector3.zero;
    //Charge Angle Increment gap for each FixedDeltaTime -> used with Mathf.sin
    float chargeIncremental = 0.0f;
    //Error Margain of Charge end position
    const float errorMargain = 0.1f;

    public void Init(Entity entity, Vector2 targetPos, float end, float chargeDuration, int damage)
    {
        //Set Flag to prevent multiple charging
        ChargeSkill.isChargning = true;
        //Initial setup
        shooter = entity as Enemy;
        startPos = shooter.transform.position;
        endPos = targetPos;

        //Calculate Charge Direction
        Vector2 dir = (targetPos - startPos);
        float scale = dir.magnitude;
        float angle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg;
        this.transform.localRotation = transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90);

        //<================== Need to be modified -> x Value as shooter's collider Size Value ==================>
        //Set Warning Scale
        transform.localScale = new Vector3(1, scale, 1);

        //Set Warning Incremental Based on EndDuration
        this.endDuration = end;
        if (endDuration != 0.0f)
        {
            Vector3 originalScale = warningZoneSprite.transform.localScale;
            warningIncremental = new Vector3(
                (1 - originalScale.x) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.y) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.z) * Time.fixedDeltaTime / (endDuration)
                );
        }

        //Set Charge Incremental Based on ChargeDuration
        this.chargeDuration = chargeDuration;
        if (chargeDuration != 0.0f)
        {
            chargeIncremental = (90.0f) * Time.fixedDeltaTime / chargeDuration ;
        }

        this.damage = damage;
        //Start showing Warning
        StartCoroutine(showWarning());
    }

    //Show warning zone
    public IEnumerator showWarning()
    {
        //Show Warning during endDuration, or if endDuration is 0, don't show warning
        while (endDuration != 0.0f && endDuration > start)
        {
            //Scale warning red zone
            warningZoneSprite.transform.localScale += warningIncremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        //Stop showing Warning and Start Charge
        if ((endDuration == 0.0f) || (start >= endDuration))
        {
            warningZoneSprite.transform.localScale = Vector3.one;
            warningZoneSprite.color = new Color(255, 0, 0, 0);
            outlineSprite.color = new Color(255, 255, 255, 0);
            start = 0.0f;
            StartCoroutine(Charge());
        }
    }
    //Charge and move shooter
    public IEnumerator Charge()
    {
        //Incremental local angle value
        float angle = 0.0f;
        //offset value where shooter will move
        float sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
        while (chargeDuration != 0.0f && chargeDuration > start && 1.0f - sinValue >= errorMargain)
        {
            //move shooter
            shooter.transform.position = Vector2.Lerp(shooter.transform.position, endPos, sinValue);
            start += Time.deltaTime;
            //update values
            angle += chargeIncremental;
            sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
            yield return new WaitForFixedUpdate();
        }

        //EndCharge and Destroy gameObject
        if ((chargeDuration == 0.0f) || (start >= chargeDuration) || 1.0f - sinValue < errorMargain)
        {
            shooter.transform.position = endPos;
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        //Free Flag to tell shooter can charge now
        ChargeSkill.isChargning = false;
        //Stop All Corutines just in case
        StopAllCoroutines();
    }

}
