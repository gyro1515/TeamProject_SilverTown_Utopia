using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEntry : Entity
{
    [SerializeField] List<Skill> skills = new List<Skill>();
    [SerializeField] Skill baseattack = null;


    //Temporary shooting test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            baseattack.UseSkill(this as Entity);
        }
    }
}
