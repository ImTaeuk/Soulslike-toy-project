using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActor : Actor
{

    protected override void Update()
    {
        base.Update();
    }

    public override void UpdateAnim()
    {
        anim.SetInteger("AnimState", (int)curOrder.orderType);
    }
    public override void CompleteOrder()
    {
        Debug.Log("Complete");
        CurOrder.done = true;
    }
}
