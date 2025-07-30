using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 패턴 1~4를 처리할 공통 패턴 노드
public class PatternNode : Node
{
    private Enemy enemy; // 적 캐릭터 참조
    private int patternIndex; // 사용할 패턴 번호

    public PatternNode(Enemy enemy, int patternIndex)
    {
        this.enemy = enemy;
        this.patternIndex = patternIndex;
    }

    public override NodeState Evaluate()
    {
        var pattern = enemy.GetPatternInfo(patternIndex); // 해당 인덱스의 패턴 정보 가져오기
        if (pattern == null || !enemy.CanUsePattern(pattern)) // 조건 불충족 시 실패
            return NodeState.Failure;

        enemy.ExecutePattern(pattern); // 패턴 실행
        return NodeState.Success; // 성공 반환
    }
}