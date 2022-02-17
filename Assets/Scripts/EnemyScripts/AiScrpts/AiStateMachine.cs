using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AiStateMachine
{
    [SerializeField] AiStateI[] states;
    [SerializeField] AiAgent agent;
    [SerializeField] AiStateId currentState;

    // shortcut for constructor - ctor
    public AiStateMachine(AiAgent _agent)
    {
        this.agent = _agent;
        int numStates = Enum.GetNames(typeof(AiStateId)).Length;
        states = new AiStateI[numStates];
    }

    public void RegisterState(AiStateI _state)
    {
        int index = (int)_state.GetId();
        states[index] = _state;
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }


    public AiStateI GetState(AiStateId _stateId)
    {
        int index = (int)_stateId;
        return states[index];
    }

    public void ChangeState(AiStateId _newState)
    {
        // first leave current State
        GetState(currentState)?.Exit(agent);
        currentState = _newState;
        // enter new state
        GetState(currentState)?.Enter(agent);
    }

    public AiStateId GetCurrentState()
    {
        return currentState;
    }

}
