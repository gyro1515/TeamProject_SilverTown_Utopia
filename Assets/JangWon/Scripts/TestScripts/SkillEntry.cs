using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temporary Skill user
public class SkillEntry : Entity
{
    PatternActor patternActor;
    public PlayerController player;
    public bool isPatternEnd = true;

    private void Start()
    {
        patternActor = GetComponent<PatternActor>();
    }


    //Temporary shooting test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPatternEnd)
        {
            isPatternEnd = false;
            StartCoroutine(patternActor.ActivePattern());
        }
    }
}
