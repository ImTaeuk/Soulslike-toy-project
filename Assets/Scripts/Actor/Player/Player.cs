using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Player : Actor
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float comboMaxInterval;
    [SerializeField] private PlayableDirector fatalTimeline;

    private float comboMaxIntervalTimer;
    private SkillBase preSkill;

    private bool isRunning => GameManager.Instance.InputSystem.IsRunning;
    public SkillBase PreSkill => preSkill;

    protected override void Update()
    {
        base.Update();

        if (comboMaxIntervalTimer >= 0 && preSkill != null)
        {
            comboMaxIntervalTimer -= Time.deltaTime;
            
            if (comboMaxIntervalTimer <= 0 )
            {
                preSkill = null;
            }
        }
        
    }

    public override void CompleteOrder()
    {
        if (bufferOrder.orderType == curOrder.orderType && curOrder.orderType == EOrderType.attack
            && bufferOrder.skill != null
            && curOrder.skill.NextSkill == bufferOrder.skill)
            anim.SetTrigger("Combo");

        preSkill = curOrder.skill;

        base.CompleteOrder();
    }

    protected override void Move(Vector3 pos)
    {
        PlayerMovementHandler handler = moveHandler as PlayerMovementHandler;
        handler.UpdateMoveHandler(isRunning ? runSpeed : moveSpeed);
    }

    public override void UpdateAnim()
    {
        moveHandler.UpdateAnim();
        anim.SetInteger("AnimState", (int)curOrder.orderType);
    }

    public void PlayFatalTimeline()
    {
        fatalTimeline.Play();
    }
}
