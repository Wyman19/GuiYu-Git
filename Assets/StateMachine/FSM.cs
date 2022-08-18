using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum StateType
{
    Idle, Patrol, Chase, React, Attack, Hit, Death,Walk, Jump, Climb,StepUp, Roll, Crouch,Pick, Lift, Push
}

[Serializable]
public class Parameter
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 2.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 5.335f;
	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.50f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayer;

	[Header("Player Climb")]
	[Tooltip("攀爬爬墙高度")]
	public float ClimbWallH;
	[Tooltip("翻上高墙的高度")]
	public float ClimbHighWall = 2.4f;
	[Tooltip("翻上中墙的高度")]
	public float ClimbMediumWall = 1.4f;
	[Tooltip("翻上矮墙的高度")]
	public float ClimbLowWall = 0.4f;
	[Tooltip("手上射线与墙的距离")]
	public float ClimbGroundedOffset = 0.2f;
	[Tooltip("墙与玩家可攀爬的距离")]
	public float WallDistance = 0.8f;
	[Tooltip("玩家攀爬速度")]
	public float ClimbSpeed = 0.7f;
	[Tooltip("上墙初始化的点")]
	public Vector3 targetPos = Vector3.zero;
	[Tooltip("墙的最高点")]
	public float WallTopPos_Y;
	[Tooltip("射线击中了墙")]
	public bool HaveWall;
	public LayerMask Wall;

	[Tooltip("到达了墙的最上方")]
	public bool IsWallTop;
	public Vector3 pos = Vector3.zero;

	[Header("Player IK")]
	public Transform LeftFoot;
	public Transform RightFoot;
	public Transform LeftHand;
	public Transform RightHand;
	public Transform RightHandHoldPos;
	public Transform LeftHandHoldPos;
	public Transform TowHand_Right;
	public Transform TowHand_Left;

	[Header("Push")]
	public Transform Neck;
	public LayerMask PushLayers;
	public float PushDistance = 0.7f;
	public bool HavePushItem;

	[Header("PickUpItem")]
	public bool Range;
	public LayerMask WeightItem;


	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 70.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -30.0f;
	[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
	public float CameraAngleOverride = 0.0f;
	[Tooltip("For locking the camera position on all axis")]
	public bool LockCameraPosition = false;

	[Header("Bag")]
	public inventory _playerBag_Left;
	public inventory _playerBag_Right;
	public inventory _playerBag_Armor;
	public int PacketNum_L;
	public int PacketNum_R;
	public Transform PacketPos_Left;
	public Transform PacketPos_Right;
	public Transform Armor_HandPos;
	public Transform Armor_HeadPos;
	public Transform Armor_BodyPos;
	public Transform Armor_LegPos;
	public Transform Armor_FootPos;

	// cinemachine
	public float _cinemachineTargetYaw;
	public float _cinemachineTargetPitch;

	// player
	public float _speed;
	public float _animationBlend;
	public float _targetRotation = 0.0f;
	public float _rotationVelocity;
	public float _verticalVelocity;
	public float _terminalVelocity = 53.0f;

	//climb
	public float _climbAcceleratedSpeed_X;
	public float _climbAcceleratedSpeed_Y;
	public bool _leftHandWallToLand;
	public bool _rightHandWallToLand;
	public bool _leftBoxCheckWall;
	public bool _rightBoxCheckWall;
	public float _hitWallHigh;
	public float _hitStepHigh;
	public bool _moveWallTop;
	public bool _haveStep;
	public bool _boxContactStep;
	public bool _boxContactWall;
	public bool _leftFootRay;
	public bool _rightFootRay;

	//Push
	public RaycastHit _hitPushItem;
	public RaycastHit _hitRightHand;
	public RaycastHit _hitLeftHand;
	public bool _rightHandPush;
	public bool _leftHandPush;

	//PickUpItem
	public RaycastHit _hitItem;
	public RaycastHit _hitWeight;
	public RaycastHit _hitWeightItemRightHand;
	public RaycastHit _hitWeightItemLeftHand;
	public bool _holdItem_R;
	public bool _holdItem_L;
	public bool _holdWeightItem;
	public float _weightDis;


	//ComboAttack
	public bool _isComboTime;
	public bool _firstCombo;
	public bool _isTwoHand;

	//crouch
	public bool _iscrouch;

	//IK
	public Vector3 _controllerCenter = Vector3.zero;
	public float _footIkOffset;

	// timeout deltatime
	public float _jumpTimeoutDelta;
	public float _fallTimeoutDelta;

	// animation IDs
	public int _animLayerIDHand;
	public int _animatorIDIdle;
	public int _animIDSpeed;
	public int _animIDGrounded;
	public int _animIDJump;
	public int _animIDFreeFall;
	public int _animIDMotionSpeed;
	public int _animIDClimbingIdleWall;
	public int _animIDClimbingHorizontalWall;
	public int _animIDClimbingVerticalWall;
	public int _animIDClimbingToWall;
	public int _animIDClimbLowWall;
	public int _animIDClimbMediumWall;
	public int _animIDClimbHighWall;
	public int _animStepUp;
	public int _animRoll;
	public int _animCrouch;
	public int _animPush;
	public int _animPick_R;
	public int _animPick_L;
	public int _animLifting;
	public int _animLiftingHold;
	public int _animAttack;
	public int _animTwoHand;

	public PlayerInput _playerInput;
	public Animator _animator;
	public CharacterController _controller;
	public StarterAssetsInputs _input;
	public GameObject _mainCamera;
	public Stamina _stamina;
	public PlayerDamageRange _playerDamageRange_R;
	public PlayerDamageRange _playerDamageRange_L;
	public Medicine_HP _medicineHP_R;
	public Medicine_HP _medicineHP_L;
	public Lock _lock;
	public Hp _hp;
	public RaycastHit _hitWall;
	public RaycastHit _hitWallTop;
	public RaycastHit _boxHitWall;
	public RaycastHit _leftFootGround;
	public RaycastHit _rightFootGround;

	public const float _threshold = 0.01f;

	public bool _hasAnimator;

	public bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

	public void AssignAnimationIDs()
	{
		_animLayerIDHand = _animator.GetLayerIndex("Hand");
		_animatorIDIdle = Animator.StringToHash("Idle Walk Run Blend");
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
		_animIDClimbingIdleWall = Animator.StringToHash("ClimbingIdleWall");
		_animIDClimbingHorizontalWall = Animator.StringToHash("ClimbingHorizontalWall");
		_animIDClimbingVerticalWall = Animator.StringToHash("ClimbingVerticalWall");
		_animIDClimbingToWall = Animator.StringToHash("ClimbingToWall");
		_animIDClimbLowWall = Animator.StringToHash("ClimbLowWall");
		_animIDClimbMediumWall = Animator.StringToHash("ClimbMediumWall");
		_animIDClimbHighWall = Animator.StringToHash("ClimbHighWall");
		_animStepUp = Animator.StringToHash("StepUp");
		_animRoll = Animator.StringToHash("Roll");
		_animCrouch = Animator.StringToHash("Crouch");
		_animPush = Animator.StringToHash("Push");
		_animPick_R = Animator.StringToHash("Pick_R");
		_animPick_L = Animator.StringToHash("Pick_L");
		_animLifting = Animator.StringToHash("Lifting");
		_animLiftingHold = Animator.StringToHash("LiftingHold");
		_animAttack = Animator.StringToHash("Attack");
		_animTwoHand = Animator.StringToHash("TwoHand");
	}

}
public class FSM : MonoBehaviour
{

    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

