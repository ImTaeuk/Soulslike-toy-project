using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeelSkill0 : SkillBase
{
    [SerializeField] private TriggerDetector weapon;
    [SerializeField] private float damage;
    [SerializeField] private bool counter;

    private List<Actor> damagedActors;

    private void Awake()
    {
        damagedActors = new List<Actor>();
    }

    public override bool Castable()
    {
        return true;
    }

    public override void OnCastSkill()
    {
        base.OnCastSkill();
        damagedActors.Clear();
    }

    protected override void OnMaincontext()
    {
        base.OnMaincontext();

        foreach (GameObject obj in weapon.insideCols)
        {
            Actor target;
            if (obj.TryGetComponent<Actor>(out target) == false || target == owner)
                continue;

            if (damagedActors.Contains(target) == false)
            {
                damagedActors.Add(target);

                bool wasCounter = false;

                if (counter == true && target.CurOrder.skill != null && target.CurOrder.skill.CastState == ECastState.preDelay)
                    wasCounter = true;

                target.TakeDamage(damage, (target.transform.position - owner.transform.position).normalized, impactPower, wasCounter, impactTime);
            }
        }
    }
}
