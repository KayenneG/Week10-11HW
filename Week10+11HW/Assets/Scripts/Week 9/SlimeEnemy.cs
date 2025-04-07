using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : BaseEnemy
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        base.Attack();
        Debug.Log(this.gameObject.name + " deals " + damageTotal + " damage");
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
