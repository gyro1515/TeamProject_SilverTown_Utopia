using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temporary Skill user
public class SkillEntry : Entity
{
    PatternActor patternActor;
    public Player player;
    public bool isPatternEnd = true;

    protected override void Start()
    {
        patternActor = GetComponent<PatternActor>();
    }


    //Temporary shooting test
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isPatternEnd)
        {
            isPatternEnd = false;
            StartCoroutine(patternActor.ActivePattern());
        }
    }
}
