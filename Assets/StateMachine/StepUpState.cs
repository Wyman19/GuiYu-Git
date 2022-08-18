using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepUpState : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo stateinfo;

    public StepUpState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        //parameter._animator.Play("Idle Walk Run Blend");
        if (parameter._hasAnimator)
        {
            parameter._animator.SetBool(parameter._animStepUp, true);
        }

    }

    public void OnUpdate()
    {

        stateinfo = parameter._animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(stateinfo.normalizedTime + "" + stateinfo.IsName("StepUp"));
        if (stateinfo.normalizedTime > 0.7f && stateinfo.IsName("StepUp"))
        {
            parameter._verticalVelocity = Mathf.Sqrt((parameter._hitStepHigh) * -2f * parameter.Gravity);
            Debug.Log("л╗╫в" + parameter._verticalVelocity);
            manager.TransitionState(StateType.Idle);
        }
        //иол╗╫в
    }

    public void OnExit()
    {
        if (parameter._hasAnimator)
        {
            parameter._animator.SetBool(parameter._animStepUp, false);
        }
    }

}







