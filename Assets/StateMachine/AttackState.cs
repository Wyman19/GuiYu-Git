using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MonoBehaviour, IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo info;
    public AttackState(FSM manager)
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
		CheckAttackCombo();

	}

    public void OnExit()
    {

    }
	#region Attack
	

	private void AttackComboStart()
	{
		parameter._isComboTime = true;
		parameter._stamina.AttackStamina();
		parameter._animator.SetBool(parameter._animAttack, false);
	}
	private void CheckAttackCombo()
	{
		if (parameter._playerDamageRange_R != null)
		{
			TowardEnemy();
			if (parameter._isComboTime || parameter._firstCombo)
			{
				//Debug.Log("1");
				if (parameter._input.mouseRight && parameter._stamina.ResidueRtamina >= 10)
				{
					//Debug.Log("2");
					parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 0);
					parameter._animator.SetBool(parameter._animAttack, true);
					parameter._firstCombo = false;
					parameter.MoveSpeed = parameter.SprintSpeed = 0.01f;
				}

			}
			else parameter._input.mouseRight = false;
		}
		else
		{
			parameter._input.switchTwoHand = false;
		}

	}
	private void GiveDamageStart()
	{
		if (parameter._playerDamageRange_R != null)
			parameter._playerDamageRange_R.isGiveDamage = true;
	}
	private void GiveDamageEnd()
	{
		if (parameter._playerDamageRange_R != null)
			parameter._playerDamageRange_R.isGiveDamage = false;
	}
	private void AttackComboEnd()
	{
		parameter._isComboTime = false;
		if (!parameter._animator.GetBool(parameter._animAttack))
		{
			parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 1);
			parameter._firstCombo = true;
			parameter.MoveSpeed = 2;
			parameter.SprintSpeed = 5.335f;
			manager.TransitionState(StateType.Idle);
		}
	}
	private void AttackEnd()
	{
		parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 1);
		parameter._stamina.AttackStamina();
		parameter._animator.SetBool(parameter._animAttack, false);
		parameter._firstCombo = true;
		parameter.MoveSpeed = 2;
		parameter.SprintSpeed = 5.335f;
		manager.TransitionState(StateType.Idle);

	}
	public void TowardEnemy()
	{
		if (parameter._lock.isLockOn && !parameter._firstCombo && parameter._lock.target != null)
		{
			manager.transform.forward = parameter._lock.target.gameObject.transform.position - manager.transform.position;
		}
	}
	#endregion
}

