using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] SkillBase skill;
    [SerializeField] Actor actor;
    [SerializeField] float interval;

    Order order;
    float timer = 0;


    private void Awake()
    {
        order = new Order(EOrderType.attack, Vector3.zero, skill);
    }

    private void Update()
    {
        if (timer <= 0)
        {
            timer = interval;

            actor.RecvOrder(order);
        }
        else
            timer -= Time.deltaTime;
    }
}
