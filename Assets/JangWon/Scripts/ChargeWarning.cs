using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChargeWarning : MonoBehaviour
{
    [SerializeField] SpriteRenderer warningZoneSprite;
    [SerializeField] SpriteRenderer OutlineSprite;

    [SerializeField] SkillEntry shooter;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 endPos;
    [SerializeField] int damage;



    float endDuration = 0.0f;
    float chargeDuration = 0.0f;
    float start = 0.0f;
    Vector3 WarningIncremental = Vector3.zero;
    Vector2 ChargeIncremental = Vector2.zero;

    public void Init(Entity entity, Vector2 targetpos, float end, float chargeDuration, int damage)
    {
        shooter = entity as SkillEntry;
        startPos = shooter.transform.position;
        endPos = targetpos;

        Vector2 dir = (targetpos - startPos);
        float scale = dir.magnitude;
        float angle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg;
        this.transform.localRotation = transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90);
        transform.localScale = new Vector3(1, scale, 1);

        this.endDuration = end;
        if (endDuration != 0.0f)
        {
            Vector3 originalScale = warningZoneSprite.transform.localScale;
            WarningIncremental = new Vector3(
                (1 - originalScale.x) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.y) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.z) * Time.fixedDeltaTime / (endDuration)
                );
        }


        this.chargeDuration = chargeDuration;
        if (chargeDuration != 0.0f)
        {
            ChargeIncremental = new Vector2(
                dir.x * Time.fixedDeltaTime / (chargeDuration),
                dir.y * Time.fixedDeltaTime / (chargeDuration)
                );
        }

        this.damage = damage;
        StartCoroutine(showWarning());
    }

    //Show warning zone
    public IEnumerator showWarning()
    {
        while (endDuration != 0.0f && endDuration > start)
        {
            warningZoneSprite.transform.localScale += WarningIncremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if ((endDuration == 0.0f) || (start >= endDuration))
        {
            warningZoneSprite.transform.localScale = Vector3.one;
            warningZoneSprite.color = new Color(255, 0, 0, 0);
            OutlineSprite.color = new Color(255, 255, 255, 0);


            start = 0.0f;
            StartCoroutine(Charge());
            StopCoroutine(showWarning());

        }
    }

    public IEnumerator Charge()
    {
        while (chargeDuration != 0.0f && chargeDuration > start)
        {
            shooter.transform.position += (Vector3)ChargeIncremental;
            start += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }


        if ((chargeDuration == 0.0f) || (start >= chargeDuration))
        {
            shooter.transform.position = endPos;
            Destroy(this, 0.1f);
            StopCoroutine(Charge());
        }
    }

}
