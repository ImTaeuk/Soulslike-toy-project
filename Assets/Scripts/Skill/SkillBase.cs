using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.WSA;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController animOverride;
    [SerializeField] private AnimatorOverrideController animOverrideTransition;

    [SerializeField] private float predelayTime;
    [SerializeField] private float maincontextTime;
    [SerializeField] private float afterdelayTime;

    [SerializeField] private SkillBase nextSkill;

    [SerializeField] private bool countable;

    [SerializeField] protected float impactTime;
    [SerializeField] protected float impactPower;

    protected Actor owner;
    protected ECastState castState;

    private float predelayTimer;
    private float afterdelayTimer;
    private float maincontextTimer;
    
    
    public abstract bool Castable();
    public ECastState CastState => castState;
    public SkillBase NextSkill => nextSkill;
    public bool Countable => countable;

    public virtual void SetOwner(Actor owner)
    {
        this.owner = owner;
    }

    public virtual void SkillPipeline()
    {
        switch (castState)
        {
            case ECastState.preDelay:

                OnPredelay();

                if (AnimDone())
                    OnTransitionPreToMain();

                break;

            case ECastState.mainContext:

                OnMaincontext();

                if (AnimDone())
                    OnTransitionMainToAfter();

                break;

            case ECastState.afterDelay:

                OnAfterdelay();

                if (AnimDone() || owner.BufferOrder.orderType != EOrderType.idle)
                    OnCompleteSkill();

                break;
        }


        SetAnimSpeed();
    }

    public virtual void OnCancelSkill()
    {
        castState = ECastState.notCast;

        owner.Anim.ResetTrigger("PreToMain");
        owner.Anim.ResetTrigger("MainToAfter");
        owner.Anim.ResetTrigger("Combo");
    }

    public virtual void OnCastSkill()
    {
        predelayTimer = predelayTime;
        castState = ECastState.preDelay;

        owner.Anim.ResetTrigger("PreToMain");
        owner.Anim.ResetTrigger("MainToAfter");

        SetAnimSpeed();

        if (countable && owner != GameManager.Instance.Player)
        {
            EffectManager.instance.PlayCountableEffect(transform, predelayTime);
        }
    }

    protected virtual void OnPredelay()
    {
        predelayTimer -= Time.deltaTime;

        AnimatorStateInfo stateInfo = owner.Anim.GetCurrentAnimatorStateInfo(0);
        if (animOverride != null && stateInfo.IsName("Predelay"))
        {
            owner.Anim.runtimeAnimatorController = animOverride;
        }
    }

    protected virtual void OnTransitionPreToMain()
    {
        castState = ECastState.mainContext;

        owner.Anim.SetTrigger("PreToMain");

        maincontextTimer = maincontextTime;

        SetAnimSpeed();
    }

    protected virtual void OnMaincontext()
    {
        maincontextTimer -= Time.deltaTime;
    }

    protected virtual void OnTransitionMainToAfter()
    {
        castState = ECastState.afterDelay;

        

        afterdelayTimer = afterdelayTime;

        SetAnimSpeed();

        if (owner.BufferOrder.orderType == EOrderType.attack
            && owner.BufferOrder.skill == nextSkill)
        {
            OnCompleteSkill();
        }
        else
        {
            owner.Anim.SetTrigger("MainToAfter");
        }
    }

    protected virtual void OnAfterdelay()
    {
        afterdelayTimer -= Time.deltaTime;
    }

    public virtual void OnCompleteSkill()
    {
        castState = ECastState.notCast;

        if (owner.CurOrder.orderType == EOrderType.attack)
            owner.CompleteOrder();

        if (owner.BufferOrder.orderType == EOrderType.attack
            && owner.BufferOrder.skill == nextSkill)
            owner.Anim.runtimeAnimatorController = animOverrideTransition;
    }

    private bool AnimDone()
    {
        bool done = owner.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f;
        bool stateDone = false;

        AnimatorStateInfo stateInfo = owner.Anim.GetCurrentAnimatorStateInfo(0);


        switch (castState)
        {
            case ECastState.notCast:
                return true;
            case ECastState.preDelay:
                stateDone = stateInfo.IsName("Predelay");
                break;
            case ECastState.mainContext:
                stateDone = stateInfo.IsName("Maincontext");
                break;
            case ECastState.afterDelay:
                stateDone = stateInfo.IsName("Afterdelay");
                break;
        }

        return done && stateDone;
    }

    private void SetAnimSpeed()
    {
        float length = 0;
        float speed = 1;

        AnimatorStateInfo info = owner.Anim.GetCurrentAnimatorStateInfo(0);

        foreach (AnimationClip clip in owner.Anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == info.shortNameHash.ToString() ||  clip.name == owner.Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                length = clip.length;
            }
        }

        switch (castState)
        {
            case ECastState.notCast:
            case ECastState.preDelay:
                speed = length / predelayTime;
                break;

            case ECastState.mainContext:
                speed = length / maincontextTime;
                break;

            case ECastState.afterDelay:
                speed = length / afterdelayTime;
                break;

            default: return;
        }

        owner.Anim.SetFloat("AnimSpeed", speed);
    }
}
