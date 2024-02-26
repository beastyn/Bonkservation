namespace Gameplay.Buffs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class BuffItemBehaviour : MonoBehaviour, IPointerClickHandler
    {
        BuffSO buffSO;

        public void SetBuff(BuffSO buffSO) => this.buffSO = buffSO;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == 0)
            {
                InventorySO.AddBuff(buffSO);
                this.gameObject.SetActive(false);
            }
        }

    }
}
