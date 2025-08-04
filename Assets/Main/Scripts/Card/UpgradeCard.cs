using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UpgradeCard : ScriptableObject
{
    [Header("카드 정보")]
    [SerializeField] protected string cardTitle = "";
    [SerializeField] protected string cardExplanation = "";
    [SerializeField] protected Sprite cardSprite;
    public string CardTitle { get { return cardTitle; } protected set { } }
    public string CardExplanation { get { return cardExplanation; } protected set { } }
    public Sprite CardSprite { get { return cardSprite; } protected set { } }
    public abstract void ApplySelectedCard();
}
