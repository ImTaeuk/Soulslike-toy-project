using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHandler : MoveHandler
{
    [SerializeField] private float camRotSpeed;

    [SerializeField] private Camera playerCam;

    [SerializeField] private float camMinRotX;
    [SerializeField] private float camMaxRotX;

    [SerializeField] private float jumpPower;

    private float dirX => GameManager.Instance.InputSystem.DirX;
    private float dirZ => GameManager.Instance.InputSystem.DirZ;

    private float axisX => GameManager.Instance.InputSystem.MouseAxisX;
    private float axisY => GameManager.Instance.InputSystem.MouseAxisY;
    private bool isRunning => GameManager.Instance.InputSystem.IsRunning;
    private bool jump => GameManager.Instance.InputSystem.Jump;
    


    public override void UpdateAnim()
    {
        anim.SetFloat("DirX", dirX); 
        anim.SetFloat("DirZ", dirZ);

        anim.SetBool("Running", isRunning);

        isMoving = !(dirX == 0 && dirZ == 0);
    }


    private void RotateActor()
    {
        transform.Rotate(0, axisX * rotSpeed, 0);
    }

    private void RotateCam()
    {
        playerCam.transform.Rotate(-axisY * camRotSpeed, 0, 0);

        Vector3 rot = playerCam.transform.rotation.eulerAngles;

        if (rot.x < camMinRotX)
            playerCam.transform.rotation = Quaternion.Euler(camMinRotX, rot.y, rot.z);
        else if (rot.x > camMaxRotX)
            playerCam.transform.rotation = Quaternion.Euler(camMaxRotX, rot.y, rot.z);
    }

    protected override void TranslateActor(float speed)
    {
        if (isMoving == false)
            return;
        else
        {
            Vector3 forward;
            Vector3 right;

            float yVel = rb.velocity.y;

            forward = playerCam.transform.right * dirX;
            right = playerCam.transform.forward * dirZ;

            Vector3 dir = (forward + right) * speed;
            rb.velocity += new Vector3(dir.x, 0, dir.z);

            Vector2 vel = new Vector2(rb.velocity.x, rb.velocity.z);

            if (vel.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;

            rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);

        anim.SetTrigger("Jump");
    }

    public void UpdateMoveHandler(float speed)
    {
        UpdateAnim();
        RotateActor();
        //RotateCam();
        TranslateActor(speed);

        if (jump)
            Jump();

        if (dirX == 0 && dirZ == 0 && actor.CurOrder.orderType == EOrderType.move)
            actor.CompleteOrder();
    }
}
