using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : IState
{
    private FSM manager;
    private Parameter parameter;


    public RollState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter._animator.SetBool(parameter._animRoll, true);
        parameter._input.roll = false;
        parameter._stamina.RollStamina();
    }

    public void OnUpdate()
    {
        AnimatorStateInfo stateinfo = parameter._animator.GetCurrentAnimatorStateInfo(0);
        bool play_ing_flag = stateinfo.IsName("Stand To Roll");
        if (parameter._animator.GetFloat("RollTime") < 1 && play_ing_flag)
        {
            parameter._animator.SetBool(parameter._animRoll, false);
            manager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {
        parameter._input.roll = false;
    }
    
}

