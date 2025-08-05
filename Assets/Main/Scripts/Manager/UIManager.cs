using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> // 해당 형식으로 상속하면 싱글턴화된 매니저 생성 가능
{
    [Header("사용할 UI")]
    [SerializeField] MainUI mainUI;
    [SerializeField] SelectCardUI selectCardUI;
    [SerializeField] EndingUI endingUI;

    public MainUI MainUI { get { return mainUI; } }
    public EndingUI EndingUI { get { return endingUI; } }
    public void SetSelectCardUIActive()
    {
        selectCardUI.SetActive();
    }
    public void SetHpBar(float hpPercentage)
    {
        mainUI.SetHpBar(hpPercentage);
    }
    public void SetMainUIActive(bool active)
    {
        mainUI.gameObject.SetActive(active);
    }
}
