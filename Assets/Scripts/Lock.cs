using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{

    public class Lock : MonoBehaviour
    {
        public bool Range;
        public Vector3 size;
        public float _forward;
        public LayerMask targetLayerMask;
        public bool isLockOn;
        public int newTargetNum;
        public bool switchTarget;

        private StarterAssetsInputs _input;
        private Vector3 _pos;
        private float _hight;
        public Collider target;
        public Collider[] targets;
        // Start is called before the first frame update
        void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();
            size = new Vector3(8, 7.5f, 8);
            _forward = 6;
        }

        // Update is called once per frame
        void Update()
        {
            SwitchLockTarget();
            LookRange();
            LockTarget();
        }
        public void SwitchLockTarget()
        {
            var value = _input.scrollWheel;
            if (value == 0) switchTarget = true;
            if (isLockOn)
            {
                
                if (value != 0 && targets.Length>1 && switchTarget)
                {
                    if (value > 0)
                    {
                        target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0;
                        newTargetNum++;
                        if (newTargetNum > targets.Length-1)
                        {
                            newTargetNum=0;
                            target = targets[newTargetNum];
                        }
                        else target = targets[newTargetNum];
                        target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                        switchTarget = false;
                    }
                    else
                    {
                        target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0;
                        newTargetNum--;
                        if (newTargetNum < 0)
                        {
                            newTargetNum = targets.Length - 1;
                            target = targets[newTargetNum];
                        }
                        else target = targets[newTargetNum];
                        target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                        switchTarget=false;
                    }
                }
            }
            
        }
        public void LockTarget()
        {
            if (target == null) isLockOn = false;
            if (_input.lockOn)
            {
                if (isLockOn)
                {
                    //取消锁定
                    isLockOn = false;
                    target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 0;
                }
                else
                {
                    //锁定
                    if (targets.Length>0) 
                    {
                        newTargetNum = 0;
                        target = targets[0];
                        target.transform.Find("StateCanva").Find("Frame").gameObject.gameObject.GetComponent<CanvasGroup>().alpha = 1;
                        isLockOn = true;
                    }
                }
                _input.lockOn = false;

            }
    }
        public void LookRange()
        {
            _pos = transform.position + transform.up * 0.9f + transform.forward * _forward;
            targets = Physics.OverlapSphere(_pos, _forward, targetLayerMask);
        }

        private void OnDrawGizmosSelected()
        {
            if (Range)
            {
                Gizmos.DrawSphere(_pos, _forward);
            }
        }
    } 
}

