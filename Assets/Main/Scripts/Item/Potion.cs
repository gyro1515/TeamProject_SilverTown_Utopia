using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [Header("회복량")]
    [SerializeField] int amountOfRecovery = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        player?.SetCurHp(amountOfRecovery);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
