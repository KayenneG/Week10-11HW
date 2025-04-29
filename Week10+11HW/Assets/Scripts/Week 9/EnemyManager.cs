using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    int rngEnm = 3;
    int chsEnm = 2;
    int slmEnm = 9;

    public void Chase()
    {
        Debug.Log("Manager Recieve");
        chsEnm -=1;
        Debug.Log("chase left: " + chsEnm);
    }
}
