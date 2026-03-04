using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State<PlayerStateMachine.EPlayerState>
{
    protected PlayerStateMachine ctx;

    protected float horiInput, vertInput;

    public PlayerState(PlayerStateMachine context, PlayerStateMachine.EPlayerState ePlayerState) : base(ePlayerState)
    {
        ctx = context;
    }

    // Constantly go forward and turn to match the input
    protected void Movement()
    {
        ctx.transform.Translate(Vector3.forward * ctx.moveSpeed * Time.deltaTime);

        Vector3 movement = new Vector3(horiInput, vertInput, 0);
        ctx.transform.localPosition += new Vector3(movement.x, movement.y, movement.z) * ctx.moveSpeed * Time.deltaTime;
    }

    // For visual tilting of the ship
    protected void XZTilting()
    {
        TiltZ(horiInput);
        TiltX(vertInput);
    }

    void TiltZ(float axis) // tilt L + R
    {
        Vector3 targetEuAng = ctx.transform.localEulerAngles;

        ctx.transform.localEulerAngles = new Vector3(
            targetEuAng.x,
            Mathf.LerpAngle(targetEuAng.y, axis * ctx.tilt, ctx.tiltSpeed),
            Mathf.LerpAngle(targetEuAng.z, -axis * ctx.tilt, ctx.tiltSpeed)
        );
    }

    void TiltX(float axis) // tilt U + D
    {
        Vector3 targetEuAng = ctx.transform.localEulerAngles;

        ctx.transform.localEulerAngles = new Vector3(
            Mathf.LerpAngle(targetEuAng.x, -axis * ctx.tilt, ctx.tiltSpeed),
            targetEuAng.y,
            targetEuAng.z
        );
    }
}
