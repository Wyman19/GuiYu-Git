using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    public class Stamina : MonoBehaviour
    {
        public float MaxStamina = 100;
        public float ResidueRtamina = 100;
        public float ReplyStaminaSpeed = 10;
        public float ReduceStaminaSpeed = 6;
        public bool isReplyToMaxStamina;
        public bool BeginReplyToMaxStamina;
        public bool isReduceStamina;
        public bool BeginReduceStamina;
        public Image StaminaImage;

        private void Start()
        {
            StaminaImage = transform.Find("StateCanva").Find("Frame").Find("State").Find("stamina").gameObject.GetComponent<Image>();
        }
        private void Update()
        {
            if (ResidueRtamina > 100) ResidueRtamina = 100;
            if (ResidueRtamina < 0) ResidueRtamina = 0;
            StaminaImage.fillAmount = ResidueRtamina / 100;
        }
        public void ClimbStamina(float WallHight)
        {
            BeginReplyToMaxStamina = true;
            isReplyToMaxStamina = false;
            ResidueRtamina -= WallHight;
        }
        public void RollStamina()
        {
            BeginReplyToMaxStamina = true;
            isReplyToMaxStamina = false;
            ResidueRtamina -= 10;
        }
        public void JumpStamina()
        {
            BeginReplyToMaxStamina = true;
            isReplyToMaxStamina = false;
            ResidueRtamina -= 10;
        }
        public void AttackStamina()
        {
            BeginReplyToMaxStamina = true;
            isReplyToMaxStamina = false;
            ResidueRtamina -= 10;
        }
        public IEnumerator ReplyToMaxStamina()
        {
            if (BeginReplyToMaxStamina)
            {
                BeginReplyToMaxStamina = false;
                var num = MaxStamina - ResidueRtamina;
                for (int i = 0; i < num; i++)
                {
                    yield return new WaitForSeconds(1 / ReplyStaminaSpeed);
                    if (!isReplyToMaxStamina) yield break;
                    ResidueRtamina += 1;
                }
            }
        }
        public IEnumerator ReduceToMaxStamina()
        {
            if (BeginReduceStamina)
            {
                isReplyToMaxStamina = false;
                BeginReplyToMaxStamina = true;
                BeginReduceStamina = false;
                var num = ResidueRtamina;
                for (int i = 0; i < num; i++)
                {
                    yield return new WaitForSeconds(1 / ReduceStaminaSpeed);
                    if (isReplyToMaxStamina) yield break;
                    ResidueRtamina -= 1;
                }
            }
        }
    }

