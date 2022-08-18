using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    private FSM manager;
    private Parameter parameter;

    public JumpState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
		// reset the fall timeout timer
		

		//Jump
		// the square root of H * -2 * G = how much velocity needed to reach desired height
		parameter._verticalVelocity = Mathf.Sqrt(parameter.JumpHeight * -2f * parameter.Gravity * 1.6f);
        Debug.Log(parameter._verticalVelocity);
        // update animator if using character
        if (parameter._hasAnimator)
        {
            parameter._animator.SetBool(parameter._animIDJump, true);
        }
        parameter._input.climb = false;

    }

	public void OnUpdate()
    {
        //ÂäµØ
        if (parameter.Grounded&& parameter._animator.GetBool(parameter._animIDFreeFall))
        {
            manager.TransitionState(StateType.Idle);
        }
        //JumpAndGravity();
    }

    public void OnExit()
    {
		parameter._input.jump = false;
        parameter._animator.SetBool(parameter._animIDJump, false);
    }
	

}
