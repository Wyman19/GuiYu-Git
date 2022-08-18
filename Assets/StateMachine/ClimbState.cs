using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : MonoBehaviour , IState
{
    private FSM manager;
    private Parameter parameter;

    public ClimbState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
		TryGetComponent<FSM>(out this.manager);
		this.parameter = manager.parameter;
		//parameter._animator.Play("Idle Walk Run Blend");
    }

    public void OnUpdate()
    {
		ClimbAcceleratedSpeed();
		Climb();
	}

	public void OnExit()
    {
		manager = null;
		parameter._input.climb = false;
		parameter._animator.SetBool(parameter._animIDClimbingToWall, false);
		parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);

	}
	#region Climb
	private void ClimbAcceleratedSpeed()
	{
		
		//�������ٶ�
		float ClimbInputRate(float input, float speed)
		{
			if (input != 0)
			{
				speed += input * Time.deltaTime;
				if (speed > 1) { speed = 1; }
				if (speed < -1) { speed = -1; }
				//Debug.Log(speed);
				return speed;
			}
			else
			{
				if (speed > 0)
				{
					speed -= Time.deltaTime;
					if (speed < 0) { speed = 0; }
				}
				else
				{
					speed += Time.deltaTime;
					if (speed > 0) { speed = 0; }
				}
				//Debug.Log(speed);
				return speed;
			}
		}
		float InputMoveY(float inputY)
		{
			float Y;
			if (inputY < 0) { parameter.IsWallTop = parameter._rightHandWallToLand = parameter._leftHandWallToLand = parameter._moveWallTop = false; }
			if (parameter.IsWallTop && inputY > 0) { Y = 0; }
			else { Y = inputY; }
			return Y;
		}
		float InputMoveX(float inputX)
		{
			float X;
			if ((inputX > 0 && !parameter._rightBoxCheckWall) || (inputX < 0 && !parameter._leftBoxCheckWall)) { X = 0; parameter._climbAcceleratedSpeed_X = parameter._climbAcceleratedSpeed_X / 1.1f; }
			else { X = inputX; }
			return X;
		}
		parameter._climbAcceleratedSpeed_X = ClimbInputRate(InputMoveX(parameter._input.move.x), parameter._climbAcceleratedSpeed_X);
		parameter._climbAcceleratedSpeed_Y = ClimbInputRate(InputMoveY(parameter._input.move.y), parameter._climbAcceleratedSpeed_Y);
		
	}

	private void Climb()
	{
		AnimatorStateInfo stateinfo = parameter._animator.GetCurrentAnimatorStateInfo(0);
		bool play_ing_flag = stateinfo.IsName("Blend Tree");
		if (parameter.HaveWall)
		{
			if (parameter._input.climb)
			{
				
				//����ǽ��
				if (parameter._hitWallHigh - 0.9f > parameter.ClimbHighWall || play_ing_flag)
				{
					//�����ת��ǰ���뷨��������
					manager.transform.forward = -parameter._hitWall.normal;
					//����������ճ��ǽ
					parameter.Gravity = 0;
					parameter._verticalVelocity = 0;
					//if (Vector3.Distance(manager.transform.position, parameter.targetPos) > 0.03f)
     //               {
     //                   Debug.Log("��ǽ");
     //                   manager.transform.position = parameter.targetPos;
     //               }
                    //Vector3 lerpTargetPos = Vector3.MoveTowards(manager.transform.position, parameter.targetPos, 0.1f);
                    //manager.transform.position = lerpTargetPos;
                    //ǽ���ƶ�
                    if (parameter._hasAnimator)
					{
						//count = 1;
						parameter._animator.SetFloat(parameter._animIDClimbingHorizontalWall, parameter._climbAcceleratedSpeed_X);
						parameter._controller.Move(manager.transform.right * parameter.ClimbSpeed * Time.deltaTime * parameter._climbAcceleratedSpeed_X / 1.4f);
						parameter._animator.SetFloat(parameter._animIDClimbingVerticalWall, parameter._climbAcceleratedSpeed_Y);
						//parameter._controller.Move(manager.transform.up * parameter.ClimbSpeed * Time.deltaTime * parameter._climbAcceleratedSpeed_Y);
					}
					// update animator if using character
					if (parameter._hasAnimator)
					{
						parameter._animator.SetBool(parameter._animIDClimbingIdleWall, true);
					}

				}
				else if(!parameter._animator.GetBool(parameter._animIDClimbingToWall))
				{
					if (parameter.ClimbMediumWall < parameter._hitWallHigh - 0.9f && parameter._hitWallHigh - 0.9f < parameter.ClimbHighWall)
					{
						if (parameter._hasAnimator)
						{
							parameter._animator.SetBool(parameter._animIDClimbHighWall, true);
						}
						//��������ClimbHighWallEvent���ٶ�				
					}
					if (parameter.ClimbMediumWall > parameter._hitWallHigh - 0.9f && parameter._hitWallHigh - 0.9f > parameter.ClimbLowWall)
					{
						if (parameter._hasAnimator)
						{
							parameter._animator.SetBool(parameter._animIDClimbMediumWall, true);
						}
						//��������ClimbMediumWallEvent���ٶ�
					}
					if (parameter._hitWallHigh - 0.9f < parameter.ClimbLowWall)
					{
						if (parameter._hasAnimator)
						{
							parameter._animator.SetBool(parameter._animIDClimbLowWall, true);
						}
						//��������ClimbLowWallEvent���ٶ�
						//F*S=1/2*m*vƽ�� v=Sqrt(2FS)
					}
					parameter._input.jump = false;

				}//����ǽ��

			}
			if (parameter._input.releaseClimb || parameter._stamina.ResidueRtamina <= 0)
			{
				parameter._input.jump = false;
				if (parameter._hasAnimator)
				{
					parameter._animator.SetBool(parameter._animIDJump, false);
					parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
				}
				parameter.Gravity = -15;
				parameter._input.climb = false;
				parameter._input.releaseClimb = false;
				manager.TransitionState(StateType.Idle);
			}//��������
		}
		
		stateinfo = parameter._animator.GetCurrentAnimatorStateInfo(0);
		play_ing_flag = stateinfo.IsName("Blend Tree");
		//Debug.Log(play_ing_flag);
		if (play_ing_flag)
		{
			//Debug.Log("moveWallTop:" + parameter._moveWallTop + parameter._rightHandWallToLand);
			if (parameter._rightHandWallToLand && !parameter.IsWallTop)
			{

				parameter.WallTopPos_Y = parameter.RightHand.position.y;
				//_controller.Move(transform.up * (WallTopPos_Y - transform.position.y - 1.5f));
				parameter.IsWallTop = true;
				parameter._input.jump = false;
				parameter._moveWallTop = true;
			}
			if (parameter._leftHandWallToLand && !parameter.IsWallTop)
			{
				parameter.WallTopPos_Y = parameter.LeftHand.position.y;
				//_controller.Move(transform.up * (WallTopPos_Y - transform.position.y - 1.5f));
				parameter.IsWallTop = true;
				parameter._input.jump = false;
				parameter._moveWallTop = true;
			}
			if (parameter.IsWallTop && parameter._moveWallTop && !(parameter._input.move.y < 0))
			{

				if (Vector3.Distance(manager.transform.position, new Vector3(manager.transform.position.x, parameter.WallTopPos_Y - 1.5f, manager.transform.position.z)) < 0.01f)
				{

					manager.transform.position = new Vector3(manager.transform.position.x, parameter.WallTopPos_Y, manager.transform.position.z);
					parameter._moveWallTop = false;
					return;
				}
				//Debug.Log("��������");
				if (manager.transform.position.y < parameter.WallTopPos_Y - 1.5f) parameter._controller.Move(manager.transform.up * Time.deltaTime / 7);
				if (manager.transform.position.y > parameter.WallTopPos_Y - 1.5f) parameter._controller.Move(-manager.transform.up * Time.deltaTime / 7);
			}

		}//����Ƿ�����ǽ�����Ϸ�
		if (parameter.IsWallTop && parameter._input.jump)
		{
			//���������水�ո���ȥ
			if (parameter._hasAnimator)
			{
				
				
				parameter._animator.SetBool(parameter._animIDClimbingToWall, true);
				//parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
			}
			parameter._jumpTimeoutDelta = 0.5f;
			parameter._input.releaseClimb = false;
			//var nextPos = Mathf.Lerp(transform.position.y, WallTopPos_Y, Time.deltaTime / 10);
			parameter._controller.Move(manager.transform.up * Time.deltaTime * 2);
			//_verticalVelocity = Mathf.Sqrt((WallTopPos_Y - transform.position.y + 1f) * -2f * Gravity);

			if (parameter.WallTopPos_Y < manager.transform.position.y)
			{
				parameter.IsWallTop = parameter._input.climb = false;
				parameter._input.jump = false;//��������jumpû�ɹ���֪��Ϊʲô
				parameter.Gravity = -15;
				parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
				parameter._animator.SetBool(parameter._animIDClimbingToWall, false);
				parameter._animator.SetBool(parameter._animIDJump, false);
				manager.TransitionState(StateType.Idle);
			}
			
		}//��ǽ�����Ϸ����ո���ȥ
		//if (!parameter.HaveWall && !parameter._animator.GetBool(parameter._animIDClimbingToWall))
		//{
		//	if (parameter._hasAnimator)
		//	{
		//		parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
		//	}
		//	parameter.Gravity = -15;
		//	parameter._input.climb = false;
		//	parameter._input.releaseClimb = false;
		//}//�ظ��ڵ�������

	}
	
	private void ClimbHighWallEvent1()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 1.8f - manager.transform.position.y - 1f) * -2f * parameter.Gravity);
		Debug.Log("��ǽ1" + parameter._verticalVelocity);
	}
	private void ClimbHighWallEvent2()
	{
		parameter._verticalVelocity = Mathf.Sqrt(1f * -2f * parameter.Gravity);
		Debug.Log("��ǽ2" + parameter._verticalVelocity);
		manager.TransitionState(StateType.Idle);
	}
	private void ClimbMediumWallEvent()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 1.4f - manager.transform.position.y) * -2f * parameter.Gravity);
		Debug.Log("��ǽ" + parameter._verticalVelocity);
		manager.TransitionState(StateType.Idle);
	}
	private void ClimbLowWallEvent()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 0.9f - manager.transform.position.y) * -2f * parameter.Gravity);
		Debug.Log("��ǽ" + parameter._verticalVelocity);
		manager.TransitionState(StateType.Idle);
	}
    #endregion
    void OnAnimatorIK(int layerIndex)
    {

        //Climb IK
        if (manager != null && parameter._animator.GetBool(parameter._animIDClimbingIdleWall) && !parameter.IsWallTop)
        {
            Physics.Raycast(parameter.LeftHand.position - manager.transform.forward * parameter.ClimbGroundedOffset * 2, manager.transform.forward, out var _hitWallTop_L, parameter.WallDistance * 2f, parameter.Wall, QueryTriggerInteraction.Ignore);
            Physics.Raycast(parameter.RightHand.position - manager.transform.forward * parameter.ClimbGroundedOffset * 2, manager.transform.forward, out var _hitWallTop_R, parameter.WallDistance * 2f, parameter.Wall, QueryTriggerInteraction.Ignore);
            parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, _hitWallTop_R.point - manager.transform.forward * 0.05f);
            parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, _hitWallTop_L.point - manager.transform.forward * 0.05f);
            //Debug.Log("�Ƿ�����handIk�ɹ���" + (parameter.LeftHand.position == (_hitWallTop_R.point - manager.transform.forward * 0.1f)));

        }
    }
}
