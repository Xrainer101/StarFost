using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    public Transform playerPos;

    [Header("Normal movement variables")] // for movement
    public float moveSpeed = 15f;
    public float speedStore;
    public float tilt = 30f;
    public float tiltSpeed = 0.1f;
    public float leanSpeed;
    public CinemachineVirtualCamera vCam;
    public CinemachineTransposer vTransposer;
    public HUDManager hudManager;

    [Header("Boost variables")]
    public ShipEmissions shipEmitters;
    public Vector3 normalOffset = new Vector3(0, 1, -8);
    public Vector3 boostOffset = new Vector3(0, 1, -9);
    public Vector3 brakeOffset = new Vector3(0, 1, -7);
    public float transitionSpeed = 5f;
    public float normalFOV = 60f;
    public float boostFOV = 70f;
    public float brakeFOV = 55f;

    [Header("Barrel roll variables")]
    public float firstTapTime, timeBetTaps;
    public float barrelTime;
    public float barrelEffectSpeed;
    public GameObject barrelDeflect;
    public ParticleSystem barrelRollEffect;

    void Awake()
    {
        speedStore = moveSpeed;
        shipEmitters = GetComponent<ShipEmissions>();
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
