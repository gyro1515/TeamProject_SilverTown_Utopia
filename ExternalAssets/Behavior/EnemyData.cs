using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 하나의 공격 패턴 정보를 담는 클래스
[System.Serializable]
public class PatternInfo
{
    public float triggerDistance; // 발동 거리
    public float damage; // 피해량
    public float cooldown; // 쿨타임
    public string animationTrigger; // 애니메이션 트리거 이름
}

// 적이 가진 모든 패턴들을 담는 클래스
public class EnemyData : MonoBehaviour
{
    public List<PatternInfo> patterns = new List<PatternInfo>(); // 패턴 리스트
}