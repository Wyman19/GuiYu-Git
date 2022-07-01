using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
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
		public float ClimbHighWall=2.4f;
		[Tooltip("翻上中墙的高度")]
		public float ClimbMediumWall=1.4f;
		[Tooltip("翻上矮墙的高度")]
		public float ClimbLowWall=0.4f;
		[Tooltip("手上射线与墙的距离")]
		public float ClimbGroundedOffset = 0.2f;
		[Tooltip("墙与玩家可攀爬的距离")]
		public float WallDistance = 0.8f;
		[Tooltip("玩家攀爬速度")]
		public float ClimbSpeed = 0.7f;		
		[Tooltip("上墙初始化的点")]
		Vector3 targetPos= Vector3.zero;
		[Tooltip("墙的最高点")]
		public float WallTopPos_Y;
		[Tooltip("射线击中了墙")]
		public bool HaveWall;
		public LayerMask Wall;

		[Tooltip("到达了墙的最上方")]
		public bool IsWallTop;
		public Vector3 pos= Vector3.zero;
		
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
		public float PushDistance=0.7f;
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
		public  inventory _playerBag_Left;
		public  inventory _playerBag_Right;
		public int PacketNum_L;
		public int PacketNum_R;
		public Transform PacketPos_Left;
		public Transform PacketPos_Right;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		//climb
		private float _climbAcceleratedSpeed_X;
		private float _climbAcceleratedSpeed_Y;
		private bool _leftHandWallToLand;
		private bool _rightHandWallToLand;
		private bool _leftBoxCheckWall;
		private bool _rightBoxCheckWall;
		private float _hitWallHigh;
		private float _hitStepHigh;
		private bool _moveWallTop;
		private bool _haveStep;
		private bool _boxContactStep;
		private bool _boxContactWall;
		private bool _leftFootRay;
		private bool _rightFootRay;

		//Push
		private RaycastHit _hitPushItem;
		private RaycastHit _hitRightHand;
		private RaycastHit _hitLeftHand;
		private bool _rightHandPush;
		private bool _leftHandPush;

		//PickUpItem
		RaycastHit _hitItem;
		RaycastHit _hitWeight;
		private RaycastHit _hitWeightItemRightHand;
		private RaycastHit _hitWeightItemLeftHand;
		private bool _holdItem_R;
		private bool _holdItem_L;
		private bool _holdWeightItem;
		private float _weightDis;


		//ComboAttack
		private bool _isComboTime;
		private bool _firstCombo;
		private bool _isTwoHand;

		//crouch
		private bool _iscrouch;

		//IK
		private Vector3 _controllerCenter=Vector3.zero;
		private float _footIkOffset;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// animation IDs
		private int _animLayerIDHand;
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDFreeFall;
		private int _animIDMotionSpeed;
		private int _animIDClimbingIdleWall;
		private int _animIDClimbingHorizontalWall;
		private int _animIDClimbingVerticalWall;
		private int _animIDClimbingToWall;
		private int _animIDClimbLowWall;
		private int _animIDClimbMediumWall;
		private int _animIDClimbHighWall;
		private int _animStepUp;
		private int _animRoll;
		private int _animCrouch;
		private int _animPush;
		private int _animPick_R;
		private int _animPick_L;
		private int _animLifting;
		private int _animLiftingHold;
		private int _animAttack;
		private int _animTwoHand;

		private PlayerInput _playerInput;
		private Animator _animator;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private Stamina _stamina;
		private PlayerDamageRange _playerDamageRange_R;
		private PlayerDamageRange _playerDamageRange_L;
		private Medicine_HP _medicineHP_R;
		private Medicine_HP _medicineHP_L;
		private Lock _lock;
		private Hp _hp;
		private RaycastHit _hitWall;
		private RaycastHit _hitWallTop;
		private RaycastHit _boxHitWall;
		private RaycastHit _leftFootGround;
		private RaycastHit _rightFootGround;

		private const float _threshold = 0.01f;

		private bool _hasAnimator;

		private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
			_firstCombo = true;
		}

		private void Start()
		{
			_playerBag_Left.PacketPos = PacketPos_Left;
			_playerBag_Right.PacketPos = PacketPos_Right;
			_hasAnimator = TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_playerInput = GetComponent<PlayerInput>();
			_stamina = GetComponent<Stamina>();
			_lock = GetComponent<Lock>();
			_hp=GetComponent<Hp>();
			_controllerCenter = _controller.center;
			AssignAnimationIDs();

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			_hasAnimator = TryGetComponent(out _animator);

            if (_speed <= 2 && Grounded && !_holdWeightItem && _firstCombo)
            {
                _stamina.isReplyToMaxStamina = true;
                _stamina.BeginReduceStamina = true;
            }
			TowHandIsNull();
			ItemToPacket();
			UserMedicine();
			CheckAttackCombo();
			Lifting();
			PickUpItem();
			HandIk();
			Push();
			Crouch();
			Roll();
			FootIk();
			ClimbAcceleratedSpeed();
			JumpAndGravity();
			GroundedCheck();
			WallCheck();
			Climb();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void AssignAnimationIDs()
		{
			_animLayerIDHand = _animator.GetLayerIndex("Hand");
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
		void OnAnimatorIK(int layerIndex)
		{
            //_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            //_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            //_animator.SetIKPosition(AvatarIKGoal.LeftHand,pos);
			//根据动画的曲线设置IK的权重，可以让动作更加的流畅
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _animator.GetFloat("LeftHandWeight"));
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _animator.GetFloat("RightHandWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _animator.GetFloat("LeftHandWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _animator.GetFloat("RightHandWeight"));


            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("LeftFootWeight"));
			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("RightFootWeight"));
			_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("LeftFootWeight"));
			_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("RightFootWeight"));

			//FootKI
			if (_leftFootRay && _animator.GetFloat("LeftFootWeight") > 0.5f)
            {
				LeftFoot.rotation = Quaternion.FromToRotation(LeftFoot.up, _leftFootGround.normal) * LeftFoot.rotation;
				//_animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(LeftFoot.up, _leftFootGround.normal)*LeftFoot.rotation);
				_animator.SetIKPosition(AvatarIKGoal.LeftFoot, new Vector3(LeftFoot.position.x, _leftFootGround.point.y + 0.054f, LeftFoot.position.z));
			}


			if (_rightFootRay && _animator.GetFloat("RightFootWeight") > 0.5f)
            {
				RightFoot.rotation = Quaternion.FromToRotation(RightFoot.up, _leftFootGround.normal) * RightFoot.rotation;
				//_animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(RightFoot.up, _rightFootGround.normal)*RightFoot.rotation);
				_animator.SetIKPosition(AvatarIKGoal.RightFoot, new Vector3(RightFoot.position.x, _rightFootGround.point.y + 0.054f, RightFoot.position.z));
			}

			//Climb IK
			if (_animator.GetBool(_animIDClimbingIdleWall) && !IsWallTop  )
            {
				Physics.Raycast(LeftHand.position - transform.forward * ClimbGroundedOffset * 2, transform.forward, out var _hitWallTop_L, WallDistance * 2f, Wall, QueryTriggerInteraction.Ignore);
				Physics.Raycast(RightHand.position - transform.forward * ClimbGroundedOffset * 2, transform.forward, out var _hitWallTop_R, WallDistance * 2f, Wall, QueryTriggerInteraction.Ignore);
				_animator.SetIKPosition(AvatarIKGoal.RightHand, _hitWallTop_R.point - transform.forward* 0.1f);
				_animator.SetIKPosition(AvatarIKGoal.LeftHand, _hitWallTop_L.point - transform.forward * 0.1f);

			}

			//Push HandIK
			if (_rightHandPush)
            {
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _hitRightHand.point - transform.forward * 0.2f);
                //RightHand.rotation = Quaternion.FromToRotation(RightHand.forward, _hitRightHand.normal) * RightHand.rotation;
            }
            else if (!_holdWeightItem && !_animator.GetBool(_animIDClimbingIdleWall)) _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);
            if (_leftHandPush)
            {
                //LeftHand.rotation = Quaternion.FromToRotation(LeftHand.forward, _hitLeftHand.normal) * LeftHand.rotation;
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _hitLeftHand.point - transform.forward * 0.2f);
            }
            else if (!_holdWeightItem && !_animator.GetBool(_animIDClimbingIdleWall)) _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);

            //Lifting
            if (_holdWeightItem && _hitWeightItemRightHand.point != Vector3.zero && _hitWeightItemLeftHand.point != Vector3.zero)
            {
                _animator.SetIKPosition(AvatarIKGoal.RightHand,new Vector3(RightHand.position.x,_hitWeightItemRightHand.point.y, RightHand.position.z)  );
				_animator.SetIKPosition(AvatarIKGoal.LeftHand, new Vector3(LeftHand.position.x, _hitWeightItemLeftHand.point.y,LeftHand.position.z) );
			}
            else if(!_holdWeightItem&&!_animator.GetBool(_animIDClimbingIdleWall))
            {
				_animator.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);
				_animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
			}

            if (_isTwoHand && _firstCombo)
            {
                _animator.SetIKPosition(AvatarIKGoal.RightHand, TowHand_Right.position);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, TowHand_Left.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, TowHand_Right.rotation);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, TowHand_Left.rotation);
            }
            else if(!_holdWeightItem &&!_animator.GetBool(_animIDClimbingIdleWall))
            {
                _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHand.position);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHand.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHand.rotation);
                //_animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHand.rotation);
            }

        }
		private void FootIk()
        {
            float FootIkOffset()
			{
				if (Mathf.Abs(_leftFootGround.point.y - _rightFootGround.point.y) > 0.02f)
				{
					if (Mathf.Abs(_leftFootGround.point.y - _rightFootGround.point.y) > _footIkOffset) _footIkOffset += Time.deltaTime / 2f;
					else if(_footIkOffset - Mathf.Abs(_leftFootGround.point.y - _rightFootGround.point.y) >0.05f) _footIkOffset -= Time.deltaTime * 2;
				}
				else if (_footIkOffset > 0) _footIkOffset -= Time.deltaTime * 2;
				if (_footIkOffset > 0.4f && !_iscrouch) _footIkOffset = 0.4f;
				if (_footIkOffset > 0.26f && _iscrouch) _footIkOffset = 0.26f;
				return _footIkOffset*2f;
            }
			if(_iscrouch) {_controller.height = 1.5f - FootIkOffset(); _controller.center = new Vector3(0, 0.78f, 0); }  
			else {_controller.height = 1.8f - FootIkOffset(); _controller.center = new Vector3(0, 0.93f, 0); }          
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
            if (HavePushItem)
            {
                _rightHandPush = Physics.Raycast(Neck.position - transform.forward * ClimbGroundedOffset + transform.right * 0.28f, transform.forward, out _hitRightHand, WallDistance, PushLayers, QueryTriggerInteraction.Ignore);
                _leftHandPush = Physics.Raycast(Neck.position - transform.forward * ClimbGroundedOffset - transform.right * 0.28f, transform.forward, out _hitLeftHand, WallDistance, PushLayers, QueryTriggerInteraction.Ignore);
            }
            else
            {
                _rightHandPush = _leftHandPush = false;

            }
        }
        #region Push
        private void Push()
        {
			Vector3 LowRayPosition = new Vector3(transform.position.x, transform.position.y + ClimbGroundedOffset, transform.position.z);
			if (_speed >= 1f) HavePushItem = Physics.Raycast(LowRayPosition, transform.forward, out _hitPushItem, PushDistance, PushLayers, QueryTriggerInteraction.Ignore);
			else HavePushItem = false;
			if (HavePushItem) 
			{
				
				_controller.radius = PushDistance-0.1f;
				MoveSpeed = SprintSpeed = 1f;
				_hitPushItem.rigidbody.velocity = transform.forward * 1f;
				if(_speed>0.1f) _animator.SetBool(_animPush, true);
				else
                {
					_animator.SetBool(_animPush, false);
					MoveSpeed = 2f; SprintSpeed = 5.335f;
				}
                
			}
			else
            {
				_animator.SetBool(_animPush, false);
				_controller.radius = 0.28f;

			}
            
		}
		private void PushEnd()
        {
			MoveSpeed = 2f; SprintSpeed = 5.335f;
		}
        #endregion
        #region LookItem
        //放射线检测
        bool Look(string ItemTag )
		{

			var pos = transform.position-transform.up*0.05f ;
			//一条向前的射线
			if (LookAround(pos, Quaternion.identity, Color.green, ItemTag ))
				return true;

			//多一个精确度就多两条对称的射线,每条射线夹角是总角度除与精度
			float subAngle = 5;
			for (int i = 0; i < 10; i++)
			{
				for(float j = 0; j < 0.3f; j+=0.04f)
                {
					if (LookAround(pos + transform.up * j, Quaternion.Euler(0, -1 * subAngle * (i + 1), 0), Color.green, ItemTag)
					|| LookAround(pos + transform.up * j, Quaternion.Euler(0, subAngle * (i + 1), 0), Color.green, ItemTag))
					return true;
                }
			}

			return false;
		}

		//射出射线检测是否有Player
		bool LookAround(Vector3 pos, Quaternion eulerAnger, Color DebugColor, string ItemTag )
		{
#if UNITY_EDITOR
			if(Range)Debug.DrawRay(pos, eulerAnger * transform.forward, DebugColor);
#endif

			if (Physics.Raycast(pos, eulerAnger * transform.forward, out var _hit, 1) && _hit.collider.CompareTag(ItemTag))
			{
				//controller.chaseTarget = hit.transform;
				if (ItemTag == "Weight") _hitWeight = _hit;
				if (ItemTag == "Item") _hitItem = _hit;
				return true;
			}
			return false;
		}
        #endregion
        #region PickUpItem
        private void PickUpItem()
        {
			//前方锥形检测物体
			Look("Item");
			//旋转，朝向物体
			if (_input.pickUpItem_R && Look("Item") && !_holdItem_R &&!_holdWeightItem)
            {
				transform.forward = new Vector3(_hitItem.transform.position.x, transform.position.y ,_hitItem.transform.position.z)  - transform.position;
				if (Vector3.Distance(transform.position, new Vector3(_hitItem.transform.position.x, transform.position.y, _hitItem.transform.position.z)) >0.4f)
				{
					//Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
					//transform.position = lerpTargetPos;
					_speed = 0.5f;
					_animator.SetFloat(_animIDSpeed, 1);
                }
                else
                {
					//播动画
					_animator.SetBool(_animPick_R,true);
					_holdItem_R = true;
					MoveSpeed = 0;
					SprintSpeed = 0;
					_input.pickUpItem_R=false;
                }
            }else if(_input.pickUpItem_R && _holdItem_R && !_isTwoHand)
            {
				//if (_playerDamageRange_R != null) _playerBag_Right.itemList.Remove(_playerDamageRange_R.thisItem);
				//if (_medicineHP_R != null) _playerBag_Right.itemList.Remove(_medicineHP_R.thisItem);
				Destroy(_playerBag_Right.copyGameObjects[PacketNum_R]);
				_playerBag_Right.gameObjects[PacketNum_R].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				_playerBag_Right.gameObjects[PacketNum_R].transform.parent = transform.parent;
				_playerBag_Right.itemList[PacketNum_R] = null;
				_playerBag_Right.gameObjects[PacketNum_R] = null;
				_medicineHP_R = null;
				_playerDamageRange_R = null;
				_holdItem_R = false;
				_input.pickUpItem_R = false;
				//把当前物品从背包中移除
			}
			else _input.pickUpItem_R = false;
			//关键帧时让物体与ik重合

			if (_input.pickUpItem_L && Look("Item") && !_holdItem_L && !_holdWeightItem)
			{
				transform.forward = new Vector3(_hitItem.transform.position.x, transform.position.y, _hitItem.transform.position.z) - transform.position;
				if (Vector3.Distance(transform.position, new Vector3(_hitItem.transform.position.x, transform.position.y, _hitItem.transform.position.z)) > 0.4f)
				{
					//Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
					//transform.position = lerpTargetPos;
					_speed = 0.5f;
					_animator.SetFloat(_animIDSpeed, 1);
				}
				else
				{
					//播动画
					_animator.SetBool(_animPick_L, true);
					_holdItem_L = true;
					MoveSpeed = 0;
					SprintSpeed = 0;
					_input.pickUpItem_L = false;
				}
			}
			else if (_input.pickUpItem_L && _holdItem_L && !_isTwoHand)
			{
				//把当前物品从背包中移除
				//if(_playerDamageRange_L != null) _playerBag_Left.itemList.Remove(_playerDamageRange_L.thisItem);
				//if (_medicineHP_L != null) _playerBag_Left.itemList.Remove(_medicineHP_L.thisItem);
				//删除备份背包的物品
				Destroy(_playerBag_Left.copyGameObjects[PacketNum_L]);
				//回复rigidbody
				_playerBag_Left.gameObjects[PacketNum_L].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				//放会地面
				_playerBag_Left.gameObjects[PacketNum_L].transform.parent = transform.parent;
				//清空背包
				_playerBag_Left.itemList[PacketNum_L] = null;
				_playerBag_Left.gameObjects[PacketNum_L] = null;
				//设置空手状态
				_medicineHP_L = null;
				_playerDamageRange_L = null;
				_holdItem_L = false;
				_input.pickUpItem_L = false;
			}
			else _input.pickUpItem_L = false;
		}
		private void SetItemPos(string RightRoLeft)
        {
			bool ismedicine = false;
			if (RightRoLeft == "Left")
            {
				_animator.SetBool(_animPick_L, false);
				_hitItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				//激活伤害判定脚本
				if (_hitItem.transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange playerDamageRange))
                {
					playerDamageRange.userAnim = _animator;
					_playerDamageRange_L = playerDamageRange;
					//把物品放入左手小背包中
					_playerBag_Left.itemList[PacketNum_L] = _playerDamageRange_L.thisItem; 
				}else if(_hitItem.transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP))
                {
					ismedicine = true;
					medicine_HP.userHp = _hp;
					_medicineHP_L = medicine_HP;	
					//把物品放入左手小背包中
					_playerBag_Left.itemList[PacketNum_L] = _medicineHP_L.thisItem;
				}
				//gameobject放入背包列表
				_playerBag_Left.gameObjects[PacketNum_L] = _hitItem.transform.gameObject;
				//copy一份放入跨包
				_playerBag_Left.copyGameObjects[PacketNum_L] = Instantiate(_hitItem.transform.gameObject, PacketPos_Left);
				_playerBag_Left.copyGameObjects[PacketNum_L].SetActive(false);
				_playerBag_Left.copyGameObjects[PacketNum_L].transform.localPosition = new Vector3(0, 0, 0);
				if(ismedicine) _playerBag_Left.copyGameObjects[PacketNum_L].transform.localRotation = new Quaternion(0, 0, 180, 0);
				else _playerBag_Left.copyGameObjects[PacketNum_L].transform.localRotation = Quaternion.identity;
				//拿到手上
				_hitItem.transform.parent = LeftHandHoldPos;
				//设置位置
				_playerBag_Left.gameObjects[PacketNum_L].transform.localPosition = new Vector3(0, 0, 0);
				_playerBag_Left.gameObjects[PacketNum_L].transform.localRotation = Quaternion.identity;
			}else
            {
				_animator.SetBool(_animPick_R, false);
				_hitItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				//激活伤害判定脚本
				if (_hitItem.transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange playerDamageRange))
                {
					playerDamageRange.userAnim = _animator;
					_playerDamageRange_R = playerDamageRange;
					//把物品放入右手小背包中
					_playerBag_Right.itemList[PacketNum_R] =_playerDamageRange_R.thisItem;
				}
				else if (_hitItem.transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP))
				{
					medicine_HP.userHp = _hp;
					_medicineHP_R = medicine_HP;
					//把物品放入右手小背包中
					_playerBag_Right.itemList[PacketNum_R] =_medicineHP_R.thisItem;
				}
				_playerBag_Right.gameObjects[PacketNum_R] = _hitItem.transform.gameObject;
				_playerBag_Right.copyGameObjects[PacketNum_R] = Instantiate(_hitItem.transform.gameObject, PacketPos_Right);
				_playerBag_Right.copyGameObjects[PacketNum_R].SetActive(false);
				_playerBag_Right.copyGameObjects[PacketNum_R].transform.localPosition = new Vector3(0, 0, 0);
				_playerBag_Right.copyGameObjects[PacketNum_R].transform.localRotation = Quaternion.identity;
				_hitItem.transform.parent = RightHandHoldPos;
				_playerBag_Right.gameObjects[PacketNum_R].transform.localPosition = new Vector3(0, 0, 0);
				_playerBag_Right.gameObjects[PacketNum_R].transform.localRotation = Quaternion.identity;
			}
			

		}
		private void PickUpItemEnd()
        {	
			MoveSpeed = 2;
			SprintSpeed = 5.335f;
		}
		#endregion

		
		public void ItemToPacket()
        {
			//按下切换按钮
			if (_input.itemToPacket_L)
            {
				_input.itemToPacket_L = false;
				var newPacketNum_L = (PacketNum_L + 1) % 2;
                //隐藏当前手上的物品，显示预备背包中的另一个物品
                if (_playerBag_Left.gameObjects[PacketNum_L] != null)
                {
					_playerBag_Left.gameObjects[PacketNum_L].SetActive(false);
					_playerBag_Left.copyGameObjects[PacketNum_L].SetActive(true);
				}

				if (_playerBag_Left.gameObjects[newPacketNum_L] != null)
                {
					_playerBag_Left.gameObjects[newPacketNum_L].SetActive(true);
					_playerBag_Left.copyGameObjects[newPacketNum_L].SetActive(false);
					_holdItem_L = true;
				}
				else _holdItem_L = false;
                _medicineHP_L = null;
                _playerDamageRange_L = null;
				PacketNum_L = newPacketNum_L;
				_playerBag_Left.PacketNum=PacketNum_L;
            }
            if (_input.itemToPacket_R)
            {
				_input.itemToPacket_R = false;
				var newPacketNum_R = (PacketNum_R + 1) % 2;
				//隐藏当前手上的物品，显示预备背包中的另一个物品
				if (_playerBag_Right.gameObjects[PacketNum_R] != null)
                {
					_playerBag_Right.gameObjects[PacketNum_R].SetActive(false);
					_playerBag_Right.copyGameObjects[PacketNum_R].SetActive(true);
				}

				if (_playerBag_Right.gameObjects[newPacketNum_R] != null)
				{
					_playerBag_Right.gameObjects[newPacketNum_R].SetActive(true);
					_playerBag_Right.copyGameObjects[newPacketNum_R].SetActive(false);
					_holdItem_R = true;
				}
				else _holdItem_R = false;
				_medicineHP_R = null;
				_playerDamageRange_R = null;
				//_holdItem_L = false;
				PacketNum_R = newPacketNum_R;
				_playerBag_Right.PacketNum = PacketNum_R;
			}
        }
		#region Lifting
		private void Lifting()
        {
			if (_holdWeightItem) SprintSpeed = 2;
			else SprintSpeed = 5.335f;
			Look("Weight");
			if (Look("Weight") &&_input.liftingItem&&!_holdWeightItem&& !Look("Item") &&!_holdItem_R&&!_holdItem_L)
            {
                transform.forward = new Vector3(_hitWeight.transform.position.x, transform.position.y, _hitWeight.transform.position.z) - transform.position;
                _weightDis = Vector3.Distance(_hitWeight.point, _hitWeight.transform.position);
				if (Vector3.Distance(transform.position, new Vector3(_hitWeight.transform.position.x, transform.position.y, _hitWeight.transform.position.z)) > _weightDis + 0.1f+_controller.radius)
                {
                    //Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
                    //transform.position = lerpTargetPos;
                    _speed = 0.5f;
                    _animator.SetFloat(_animIDSpeed, 1f);
                }
                else
                {
                    //播动画
                    _animator.SetBool(_animLifting, true);
					_animator.SetBool(_animLiftingHold, true);
					_holdWeightItem = true;
					MoveSpeed = 0;
					SprintSpeed = 0;
					_input.liftingItem = false;
				}
			}
			else if(_input.liftingItem && _holdWeightItem || _stamina.ResidueRtamina<=0 && _holdWeightItem)
			{
				_animator.SetLayerWeight(_animLayerIDHand, 0);
				transform.GetChild(4).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				transform.GetChild(4).transform.parent = transform.parent;
				_animator.SetBool(_animLiftingHold, false);
				_holdWeightItem = false;
				_stamina.BeginReplyToMaxStamina = true;
			}
			else _input.liftingItem = false;	
			
			if (_holdWeightItem)
            {
				_rightHandPush = Physics.Raycast(RightHand.position-transform.up*0.5f, transform.up , out _hitWeightItemRightHand, 1f, WeightItem , QueryTriggerInteraction.Ignore);
				_leftHandPush = Physics.Raycast(LeftHand.position - transform.up * 0.5f, transform.up, out _hitWeightItemLeftHand, 1f, WeightItem, QueryTriggerInteraction.Ignore);
			}

		}
		//关键帧时让物体与ik重合
		private void LiftingSetItemPos()
		{
			//var dis=Vector3.Distance(_hitItme.point,_hitItme.transform.position);
			_animator.SetBool(_animLifting, false);
            if (Look("Weight"))
            {
				_hitWeight.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				_hitWeight.transform.parent = RightHandHoldPos;
			}
            else
            {
				_animator.SetBool(_animLiftingHold, false);
				_holdWeightItem = false;
				_stamina.BeginReplyToMaxStamina = true;
			}
			//_hitItme.transform.position = RightHandHoldPos.position + transform.forward*dis/2;
			//_hitItme.transform.up = transform.up;
			//s_hitItme.transform.Rotate(0, 60, 0);

		}
		private void LiftingItemEnd()
		{
			_animator.SetLayerWeight(_animLayerIDHand, 1);
			if(RightHandHoldPos.GetChild(1)!=null) RightHandHoldPos.GetChild(1).transform.parent = transform;
            if (transform.GetChild(4) != null)
            {
				transform.GetChild(4).transform.up = transform.up;
				transform.GetChild(4).transform.forward = transform.forward;
				transform.GetChild(4).transform.position = transform.position + transform.up * 1.1f + transform.forward * (_weightDis+_controller.radius-0.125f);
            }
			MoveSpeed = 2;
			SprintSpeed = 5.335f;
		}
		#endregion
		#region Attack
		public void TowardEnemy()
        {
			if (_lock.isLockOn && !_firstCombo && _lock.target!=null)
            {
				transform.forward = _lock.target.gameObject.transform.position - transform.position;
            }
        }
		private void SwitchTwoHand()
        {
            if (_input.switchTwoHand && !_isTwoHand)
            {
				//如果两个手都有东西放下左手的东西
				if(_holdItem_L && _holdItem_R)
                {
					LeftHandHoldPos.GetChild(0).transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
					LeftHandHoldPos.GetChild(0).transform.parent = transform.parent;
					_playerDamageRange_L = null;
					_holdItem_L = false;
				}
				_animator.SetLayerWeight(_animLayerIDHand, 1);
				_isTwoHand = true;
                _animator.SetBool(_animTwoHand, true);
                _input.switchTwoHand = false;
            }
            else if(_input.switchTwoHand && _isTwoHand)
			{
				_animator.SetLayerWeight(_animLayerIDHand, 0);
				_isTwoHand = false;
				_animator.SetBool(_animTwoHand, false);
				_input.switchTwoHand = false;
			}
        }
        private void AttackComboStart()
        {
			_isComboTime=true;
			_stamina.AttackStamina();
			_animator.SetBool(_animAttack, false);
		}
		private void CheckAttackCombo()
        {
            if (_playerDamageRange_R != null)
            {
				TowardEnemy();
				SwitchTwoHand();
				if (_isComboTime || _firstCombo)
				{
					Debug.Log("1");
					if (_input.mouseRight && _stamina.ResidueRtamina >= 10)
					{
						Debug.Log("2");
						_animator.SetLayerWeight(_animLayerIDHand, 0);
						_animator.SetBool(_animAttack, true);
						_firstCombo = false;
						MoveSpeed = SprintSpeed = 0.01f;
					}

				}
				else _input.mouseRight = false;
            }else 
            {
				_input.switchTwoHand=false;
			}
			
        }
		private void GiveDamageStart()
        {
			if(_playerDamageRange_R!=null)
				_playerDamageRange_R.isGiveDamage = true;
        }
        private void GiveDamageEnd()
        {
			if (_playerDamageRange_R != null)
				_playerDamageRange_R.isGiveDamage = false;
		}
        private void AttackComboEnd()
        {
			_isComboTime = false;
            if (!_animator.GetBool(_animAttack))
            {
				_animator.SetLayerWeight(_animLayerIDHand, 1);
				_firstCombo = true;
				MoveSpeed = 2;
				SprintSpeed = 5.335f;
			}
        }
        private void AttackEnd()
        {
			_animator.SetLayerWeight(_animLayerIDHand, 1);
			_stamina.AttackStamina();
			_animator.SetBool(_animAttack, false);
			_firstCombo = true;
			MoveSpeed = 2;
			SprintSpeed = 5.335f;
		}
		#endregion
		public void UserMedicine()
        {
			if (_medicineHP_R != null)
			{

                if (_input.mouseRight)
                {
					_input.mouseRight = false;
                    _medicineHP_R.Use();
                } 
			}
			if (_medicineHP_L != null)
			{

                if (_input.mouseLeft)
                {
					_input.mouseLeft = false;
                    _medicineHP_L.Use();
                }  
			}
		}
		public void TowHandIsNull()
        {
			if(_medicineHP_L==null&& _playerDamageRange_L==null) _input.mouseLeft = false;
			if(_medicineHP_R==null&& _playerDamageRange_R==null) _input.mouseRight = false;
        }

		private void Roll()
        {
			AnimatorStateInfo stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
			bool play_ing_flag = stateinfo.IsName("Stand To Roll");
			if (_input.roll && _stamina.ResidueRtamina>=10)
            {
				_animator.SetBool(_animRoll, true);
				_input.roll = false;
				_stamina.RollStamina();
			}
			else if(_animator.GetFloat("RollTime")<1 && play_ing_flag) _animator.SetBool(_animRoll, false);
			else _input.roll=false;


		}
		private void Crouch()
        {
			if (_input.crouch && !_iscrouch)
            {
				_input.crouch = false;
				_iscrouch = true;
				MoveSpeed = 0.5f;
				SprintSpeed = 0.5f; 
				if (_animator)
                {
					_animator.SetBool(_animCrouch, true);
                }
            }
			if (_input.crouch && _iscrouch)
            {
				_input.crouch = false;
				_iscrouch = false;
				MoveSpeed = 2.0f;
				SprintSpeed = 5.335f;
				if (_animator)
                {
					_animator.SetBool(_animCrouch, false);
                }
            }
        }
        private void WallCheck()
        {
			Vector3 LowRayPosition = new Vector3(transform.position.x, transform.position.y + ClimbGroundedOffset, transform.position.z);
            HaveWall = Physics.Raycast(LowRayPosition, Quaternion.AngleAxis(60, -transform.right) * transform.forward, out _hitWall, WallDistance, Wall, QueryTriggerInteraction.Ignore);
			//角色需要移动到的点
            targetPos = _hitWall.point + Quaternion.AngleAxis(60, -transform.right) * _hitWall.normal * WallDistance;

			//从hit处开始向上检测,检测墙的高度
			//if (!HaveWall) _hitWallHigh = 0;
			//if (HaveWall) _boxContactWall = Physics.CheckBox(_hitWall.point + transform.up*_hitWallHigh, new Vector3(0.25f, 0.9f, 0.25f), Quaternion.identity,Wall, QueryTriggerInteraction.Ignore);
			//if (_boxContactWall) _hitWallHigh += Time.deltaTime * 7;
			//Debug.Log("墙高" + (_hitWall.point.y + _hitWallHigh - 0.9f));
			//if(!HaveWall) _hitWallHigh = 0;
			if (HaveWall)
            {
				var hitWall = false;
				for (float i = 0; i < ClimbHighWall; i += 0.05f)
				{
					//射线检测墙体
					Debug.DrawRay(_hitWall.point + transform.up * i - transform.forward * 0.5f, transform.forward);
					hitWall = Physics.Raycast(_hitWall.point + transform.up * i - transform.forward * 0.5f, transform.forward, out var hitPos, WallDistance * 2f, Wall, QueryTriggerInteraction.Ignore);
					if (!hitWall)
					{
						//如果没有击中墙体生成盒子检测可否翻越
						_boxContactWall = Physics.CheckBox(hitPos.point + transform.up * (i+0.9f), new Vector3(0.25f, 0.9f, 0.25f), Quaternion.identity, Wall, QueryTriggerInteraction.Ignore);
						if (!_boxContactWall)
						{
							
							Debug.Log("获取了墙的高度"+_hitWallHigh);
							//如果盒子没有触碰到墙就获取高度
							_hitWallHigh = i+0.9f;
							break;
						}

					}
				}
				if (_boxContactWall || hitWall)
                {
					//超过最高攀爬高度
					_hitWallHigh = ClimbHighWall + 0.1f + 0.9f;
				}
			}
			

			//从Character Controller不能上的台阶高度开始检测
			if (!HaveWall) { _haveStep = Physics.Raycast(transform.position + transform.up * 0.28f, transform.forward, 0.3f, Wall, QueryTriggerInteraction.Ignore); }
			else { _haveStep = false; }
			if (_haveStep) { _boxContactStep = Physics.CheckBox(transform.position + transform.up * (0.28f + _hitStepHigh + 0.9f) + transform.forward * 0.5f, new Vector3(0.25f, 0.9f, 0.25f) , Quaternion.identity, Wall, QueryTriggerInteraction.Ignore); }
            else { _hitStepHigh = 0; }
			if(_boxContactStep) _hitStepHigh += Time.deltaTime * 7;//Debug.Log(_haveStep + "" + _boxContactStep + ""+_hitStepHigh);

			//判断是否到最高处
			
			_leftHandWallToLand = !Physics.Raycast(LeftHand.position - transform.forward * ClimbGroundedOffset*2, transform.forward, out var _hitWallTop_L, WallDistance * 2f, Wall,  QueryTriggerInteraction.Ignore);
			_rightHandWallToLand = !Physics.Raycast(RightHand.position - transform.forward * ClimbGroundedOffset * 2, transform.forward, out var _hitWallTop_R, WallDistance * 2f, Wall, QueryTriggerInteraction.Ignore);
			if ((_leftHandWallToLand || _rightHandWallToLand) && _animator.GetBool(_animIDClimbingIdleWall))
            {
				if (_hitWallTop_L.point.y > _hitWallTop_R.point.y) _hitWallTop = _hitWallTop_L;
				else _hitWallTop = _hitWallTop_R;
				//Debug.Log(_hitWallTop.point.y);
			}

			//判断左右两边是否可以攀爬
			_leftBoxCheckWall = Physics.CheckBox(_hitWall.point + -transform.right * WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.25f, 0.8f, 0.25f), Quaternion.identity, Wall, QueryTriggerInteraction.Ignore);
			_rightBoxCheckWall = Physics.CheckBox(_hitWall.point + transform.right * WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.25f, 0.8f, 0.25f), Quaternion.identity, Wall, QueryTriggerInteraction.Ignore);
			//Debug.Log("_leftBoxCheckWall:" + _leftBoxCheckWall + "\n" + "_rightBoxCheckWall:" + _rightBoxCheckWall);
		}
		#region Climb
        private void ClimbAcceleratedSpeed()
        {
			//攀爬加速度
			float ClimbInputRate(float input, float speed)
			{
				if (input!=0)
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
				if (inputY < 0) { IsWallTop = _rightHandWallToLand = _leftHandWallToLand = _moveWallTop = false; }
				if (IsWallTop && inputY > 0) { Y = 0; }
				else { Y = inputY; }
				return Y;
			}
			float InputMoveX(float inputX)
			{
				float X;
				if ((inputX > 0 && !_rightBoxCheckWall) || (inputX < 0 && !_leftBoxCheckWall)) { X = 0; _climbAcceleratedSpeed_X = _climbAcceleratedSpeed_X / 1.1f; }
				else { X = inputX; }
				return X;
			}
			_climbAcceleratedSpeed_X = ClimbInputRate(InputMoveX(_input.move.x), _climbAcceleratedSpeed_X);
			_climbAcceleratedSpeed_Y = ClimbInputRate(InputMoveY(_input.move.y), _climbAcceleratedSpeed_Y);
		}
		
		private void Climb()
        {
			AnimatorStateInfo stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
			bool play_ing_flag = stateinfo.IsName("Blend Tree");
			if (HaveWall)
			{
                if (_input.climb)
                {
					//攀爬墙壁
					if ( _hitWallHigh - 0.9f > ClimbHighWall|| play_ing_flag)
					{
						//玩家旋转，前面与法向量对齐
						transform.forward = -_hitWall.normal;
						//消除重力，粘着墙
						Gravity = 0;
                        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                        {
                            transform.position = targetPos;
                            return;
                        }
                        Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, targetPos, 0.01f);
                        transform.position = lerpTargetPos;
                        //墙上移动
                        if (_hasAnimator)
						{
							//count = 1;
							_animator.SetFloat(_animIDClimbingHorizontalWall, _climbAcceleratedSpeed_X);
							_controller.Move(transform.right * ClimbSpeed * Time.deltaTime * _climbAcceleratedSpeed_X/1.4f);
							_animator.SetFloat(_animIDClimbingVerticalWall, _climbAcceleratedSpeed_Y);
							//_controller.Move(transform.up * ClimbSpeed * Time.deltaTime * _climbAcceleratedSpeed_Y);
						}
						// update animator if using character
						if (_hasAnimator)
						{
							_animator.SetBool(_animIDClimbingIdleWall, true);
						}

                    }
                    else 
					{   
						if (ClimbMediumWall < _hitWallHigh - 0.9f && _hitWallHigh - 0.9f < ClimbHighWall)
							{
								if (_hasAnimator)
								{
									_animator.SetBool(_animIDClimbHighWall, true);
								}
								//动画触发ClimbHighWallEvent给速度				
							}
						if (ClimbMediumWall > _hitWallHigh - 0.9f && _hitWallHigh - 0.9f > ClimbLowWall)
						{
						if (_hasAnimator)
						{
							_animator.SetBool(_animIDClimbMediumWall, true);
						}
						//动画触发ClimbMediumWallEvent给速度
						}
						if (_hitWallHigh - 0.9f < ClimbLowWall )
						{								
							if (_hasAnimator)
							{
								_animator.SetBool(_animIDClimbLowWall, true);
							}
						//动画触发ClimbLowWallEvent给速度
						//F*S=1/2*m*v平方 v=Sqrt(2FS)
						}
						_input.jump =  false;
						
					}//翻上墙壁

				}			
				if (_input.releaseClimb ||_stamina.ResidueRtamina <= 0)
				{
					_input.jump = false;
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, false);
                        _animator.SetBool(_animIDClimbingIdleWall, false);
                    }
                    Gravity = -15;
					_input.climb = false;
					_input.releaseClimb = false;
				}//放弃攀爬
			}
			else if (_haveStep && _speed>2)
            {
				if (_hasAnimator)
				{
					_animator.SetBool(_animStepUp,true);
				}
				//动画中触发StepUpEvent
			}//上台阶
			stateinfo = _animator.GetCurrentAnimatorStateInfo(0);  
			play_ing_flag = stateinfo.IsName("Blend Tree");
			//Debug.Log(play_ing_flag);
            if (play_ing_flag)
            {
				Debug.Log("moveWallTop:" + _moveWallTop);
				if (_rightHandWallToLand && !IsWallTop)
				{
					
					WallTopPos_Y = RightHand.position.y;
					//_controller.Move(transform.up * (WallTopPos_Y - transform.position.y - 1.5f));
					IsWallTop = true; 
					_input.jump = false;
					_moveWallTop=true;
				}
				if (_leftHandWallToLand && !IsWallTop)
				{
					WallTopPos_Y = LeftHand.position.y;
					//_controller.Move(transform.up * (WallTopPos_Y - transform.position.y - 1.5f));
					IsWallTop = true; 
					_input.jump = false;
					_moveWallTop = true;
				}
                if (IsWallTop && _moveWallTop && !(_input.move.y<0))
                {

					if (Vector3.Distance(transform.position, new Vector3(transform.position.x, WallTopPos_Y-1.5f, transform.position.z) ) < 0.01f)
					{
						
						transform.position = new Vector3(transform.position.x, WallTopPos_Y, transform.position.z);
						_moveWallTop = false;
						return;
					}
					Debug.Log("缩进距离");
					if(transform.position.y< WallTopPos_Y - 1.5f) _controller.Move(transform.up*Time.deltaTime/7);
					if(transform.position.y> WallTopPos_Y - 1.5f) _controller.Move(-transform.up*Time.deltaTime/7);
				}

            }//检测是否爬到墙的最上方
			if (IsWallTop && _input.jump)
			{
				//爬到最上面按空格上去
				if (_hasAnimator)
				{
					
					_animator.SetBool(_animIDClimbingToWall, true);
				}
				_jumpTimeoutDelta = 0.5f;
				_input.releaseClimb = false;
				//var nextPos = Mathf.Lerp(transform.position.y, WallTopPos_Y, Time.deltaTime / 10);
				_controller.Move(transform.up * Time.deltaTime*2);                                                                                                                                                                                    
				//_verticalVelocity = Mathf.Sqrt((WallTopPos_Y - transform.position.y + 1f) * -2f * Gravity);

				if (WallTopPos_Y < transform.position.y)
				{
					IsWallTop = _input.climb = false;
					_input.jump = false;//这里设置jump没成功不知道为什么
					Gravity = -15;
					_animator.SetBool(_animIDClimbingIdleWall, false);
					_animator.SetBool(_animIDJump, false);
				}
			}//到墙的最上方按空格翻上去
            if (!HaveWall && !_animator.GetBool(_animIDClimbingToWall))
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDClimbingIdleWall, false);
                }
                Gravity = -15;
                _input.climb = false;
                _input.releaseClimb = false;
            }//回复在地面的情况

        }
		private void StepUpEvent()
        {
			_verticalVelocity = Mathf.Sqrt((_hitStepHigh) * -2f * Gravity);
			Debug.Log("台阶" + _verticalVelocity);
		}
		private void ClimbHighWallEvent1()
        {
			_verticalVelocity = Mathf.Sqrt((_hitWall.point.y + _hitWallHigh - 1.8f - transform.position.y -1f) * -2f * Gravity); 
			Debug.Log("高墙1" + _verticalVelocity);
		}
		private void ClimbHighWallEvent2()
        {
			_verticalVelocity = Mathf.Sqrt(1f * -2f * Gravity);
			Debug.Log("高墙2" + _verticalVelocity);
		}
		private void ClimbMediumWallEvent()
        {
			_verticalVelocity = Mathf.Sqrt((_hitWall.point.y + _hitWallHigh - 1.4f - transform.position.y) * -2f * Gravity); 
			Debug.Log("中墙" + _verticalVelocity);
		}
		private void ClimbLowWallEvent()
        {
			_verticalVelocity = Mathf.Sqrt((_hitWall.point.y + _hitWallHigh - 0.9f - transform.position.y) * -2f * Gravity); 
			Debug.Log("矮墙" + _verticalVelocity);
		}
