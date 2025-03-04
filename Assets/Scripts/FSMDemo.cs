using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UDEV.PlatfromGame;

public class FSMDemo : MonoBehaviour
{
    public enum StateDemo
    {
        State1,
        State2,
        State3,
    }
    private StateMachine<StateDemo> m_state;
    public Animator anim;
    private void Awake() 
    {
        m_state = StateMachine<StateDemo>.Initialize(this);
        m_state.ChangeState(StateDemo.State1);
        // anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_state.ChangeState(StateDemo.State2);
        }
    }

    #region FSM
    void State1_Enter()
    {
        Debug.Log("State 1 Enter");
    }
    void State1_Update()
    {
        Helper.PlayAnim(anim, "Swim");
    }
    void State1_FixedUpdate()
    {
        Debug.Log("State 1 FixedUpdate");
    }
    void State1_Exit()
    {
        Debug.Log("State 1 Exit");
    }
    void State1_Finally()
    {
        Debug.Log("State 1 Finally");
    }

    void State2_Enter()
    {
        Debug.Log("State 2 Enter");
    }
    void State2_Update()
    {
        Debug.Log("State 2 Update");
    }
    void State2_FixedUpdate()
    {
        Debug.Log("State 2 FixedUpdate");
    }
    void State2_Exit()
    {
        Debug.Log("State 2 Exit");
    }
    void State2_Finally()
    {
        Debug.Log("State 2 Finally");
    }
    #endregion
}
