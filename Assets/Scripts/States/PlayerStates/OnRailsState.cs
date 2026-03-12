using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRailsState : PlayerState
{
    [SerializeField] private Transform rail;

    Vector2 movementBounds = new Vector2(18f, 9f);

    public OnRailsState(PlayerStateMachine context, PlayerStateMachine.EPlayerState ePlayerState) : base(context, ePlayerState)
    {
        
    }

    public override void EnterState()
    {
        Debug.Log("On rails mode!");
        ctx.firstTapTime = 0f;
        ctx.timeBetTaps = 0.25f;
        ctx.tapCountL = 0;
        ctx.tapCountR = 0;

        ctx.coroutineActive = false;
        ctx.isRolling = false;
        ctx.barrelDeflect.SetActive(false);
        ctx.barrelRollEffect.Stop();
    }
    public override void UpdateState()
    {
        horiInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        Movement();
        XZTilting();
        ClampToScreen();
        ShoulderInputs();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void ExitState()
    {
        Debug.Log("Exiting on rails mode");
    }
    public override void OnStateTriggerEnter(Collider other)
    {
        
    }
    public override void OnStateTriggerStay(Collider other)
    {
        
    }
    public override void OnStateTriggerExit(Collider other)
    {
        
    }

    // Clamp the ship to the view of the camera
    void ClampToScreen()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(ctx.transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.08f, 0.92f);
        pos.y = Mathf.Clamp(pos.y, 0.08f, 0.92f);
        ctx.transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void ClampToBounds()
    {
        float clampedX = Mathf.Clamp(ctx.transform.localPosition.x, -movementBounds.x, movementBounds.x);
        float clampedY = Mathf.Clamp(ctx.transform.localPosition.y, -movementBounds.y, movementBounds.y);
        ctx.transform.position = new Vector3(clampedX, clampedY, ctx.transform.localPosition.z);
    }
}
