using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Parring
    [Header("패링")]
    [SerializeField] private const float parringDelay = 1.0f;
    [SerializeField] private const float invincibleDelay = 0.25f;
    [SerializeField] private float parringStartTime = 0;

    //Attack
    [SerializeField] Skill baseAttack = null;

    protected override void Awake()
    {
        base.Awake();
        baseAttack = Instantiate(baseAttack);
        parringStartTime -= parringDelay;
    }

    /*protected override void Start()// 사용 안한다면 마지막에 지우기
    {
        base.Start();
        // Awake로 이동
        *//*baseAttack = Instantiate(baseAttack);
        parringStartTime -= parringDelay;*//*
    }*/

    protected override void Update()
    {
       /* direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction = new Vector2(direction.x, direction.y).normalized;*/

        if (Input.GetMouseButtonDown(1))
            Parring();

        if (Input.GetMouseButtonDown(0))
            BaseAttack((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position);

        // 방향을 정하고 애니메이션이 실행되도록
        base.Update();

    }

    /*protected override void FixedUpdate()
    {
        base.FixedUpdate(); // 무브는 부모에서
    }*/

    private void Parring()
    {
        if (parringStartTime + parringDelay >= Time.fixedTime)
            return;
        parringStartTime = Time.fixedTime;

        Debug.Log("Parring tried at : " + parringStartTime.ToString());
    }
    protected override void TakeDamage(int damage)
    {
        if (Time.fixedTime - parringStartTime <= invincibleDelay)
        {
            Debug.Log("Parring success");
            return;
        }
        // 패링이 된다면 데미지가 받지 않도록
        base.TakeDamage(damage);
    }
    private void BaseAttack(Vector2 mousepos)
    {
        if (baseAttack == null)
        {
            Debug.Log("Player BaseAttack is null");
            return;
        }
        if ((mousepos.magnitude) < 0.9f)
            baseAttack.UseSkill(this as Entity, Vector2.zero);
        else
            baseAttack.UseSkill(this as Entity, mousepos.normalized);
    }
}
