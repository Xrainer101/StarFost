using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// This is a state machine that uses enums from the EPlayerState list
public class PlayerStateMachine : StateMachine<PlayerStateMachine.EPlayerState>
{
    // All player states
    public enum EPlayerState
    {
        OnRails,
        AllRange,
        Death,
    }

    [Header("State Variables (Shared)")] // for movement
    public float moveSpeed = 15f;
    public float tilt = 30f;
    public float tiltSpeed = 0.1f;
    public float leanSpeed;
    public float firstTapTime, timeBetTaps;
    public bool coroutineActive;
    public int tapCountL, tapCountR;
    public bool rollLeft, rollRight;
    public float barrelTime;
    public bool isRolling;
    public GameObject barrelDeflect;
    public ParticleSystem barrelRollEffect;

    void Awake()
    {
        InitializeStates(); // Initialize the states before the game starts
    }

    private void InitializeStates()
    {
        // Add newly constructed states to inherited StateManager "States" dictionary and set initial state
        States.Add(EPlayerState.OnRails, new OnRailsState(this, EPlayerState.OnRails));
        States.Add(EPlayerState.AllRange, new OnRailsState(this, EPlayerState.AllRange));
        States.Add(EPlayerState.Death, new OnRailsState(this, EPlayerState.Death));
        CurrentState = States[EPlayerState.OnRails];
    }
}
