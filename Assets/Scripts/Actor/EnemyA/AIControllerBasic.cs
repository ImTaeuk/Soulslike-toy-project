using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerBasic : MonoBehaviour
{
    [SerializeField] private Actor actor;
    [SerializeField] private SkillBase skill;
    [SerializeField] private float attackRange;

    private Order order;

    private void Awake()
    {
        order = new Order(EOrderType.idle, Vector3.zero, null);
    }

    private void Update()
    {
        float dist = (GameManager.Instance.Player.transform.position - actor.transform.position).magnitude;
        if (attackRange < dist)
        {
            order.orderType = EOrderType.move;
            order.skill = null;
            order.tarPos = GameManager.Instance.Player.transform.position;
            order.done = false;
        }
        else
        {
            order.orderType = EOrderType.attack;
            order.skill = skill;
            order.tarPos = GameManager.Instance.Player.transform.position;
            order.done = false; 
        }

        actor.RecvOrder(order);
    }


}
