using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] private Player Player;

    [SerializeField] private SkillBase[] skills;
    [SerializeField] private SkillBase hardAttackSkill;
    [SerializeField] private SkillBase fatalSkill;


    private Order order;

    private float dirX;
    private float dirZ;

    private float mouseAxisX;
    private float mouseAxisY;

    private bool isRunning;

    private bool jumpPressed;

    public float DirX => dirX;
    public float DirZ => dirZ;

    public float MouseAxisX => mouseAxisX;
    public float MouseAxisY => mouseAxisY;
    public bool IsRunning => isRunning;
    public bool Jump => jumpPressed;

    private void Awake()
    {
        order = new Order(EOrderType.idle, Vector3.zero, null);
    }

    // Update is called once per frame
    void Update()
    {
        // Move < Attack < Fatal
        MoveInput();
        AttackInput();
        OrderToFatalSkill();

        if (order.orderType == EOrderType.move || order.orderType == EOrderType.attack)
        {
            if (order.orderType == EOrderType.move && Player.BufferOrder.orderType == EOrderType.attack)
                return;

            Player.RecvOrder(order); // send order
            order.Init();
        }

    }

    private void MoveInput()
    {
        bool isMoveKeyPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A);

        dirX = Input.GetAxis("Horizontal");
        dirZ = Input.GetAxis("Vertical");

        mouseAxisX = Input.GetAxis("Mouse X");
        mouseAxisY = Input.GetAxis("Mouse Y");

        if (Player.Jumpable)
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
        else jumpPressed = false;

        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (order.orderType == EOrderType.attack)
            return;

        if (isMoveKeyPressed)
        {
            order.orderType = EOrderType.move;
            order.skill = null;

            Vector3 dir = (Camera.main.transform.right * dirX + Camera.main.transform.forward * dirZ).normalized;

            order.tarPos = Player.transform.position + dir;
        }
        
    }

    private void AttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            order.orderType = EOrderType.attack;

            order.skill = skills[0];

            if (Player.CurOrder.skill != null || Player.PreSkill != null)
            {
                for (int i = 0; i < skills.Length; i++)
                {
                    if (Player.CurOrder.skill == skills[i])
                    {
                        order.skill = skills[(i + 1) % skills.Length];
                        return;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            order.orderType = EOrderType.attack;

            order.skill = hardAttackSkill;
        }
    }

    private void OrderToFatalSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && fatalSkill.Castable())
        {
            order.orderType = EOrderType.attack;
            order.skill = fatalSkill;
            order.done = false;
            order.tarPos = Player.transform.position;
        }
    }
}
