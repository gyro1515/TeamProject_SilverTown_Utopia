using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    [Header("카드 정보")]
    [SerializeField] Button cardByButton; // 버튼으로 만들어진 카드
    [SerializeField] TextMeshProUGUI cardTitle;
    [SerializeField] TextMeshProUGUI cardExplanation;
    [SerializeField] Image cardImg;
    UpgradeCard upgradeCard;

    private void Awake()
    {
        cardByButton.onClick.AddListener(SeletCard);

    }
    void SeletCard()
    {
        upgradeCard.ApplySelectedCard();
    }
    public void SetCardData(UpgradeCard _upgradeCard)
    {
        upgradeCard = _upgradeCard;
        cardTitle.text = upgradeCard.CardTitle;
        cardExplanation.text = upgradeCard.CardExplanation;
        cardImg.sprite = upgradeCard.CardSprite;
    }
}
