using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Wire : MonoBehaviour
{
    private Entity shooter;
    public bool isClamped = false;
    const float maxWireDistance = 5.0f;
    float wireLength = 0.0f;

    //When Shoot Wire, wire speed
    [SerializeField] float wireDuration = 1.0f;
    //When Clamped, player movement duration
    [SerializeField] float moveDuration = 0.0f;

    float wireIncremental = 0.0f;
    Vector2 clampedDestination = Vector2.zero;

    float start = 0.0f;

    //Error Margain of Charge end position
    const float errorMargain = 0.1f;



    public void Init(Entity shooter, Vector2 direction)
    {
        this.shooter = shooter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angle - 90);

        if (wireDuration > 0.0f)
        {
            wireIncremental = maxWireDistance * Time.fixedDeltaTime / wireDuration;
        }
        StartCoroutine(ShootWire());
    }

    public IEnumerator ShootWire()
    {
        while (!isClamped && wireLength < maxWireDistance)
        {
            transform.position = shooter.transform.position;
            transform.localScale += Vector3.up*wireIncremental;
            wireLength = transform.localScale.y;
            yield return new WaitForFixedUpdate();
        }

        if (isClamped)
        {
            StartCoroutine(MovePosition());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            start += Time.fixedDeltaTime;
            isClamped = true;
            clampedDestination = collision.transform.position;
            transform.position = clampedDestination;
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
    }

    public IEnumerator MovePosition()
    {
        Player player = shooter as Player;
        float incremental = 90 * Time.fixedDeltaTime / start;
        float angle = 0.0f;
        float sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
        while (start > 0 || 1.0f - sinValue < errorMargain)
        {
            if (transform.localScale.y < 0)
                break;
            if (player.IsColliding())
                break;
            transform.localScale += Vector3.down * wireIncremental;
            shooter.transform.position = Vector2.Lerp(shooter.transform.position, clampedDestination, sinValue);
            start -= Time.fixedDeltaTime;
            //update values
            angle += incremental;
            sinValue = Mathf.Sin(angle * Mathf.Deg2Rad);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);

    }


}
