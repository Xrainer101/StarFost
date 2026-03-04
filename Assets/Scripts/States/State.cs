using System;
using UnityEngine;

// The base state class
public abstract class State<EState> where EState : Enum // EState is an enum that is specifically meant for states
{
    // Reference to the state's key in the state machine dictionary
    public EState StateKey {get; private set;}

    // Constructor with reference to its key (might delete later)
    public State(EState key)
    {
        StateKey = key;
    }

    // Abstract methods that will be called by state machine
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void OnStateTriggerEnter(Collider other);
    public abstract void OnStateTriggerStay(Collider other);
    public abstract void OnStateTriggerExit(Collider other);
}
