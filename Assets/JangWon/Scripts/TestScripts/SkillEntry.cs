using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEntry : Entity
{
    [SerializeField] List<Skill> skills = new List<Skill>();
    [SerializeField] Skill baseAttack = null;
    [SerializeField] PlayerController player;

    private void Start()
    {
        baseAttack = Instantiate(baseAttack);
    }


    //Temporary shooting test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //baseattack.PositionCenter = player.transform.position;
            baseAttack.UseSkill(this as Entity, player.transform.position - this.transform.position);
        }
    }
}
