using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoDamage : BasePowerUp
{
    public PlayerRPG player;
    public override void PowerUp()
    {
        base.PowerUp();
        player.DamageBoost();
        Destroy(this.gameObject);
    }
}