using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base state machine class
public class StateMachine<EState> : MonoBehaviour where EState : Enum // Estate is an enum that is specifically a state
{
    // Dictionary for holding states and their enum key
    protected Dictionary<EState, State<EState>> States = new Dictionary<EState, State<EState>>();

    public State<EState> CurrentState {get; protected set;} // Current state
    protected bool IsTransitioningState = false; // To ensure the transition completes before another transition starts

    // Start is called before the first frame update
    void Start()
    {
        CurrentState.EnterState(); // Enter the current state (set in implemented state machines)
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsTransitioningState) // If not transitioning
        {
            CurrentState.UpdateState(); // Run the state's fixed update method
        }
    }

    void FixedUpdate()
    {
        if(!IsTransitioningState) // If not transitioning
        {
            CurrentState.FixedUpdateState(); // Run the current state's fixed update method
        }  
    }

    //Transition method that can be called anywhere
    public void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;        // Transition is happening
        CurrentState.ExitState();           // Call current state's exit method
        CurrentState = States[stateKey];    // Set current state to the enum passed in
        CurrentState.EnterState();          // Call new current state's enter method
        IsTransitioningState = false;       // No transition happening
    }

    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnStateTriggerEnter(other);    // Call the state's trigger enter method
    }

    void OnTriggerStay(Collider other)
    {
        CurrentState.OnStateTriggerStay(other);     // Call the state's trigger stay method
    }

    void OnTriggerExit(Collider other)
    {
        CurrentState.OnStateTriggerExit(other);     // Call the state's trigger exit method
    }
}
