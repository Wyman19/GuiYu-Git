using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


    public class ScenesManage : MonoBehaviour
    {

        public bool firstEnterScene;
        public Transform playStart;
        public PlayerInput palyerInputs;
        public Animator animator;
        public GameObject StateCanva;

        private void OnEnable()
        {
            
        }
        private void Awake()
        {
            ThirdPersonController.Instance.transform.TryGetComponent<PlayerInput>(out palyerInputs);
            ThirdPersonController.Instance.TryGetComponent<Animator>(out animator);
            StateCanva=ThirdPersonController.Instance.transform.Find("StateCanva").gameObject;
            animator.enabled = false;
            ThirdPersonController.Instance.enabled = false;
            ThirdPersonController.Instance.gameObject.transform.position = playStart.position;
            Debug.Log(playStart.position);
            Debug.Log("设置角色初始点" + ThirdPersonController.Instance.gameObject.transform.position);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            

        }

        // Update is called once per frame
        void Update()
        {
            SetPlayStart();
            ManinMenu();
        }

        void SetPlayStart()
        {
            if (firstEnterScene)
            {

                
                if (ThirdPersonController.Instance.gameObject.transform.position == playStart.position)
                {
                    firstEnterScene = false;
                    animator.enabled = true;
                    ThirdPersonController.Instance.enabled = true;
                    //this.enabled = false;
                    return;
                }
            }
        }
        void ManinMenu()
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                Debug.Log("进入Main Menu");
                Debug.Log(palyerInputs.name);
                palyerInputs.enabled = false;
                StateCanva.SetActive(false);
            }else
            {
                palyerInputs.enabled =true;
                StateCanva.SetActive(true);
            }
        }

    }