#endregion
        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			//检测有没有与地面碰撞（点，半径，Layer，指定该查询是否应该命中触发器）
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayer, QueryTriggerInteraction.Ignore);

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDGrounded, Grounded);
			}

			//脚部IK射线检测
			_leftFootRay=Physics.Raycast(LeftFoot.position + transform.up*0.5f, -transform.up , out _leftFootGround, 1.3f, GroundLayer,  QueryTriggerInteraction.Ignore);
			_rightFootRay=Physics.Raycast(RightFoot.position + transform.up * 0.5f, -transform.up , out _rightFootGround, 1.3f, GroundLayer,  QueryTriggerInteraction.Ignore);
			//Debug.Log(LeftFoot.position.y);//IK离地0.05
		}

		private void CameraRotation()
		{
            if (_lock.isLockOn)
            {
				CinemachineCameraTarget.transform.forward = _lock.target.transform.position - CinemachineCameraTarget.transform.position;
			}
            else
            {
				// if there is an input and camera position is not fixed
				if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
				{
					//Don't multiply mouse input by Time.deltaTime;
					float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

					_cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
					_cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
				}

				// clamp our rotations so our values are limited 360 degrees
				_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Cinemachine will follow this target
				CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
			}
			
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero || _input.climb) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			//float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (_speed < targetSpeed - speedOffset || _speed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}
			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			//在没攀爬的状态下才可以旋转
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero && !_input.climb)
			{
				_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
				
				// rotate to face input direction relative to camera position
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}


			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// move the player
			_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

			// update animator if using character
			if (_hasAnimator)
			{
				if(!_input.pickUpItem_R && !_input.pickUpItem_L && !_input.liftingItem) _animator.SetFloat(_animIDSpeed, _animationBlend);
				_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
			}
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// update animator if using character
				if (_hasAnimator)
				{
					
					_animator.SetBool(_animIDJump, false);
					_animator.SetBool(_animIDFreeFall, false);
					_animator.SetBool(_animIDClimbingToWall, false);
					_animator.SetBool(_animIDClimbingIdleWall, false);
					_animator.SetBool(_animIDClimbLowWall, false);
					_animator.SetBool(_animIDClimbMediumWall, false);
					_animator.SetBool(_animIDClimbHighWall, false);
					_animator.SetBool(_animStepUp, false);
					IsWallTop =false;
					
				}

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f &&(_hitWallHigh - 0.9f > ClimbHighWall|| !HaveWall) && _stamina.ResidueRtamina>=10 &&!_holdWeightItem )
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity*1.6f);
					
					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDJump, true);
					}
				}else _input.jump = false;

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_input.jump = false;
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}else if (_input.climb) {  }
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
					// update animator if using character
					if (_hasAnimator)
					{
						_animator.SetBool(_animIDFreeFall, true);
					}
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
				//Debug.Log(_verticalVelocity);
				//a=Gravity * Time.deltaTime
				_verticalVelocity += Gravity * Time.deltaTime;
            }
        }

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
			var LowRayPosition = new Vector3(transform.position.x, transform.position.y + ClimbGroundedOffset, transform.position.z);
			//跳跃检测
			if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

			//判断是否在墙上
            if (HaveWall) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + ClimbGroundedOffset, transform.position.z), (Quaternion.AngleAxis(60, -transform.right) * transform.forward) *WallDistance);
			if(HaveWall) Gizmos.DrawSphere(targetPos, 0.1f);

			//判断是否爬到最上面
			if (_leftHandWallToLand) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			//Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.53f, transform.position.z), transform.forward*WallDistance/2);
			Gizmos.DrawRay(LeftHand.position - transform.forward * ClimbGroundedOffset * 2, transform.forward*WallDistance * 1.5f);
			if (_rightHandWallToLand) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawRay(RightHand.position - transform.forward * ClimbGroundedOffset * 2, transform.forward*WallDistance * 1.5f);

			//长方形体检测墙壁是否可以翻越
			if (HaveWall) 
			{
				Gizmos.DrawCube(_hitWall.point + transform.up * _hitWallHigh, new Vector3(0.5f, 1.8f, 0.5f)); 
				//if(_hitWallOffset>)
			}

			//从Character Controller不能上的台阶高度开始检测
			if (_haveStep) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawRay ( transform.position + transform.up * 0.28f, transform.forward * 0.295f);
			if (_haveStep) Gizmos.DrawCube(transform.position + transform.up * (0.28f + _hitStepHigh +0.9f) + transform.forward * 0.5f, new Vector3(0.5f, 1.8f, 0.5f));	


			//攀爬时长方形检测左右两边是否可以攀爬
			if (HaveWall)
            {
				if (_rightBoxCheckWall) Gizmos.color = transparentGreen;
				else Gizmos.color = transparentRed;
				Gizmos.DrawCube(_hitWall.point + transform.right * WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.5f, 1.6f, 0.5f));
				if (_leftBoxCheckWall) Gizmos.color = transparentGreen;
				else Gizmos.color = transparentRed;
				Gizmos.DrawCube(_hitWall.point + -transform.right * WallDistance * 1.6f + transform.up * 0.2f, new Vector3(0.5f, 1.6f, 0.5f));
            }
				
			//检测墙的高度
			if (HaveWall)
            {
				if (_hitWallHigh - 0.9f > ClimbLowWall) Gizmos.color = transparentGreen;
				else Gizmos.color = transparentRed;
				Gizmos.DrawSphere(_hitWall.point+transform.up*ClimbLowWall, 0.1f);
				if (_hitWallHigh - 0.9f > ClimbMediumWall) Gizmos.color = transparentGreen;
				else Gizmos.color = transparentRed;
				Gizmos.DrawSphere(_hitWall.point+transform.up*ClimbMediumWall, 0.1f);
				if (_hitWallHigh - 0.9f > ClimbHighWall) Gizmos.color = transparentGreen;
				else Gizmos.color = transparentRed;
				Gizmos.DrawSphere(_hitWall.point+transform.up*ClimbHighWall, 0.1f);
			}
			//for(float i = 0; i < ClimbHighWall; i += 0.05f)
   //         {
			//	Gizmos.DrawRay(_hitWall.point + transform.up*i,transform.forward);
   //         }
			


			//脚部Ik检测
			if(_leftFootRay) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			Gizmos.DrawRay(LeftFoot.position + transform.up * 0.2f, -transform.up*0.7f);
			Gizmos.DrawSphere(_leftFootGround.point, 0.1f);
			if (_rightFootRay) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed; 
			Gizmos.DrawRay(RightFoot.position + transform.up * 0.2f, -transform.up*0.7f);
			Gizmos.DrawSphere(_rightFootGround.point, 0.1f);


		}
#endif
	}
}