using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class BurrowManager : MonoBehaviour
{
    [SerializeField] private int activeBurrows = 0; // underground
    [SerializeField] private int inactiveBurrows = 6; // above ground
    public void UpdateBurrow()
    {
        if (transform.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing)
        {
            if (activeBurrows <= (transform.childCount / 2) + 1)
            {
                transform.GetChild(activeBurrows + 1).GetComponent<CaterpillerEnemy>().isBurrowing = true;
                activeBurrows++;
                inactiveBurrows--;
            }
        }

        if (transform.GetChild(0).GetComponent<CaterpillerEnemy>().isBurrowing == false && inactiveBurrows < 6)
        {
            if(activeBurrows > 0)
            {
                transform.GetChild(inactiveBurrows + 1).GetComponent<CaterpillerEnemy>().isBurrowing = false;
                inactiveBurrows++;
                activeBurrows--;
            }
        }
    }

}
