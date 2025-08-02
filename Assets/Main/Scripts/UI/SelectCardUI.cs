using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardUI : MonoBehaviour
{
    // 원래는 버튼을 상속받아 카드 선택용 버튼을 만들어야 하지만, 시간이 부족한 관계로 노가다를...
    [Header("선택할 카드 UI")]
    [SerializeField] List<CardButton> cards = new List<CardButton>();
    [Header("선택할 수 있는 카드 효과들")]
    [SerializeField] List<UpgradeCard> upgradeCardPrefabs = new List<UpgradeCard>();
    HashSet<int> selectedUpgradeCards = new HashSet<int>();

    // 아래는 테스트 용도, 추후 지울 수도...?
    private bool isActive = false;
    private void Awake()
    {
        isActive = gameObject.activeSelf;
    }
    public void SetCard() // 클리어할때마다 다른 카드 세팅하기
    {
        // 선택 가능한 카드 개수에 따라 카드 효과 넣어주기
        for (int i = 0; i < cards.Count; i++)
        {
            int tmpIdx = -1;
            do
            {
                tmpIdx = Random.Range(0, upgradeCardPrefabs.Count); // 우선 랜덤으로 하나 뽑고
            }
            while (selectedUpgradeCards.Contains(tmpIdx)); // 처음 뽑은 카드라면 종료

            selectedUpgradeCards.Add(tmpIdx);
            cards[i].SetCardData(upgradeCardPrefabs[tmpIdx]);
        }
        selectedUpgradeCards.Clear(); // 계속 빈공간 유지
    }
    public void SetActive()
    {
        isActive = !isActive;
        if (isActive) SetCard();
        gameObject.SetActive(isActive);
    }

}
