using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushState : MonoBehaviour, IState
{
    private FSM manager;
    private Parameter parameter;


    public PushState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        TryGetComponent<FSM>(out this.manager);
        this.parameter = manager.parameter;
    }

    public void OnUpdate()
    {
        Push();
    }

    public void OnExit()
    {
        parameter._animator.SetBool(parameter._animPush, false);
        parameter.MoveSpeed = 2f; parameter.SprintSpeed = 5.335f;
        parameter._controller.radius = 0.28f;

    }
    #region Push
    private void Push()
    {
        Vector3 LowRayPosition = new Vector3(transform.position.x, transform.position.y + parameter.ClimbGroundedOffset, transform.position.z);
        if (parameter._speed >= 1f) parameter.HavePushItem = Physics.Raycast(LowRayPosition, transform.forward, out parameter._hitPushItem, parameter.PushDistance, parameter.PushLayers, QueryTriggerInteraction.Ignore);
        else parameter.HavePushItem = false;
        if (parameter.HavePushItem)
        {

            parameter._controller.radius = parameter.PushDistance - 0.1f;
            parameter.MoveSpeed = parameter.SprintSpeed = 1f;
            parameter._hitPushItem.rigidbody.velocity = transform.forward * 1f;
            if (parameter._speed > 0.1f) parameter._animator.SetBool(parameter._animPush, true);
            else
            {
                parameter._animator.SetBool(parameter._animPush, false);
                parameter.MoveSpeed = 2f; parameter.SprintSpeed = 5.335f;
                manager.TransitionState(StateType.Idle);
            }

        }
        else
        {
            parameter._animator.SetBool(parameter._animPush, false);
            parameter._controller.radius = 0.28f;
            manager.TransitionState(StateType.Idle);
        }

    }
    private void PushEnd()
    {
        parameter.MoveSpeed = 2f; parameter.SprintSpeed = 5.335f;
    }
    #endregion
}

