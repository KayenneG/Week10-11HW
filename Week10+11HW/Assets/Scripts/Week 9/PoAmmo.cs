using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoAmmo : BasePowerUp
{
    public PlayerRPG player;
    public override void PowerUp()
    {
        base.PowerUp();
        player.AmmoBoost();
        Destroy(this.gameObject);
    }
}
