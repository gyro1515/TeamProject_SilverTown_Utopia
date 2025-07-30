using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern Data", menuName = "Scriptable Object/Pattern")]
public class Pattern : ScriptableObject
{
    public enum PositionState
    {
        Origin = 0,
        Fixed,
        Player,
        PlayerRandom,
        PlayerFixed
    }
    [SerializeField] public Entity Player;
    [SerializeField] public Skill[] skills;
    [SerializeField] public float[] activeTime;
    [SerializeField] public PositionState[] positionStates;
    [SerializeField] public Vector2[] positions;

    private void Awake()
    {
        int count = skills.Length;
        if (count != activeTime.Length)
        {
            Debug.Log("Unmatched List size of skill and active time");
            return;
        }
        if (count != positionStates.Length)
        {
            Debug.Log("Unmatched List size of skill and position state");
            return;
        }
        if (count != positions.Length)
        {
            Debug.Log("Unmatched List size of skill and position");
            return;
        }
    }

    public void Init()
    {
        int count = skills.Length;

        for (int i = 0; i < count; i++)
        {
            skills[i] = Instantiate(skills[i]);
        }
    }
}
