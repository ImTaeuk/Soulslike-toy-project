using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    [SerializeField] protected float rotSpeed;


    protected Animator anim;
    protected Rigidbody rb;
    protected bool isMoving = false;
    protected Actor actor;

    Vector3 dir;
    float dirX;
    float dirZ;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponentInChildren<Rigidbody>();

        actor = GetComponent<Actor>();

        dir = transform.forward;
    }


    public virtual void UpdateAnim()
    {
        anim.SetFloat("DirX", dirX);
        anim.SetFloat("DirZ", dirZ);

        isMoving = !(dirX == 0 && dirZ == 0);
    }


    public void Rotate(Vector3 tarDir)
    {
        Quaternion toRot = Quaternion.LookRotation(tarDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotSpeed * Time.deltaTime);
    }

    protected virtual void TranslateActor(float speed)
    {
        if (isMoving == false)
            return;
        else
        {
            rb.velocity += dir * speed;

            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    public virtual void UpdateMoveHandler(Vector3 pos, float speed)
    {
        dir = (pos - transform.position).normalized;

        if (dir.magnitude < 0.05f && actor.CurOrder.orderType == EOrderType.move)
        {
            dir = Vector3.zero;
            actor.CompleteOrder();
        }

        dirX = dir.x;
        dirZ = dir.z;

        UpdateAnim();
        Rotate(dir);

        TranslateActor(speed);
    }
}
