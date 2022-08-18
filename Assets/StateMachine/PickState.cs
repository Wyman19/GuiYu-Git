using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickState : MonoBehaviour, IState
{
    private FSM manager;
    private Parameter parameter;

    public PickState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
		TryGetComponent<FSM>(out this.manager);
		this.parameter = manager.parameter;
		parameter._input.liftingItem = false;
	}

    public void OnUpdate()
    {
		PickUpItem();

	}

    public void OnExit()
    {

    }
	#region PickUpItem
	private void PickUpItem()
	{
		//前方锥形检测物体
		manager.Look("Item");
		//旋转，朝向物体
		if (parameter._input.pickUpItem_R && manager.Look("Item") && !parameter._holdItem_R && !parameter._holdWeightItem)
		{
			manager.transform.forward = new Vector3(parameter._hitItem.transform.position.x, manager.transform.position.y, parameter._hitItem.transform.position.z) - manager.transform.position;
			if (Vector3.Distance(manager.transform.position, new Vector3(parameter._hitItem.transform.position.x, manager.transform.position.y, parameter._hitItem.transform.position.z)) > 0.4f)
			{
				//Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
				//transform.position = lerpTargetPos;
				parameter._speed = 0.5f;
				parameter._animator.SetFloat(parameter._animIDSpeed, 1);
			}
			else
			{
				//播动画
				parameter._animator.SetBool(parameter._animPick_R, true);
				parameter._holdItem_R = true;
				parameter.MoveSpeed = 0;
				parameter.SprintSpeed = 0;
				parameter._input.pickUpItem_R = false;
			}
		}
		else if (parameter._input.pickUpItem_R && parameter._holdItem_R && !parameter._isTwoHand)
		{
			//if (_playerDamageRange_R != null) _playerBag_Right.itemList.Remove(_playerDamageRange_R.thisItem);
			//if (_medicineHP_R != null) _playerBag_Right.itemList.Remove(_medicineHP_R.thisItem);
			
			manager.DestroyGameObject(parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R]);
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.parent = manager.transform.parent;
			parameter._playerBag_Right.itemList[parameter.PacketNum_R] = null;
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R] = null;
			parameter._medicineHP_R = null;
			parameter._playerDamageRange_R = null;
			parameter._holdItem_R = false;
			parameter._input.pickUpItem_R = false;
			//把当前物品从背包中移除
		}
		else parameter._input.pickUpItem_R = false;
		//关键帧时让物体与ik重合

		if (parameter._input.pickUpItem_L && manager.Look("Item") && !parameter._holdItem_L && !parameter._holdWeightItem)
		{
			manager.transform.forward = new Vector3(parameter._hitItem.transform.position.x, manager.transform.position.y, parameter._hitItem.transform.position.z) - manager.transform.position;
			if (Vector3.Distance(manager.transform.position, new Vector3(parameter._hitItem.transform.position.x, manager.transform.position.y, parameter._hitItem.transform.position.z)) > 0.4f)
			{
				//Vector3 lerpTargetPos = Vector3.MoveTowards(transform.position, _hitItme.transform.position, 0.1f);
				//transform.position = lerpTargetPos;
				parameter._speed = 0.5f;
				parameter._animator.SetFloat(parameter._animIDSpeed, 1);
			}
			else
			{
				//播动画
				parameter._animator.SetBool(parameter._animPick_L, true);
				parameter._holdItem_L = true;
				parameter.MoveSpeed = 0;
				parameter.SprintSpeed = 0;
				parameter._input.pickUpItem_L = false;
			}
		}
		else if (parameter._input.pickUpItem_L && parameter._holdItem_L && !parameter._isTwoHand)
		{
			//把当前物品从背包中移除
			//if(_playerDamageRange_L != null) _playerBag_Left.itemList.Remove(_playerDamageRange_L.thisItem);
			//if (_medicineHP_L != null) _playerBag_Left.itemList.Remove(_medicineHP_L.thisItem);
			//删除备份背包的物品
			manager.DestroyGameObject(parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L]);
			//回复rigidbody
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			//放会地面
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.parent = manager.transform.parent;
			//清空背包
			parameter._playerBag_Left.itemList[parameter.PacketNum_L] = null;
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L] = null;
			//设置空手状态
			parameter._medicineHP_L = null;
			parameter._playerDamageRange_L = null;
			parameter._holdItem_L = false;
			parameter._input.pickUpItem_L = false;
		}
		else parameter._input.pickUpItem_L = false;
	}
	private void SetItemPos(string RightRoLeft)
	{
		bool ismedicine = false;
		if (RightRoLeft == "Left")
		{
			parameter._animator.SetBool(parameter._animPick_L, false);
			parameter._hitItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			//激活伤害判定脚本
			if (parameter._hitItem.transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange playerDamageRange))
			{
				playerDamageRange.userAnim = parameter._animator;
				parameter._playerDamageRange_L = playerDamageRange;
				//把物品放入左手小背包中
				parameter._playerBag_Left.itemList[parameter.PacketNum_L] = parameter._playerDamageRange_L.thisItem;
			}
			else if (parameter._hitItem.transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP))
			{
				ismedicine = true;
				medicine_HP.userHp = parameter._hp;
				parameter._medicineHP_L = medicine_HP;
				//把物品放入左手小背包中
				parameter._playerBag_Left.itemList[parameter.PacketNum_L] = parameter._medicineHP_L.thisItem;
			}
			//gameobject放入背包列表
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L] = parameter._hitItem.transform.gameObject;
			//copy一份放入跨包
			parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L] = manager.InstantiateGamgeObject(parameter._hitItem.transform.gameObject, parameter.PacketPos_Left);
			parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L].SetActive(false);
			parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L].transform.localPosition = new Vector3(0, 0, 0);
			if (ismedicine) parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L].transform.localRotation = new Quaternion(0, 0, 180, 0);
			else parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L].transform.localRotation = Quaternion.identity;
			//拿到手上
			parameter._hitItem.transform.parent = parameter.LeftHandHoldPos;
			//设置位置
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.localPosition = new Vector3(0, 0, 0);
			parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.localRotation = Quaternion.identity;
		}
		else
		{
			parameter._animator.SetBool(parameter._animPick_R, false);
			parameter._hitItem.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			//激活伤害判定脚本
			if (parameter._hitItem.transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange playerDamageRange))
			{
				playerDamageRange.userAnim = parameter._animator;
				parameter._playerDamageRange_R = playerDamageRange;
				//把物品放入右手小背包中
				parameter._playerBag_Right.itemList[parameter.PacketNum_R] = parameter._playerDamageRange_R.thisItem;
			}
			else if (parameter._hitItem.transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP))
			{
				medicine_HP.userHp = parameter._hp;
				parameter._medicineHP_R = medicine_HP;
				//把物品放入右手小背包中
				parameter._playerBag_Right.itemList[parameter.PacketNum_R] = parameter._medicineHP_R.thisItem;
			}
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R] = parameter._hitItem.transform.gameObject;
			parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R] = manager.InstantiateGamgeObject(parameter._hitItem.transform.gameObject, parameter.PacketPos_Right);
			parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R].SetActive(false);
			parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R].transform.localPosition = new Vector3(0, 0, 0);
			parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R].transform.localRotation = Quaternion.identity;
			parameter._hitItem.transform.parent = parameter.RightHandHoldPos;
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.localPosition = new Vector3(0, 0, 0);
			parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.localRotation = Quaternion.identity;
		}


	}
	private void PickUpItemEnd()
	{
		parameter.MoveSpeed = 2;
		parameter.SprintSpeed = 5.335f;
		manager.TransitionState(StateType.Idle);
	}
	#endregion
	
}
