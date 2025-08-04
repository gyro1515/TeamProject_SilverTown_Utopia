using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AudioTest : MonoBehaviour
{
    [Header("Dropdowns")]
    public Dropdown bgmDropdown;
    public Dropdown seDropdown;

    [Header("Buttons")]
    public Button bgmPlayButton;
    public Button bgmStopButton;
    public Button sePlayButton;

    // 내부적으로 선택된 인덱스를 enum으로 변환하기 위해 참조용 변수
    private Array bgmEnums;
    private Array seEnums;

    void Start()
    {
        // Enum 목록 받아오기
        bgmEnums = Enum.GetValues(typeof(BGMType));
        seEnums = Enum.GetValues(typeof(SEType));

        // 드롭다운 옵션 채우기
        bgmDropdown.ClearOptions();
        List<string> bgmNames = new List<string>();
        foreach (BGMType type in bgmEnums)
            bgmNames.Add(type.ToString());
        bgmDropdown.AddOptions(bgmNames);

        seDropdown.ClearOptions();
        List<string> seNames = new List<string>();
        foreach (SEType type in seEnums)
            seNames.Add(type.ToString());
        seDropdown.AddOptions(seNames);

        // 버튼 리스너 등록
        bgmPlayButton.onClick.AddListener(OnBGMPlayClicked);
        bgmStopButton.onClick.AddListener(OnBGMStopClicked);
        sePlayButton.onClick.AddListener(OnSEPlayClicked);
    }

    void OnBGMPlayClicked()
    {
        var selectedIdx = bgmDropdown.value;
        BGMType type = (BGMType)bgmEnums.GetValue(selectedIdx);
        AudioManager.Instance.PlayBGM(type);
    }

    void OnBGMStopClicked()
    {
        AudioManager.Instance.StopBGM();
    }

    void OnSEPlayClicked()
    {
        var selectedIdx = seDropdown.value;
        SEType type = (SEType)seEnums.GetValue(selectedIdx);
        AudioManager.Instance.PlaySE(type);
    }
}
