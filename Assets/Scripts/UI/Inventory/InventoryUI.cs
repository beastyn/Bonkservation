namespace UI
{
    using Gameplay;
    using Gameplay.Buffs;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] SlotUI[] slots;
        [SerializeField] GameObject inactiveLayer;

        void OnEnable()
        {
            InventorySO.InvChangeEvent += OnBuffAddedToInvEvent;
            SlotUI.SlotActivatedEvent += OnSlotActivatedEvent;
            BuffManager.BuffActivateEvent += OnBuffActivatedEvent;
            BuffManager.BuffDeactivateEvent += OnBuffDeactivatedEvent;

        }

        void OnDisable()
        {
            InventorySO.InvChangeEvent -= OnBuffAddedToInvEvent;
            SlotUI.SlotActivatedEvent -= OnSlotActivatedEvent;
            BuffManager.BuffActivateEvent -= OnBuffActivatedEvent;
            BuffManager.BuffDeactivateEvent -= OnBuffDeactivatedEvent;
        }

        void Start()
        {
            InventorySO.InventoryInit();
        }

        void OnBuffAddedToInvEvent(BuffSO buff)
        {
            foreach(var slot in this.slots)
                slot.ClearSlot();

            for (var i = 0; i < InventorySO.Slots.Count ; i++)
            {
                if (i < this.slots.Length)
                    this.slots[i].InitBuffSlot(InventorySO.Slots[i]);
            }
        }

        void OnSlotActivatedEvent(BuffSO buffSO) => InventorySO.RemoveBuff(buffSO);

        void OnBuffActivatedEvent(BuffSO buffSO) { if (BuffManager.HaveActiveBuff()) this.inactiveLayer.SetActive(true); }
        void OnBuffDeactivatedEvent(BuffSO buffSO) { if (!BuffManager.HaveActiveBuff()) this.inactiveLayer.SetActive(false); }
    }
}