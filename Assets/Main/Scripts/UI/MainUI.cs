using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("MainUI에서 변경할 것들")]
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] Slider playerHpBar;
    [SerializeField] List<Image> skillIcons = new List<Image>(); 

    public void SetHpBar(float hpPercentage)
    {
        playerHpBar.value = hpPercentage;
    }
    public void SetLevelTxt(int curLevel)
    {
        levelTxt.text = curLevel.ToString();
    }
    public void SetSkillIcon(int skillIdx, Sprite skillIconSprite)
    {
        if (skillIdx >= skillIcons.Count)
        {
            return;
        }
        skillIcons[skillIdx].sprite = skillIconSprite;
    }
}
