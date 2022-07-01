using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool climb;
		public bool releaseClimb;
		public bool sprint;
		public bool roll;
		public bool crouch;
		public bool pickUpItem_R;
		public bool pickUpItem_L;
		public bool liftingItem;
		public bool mouseRight;
		public bool mouseLeft;
		public bool switchTwoHand;
		public bool lockOn;
		public float scrollWheel;
		public bool itemToPacket_L;
		public bool itemToPacket_R;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM 
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}
		public void OnClimb(InputValue value)
		{
			ClimbInput(value.isPressed);
		}
		public void OnReleaseClimb(InputValue value)
		{
			ReleaseClimbInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnRoll(InputValue value)
		{
			RollInput(value.isPressed);
		}
		public void OnCrouch(InputValue value)
        {
			CrouchInput(value.isPressed);
        }
		public void OnPickUpItem_R(InputValue value)
        {
			PickUpItem_RInput(value.isPressed);
        }public void OnPickUpItem_L(InputValue value)
        {
			PickUpItem_LInput(value.isPressed);
        }public void OnLifting(InputValue value)
        {
			LiftingInput(value.isPressed);
        }public void OnMouseRight(InputValue value)
        {
			MouseRightInput(value.isPressed);
        }public void OnMouseLeft(InputValue value)
        {
			MouseLeftInput(value.isPressed);
        }
		public void OnSwitchTwoHand(InputValue value)
        {
			SwitchTwoHandInput(value.isPressed);
        }
		public void OnPause(InputValue value)
        {
            if (value.isPressed)
            {
				if (!PauseSetting._isPause)
				{
					SetCursorState(false);
					UIManager.Instance.PushPanel(UIPanelType.PauseSetting);
				}
				else if (PauseSetting._isPause)
				{
					SetCursorState(true);
					UIManager.Instance.PopPanel();
				}
			}
        }public void OnBag(InputValue value)
        {
            if (value.isPressed)
            {
				if (!Bag._isPause)
				{
					SetCursorState(false);
					UIManager.Instance.PushPanel(UIPanelType.Bag);
				}
				else if (Bag._isPause)
				{
					SetCursorState(true);
					UIManager.Instance.PopPanel();
				}
			}
        }
		public void OnLock(InputValue value)
        {
			LockInput(value.isPressed);
        }public void OnScrollWheel(InputValue value)
        {
			ScrollWheelInput(value.Get<float>());
        }public void OnItemToPacket_R(InputValue value)
        {
			ItemToPacket_RInput(value.isPressed);
        }public void OnItemToPacket_L(InputValue value)
        {
			ItemToPacket_LInput(value.isPressed);
        }
#else
		// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
		public void ClimbInput(bool newClimbState)
		{
			climb = newClimbState;
		}
		public void ReleaseClimbInput(bool newReleaseClimbState)
		{
			releaseClimb = newReleaseClimbState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void RollInput(bool newRollState)
		{
			roll = newRollState;
		}
		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;
		}
		public void PickUpItem_RInput(bool newPickUpItem_RState)
		{
			pickUpItem_R = newPickUpItem_RState;
		}public void PickUpItem_LInput(bool newPickUpItem_LState)
		{
			pickUpItem_L = newPickUpItem_LState;
		}
		public void LiftingInput(bool newLiftingState)
		{
			liftingItem = newLiftingState;
		}public void MouseRightInput(bool newMouseRightState)
		{
			mouseRight = newMouseRightState;
		}public void MouseLeftInput(bool newMouseLeftState)
		{
			mouseLeft = newMouseLeftState;
		}public void SwitchTwoHandInput(bool newSwitchTwoHandState)
		{
			switchTwoHand = newSwitchTwoHandState;
		}public void LockInput(bool newLockState)
		{
			lockOn = newLockState;
		}public void ScrollWheelInput(float newScrollWheel)
		{
			scrollWheel = newScrollWheel;
		}public void ItemToPacket_LInput(bool newItemToPacket_L)
		{
			itemToPacket_L = newItemToPacket_L;
		}public void ItemToPacket_RInput(bool newItemToPacket_R)
		{
			itemToPacket_R = newItemToPacket_R;
		}


#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}