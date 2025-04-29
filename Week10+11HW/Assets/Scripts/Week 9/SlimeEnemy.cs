using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : BaseEnemy
{

    protected override void Start()
    {
        player = FindAnyObjectByType<PlayerRPG>();
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    protected override void Update()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) < attackRange)
        {
            Hit();
        }
            
    }

    protected override void Hit()
    {
        base.Hit();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
