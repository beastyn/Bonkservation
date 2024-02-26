namespace UI
{
    using Gameplay.Buffs;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class SlotUI : MonoBehaviour
    {
        public static UnityAction<BuffSO> SlotActivatedEvent; 

        [SerializeField] Image buffImage;
        [SerializeField] Button buffActivationButton;
        BuffSO buffSO;

        void OnEnable()
        {
            this.buffActivationButton.onClick.AddListener(ActivateBuff);
        }

        public void InitBuffSlot(BuffSO buffSO)
        {
            this.buffSO = buffSO;
            this.buffImage.sprite = buffSO.BuffItemVisual;
            this.buffActivationButton.interactable = true;
        }

        void ActivateBuff()
        {
            this.buffSO.BuffAction();
            SlotActivatedEvent?.Invoke(this.buffSO);
        }

        public void ClearSlot()
        {
            this.buffSO = null;
            this.buffImage.sprite = null;
            this.buffActivationButton.interactable = false;
        }

    }
}