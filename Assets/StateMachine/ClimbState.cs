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
		
		//攀爬加速度
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
				
				//攀爬墙壁
				if (parameter._hitWallHigh - 0.9f > parameter.ClimbHighWall || play_ing_flag)
				{
					//玩家旋转，前面与法向量对齐
					manager.transform.forward = -parameter._hitWall.normal;
					//消除重力，粘着墙
					parameter.Gravity = 0;
					parameter._verticalVelocity = 0;
					//if (Vector3.Distance(manager.transform.position, parameter.targetPos) > 0.03f)
     //               {
     //                   Debug.Log("上墙");
     //                   manager.transform.position = parameter.targetPos;
     //               }
                    //Vector3 lerpTargetPos = Vector3.MoveTowards(manager.transform.position, parameter.targetPos, 0.1f);
                    //manager.transform.position = lerpTargetPos;
                    //墙上移动
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
						//动画触发ClimbHighWallEvent给速度				
					}
					if (parameter.ClimbMediumWall > parameter._hitWallHigh - 0.9f && parameter._hitWallHigh - 0.9f > parameter.ClimbLowWall)
					{
						if (parameter._hasAnimator)
						{
							parameter._animator.SetBool(parameter._animIDClimbMediumWall, true);
						}
						//动画触发ClimbMediumWallEvent给速度
					}
					if (parameter._hitWallHigh - 0.9f < parameter.ClimbLowWall)
					{
						if (parameter._hasAnimator)
						{
							parameter._animator.SetBool(parameter._animIDClimbLowWall, true);
						}
						//动画触发ClimbLowWallEvent给速度
						//F*S=1/2*m*v平方 v=Sqrt(2FS)
					}
					parameter._input.jump = false;

				}//翻上墙壁

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
			}//放弃攀爬
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
				//Debug.Log("缩进距离");
				if (manager.transform.position.y < parameter.WallTopPos_Y - 1.5f) parameter._controller.Move(manager.transform.up * Time.deltaTime / 7);
				if (manager.transform.position.y > parameter.WallTopPos_Y - 1.5f) parameter._controller.Move(-manager.transform.up * Time.deltaTime / 7);
			}

		}//检测是否爬到墙的最上方
		if (parameter.IsWallTop && parameter._input.jump)
		{
			//爬到最上面按空格上去
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
				parameter._input.jump = false;//这里设置jump没成功不知道为什么
				parameter.Gravity = -15;
				parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
				parameter._animator.SetBool(parameter._animIDClimbingToWall, false);
				parameter._animator.SetBool(parameter._animIDJump, false);
				manager.TransitionState(StateType.Idle);
			}
			
		}//到墙的最上方按空格翻上去
		//if (!parameter.HaveWall && !parameter._animator.GetBool(parameter._animIDClimbingToWall))
		//{
		//	if (parameter._hasAnimator)
		//	{
		//		parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
		//	}
		//	parameter.Gravity = -15;
		//	parameter._input.climb = false;
		//	parameter._input.releaseClimb = false;
		//}//回复在地面的情况

	}
	
	private void ClimbHighWallEvent1()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 1.8f - manager.transform.position.y - 1f) * -2f * parameter.Gravity);
		Debug.Log("高墙1" + parameter._verticalVelocity);
	}
	private void ClimbHighWallEvent2()
	{
		parameter._verticalVelocity = Mathf.Sqrt(1f * -2f * parameter.Gravity);
		Debug.Log("高墙2" + parameter._verticalVelocity);
		manager.TransitionState(StateType.Idle);
	}
	private void ClimbMediumWallEvent()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 1.4f - manager.transform.position.y) * -2f * parameter.Gravity);
		Debug.Log("中墙" + parameter._verticalVelocity);
		manager.TransitionState(StateType.Idle);
	}
	private void ClimbLowWallEvent()
	{
		parameter._verticalVelocity = Mathf.Sqrt((parameter._hitWall.point.y + parameter._hitWallHigh - 0.9f - manager.transform.position.y) * -2f * parameter.Gravity);
		Debug.Log("矮墙" + parameter._verticalVelocity);
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
            //Debug.Log("是否设置handIk成功：" + (parameter.LeftHand.position == (_hitWallTop_R.point - manager.transform.forward * 0.1f)));

        }
    }
}
