using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftState : MonoBehaviour, IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    public LiftState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
		TryGetComponent<FSM>(out this.manager);
		this.parameter = manager.parameter;
		parameter._input.pickUpItem_L = parameter._input.pickUpItem_R = false;
	}

    public void OnUpdate()
    {
		Lifting();

	}

    public void OnExit()
    {
		parameter._input.liftingItem = false;
		parameter._input.pickUpItem_R = false;
		manager = null;

	}
	#region Lifting
	private void Lifting()
	{
		if (parameter._holdWeightItem) parameter.SprintSpeed = 2;
		else parameter.SprintSpeed = 5.335f;
		manager.Look("Weight");
		if (manager.Look("Weight") && parameter._input.liftingItem && !parameter._holdWeightItem && !manager.Look("Item") && !parameter._holdItem_R && !parameter._holdItem_L)
		{
			manager.transform.forward = new Vector3(parameter._hitWeight.transform.position.x, manager.transform.position.y, parameter._hitWeight.transform.position.z) - manager.transform.position;
			parameter._weightDis = Vector3.Distance(parameter._hitWeight.point, parameter._hitWeight.transform.position);
			if (Vector3.Distance(manager.transform.position, new Vector3(parameter._hitWeight.transform.position.x, manager.transform.position.y, parameter._hitWeight.transform.position.z)) > parameter._weightDis + 0.1f + parameter._controller.radius)
			{
				//Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
				//transform.position = lerpTargetPos;
				parameter._speed = 0.5f;
				parameter._animator.SetFloat(parameter._animIDSpeed, 1f);
			}
			else
			{
				//播动画
				parameter._animator.SetBool(parameter._animLifting, true);
				parameter._animator.SetBool(parameter._animLiftingHold, true);
				parameter._holdWeightItem = true;
				parameter.MoveSpeed = 0;
				parameter.SprintSpeed = 0;
				parameter._input.liftingItem = false;
			}
		}
		else if (parameter._input.liftingItem && parameter._holdWeightItem || parameter._stamina.ResidueRtamina <= 0 && parameter._holdWeightItem)
		{
			parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 0);
			manager.transform.GetChild(4).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			manager.transform.GetChild(4).transform.parent = manager.transform.parent;
			parameter._animator.SetBool(parameter._animLiftingHold, false);
			parameter._holdWeightItem = false;
			parameter._stamina.BeginReplyToMaxStamina = true;
			manager.TransitionState(StateType.Idle);
		}
		else parameter._input.liftingItem = false;

		

	}
	//关键帧时让物体与ik重合
	private void LiftingSetItemPos()
	{
		//var dis=Vector3.Distance(_hitItme.point,_hitItme.transform.position);
		parameter._animator.SetBool(parameter._animLifting, false);
		if (manager.Look("Weight"))
		{
			parameter._hitWeight.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			parameter._hitWeight.transform.parent = parameter.RightHandHoldPos;
		}
		else
		{
			parameter._animator.SetBool(parameter._animLiftingHold, false);
			parameter._holdWeightItem = false;
			parameter._stamina.BeginReplyToMaxStamina = true;
		}
		//_hitItme.transform.position = RightHandHoldPos.position + transform.forward*dis/2;
		//_hitItme.transform.up = transform.up;
		//s_hitItme.transform.Rotate(0, 60, 0);

	}
	private void LiftingItemEnd()
	{
		parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 1);
		if (parameter.RightHandHoldPos.GetChild(0) != null) parameter.RightHandHoldPos.GetChild(0).transform.parent = manager.transform;
		if (manager.transform.GetChild(4) != null)
		{
			manager.transform.GetChild(4).transform.up = manager.transform.up;
			manager.transform.GetChild(4).transform.forward = manager.transform.forward;
			manager.transform.GetChild(4).transform.position = manager.transform.position + manager.transform.up * 1.1f + manager.transform.forward * (parameter._weightDis + parameter._controller.radius - 0.125f);
		}
		parameter.MoveSpeed = 2;
		parameter.SprintSpeed = 5.335f;
		//manager.TransitionState(StateType.Idle);
	}
    #endregion
    private void OnAnimatorIK(int layerIndex)
    {
		if(manager != null) 
		{ 
			if (parameter._holdWeightItem && parameter._hitWeightItemRightHand.point != Vector3.zero && parameter._hitWeightItemLeftHand.point != Vector3.zero)
			{
				parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, new Vector3(parameter.RightHand.position.x, parameter._hitWeightItemRightHand.point.y, parameter.RightHand.position.z));
				parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, new Vector3(parameter.LeftHand.position.x, parameter._hitWeightItemLeftHand.point.y, parameter.LeftHand.position.z));
			}
			else if (!parameter._holdWeightItem && !parameter._animator.GetBool(parameter._animIDClimbingIdleWall))
			{
				parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter.RightHand.position);
				parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter.LeftHand.position);
			}
		}
	}
}

