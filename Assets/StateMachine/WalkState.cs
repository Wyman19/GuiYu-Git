using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : IState
{
	private FSM manager;
	private Parameter parameter;

	public const float _threshold = 0.01f;

	public WalkState(FSM manager)
	{
		this.manager = manager;
		this.parameter = manager.parameter;
	}
	public void OnEnter()
	{
		//parameter._animator.Play("Idle Walk Run Blend");


	}

	public void OnUpdate()
	{
		CameraRotation();
		Move();
		if (parameter._input.jump)
        {
			manager.TransitionState(StateType.Jump);
		}
	}

	public void OnExit()
	{

	}
	private void CameraRotation()
	{
        // if there is an input and camera position is not fixed
        if (parameter._input.look.sqrMagnitude >= _threshold && !parameter.LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = parameter.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            parameter._cinemachineTargetYaw += parameter._input.look.x * deltaTimeMultiplier;
            parameter._cinemachineTargetPitch += parameter._input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        parameter._cinemachineTargetYaw = ClampAngle(parameter._cinemachineTargetYaw, float.MinValue, float.MaxValue);
        parameter._cinemachineTargetPitch = ClampAngle(parameter._cinemachineTargetPitch, parameter.BottomClamp, parameter.TopClamp);

        // Cinemachine will follow this target
        parameter.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(parameter._cinemachineTargetPitch + parameter.CameraAngleOverride, parameter._cinemachineTargetYaw, 0.0f);

    }
	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = parameter._input.sprint ? parameter.SprintSpeed : parameter.MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (parameter._input.move == Vector2.zero || parameter._input.climb) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		//float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = parameter._input.analogMovement ? parameter._input.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (parameter._speed < targetSpeed - speedOffset || parameter._speed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			parameter._speed = Mathf.Lerp(parameter._speed, targetSpeed * inputMagnitude, Time.deltaTime * parameter.SpeedChangeRate);

			// round speed to 3 decimal places
			parameter._speed = Mathf.Round(parameter._speed * 1000f) / 1000f;
		}
		else
		{
			parameter._speed = targetSpeed;
		}
		parameter._animationBlend = Mathf.Lerp(parameter._animationBlend, targetSpeed, Time.deltaTime * parameter.SpeedChangeRate);

		// normalise input direction
		Vector3 inputDirection = new Vector3(parameter._input.move.x, 0.0f, parameter._input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		//在没攀爬的状态下才可以旋转
		// if there is a move input rotate player when the player is moving
		if (parameter._input.move != Vector2.zero && !parameter._input.climb)
		{
			if (parameter._mainCamera == null)
			{
				parameter._mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			parameter._targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + parameter._mainCamera.transform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(manager.transform.eulerAngles.y, parameter._targetRotation, ref parameter._rotationVelocity, parameter.RotationSmoothTime);

			// rotate to face input direction relative to camera position
			manager.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}


		Vector3 targetDirection = Quaternion.Euler(0.0f, parameter._targetRotation, 0.0f) * Vector3.forward;

		// move the player
		parameter._controller.Move(targetDirection.normalized * (parameter._speed * Time.deltaTime) + new Vector3(0.0f, parameter._verticalVelocity, 0.0f) * Time.deltaTime);

		// update animator if using character
		if (parameter._hasAnimator)
		{
			if (!parameter._input.pickUpItem_R && !parameter._input.pickUpItem_L && !parameter._input.liftingItem) parameter._animator.SetFloat(parameter._animIDSpeed, parameter._animationBlend);
			parameter._animator.SetFloat(parameter._animIDMotionSpeed, inputMagnitude);
		}
	}
	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}
