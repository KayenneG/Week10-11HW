using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEnemy : BaseEnemy
{
    // Start is called before the first frame update


    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Hit()
    {
        base.Hit();
    }
}
