using System;
using UnityEngine;
using MonsterLove.StateMachine;

public class FSMDemo2 : MonoBehaviour
{
    public enum StateDemo
    {
        Start,
        Running,
        End,
    }

    private StateMachine<StateDemo> m_state;

    private void Awake()
    {
        m_state = StateMachine<StateDemo>.Initialize(this);
        ChangeStateByName("Start");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeStateByName("Running");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeStateByName("End");
        }
    }

    private void ChangeStateByName(string stateName)
    {
        if (Enum.TryParse(stateName, out StateDemo newState))
        {
            Debug.Log($"Changing state to: {newState}");
            m_state.ChangeState(newState);
        }
        else
        {
            Debug.LogWarning($"State {stateName} not found!");
        }
    }

    #region FSM Methods

    private void Start_Enter() => Debug.Log("Entered Start State");
    private void Start_Update() => Debug.Log("Updating Start State");
    private void Start_Exit() => Debug.Log("Exiting Start State");

    private void Running_Enter() => Debug.Log("Entered Running State");
    private void Running_Update() => Debug.Log("Updating Running State");
    private void Running_Exit() => Debug.Log("Exiting Running State");

    private void End_Enter() => Debug.Log("Entered End State");
    private void End_Update() => Debug.Log("Updating End State");
    private void End_Exit() => Debug.Log("Exiting End State");

    #endregion
}
