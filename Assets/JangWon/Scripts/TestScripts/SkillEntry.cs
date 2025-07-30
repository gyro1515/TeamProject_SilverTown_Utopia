using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillEntry : Entity
{
    PatternActor actor;
    public PlayerController player;
    public bool isPatternEnd = true;

    private void Start()
    {
        actor = GetComponent<PatternActor>();
    }


    //Temporary shooting test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPatternEnd)
        {
            actor.StartCoroutine("ActivePattern");
        }
    }
}
