using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : IState
{
    private FSM manager;
    private Parameter parameter;

    public CrouchState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter._input.crouch = false;
        parameter._iscrouch = true;
        parameter.MoveSpeed = 0.5f;
        parameter.SprintSpeed = 0.5f;
        if (parameter._animator)
        {
            parameter._animator.SetBool(parameter._animCrouch, true);
        }
    }

    public void OnUpdate()
    {
        if (parameter._input.crouch && parameter._iscrouch)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {
        parameter._input.crouch = false;
        parameter._iscrouch = false;
        parameter.MoveSpeed = 2.0f;
        parameter.SprintSpeed = 5.335f;
        if (parameter._animator)
        {
            parameter._animator.SetBool(parameter._animCrouch, false);
        }
    }
    private void Crouch()
    {
        if (parameter._input.crouch && !parameter._iscrouch)
        {
            parameter._input.crouch = false;
            parameter._iscrouch = true;
            parameter.MoveSpeed = 0.5f;
            parameter.SprintSpeed = 0.5f;
            if (parameter._animator)
            {
                parameter._animator.SetBool(parameter._animCrouch, true);
            }
        }
        if (parameter._input.crouch && parameter._iscrouch)
        {
            parameter._input.crouch = false;
            parameter._iscrouch = false;
            parameter.MoveSpeed = 2.0f;
            parameter.SprintSpeed = 5.335f;
            if (parameter._animator)
            {
                parameter._animator.SetBool(parameter._animCrouch, false);
            }
        }
    }
}
