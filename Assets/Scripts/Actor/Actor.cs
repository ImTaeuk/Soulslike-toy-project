using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Actor : MonoBehaviour
{
    [SerializeField] protected float moveSpeed;

    protected Animator anim;

    protected Order curOrder;
    protected Order bufferOrder;
    protected MoveHandler moveHandler;

    private Rigidbody rb;
    private bool onPlane = false;
    protected float impactTimer;

    public bool Jumpable => Mathf.Abs(rb.velocity.y) < 0.1f && onPlane && (CurOrder.orderType == EOrderType.idle || CurOrder.orderType == EOrderType.move);
    public Order CurOrder => curOrder;
    public Order BufferOrder => bufferOrder;
    public Animator Anim => anim;
    public MoveHandler MoveHandler => moveHandler;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        moveHandler = GetComponent<MoveHandler>();
        rb = GetComponent<Rigidbody>();

        curOrder = new Order(EOrderType.idle, Vector3.zero, null);
        bufferOrder = new Order(EOrderType.idle, Vector3.zero, null);

        EquipSkills();
    }

    protected virtual void Update()
    {
        ProcessOrder();
        UpdateAnim();
    }

    public void TakeDamage(float amount, Vector3 impactDir, float impactPower, bool counter = false, float impactTime = 0)
    {
        if (curOrder.orderType == EOrderType.attack && counter == true && curOrder.skill != null && curOrder.skill.Countable == true)
        {
            Debug.Log(gameObject.name + " Critical! : " + amount * 2);
            if (impactTime > 0)
                TakeImpact(impactDir, impactPower, impactTime);
        }
        else Debug.Log(gameObject.name + " damaged! : " + amount);

        // Todo: Damage 받는 부분 Status 구현 시 적용
    }

    protected virtual void ProcessOrder()
    {
        switch (CurOrder.orderType)
        {
            case EOrderType.idle:
                break;

            case EOrderType.move:
                Move(CurOrder.tarPos);
                break;

            case EOrderType.attack:

                if (curOrder.skill.CastState == ECastState.notCast)
                    curOrder.skill.OnCastSkill();
                else
                    CurOrder.skill.SkillPipeline();

                break;

            case EOrderType.death:
                break;


            case EOrderType.damaged:
                break;

            case EOrderType.impacted:

                if (impactTimer < 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Impacted") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
                {
                    CompleteOrder();
                }
                else
                {
                    impactTimer -= Time.deltaTime;
                }

                break;

            case EOrderType.fatalHit:

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fatal Hit") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
                {
                    CompleteOrder();
                }

                break;
        }

        TakeOverOrder();
    }

    protected virtual void TakeOverOrder()
    {
        if (curOrder.done == true)
        {
            CurOrder.Copy(bufferOrder);

            bufferOrder.orderType = EOrderType.idle;
            bufferOrder.tarPos = transform.position;
            bufferOrder.skill = null;
        }
    }

    protected virtual void Move(Vector3 pos)
    {
        moveHandler.UpdateMoveHandler(pos, moveSpeed);
    }

    public void RecvOrder(Order order)
    {
        bufferOrder.Copy(order);
    }

    public virtual void UpdateAnim()
    {
        moveHandler.UpdateAnim();
        anim.SetInteger("AnimState", (int)curOrder.orderType);
    }

    public virtual void CompleteOrder()
    {
        CurOrder.done = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Plane"))
        {
            onPlane = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Plane"))
        {
            onPlane = false;
        }
    }

    protected virtual void TakeImpact(Vector3 dir, float impactPower, float impactTime)
    {
        rb.velocity = dir.normalized * impactPower;

        curOrder.done = false;

        anim.SetTrigger("Impacted");

        if (CurOrder.skill != null)
        {
            curOrder.skill.OnCancelSkill();
            curOrder.skill = null;
        }

        curOrder.tarPos = transform.position;
        curOrder.orderType = EOrderType.impacted;

        impactTimer = impactTime;
    }

    private void EquipSkills()
    {
        SkillBase[] skills = GetComponentsInChildren<SkillBase>();

        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SetOwner(this);
        }
    }

    public void TakeFatalHit(float damage)
    {
        rb.velocity = Vector3.zero;

        curOrder.done = false;

        if (CurOrder.skill != null)
        {
            curOrder.skill.OnCancelSkill();
            curOrder.skill = null;
        }

        curOrder.tarPos = transform.position;
        curOrder.orderType = EOrderType.fatalHit;

        // Todo: Damage 받는 부분 Status 구현 시 적용
    }
}
