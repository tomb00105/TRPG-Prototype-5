using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : ActionBase
{
    private void Awake()
    {
        actionName = "Weapon Attack";
        damage = 2;
        actionDamageStat = "HP";
        range = 1;
        positiveHeightRange = 1;
        negativeHeightRange = 2;
        targetType = "Player Character";
        actionType = "Attacking";
        mPCost = 0;
        aPCost = 0;
        hPCost = 0;
        accuracy = 85;
    }
}
