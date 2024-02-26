namespace Gameplay
{
    using Gameplay.Buffs;
    using BonkUtils;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Unity.VisualScripting;

    [CreateAssetMenu(fileName = "Inventory", menuName = "Bonk/Inventory")]
    public class InventorySO : ScriptableObject
    {
        public static UnityAction<BuffSO> InvChangeEvent;
        [SerializeField] static int inventorySize = 3;

        static List<BuffSO> slots;
        public static List<BuffSO> Slots => slots;

        public static void InventoryInit()
        {
            if (slots == null) slots = new();
            else               slots.Clear();
        }

        public static void AddBuff(BuffSO buff)
        {
            slots.Insert(0, buff);
            if (slots.Count > inventorySize) slots.RemoveAt(inventorySize);
            InvChangeEvent?.Invoke(buff);
        }

        public static void RemoveBuff(BuffSO buff)
        {
            slots.Remove(buff);
            InvChangeEvent?.Invoke(buff);
        }

    }
}
