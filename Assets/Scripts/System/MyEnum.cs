using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOrderType
{
    idle = 0,
    move = 1,
    attack = 2,
    damaged = 3,
    death = 4,
    jump = 5,
    dodge = 6,
    impacted = 7,
    fatalHit = 8,
}

public enum ECastState
{
    notCast = 0,
    preDelay = 1,
    mainContext = 2,
    afterDelay = 3,
}