	public const float _threshold = 0.01f;

	public Parameter parameter;

	public static FSM Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		// get a reference to our main camera
		if (parameter._mainCamera == null)
		{
			parameter._mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		parameter._firstCombo = true;
	}
	void Start()
    {
		//climbState = gameObject.AddComponent<ClimbState>();
		
		states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Death, new DeathState(this));
		states.Add(StateType.Walk, new WalkState(this));
		states.Add(StateType.Jump, new JumpState(this));
		states.Add(StateType.Roll, new RollState(this));
		states.Add(StateType.Crouch, new CrouchState(this));
		states.Add(StateType.StepUp, new StepUpState(this));
		states.Add(StateType.Climb,gameObject.AddComponent<ClimbState>());
		states.Add(StateType.Pick, gameObject.AddComponent<PickState>());
		states.Add(StateType.Lift, gameObject.AddComponent<LiftState>());
		states.Add(StateType.Push, gameObject.AddComponent<PushState>());
		states.Add(StateType.Attack, gameObject.AddComponent<AttackState>());

		TransitionState(StateType.Idle);

		parameter._playerBag_Left.PacketPos = parameter.PacketPos_Left;
		parameter._playerBag_Right.PacketPos = parameter.PacketPos_Right;
		parameter._playerBag_Left.HoldPos = parameter.LeftHandHoldPos;
		parameter._playerBag_Right.HoldPos = parameter.RightHandHoldPos;
		parameter._playerBag_Armor.FootPos = parameter.Armor_FootPos;
		parameter._playerBag_Armor.HandPos = parameter.Armor_HandPos;
		parameter._playerBag_Armor.HeadPos = parameter.Armor_HeadPos;
		parameter._playerBag_Armor.LegPos = parameter.Armor_LegPos;
		parameter._playerBag_Armor.BodyPos = parameter.Armor_BodyPos;
		parameter._hasAnimator = TryGetComponent<Animator>(out parameter._animator);
		parameter._controller = GetComponent<CharacterController>();
		parameter._input = GetComponent<StarterAssetsInputs>();
		parameter._playerInput = GetComponent<PlayerInput>();
		parameter._stamina = GetComponent<Stamina>();
		parameter._lock = GetComponent<Lock>();
		parameter._hp = GetComponent<Hp>();
		parameter._controllerCenter = parameter._controller.center;
		parameter.AssignAnimationIDs();

