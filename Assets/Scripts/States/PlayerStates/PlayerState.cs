using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public abstract class PlayerState : State<PlayerStateMachine.EPlayerState>
{
    protected PlayerStateMachine ctx;

    protected float horiInput, vertInput;
    protected bool actionReady;
    protected bool coroutineActive;
    protected bool somersault;

    protected bool rollLeft, rollRight;
    protected int tapCountL, tapCountR;
    protected bool isRolling;

    public PlayerState(PlayerStateMachine context, PlayerStateMachine.EPlayerState ePlayerState) : base(ePlayerState)
    {
        ctx = context;
    }

    // Constantly go forward and turn to match the input
    protected void Movement()
    {
        if(!somersault) {
            ctx.transform.Translate(Vector3.forward * ctx.moveSpeed * Time.deltaTime);

            Vector3 movement = new Vector3(horiInput, vertInput, 0);
            ctx.transform.position += new Vector3(movement.x, movement.y, movement.z) * ctx.moveSpeed * Time.deltaTime;
            if (rollLeft || rollRight) ctx.transform.position += new Vector3(movement.x * 1.5f, movement.y, movement.z) * ctx.moveSpeed * Time.deltaTime;
        }
    }

    // For visual tilting of the ship
    protected void XZTilting()
    {
        if(!somersault){
            TiltZ(horiInput);
            TiltX(vertInput);
        }
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
        if(Input.GetKey(KeyCode.LeftAlt)) // lean left
        {
            Vector3 newEuAng = ctx.transform.localEulerAngles;
            ctx.transform.localEulerAngles = new Vector3(newEuAng.x, newEuAng.y, Mathf.LerpAngle(newEuAng.z, 95, ctx.leanSpeed));
        }

        if(Input.GetKey(KeyCode.Space)) // lean right
        {
            Vector3 newEuAng = ctx.transform.localEulerAngles;
            ctx.transform.localEulerAngles = new Vector3(newEuAng.x, newEuAng.y, Mathf.LerpAngle(newEuAng.z, -95, ctx.leanSpeed));
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt)) // Double tap L
        {
            tapCountL += 1;
            if(tapCountL == 1 && !coroutineActive)
            {
                ctx.firstTapTime = Time.time;
                ctx.StartCoroutine(DoubleTapped());
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) // Double tap R
        {
            tapCountR += 1;
            if(tapCountR == 1 && !coroutineActive)
            {
                ctx.firstTapTime = Time.time;
                ctx.StartCoroutine(DoubleTapped());
            }
        }
    }

    IEnumerator DoubleTapped()
    {
        // Ensure you cannot barrel roll while barrel rolling
        coroutineActive = true;

        while(Time.time < ctx.firstTapTime + ctx.timeBetTaps)
        {
            if(tapCountL == 2)
            {
                // Let the code know you are rolling left
                rollLeft = true;
                ctx.StartCoroutine(BarrelRoll(ctx.barrelTime));
                yield return new WaitForSeconds(ctx.barrelTime);
                rollLeft = false;
                break;
            }

            if(tapCountR == 2)
            {
                // Let the code know you are rolling right
                rollRight = true;
                ctx.StartCoroutine(BarrelRoll(ctx.barrelTime));
                yield return new WaitForSeconds(ctx.barrelTime);
                rollRight = false;
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        // Reset all variables after barrel roll ends
        tapCountL = 0;
        tapCountR = 0;
        ctx.firstTapTime = 0f;
        coroutineActive = false;
    }

    IEnumerator BarrelRoll(float dur)
    {
        float startRot = ctx.transform.localEulerAngles.z;
        float endRot = startRot + 360f;
        float t = 0f;

        // Bring deflection object up and roll particle effect
        ctx.barrelDeflect.SetActive(true);
        ctx.barrelRollEffect.Play();
        isRolling = true;

        // Change the direction of the effect for each barrel roll direction
        var donutShape = ctx.barrelRollEffect.shape;
        if(rollLeft)
            donutShape.arcSpeedMultiplier = -ctx.barrelEffectSpeed;
        if(rollRight)
            donutShape.arcSpeedMultiplier = ctx.barrelEffectSpeed;

        // For the barrel time duration, spin on the z axis
        while(t < dur)
        {
            t += Time.deltaTime;
            // Smoothly find target rotation
            float zRot = Mathf.Lerp(startRot, endRot, t / dur) % 360.0f;

            // Change direction based on whether you move left or right
            if(rollLeft)
                ctx.transform.localEulerAngles = new Vector3(ctx.transform.localEulerAngles.x, ctx.transform.localEulerAngles.y, zRot);
            if(rollRight)
                ctx.transform.localEulerAngles = new Vector3(ctx.transform.localEulerAngles.x, ctx.transform.localEulerAngles.y, -zRot);

            yield return null;
        }

        // Turn off effects and deflection
        ctx.barrelDeflect.SetActive(false);
        ctx.barrelRollEffect.Stop();
        isRolling = false;
    }

    protected void SpeedInput()
    {
        bool hasEnergy = ctx.hudManager.boostMeter.value > 0f;
        bool canAct = actionReady && hasEnergy;

        if(Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S) && canAct) // Somersault
        {
            ctx.StartCoroutine(Somersault(2.5f));
            return;
        }

        if(somersault) return;

        if(Input.GetKey(KeyCode.LeftShift) && hasEnergy) // Boost
        {
            SpeedAction(20f, ctx.boostOffset, ctx.boostFOV);
            ctx.shipEmitters.EmitBoost();
            ctx.hudManager.actionCooling = false; // Drain the boost meter
        } else if(Input.GetKey(KeyCode.LeftControl) && hasEnergy) // Brake
        {
            SpeedAction(5f, ctx.brakeOffset, ctx.brakeFOV);
            ctx.shipEmitters.EmitBrake();
            ctx.hudManager.actionCooling = false; // Drain the boost meter
        } else // Normal speed
        {
            SpeedAction(ctx.speedStore, ctx.normalOffset, ctx.normalFOV);
            ctx.hudManager.actionCooling = true; // Regen the boost meter
            ctx.shipEmitters.EmitNorm();

            if(ctx.hudManager.boostMeter.value < 1f)
            {
                actionReady = false;
            }
        }

        // If the boost meter is full, you can do an action
        if(ctx.hudManager.boostMeter.value >= 1f)
            actionReady = true;
    }

    // Sets the speed, camera offset, and FOV smoothly
    protected void SpeedAction(float newSpeed, Vector3 targetOffset, float targetFOV)
    {
        // Debug.Log(targetOffset.ToString());
        // Debug.Log(targetFOV.ToString());
        ctx.moveSpeed = newSpeed;
        if(ctx.vTransposer != null){
            ctx.vTransposer.m_FollowOffset = Vector3.Lerp(
                ctx.vTransposer.m_FollowOffset,
                targetOffset,
                ctx.transitionSpeed * Time.deltaTime
            );
        }
        ctx.vCam.m_Lens.FieldOfView = Mathf.Lerp(
            ctx.vCam.m_Lens.FieldOfView,
            targetFOV,
            ctx.transitionSpeed * Time.deltaTime
        );
    }

    IEnumerator Somersault(float dur)
    {
        // Radius of the somersault loop
        float radius = 5f;

        somersault = true;
        //ctx.vCam.LookAt = ctx.playerPos;

        Vector3 startPos = ctx.transform.position;
        // Debug.Log(startPos.ToString());
        // Debug.Log(ctx.transform.position.ToString());
        float startRotX = ctx.transform.localEulerAngles.x;

        float t = 0f;

        while(t < dur)
        {
            ctx.shipEmitters.EmitBoost(); // Make the thrusters look more powerful
            ctx.vCam.m_Lens.FieldOfView = Mathf.Lerp(
                ctx.vCam.m_Lens.FieldOfView,
                ctx.boostFOV,
                ctx.transitionSpeed * Time.deltaTime
            );

            t += Time.deltaTime;

            // Get the exact percentage of completion (0 - 1)
            float percent = t / dur;

            // Convert to degrees (for rotation) and radians (for math)
            float degrees = percent * 360f;
            float radians = degrees * Mathf.Deg2Rad;

            // Cosine calculates up/down curve (y-axis) curve
            // Sine calculates forward/backward (z-axis) curve
            float yOffset = radius * (1f - Mathf.Cos(radians));
            float zOffset = radius * Mathf.Sin(radians);

            ctx.transform.position = startPos + new Vector3(0f, yOffset, zOffset);

            ctx.transform.localEulerAngles = new Vector3(startRotX - degrees, 0f, 0f);

            ctx.hudManager.actionCooling = false;
            yield return null;
        }

        // Debug.Log(ctx.transform.position.ToString());
        // Debug.Log("Snapped position: " + ctx.transform.position.ToString());
        ctx.transform.localEulerAngles = new Vector3(startRotX, 0f, 0f);

        //ctx.vCam.LookAt = null;
        ctx.shipEmitters.EmitNorm();
        somersault = false;
    }
}
