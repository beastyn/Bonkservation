namespace UI
{
    using Gameplay.Buffs;
    using Player;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class SlotUI : MonoBehaviour
    {
        public static UnityAction<BuffSO> SlotActivatedEvent;

        [SerializeField] Image buffImage;
        [SerializeField] float slotNum;
        BuffSO buffSO;

        void OnEnable() => PlayerBonkInputController.SlotButton += OnSlotButton;
        void OnDisable() => PlayerBonkInputController.SlotButton -= OnSlotButton;

        public void InitBuffSlot(BuffSO buffSO)
        {
            this.buffSO = buffSO;
            this.buffImage.sprite = buffSO.BuffItemVisual;
        }

        void OnSlotButton(float slotButton)
        {
            if (slotButton != slotNum) return;
            this.buffSO.BuffAction();
            SlotActivatedEvent?.Invoke(this.buffSO);
        }

        public void ClearSlot()
        {
            this.buffSO = null;
            this.buffImage.sprite = null;
        }

    }
}