		// reset our timeouts on start
		parameter._jumpTimeoutDelta = parameter.JumpTimeout;
		parameter._fallTimeoutDelta = parameter.FallTimeout;
	}
	

	void Update()
    {
		FootIk();
		HandIk();
		Gravity();
		GroundedCheck();
		CameraRotation();
		WallCheck();
		currentState.OnUpdate();
		Move();
	}

    public void TransitionState(StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
		Debug.Log("进入：" + type +"State");
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
	void OnAnimatorIK(int layerIndex)
	{
		//_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
		//_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
		//_animator.SetIKPosition(AvatarIKGoal.LeftHand,pos);
		//根据动画的曲线设置IK的权重，可以让动作更加的流畅
		parameter._animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, parameter._animator.GetFloat("LeftHandWeight"));
		parameter._animator.SetIKPositionWeight(AvatarIKGoal.RightHand, parameter._animator.GetFloat("RightHandWeight"));
		parameter._animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, parameter._animator.GetFloat("LeftHandWeight"));
		parameter._animator.SetIKRotationWeight(AvatarIKGoal.RightHand, parameter._animator.GetFloat("RightHandWeight"));


		parameter._animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, parameter._animator.GetFloat("LeftFootWeight"));
		parameter._animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, parameter._animator.GetFloat("RightFootWeight"));
		parameter._animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, parameter._animator.GetFloat("LeftFootWeight"));
		parameter._animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, parameter._animator.GetFloat("RightFootWeight"));

		//FootKI
		if (parameter._leftFootRay && parameter._animator.GetFloat("LeftFootWeight") > 0.5f)
		{
			parameter.LeftFoot.rotation = Quaternion.FromToRotation(parameter.LeftFoot.up, parameter._leftFootGround.normal) * parameter.LeftFoot.rotation;
			//_animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(LeftFoot.up, _leftFootGround.normal)*LeftFoot.rotation);
			parameter._animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(parameter.LeftFoot.position.x, parameter._leftFootGround.point.y + 0.054f, parameter.LeftFoot.position.z));
		}


		if (parameter._rightFootRay && parameter._animator.GetFloat("RightFootWeight") > 0.5f)
		{
			parameter.RightFoot.rotation = Quaternion.FromToRotation(parameter.RightFoot.up, parameter._leftFootGround.normal) * parameter.RightFoot.rotation;
			//_animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(RightFoot.up, _rightFootGround.normal)*RightFoot.rotation);
			parameter._animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(parameter.RightFoot.position.x, parameter._rightFootGround.point.y + 0.054f, parameter.RightFoot.position.z));
		}


		////Push HandIK
		//if (parameter._rightHandPush)
		//{
		//	parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter._hitRightHand.point - transform.forward * 0.2f);
		//	//RightHand.rotation = Quaternion.FromToRotation(RightHand.forward, _hitRightHand.normal) * RightHand.rotation;
		//}
		//else if (!parameter._holdWeightItem && !parameter._animator.GetBool(parameter._animIDClimbingIdleWall)) parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter.RightHand.position);
		//if (parameter._leftHandPush)
		//{
		//	//LeftHand.rotation = Quaternion.FromToRotation(LeftHand.forward, _hitLeftHand.normal) * LeftHand.rotation;
		//	parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter._hitLeftHand.point - transform.forward * 0.2f);
		//}
		//else if (!parameter._holdWeightItem && !parameter._animator.GetBool(parameter._animIDClimbingIdleWall)) parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter.LeftHand.position);

		////Lifting
		//if (parameter._holdWeightItem && parameter._hitWeightItemRightHand.point != Vector3.zero && parameter._hitWeightItemLeftHand.point != Vector3.zero)
		//{
		//	parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, new Vector3(parameter.RightHand.position.x, parameter._hitWeightItemRightHand.point.y, parameter.RightHand.position.z));
		//	parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, new Vector3(parameter.LeftHand.position.x, parameter._hitWeightItemLeftHand.point.y, parameter.LeftHand.position.z));
		//}
		//else if (!parameter._holdWeightItem && !parameter._animator.GetBool(parameter._animIDClimbingIdleWall))
		//{
		//	parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter.RightHand.position);
		//	parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter.LeftHand.position);
		//}

		//if (parameter._isTwoHand && parameter._firstCombo)
		//{
		//	parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter.TowHand_Right.position);
		//	parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter.TowHand_Left.position);
		//	parameter._animator.SetIKRotation(AvatarIKGoal.RightHand, parameter.TowHand_Right.rotation);
		//	parameter._animator.SetIKRotation(AvatarIKGoal.LeftHand, parameter.TowHand_Left.rotation);
		//}
		//else if (!parameter._holdWeightItem && !parameter._animator.GetBool(parameter._animIDClimbingIdleWall))
		//{
		//	parameter._animator.SetIKPosition(AvatarIKGoal.RightHand, parameter.RightHand.position);
		//	parameter._animator.SetIKPosition(AvatarIKGoal.LeftHand, parameter.LeftHand.position);
		//	parameter._animator.SetIKRotation(AvatarIKGoal.RightHand, parameter.RightHand.rotation);
		//	//_animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);
		//}

	}
	private void FootIk()
	{
		float FootIkOffset()
		{
			if (Mathf.Abs(parameter._leftFootGround.point.y - parameter._rightFootGround.point.y) > 0.02f)
			{
				if (Mathf.Abs(parameter._leftFootGround.point.y - parameter._rightFootGround.point.y) > parameter._footIkOffset) parameter._footIkOffset += Time.deltaTime / 2f;
				else if (parameter._footIkOffset - Mathf.Abs(parameter._leftFootGround.point.y - parameter._rightFootGround.point.y) > 0.05f) parameter._footIkOffset -= Time.deltaTime * 2;
			}
			else if (parameter._footIkOffset > 0) parameter._footIkOffset -= Time.deltaTime * 2;
			if (parameter._footIkOffset > 0.4f && !parameter._iscrouch) parameter._footIkOffset = 0.4f;
			if (parameter._footIkOffset > 0.26f && parameter._iscrouch) parameter._footIkOffset = 0.26f;
			return parameter._footIkOffset * 2f;
		}
		if (parameter._iscrouch) { parameter._controller.height = 1.5f - FootIkOffset(); parameter._controller.center = new Vector3(0, 0.78f, 0); }
		else { parameter._controller.height = 1.8f - FootIkOffset(); parameter._controller.center = new Vector3(0, 0.93f, 0); }
		//        Vector3 tempPos = _controller.center;
		//        Debug.Log(_controller.center);
		//        //if (Mathf.Abs(_leftFootGround.point.y - _rightFootGround.point.y) > 0.02f)
		//        //{
		//            if (_leftFootGround.point.y > _rightFootGround.point.y) _controller.center = new Vector3(tempPos.x, _leftFootGround.point.y  + 0.9f, tempPos.z);
		//else if (_leftFootGround.point.y < _rightFootGround.point.y) { _controller.center = new Vector3(tempPos.x, _rightFootGround.point.y + 0.9f, tempPos.z); }
		//        //}
		//        else _controller.center = new Vector3(tempPos.x, 0.9f, tempPos.z);
	}
	private void HandIk()
	{
		//Debug.Log(_rightHandPush);
		if (parameter.HavePushItem)
		{
			parameter._rightHandPush = Physics.Raycast(parameter.Neck.position - transform.forward * parameter.ClimbGroundedOffset + transform.right * 0.28f, transform.forward, out parameter._hitRightHand, parameter.WallDistance, parameter.PushLayers, QueryTriggerInteraction.Ignore);
			parameter._leftHandPush = Physics.Raycast(parameter.Neck.position - transform.forward * parameter.ClimbGroundedOffset - transform.right * 0.28f, transform.forward, out parameter._hitLeftHand, parameter.WallDistance, parameter.PushLayers, QueryTriggerInteraction.Ignore);
		}
		else
		{
			parameter._rightHandPush = parameter._leftHandPush = false;

		}
		if (parameter._holdWeightItem)
		{
			parameter._rightHandPush = Physics.Raycast(parameter.RightHand.position - transform.up * 0.5f, transform.up, out parameter._hitWeightItemRightHand, 1f, parameter.WeightItem, QueryTriggerInteraction.Ignore);
			parameter._leftHandPush = Physics.Raycast(parameter.LeftHand.position - transform.up * 0.5f, transform.up, out parameter._hitWeightItemLeftHand, 1f, parameter.WeightItem, QueryTriggerInteraction.Ignore);
		}
	}
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - parameter.GroundedOffset, transform.position.z);
		//检测有没有与地面碰撞（点，半径，Layer，指定该查询是否应该命中触发器）
		parameter.Grounded = Physics.CheckSphere(spherePosition, parameter.GroundedRadius, parameter.GroundLayer, QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (parameter._hasAnimator)
		{
			parameter._animator.SetBool(parameter._animIDGrounded, parameter.Grounded);
		}

		//脚部IK射线检测
		parameter._leftFootRay = Physics.Raycast(parameter.LeftFoot.position + transform.up * 0.5f, -transform.up, out parameter._leftFootGround, 1.3f, parameter.GroundLayer, QueryTriggerInteraction.Ignore);
		parameter._rightFootRay = Physics.Raycast(parameter.RightFoot.position + transform.up * 0.5f, -transform.up, out parameter._rightFootGround, 1.3f, parameter.GroundLayer, QueryTriggerInteraction.Ignore);
		//Debug.Log(LeftFoot.position.y);//IK离地0.05
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
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, parameter._targetRotation, ref parameter._rotationVelocity, parameter.RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
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
	private void Gravity()
	{
		if (parameter.Grounded)
		{
			parameter._fallTimeoutDelta = parameter.FallTimeout;
			// update animator if using character
			if (parameter._hasAnimator && currentState != states[StateType.Jump])
			{

				//parameter._animator.SetBool(parameter._animIDJump, false);
				parameter._animator.SetBool(parameter._animIDFreeFall, false);
				//parameter._animator.SetBool(parameter._animIDClimbingToWall, false);
				//parameter._animator.SetBool(parameter._animIDClimbingIdleWall, false);
				parameter._animator.SetBool(parameter._animIDClimbLowWall, false);
				parameter._animator.SetBool(parameter._animIDClimbMediumWall, false);
				parameter._animator.SetBool(parameter._animIDClimbHighWall, false);
				//parameter._animator.SetBool(parameter._animStepUp, false);
				parameter.IsWallTop = false;

			}
			// stop our velocity dropping infinitely when grounded
			if (parameter._verticalVelocity < 0.0f)
			{
				parameter._verticalVelocity = -2f;
			}


			// jump timeout
			if (parameter._jumpTimeoutDelta >= 0.0f)
			{
				parameter._input.jump = false;
				parameter._jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else if (currentState == states[StateType.Climb]) 
		{ 
			//parameter._input.climb = false; 
		}
		else
		{
			// reset the jump timeout timer
			parameter._jumpTimeoutDelta = parameter.JumpTimeout;

			// fall timeout
			if (parameter._fallTimeoutDelta >= 0.0f)
			{
				parameter._fallTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				// update animator if using character
				if (parameter._hasAnimator)
				{
					parameter._animator.SetBool(parameter._animIDFreeFall, true);
					//parameter._animator.SetBool(parameter._animIDJump, false);
				}
			}

			// if we are not grounded, do not jump
			parameter._input.jump = false;
			
		}
		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (parameter._verticalVelocity < parameter._terminalVelocity)
		{

			//Debug.Log(parameter._verticalVelocity);
			//a=Gravity * Time.deltaTime
			parameter._verticalVelocity += parameter.Gravity * Time.deltaTime;
		}
	}
	private void WallCheck()
	{
		Vector3 LowRayPosition = new Vector3(transform.position.x, transform.position.y + parameter.ClimbGroundedOffset, transform.position.z);
		parameter.HaveWall = Physics.Raycast(LowRayPosition, Quaternion.AngleAxis(60, -transform.right) * transform.forward, out parameter._hitWall, parameter.WallDistance, parameter.Wall, QueryTriggerInteraction.Ignore);
		//角色需要移动到的点
		parameter.targetPos = parameter._hitWall.point + Quaternion.AngleAxis(60, -transform.right) * parameter._hitWall.normal * 0.6f;

		//从hit处开始向上检测,检测墙的高度
		//if (!HaveWall) _hitWallHigh = 0;
		//if (HaveWall) _boxContactWall = Physics.CheckBox(_hitWall.point + transform.up*_hitWallHigh, new Vector3(0.25f, 0.9f, 0.25f), Quaternion.identity,Wall, QueryTriggerInteraction.Ignore);
		//if (_boxContactWall) _hitWallHigh += Time.deltaTime * 7;
		//Debug.Log("墙高" + (_hitWall.point.y + _hitWallHigh - 0.9f));
		//if(!HaveWall) _hitWallHigh = 0;
		if (parameter.HaveWall)
		{
			var hitWall = false;
			for (float i = 0; i < parameter.ClimbHighWall; i += 0.05f)
			{
				//射线检测墙体
				Debug.DrawRay(parameter._hitWall.point + transform.up * i - transform.forward * 0.5f, transform.forward);
				hitWall = Physics.Raycast(parameter._hitWall.point + transform.up * i - transform.forward * 0.5f, transform.forward, out var hitPos, parameter.WallDistance * 2f, parameter.Wall, QueryTriggerInteraction.Ignore);
				if (!hitWall)
				{
					//如果没有击中墙体生成盒子检测可否翻越
					parameter._boxContactWall = Physics.CheckBox(hitPos.point + transform.up * (i + 0.9f), new Vector3(0.25f, 0.9f, 0.25f), Quaternion.identity, parameter.Wall, QueryTriggerInteraction.Ignore);
					if (!parameter._boxContactWall)
					{

						//Debug.Log("获取了墙的高度" + parameter._hitWallHigh);
						//如果盒子没有触碰到墙就获取高度
						parameter._hitWallHigh = i + 0.9f;
						break;
					}

				}
			}
			if (parameter._boxContactWall || hitWall)
			{
				//超过最高攀爬高度
				parameter._hitWallHigh = parameter.ClimbHighWall + 0.1f + 0.9f;
			}
		}


		//从Character Controller不能上的台阶高度开始检测
		if (!parameter.HaveWall) { parameter._haveStep = Physics.Raycast(transform.position + transform.up * 0.28f, transform.forward, 0.3f, parameter.Wall, QueryTriggerInteraction.Ignore); }
		else { parameter._haveStep = false; }
		if (parameter._haveStep) { parameter._boxContactStep = Physics.CheckBox(transform.position + transform.up * (0.28f + parameter._hitStepHigh + 0.9f) + transform.forward * 0.5f, new Vector3(0.25f, 0.9f, 0.25f), Quaternion.identity, parameter.Wall, QueryTriggerInteraction.Ignore); }
		else { parameter._hitStepHigh = 0; }
		if (parameter._boxContactStep) parameter._hitStepHigh += Time.deltaTime * 7;//Debug.Log(_haveStep + "" + _boxContactStep + ""+_hitStepHigh);

		//判断是否到最高处

		parameter._leftHandWallToLand = !Physics.Raycast(parameter.LeftHand.position - transform.forward * parameter.ClimbGroundedOffset * 2, transform.forward, out var _hitWallTop_L, parameter.WallDistance * 2f, parameter.Wall, QueryTriggerInteraction.Ignore);
		parameter._rightHandWallToLand = !Physics.Raycast(parameter.RightHand.position - transform.forward * parameter.ClimbGroundedOffset * 2, transform.forward, out var _hitWallTop_R, parameter.WallDistance * 2f, parameter.Wall, QueryTriggerInteraction.Ignore);
		if ((parameter._leftHandWallToLand || parameter._rightHandWallToLand) && parameter._animator.GetBool(parameter._animIDClimbingIdleWall))
		{
			if (_hitWallTop_L.point.y > _hitWallTop_R.point.y) parameter._hitWallTop = _hitWallTop_L;
			else parameter._hitWallTop = _hitWallTop_R;
			//Debug.Log(_hitWallTop.point.y);
		}

		//判断左右两边是否可以攀爬
		parameter._leftBoxCheckWall = Physics.CheckBox(parameter._hitWall.point + -transform.right * parameter.WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.25f, 0.8f, 0.25f), Quaternion.identity, parameter.Wall, QueryTriggerInteraction.Ignore);
		parameter._rightBoxCheckWall = Physics.CheckBox(parameter._hitWall.point + transform.right * parameter.WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.25f, 0.8f, 0.25f), Quaternion.identity, parameter.Wall, QueryTriggerInteraction.Ignore);
		//Debug.Log("_leftBoxCheckWall:" + _leftBoxCheckWall + "\n" + "_rightBoxCheckWall:" + _rightBoxCheckWall);
	}
	#region LookItem
	//放射线检测
	public bool Look(string ItemTag)
	{

		var pos = transform.position - transform.up * 0.05f;
		//一条向前的射线
		if (LookAround(pos, Quaternion.identity, Color.green, ItemTag))
			return true;

		//多一个精确度就多两条对称的射线,每条射线夹角是总角度除与精度
		float subAngle = 5;
		for (int i = 0; i < 10; i++)
		{
			for (float j = 0; j < 0.3f; j += 0.04f)
			{
				if (LookAround(pos + transform.up * j, Quaternion.Euler(0, -1 * subAngle * (i + 1), 0), Color.green, ItemTag)
				|| LookAround(pos + transform.up * j, Quaternion.Euler(0, subAngle * (i + 1), 0), Color.green, ItemTag))
					return true;
			}
		}

		return false;
	}

	//射出射线检测是否有Player
	bool LookAround(Vector3 pos, Quaternion eulerAnger, Color DebugColor, string ItemTag)
	{
#if UNITY_EDITOR
		if (parameter.Range) Debug.DrawRay(pos, eulerAnger * transform.forward, DebugColor);
#endif

		if (Physics.Raycast(pos, eulerAnger * transform.forward, out var _hit, 1) && _hit.collider.CompareTag(ItemTag))
		{
			//controller.chaseTarget = hit.transform;
			if (ItemTag == "Weight") parameter._hitWeight = _hit;
			if (ItemTag == "Item") parameter._hitItem = _hit;
			return true;
		}
		return false;
	}
	#endregion
	public void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
	public GameObject InstantiateGamgeObject(GameObject gameObject,Transform transform)
    {
		return Instantiate(gameObject,transform);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
		var LowRayPosition = new Vector3(transform.position.x, transform.position.y + parameter.ClimbGroundedOffset, transform.position.z);
		//跳跃检测
		if (parameter.Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - parameter.GroundedOffset, transform.position.z), parameter.GroundedRadius);

		//判断是否在墙上
		if (parameter.HaveWall) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + parameter.ClimbGroundedOffset, transform.position.z), (Quaternion.AngleAxis(60, -transform.right) * transform.forward) * parameter.WallDistance);
		if (parameter.HaveWall) Gizmos.DrawSphere(parameter.targetPos, 0.1f);

		Gizmos.DrawSphere(parameter.targetPos, 0.1f);

		//判断是否爬到最上面
		if (parameter._leftHandWallToLand) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		//Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.53f, transform.position.z), transform.forward*WallDistance/2);
		Gizmos.DrawRay(parameter.LeftHand.position - transform.forward * parameter.ClimbGroundedOffset * 2, transform.forward * parameter.WallDistance * 1.5f);
		if (parameter._rightHandWallToLand) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawRay(parameter.RightHand.position - transform.forward * parameter.ClimbGroundedOffset * 2, transform.forward * parameter.WallDistance * 1.5f);

		//长方形体检测墙壁是否可以翻越
		if (parameter.HaveWall)
		{
			Gizmos.DrawCube(parameter._hitWall.point + transform.up * parameter._hitWallHigh, new Vector3(0.5f, 1.8f, 0.5f));
			//if(_hitWallOffset>)
		}

		//从Character Controller不能上的台阶高度开始检测
		if (parameter._haveStep) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawRay(transform.position + transform.up * 0.28f, transform.forward * 0.295f);
		if (parameter._haveStep) Gizmos.DrawCube(transform.position + transform.up * (0.28f + parameter._hitStepHigh + 0.9f) + transform.forward * 0.5f, new Vector3(0.5f, 1.8f, 0.5f));


		//攀爬时长方形检测左右两边是否可以攀爬
		if (parameter.HaveWall)
		{
			if (parameter._rightBoxCheckWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawCube(parameter._hitWall.point + transform.right * parameter.WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.5f, 1.6f, 0.5f));
			if (parameter._leftBoxCheckWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawCube(parameter._hitWall.point + -transform.right * parameter.WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.5f, 1.6f, 0.5f));
		}

		//检测墙的高度
		if (parameter.HaveWall)
		{
			if (parameter._hitWallHigh - 0.9f > parameter.ClimbLowWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawSphere(parameter._hitWall.point + transform.up * parameter.ClimbLowWall, 0.1f);
			if (parameter._hitWallHigh - 0.9f > parameter.ClimbMediumWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawSphere(parameter._hitWall.point + transform.up * parameter.ClimbMediumWall, 0.1f);
			if (parameter._hitWallHigh - 0.9f > parameter.ClimbHighWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawSphere(parameter._hitWall.point + transform.up * parameter.ClimbHighWall, 0.1f);
		}
		//for(float i = 0; i < ClimbHighWall; i += 0.05f)
		//         {
		//	Gizmos.DrawRay(_hitWall.point + transform.up*i,transform.forward);
		//         }



		//脚部Ik检测
		if (parameter._leftFootRay) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawRay(parameter.LeftFoot.position + transform.up * 0.2f, -transform.up * 0.7f);
		Gizmos.DrawSphere(parameter._leftFootGround.point, 0.1f);
		if (parameter._rightFootRay) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;
		Gizmos.DrawRay(parameter.RightFoot.position + transform.up * 0.2f, -transform.up * 0.7f);
		Gizmos.DrawSphere(parameter._rightFootGround.point, 0.1f);


	}
#endif
}