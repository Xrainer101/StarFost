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
        if (ctx.rollLeft || ctx.rollRight) ctx.transform.localPosition += new Vector3(movement.x * 1.5f, movement.y, movement.z) * ctx.moveSpeed * Time.deltaTime;
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

    protected void ShoulderInputs()
    {
        if(Input.GetKey(KeyCode.LeftControl)) // lean left
        {
            Vector3 newEuAng = ctx.transform.localEulerAngles;
            ctx.transform.localEulerAngles = new Vector3(newEuAng.x, newEuAng.y, Mathf.LerpAngle(newEuAng.z, 95, ctx.leanSpeed));
        }

        if(Input.GetKey(KeyCode.Space)) // lean right
        {
            Vector3 newEuAng = ctx.transform.localEulerAngles;
            ctx.transform.localEulerAngles = new Vector3(newEuAng.x, newEuAng.y, Mathf.LerpAngle(newEuAng.z, -95, ctx.leanSpeed));
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)) // Double tap L
        {
            ctx.tapCountL += 1;
            if(ctx.tapCountL == 1 && !ctx.coroutineActive)
            {
                ctx.firstTapTime = Time.time;
                ctx.StartCoroutine(DoubleTapped());
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) // Double tap R
        {
            ctx.tapCountR += 1;
            if(ctx.tapCountR == 1 && !ctx.coroutineActive)
            {
                ctx.firstTapTime = Time.time;
                ctx.StartCoroutine(DoubleTapped());
            }
        }
    }

    IEnumerator DoubleTapped()
    {
        ctx.coroutineActive = true;

        while(Time.time < ctx.firstTapTime + ctx.timeBetTaps)
        {
            if(ctx.tapCountL == 2)
            {
                ctx.rollLeft = true;
                ctx.StartCoroutine(BarrelRoll(ctx.barrelTime));
                yield return new WaitForSeconds(ctx.barrelTime);
                ctx.rollLeft = false;
                break;
            }

            if(ctx.tapCountR == 2)
            {
                ctx.rollRight = true;
                ctx.StartCoroutine(BarrelRoll(ctx.barrelTime));
                yield return new WaitForSeconds(ctx.barrelTime);
                ctx.rollRight = false;
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        ctx.tapCountL = 0;
        ctx.tapCountR = 0;
        ctx.firstTapTime = 0f;
        ctx.coroutineActive = false;
    }

    IEnumerator BarrelRoll(float dur)
    {
        float startRot = ctx.transform.localEulerAngles.z;
        float endRot = startRot + 360f;
        float t = 0f;

        ctx.barrelDeflect.SetActive(true);
        ctx.barrelRollEffect.Play();
        ctx.isRolling = true;

        while(t < dur)
        {
            t += Time.deltaTime;
            float zRot = Mathf.Lerp(startRot, endRot, t / dur) % 360.0f;

            if(ctx.rollLeft)
                ctx.transform.localEulerAngles = new Vector3(ctx.transform.localEulerAngles.x, ctx.transform.localEulerAngles.y, zRot);
            if(ctx.rollRight)
                ctx.transform.localEulerAngles = new Vector3(ctx.transform.localEulerAngles.x, ctx.transform.localEulerAngles.y, -zRot);

            yield return null;
        }

        ctx.barrelDeflect.SetActive(false);
        ctx.barrelRollEffect.Stop();
        ctx.isRolling = false;
    }
}
