using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UpgradeCard : ScriptableObject
{
    [Header("카드 정보")]
    [SerializeField] string cardTitle = "";
    [SerializeField] string cardExplanation = "";
    [SerializeField] Sprite cardSprite;
    public string CardTitle { get { return cardTitle; } private set { } }
    public string CardExplanation { get { return cardExplanation; } private set { } }
    public Sprite CardSprite { get { return cardSprite; } private set { } }
    public abstract void ApplySelectedCard();
}
