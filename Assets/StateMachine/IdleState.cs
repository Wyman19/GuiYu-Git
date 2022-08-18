using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private FSM manager;
    private Parameter parameter;

    private AnimatorStateInfo stateinfo; 

    public IdleState(FSM manager)
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
        ItemToPacket();
        if (parameter._input.jump && !parameter.HaveWall && parameter._stamina.ResidueRtamina >= 10)
        {
            manager.TransitionState(StateType.Jump);
        }
        if (parameter._input.climb && parameter.HaveWall)
        {
            manager.TransitionState(StateType.Climb);
        }
        if (parameter._haveStep && parameter._speed > 2)
        {
            manager.TransitionState(StateType.StepUp);
        }//上台阶
        if (parameter._input.roll && parameter._stamina.ResidueRtamina >= 10)
        {
            manager.TransitionState(StateType.Roll);
        }
        else parameter._input.roll = false;
        if (parameter._input.crouch && !parameter._iscrouch)
        {
            manager.TransitionState(StateType.Crouch);
        }
        if (((parameter._input.pickUpItem_R || parameter._input.pickUpItem_L) && manager.Look("Item")) || (parameter._holdItem_L && parameter._input.pickUpItem_L) || (parameter._holdItem_R && parameter._input.pickUpItem_R))
        {
            manager.TransitionState(StateType.Pick);
        }
        if ((manager.Look("Weight") && parameter._input.liftingItem) || (parameter._holdWeightItem && parameter._input.liftingItem) || (parameter._stamina.ResidueRtamina <= 0 && parameter._holdWeightItem))
        {
            manager.TransitionState(StateType.Lift);
        }
        Vector3 LowRayPosition = new Vector3(manager.transform.position.x, manager.transform.position.y + parameter.ClimbGroundedOffset, manager.transform.position.z);
        if (parameter._speed >= 1f) parameter.HavePushItem = Physics.Raycast(LowRayPosition, manager.transform.forward, out parameter._hitPushItem, parameter.PushDistance, parameter.PushLayers, QueryTriggerInteraction.Ignore);
        else parameter.HavePushItem = false;
        if (parameter.HavePushItem)
        {
            manager.TransitionState(StateType.Push);
        }

        SwitchTwoHand();
        if (parameter._playerDamageRange_R != null&& parameter._firstCombo)
        {
            manager.TransitionState(StateType.Attack);
        }
    }

    public void OnExit()
    {

    }
    public void ItemToPacket()
    {
        //按下切换按钮
        if (parameter._input.itemToPacket_L)
        {
            parameter._input.itemToPacket_L = false;
            var newPacketNum_L = (parameter.PacketNum_L + 1) % 2;
            //隐藏当前手上的物品，显示预备背包中的另一个物品
            if (parameter._playerBag_Left.gameObjects[parameter.PacketNum_L] != null)
            {
                parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].SetActive(false);
                parameter._playerBag_Left.copyGameObjects[parameter.PacketNum_L].SetActive(true);
            }

            if (parameter._playerBag_Left.gameObjects[newPacketNum_L] != null)
            {
                parameter._playerBag_Left.gameObjects[newPacketNum_L].SetActive(true);
                parameter._playerBag_Left.copyGameObjects[newPacketNum_L].SetActive(false);
                parameter._holdItem_L = true;
            }
            else parameter._holdItem_L = false;
            //清除手的状态
            parameter._medicineHP_L = null;
            parameter._playerDamageRange_L = null;
            parameter.PacketNum_L = newPacketNum_L;
            //重新获取状态
            if (parameter._playerBag_Left.gameObjects[parameter.PacketNum_L] != null)
            {
                parameter._playerDamageRange_L = parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange damageRange) ? damageRange : null;
                parameter._medicineHP_L = parameter._playerBag_Left.gameObjects[parameter.PacketNum_L].transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP) ? medicine_HP : null;
            }
            parameter._playerBag_Left.PacketNum = parameter.PacketNum_L;
        }
        if (parameter._input.itemToPacket_R)
        {
            parameter._input.itemToPacket_R = false;
            var newPacketNum_R = (parameter.PacketNum_R + 1) % 2;
            //隐藏当前手上的物品，显示预备背包中的另一个物品
            if (parameter._playerBag_Right.gameObjects[parameter.PacketNum_R] != null)
            {
                parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].SetActive(false);
                parameter._playerBag_Right.copyGameObjects[parameter.PacketNum_R].SetActive(true);
            }

            if (parameter._playerBag_Right.gameObjects[newPacketNum_R] != null)
            {
                parameter._playerBag_Right.gameObjects[newPacketNum_R].SetActive(true);
                parameter._playerBag_Right.copyGameObjects[newPacketNum_R].SetActive(false);
                parameter._holdItem_R = true;
            }
            else parameter._holdItem_R = false;
            parameter._medicineHP_R = null;
            parameter._playerDamageRange_R = null;
            //_holdItem_L = false;
            parameter.PacketNum_R = newPacketNum_R;
            //重新获取状态
            if (parameter._playerBag_Right.gameObjects[parameter.PacketNum_R] != null)
            {
                parameter._playerDamageRange_R = parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.TryGetComponent<PlayerDamageRange>(out PlayerDamageRange damageRange) ? damageRange : null;
                parameter._medicineHP_R = parameter._playerBag_Right.gameObjects[parameter.PacketNum_R].transform.TryGetComponent<Medicine_HP>(out Medicine_HP medicine_HP) ? medicine_HP : null;
            }
            parameter._playerBag_Right.PacketNum = parameter.PacketNum_R;
        }
    }
    private void SwitchTwoHand()
    {
        if (parameter._input.switchTwoHand && !parameter._isTwoHand)
        {
            //如果两个手都有东西放下左手的东西
            if (parameter._holdItem_L && parameter._holdItem_R)
            {
                parameter.LeftHandHoldPos.GetChild(0).transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                parameter.LeftHandHoldPos.GetChild(0).transform.parent = manager.transform.parent;
                parameter._playerDamageRange_L = null;
                parameter._holdItem_L = false;
            }
            parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 1);
            parameter._isTwoHand = true;
            parameter._animator.SetBool(parameter._animTwoHand, true);
            parameter._input.switchTwoHand = false;
        }
        else if (parameter._input.switchTwoHand && parameter._isTwoHand)
        {
            parameter._animator.SetLayerWeight(parameter._animLayerIDHand, 0);
            parameter._isTwoHand = false;
            parameter._animator.SetBool(parameter._animTwoHand, false);
            parameter._input.switchTwoHand = false;
        }
    }
}







