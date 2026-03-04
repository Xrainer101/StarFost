using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRailsState : PlayerState
{
    public OnRailsState(PlayerStateMachine context, PlayerStateMachine.EPlayerState ePlayerState) : base(context, ePlayerState)
    {
        
    }

    public override void EnterState()
    {
        Debug.Log("On rails mode!");
    }
    public override void UpdateState()
    {
        horiInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        XZTilting();
        ClampToScreen();
    }
    public override void FixedUpdateState()
    {
        Movement();
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
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        ctx.transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
