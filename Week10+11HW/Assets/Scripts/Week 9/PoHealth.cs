using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoHealth : BasePowerUp
{
    public PlayerRPG player;
    public override void PowerUp()
    {
        base.PowerUp();
        player.HealthBoost();
        Destroy(this.gameObject);
    }
}