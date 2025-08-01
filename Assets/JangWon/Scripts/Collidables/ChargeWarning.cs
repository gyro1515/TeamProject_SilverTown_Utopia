using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChargeWarning : MonoBehaviour
{
    [SerializeField] SpriteRenderer warningZoneSprite;
    [SerializeField] SpriteRenderer outlineSprite;

    [SerializeField] SkillEntry shooter;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 endPos;
    [SerializeField] int damage;



    float endDuration = 0.0f;
    float chargeDuration = 0.0f;
    float start = 0.0f;
    Vector3 warningIncremental = Vector3.zero;
    float chargeIncremental = 0.0f;
    const float errorMargain = 0.1f;

    public void Init(Entity entity, Vector2 targetPos, float end, float chargeDuration, int damage)
    {
        ChargeSkill.isChargning = true;
        shooter = entity as SkillEntry;
        startPos = shooter.transform.position;
        endPos = targetPos;

        Vector2 dir = (targetPos - startPos);
        float scale = dir.magnitude;
        float angle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg;
        this.transform.localRotation = transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90);
        transform.localScale = new Vector3(1, scale, 1);

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


        this.chargeDuration = chargeDuration;
        if (chargeDuration != 0.0f)
        {
            chargeIncremental = (90.0f) * Time.fixedDeltaTime / chargeDuration ;
        }

        this.damage = damage;
        StartCoroutine(showWarning());
    }

    //Show warning zone
    public IEnumerator showWarning()
    {
        while (endDuration != 0.0f && endDuration > start)
        {
            warningZoneSprite.transform.localScale += warningIncremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if ((endDuration == 0.0f) || (start >= endDuration))
        {
            warningZoneSprite.transform.localScale = Vector3.one;
            warningZoneSprite.color = new Color(255, 0, 0, 0);
            outlineSprite.color = new Color(255, 255, 255, 0);
            start = 0.0f;
            StartCoroutine(Charge());
        }
    }

    public IEnumerator Charge()
    {
        float angle = 0.0f;
        float sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
        while (chargeDuration != 0.0f && chargeDuration > start)
        {
            shooter.transform.position = Vector2.Lerp(shooter.transform.position, endPos, sinValue);
            start += Time.deltaTime;
            angle += chargeIncremental;
            sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
            yield return new WaitForFixedUpdate();
        }

        if ((chargeDuration == 0.0f) || (start >= chargeDuration) || 1.0f - sinValue < errorMargain)
        {
            shooter.transform.position = endPos;
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        ChargeSkill.isChargning = false;
        StopAllCoroutines();
    }

}
