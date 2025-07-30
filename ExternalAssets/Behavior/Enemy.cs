using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 적의 동작과 행동 트리를 담당하는 클래스
public class Enemy : MonoBehaviour
{
    public Transform player; // 추적할 플레이어
    public float moveSpeed = 2f; // 이동 속도
    public EnemyData data; // 패턴 정보

    private Node rootNode; // 루트 노드 (Selector)
    private float[] lastUsedTime; // 패턴별 마지막 사용 시간 기록

    void Start()
    {
        rootNode = CreateBehaviorTree(); // 행동 트리 생성
        lastUsedTime = new float[data.patterns.Count]; // 패턴 수만큼 초기화
    }

    void Update()
    {
        rootNode.Evaluate(); // 매 프레임마다 트리 평가 실행
    }

    private Node CreateBehaviorTree()
    {
        var patternNodes = new List<Node>(); // 패턴 노드 리스트 생성
        for (int i = 0; i < data.patterns.Count; i++)
        {
            patternNodes.Add(new PatternNode(this, i)); // 각각의 패턴 노드 추가
        }
        return new Selector(patternNodes); // Selector로 묶어 루트 노드 생성
    }

    public PatternInfo GetPatternInfo(int index)
    {
        if (index < 0 || index >= data.patterns.Count) return null; // 유효하지 않으면 null
        return data.patterns[index]; // 해당 인덱스의 패턴 반환
    }

    public bool CanUsePattern(PatternInfo pattern)
    {
        int index = data.patterns.IndexOf(pattern); // 패턴의 인덱스 찾기
        float dist = DistanceToPlayer(); // 플레이어와 거리 계산

        if (dist > pattern.triggerDistance) return false; // 거리가 멀면 발동 불가
        if (Time.time - lastUsedTime[index] < pattern.cooldown) return false; // 쿨타임 미충족

        return true; // 조건 충족 시 실행 가능
    }

    public void ExecutePattern(PatternInfo pattern)
    {
        int index = data.patterns.IndexOf(pattern);
        lastUsedTime[index] = Time.time; // 마지막 사용 시간 갱신

        if (!string.IsNullOrEmpty(pattern.animationTrigger))
        {
            Debug.Log($"{gameObject.name} ▶ 패턴 {index + 1} 발동! (애니: {pattern.animationTrigger})");
        }
        else
        {
            Debug.Log($"{gameObject.name} ▶ 패턴 {index + 1} 발동!");
        }

        // 이 부분에 실제 데미지, 범위 공격 등 구현 가능
    }

    public float DistanceToPlayer()
    {
        if (player == null) return float.MaxValue; // 플레이어 없으면 최대값 반환
        return Vector3.Distance(transform.position, player.position); // 거리 계산
    }
}