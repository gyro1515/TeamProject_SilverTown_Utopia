using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    [SerializeField] Collider2D _collider;
    [SerializeField] SpriteRenderer warningZoneSprite;
    [SerializeField] SpriteRenderer OutlineSprite;
    string ShooterTag;
    float start = 0.0f;
    float endDuration = 0.0f;
    float attackRemain = 0.0f;
    Vector3 incremental = Vector3.zero;

    public void Init(/*Entity entity,*/ Vector2 pos, float end, float remain, float attackAngle)
    {
        //ShooterTag = entity.gameObject.tag;
        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, attackAngle));
        endDuration = end;
        attackRemain = remain;
        if (endDuration != 0.0f)
        {
            Vector3 originalScale = warningZoneSprite.transform.localScale;
            incremental = new Vector3(
                (1 - originalScale.x) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.y) * Time.fixedDeltaTime / (endDuration),
                (1 - originalScale.z) * Time.fixedDeltaTime / (endDuration)
                );
        }
        StartCoroutine(showWarning());
    }


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
            warningZoneSprite.color = new Color(255, 0, 0, 0);
            OutlineSprite.color = new Color(255, 255, 255, 0);

            Debug.Log("ApplyDamage");
            ApplyDamage();
            StopCoroutine(showWarning());
        }
    }

    void ApplyDamage()
    {
        _collider.enabled = true;
        Destroy(gameObject, attackRemain);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            return;
        if (collision.gameObject.tag == ShooterTag)
            return;
        //if entity, give damage
        /*
        if (collision.gameObject.GetComponent<Entity>() != null)
        {
            Entity entity = collision.gameObject.GetComponent<Entity>();
            entity.TakeDamage(damage)
        }
        */
    }
}
