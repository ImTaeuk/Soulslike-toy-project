using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Order
{
    public EOrderType orderType;
    public Vector3 tarPos;
    public SkillBase skill;
    public bool done;

    public Order(EOrderType orderType, Vector3 tarpos, SkillBase skill)
    {
        this.orderType = orderType;
        this.tarPos = tarpos;
        this.skill = skill;

        if (orderType == EOrderType.idle || orderType == EOrderType.move)
            this.done = true;
        else
            this.done = false;
    }

    public void Copy(Order order)
    {
        this.orderType = order.orderType;
        this.tarPos = order.tarPos;

        this.skill = order.skill;

        if (orderType == EOrderType.idle || orderType == EOrderType.move)
            this.done = true;
        else 
            this.done = false;
    }

    public void Init()
    {
        this.orderType = EOrderType.idle;
        this.tarPos = Vector3.zero;
        this.skill = null;
        this.done = true;
    }
}
