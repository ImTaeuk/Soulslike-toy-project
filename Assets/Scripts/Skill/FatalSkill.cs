using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatalSkill : SkillBase
{
    [SerializeField] private TriggerDetector weapon;
    [SerializeField] private float damage;

    private Actor target;
    private Actor onColTarget;

    public override bool Castable()
    {
        return onColTarget != null && onColTarget.CurOrder.orderType == EOrderType.impacted;
    }

    public override void OnCastSkill()
    {
        base.OnCastSkill();
        target = onColTarget;
        target.TakeFatalHit(damage);

        owner.transform.LookAt(target.transform.position);

        GameManager.Instance.Player.PlayFatalTimeline();
    }

    private void OnTriggerEnter(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if (actor == null)
            return;
        else
        {
            onColTarget = actor;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Actor actor = other.gameObject.GetComponent<Actor>();

        if (actor == null)
            return;
        else
        {
            onColTarget = actor;
        }
    }
}
