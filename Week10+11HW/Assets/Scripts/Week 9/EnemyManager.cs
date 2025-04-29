using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject weindigo;
    public GameObject feind;
    public GameObject growth;

    int rngEnm = 3;
    int chsEnm = 2;
    int slmEnm = 9;

    public void Chase()
    {
        chsEnm -=1;
        if(chsEnm == 0)
        {
            WeindigoGone();
        }
    }
    public void Range()
    {
        rngEnm -= 1;
        if (rngEnm == 0)
        {
            FeindGone();
        }
    }
    public void Mushroom()
    {
        Debug.Log("Shroom Destroy Recieve");
        slmEnm -= 1;
        Debug.Log("SHroom Decrease");
        if (slmEnm == 0)
        {
            SlimeGone();

        }
    }

    public void SlimeGone()
    {
        growth.SetActive(true);
    }
    public void FeindGone()
    {
        feind.SetActive(true);
    }
    public void WeindigoGone()
    {
        weindigo.SetActive(true);
    }
}
