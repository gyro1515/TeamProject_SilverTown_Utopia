using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardUI : MonoBehaviour
{
    // 원래는 버튼을 상속받아 카드 선택용 버튼을 만들어야 하지만, 시간이 부족한 관계로 노가다를...
    private bool isInitialized = false;
    [Header("플레이어")]
    [SerializeField] Player player;
    [Header("선택할 카드 UI")]
    [SerializeField] List<CardButton> cards = new List<CardButton>();
    [Header("선택할 수 있는 카드 효과들")]
    [SerializeField] List<UpgradeCard> upgradeCard = new List<UpgradeCard>();
    HashSet<int> selectedUpgradeCards = new HashSet<int>();

    [Header("복사할 스킬 강화")]
    [SerializeField] ProjectileSkillCard[] projectileSkillCardPrefabs = new ProjectileSkillCard[3];
    [SerializeField] RangedSkillCard[] rangedSkillCardPrefabs = new RangedSkillCard[2];
    [SerializeField] AbilityCard[] AbilityCardPrefabs = new AbilityCard[3];

    [Header("스킬 이미지")]
    [SerializeField] Sprite baseAttackSprite;
    [SerializeField] Sprite[] skillSprites;



    // 아래는 테스트 용도, 추후 지울 수도...?
    private bool isActive = false;
    private void Awake()
    {
        isActive = gameObject.activeSelf;
    }

    private void InitializeCards()
    {
        if (player == null)
        {
            Debug.Log("Undefined Player in selectCardUI");
            return;
        }
        //Add stat upgrades
        foreach (AbilityCard card in AbilityCardPrefabs)
        {
            if (card == null) continue;
            AbilityCard copycard = Instantiate(card, transform);
            copycard.SetCard(player);
            upgradeCard.Add(copycard);
        }
        //Add Skill Upgrades
        AddUpgrades(player.baseAttack, baseAttackSprite);
        for(int i = 0; i < player.playerSkills.Count; i++)
        {
            AddUpgrades(player.playerSkills[i], skillSprites[i]);
        }
        isInitialized = true;
        Debug.Log("InitializeCards");
    }


    public void SetCard() // 클리어할때마다 다른 카드 세팅하기
    {
        //최초 실행시 카드 세팅
        if (!isInitialized)
        {
            InitializeCards();
        }
        // 선택 가능한 카드 개수에 따라 카드 효과 넣어주기
        for (int i = 0; i < cards.Count; i++)
        {
            int tmpIdx = -1;
            do
            {
                tmpIdx = Random.Range(0, upgradeCard.Count); // 우선 랜덤으로 하나 뽑고
            }
            while (selectedUpgradeCards.Contains(tmpIdx)); // 처음 뽑은 카드라면 종료
            if (upgradeCard[tmpIdx] is ProjectileSkillCard)
            {
                ProjectileSkillCard tempcard = upgradeCard[tmpIdx] as ProjectileSkillCard;
                if (tempcard.isAuto && tempcard.skill.isAuto)
                {
                    upgradeCard.Remove(tempcard);
                    i--;
                    continue;
                }
            }
            selectedUpgradeCards.Add(tmpIdx);
            cards[i].SetCardData(upgradeCard[tmpIdx]);
        }
        selectedUpgradeCards.Clear(); // 계속 빈공간 유지
    }
    public void SetActive()
    {
        isActive = true;
        if (isActive) SetCard();
        gameObject.SetActive(isActive);
    }

    private void AddUpgrades(Skill skill, Sprite sprite)
    {
        if (skill is RangedSkill)
        {
            foreach (RangedSkillCard card in rangedSkillCardPrefabs)
            {
                RangedSkillCard copycard = Instantiate(card,transform);
                copycard.SetCard(skill as RangedSkill, sprite);
                upgradeCard.Add(copycard);
            }

        }
        else if (skill is ProjectileSkill)
        {
            foreach (ProjectileSkillCard card in projectileSkillCardPrefabs)
            {
                ProjectileSkillCard copycard = Instantiate(card, transform);
                copycard.SetCard(skill as ProjectileSkill, sprite);
                upgradeCard.Add(copycard);
            }
        }
    }


}
