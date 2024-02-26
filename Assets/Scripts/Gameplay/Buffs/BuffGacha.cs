namespace Gameplay.Buffs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;
    using Unity.VisualScripting;

    public class BuffGacha : MonoBehaviour,IPointerClickHandler
    {
        public static UnityAction<BuffSO> GotGachaItemEvent;

        [SerializeField] BuffGachaSO buffGachaSO;
        [SerializeField] SpriteRenderer buffGachaItem;
        [SerializeField] BuffItemBehaviour buffItemSetting;

        void Start ()
        {
            this.buffGachaSO.PrepareGacha();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == 0 && this.buffGachaSO.CanActivateGacha)
            {
                this.buffGachaSO.ActivateGacha();
                var buffItem = BuffManager.GetRandomBuff();
                this.buffGachaItem.sprite = buffItem.BuffItemVisual;
                this.buffItemSetting.SetBuff(buffItem);
                this.buffGachaItem.gameObject.SetActive(true);
                GotGachaItemEvent?.Invoke(buffItem);
            }
        }
    }
}
