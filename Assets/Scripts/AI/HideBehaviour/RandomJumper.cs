namespace AI
{
    using DG.Tweening;
    using Gameplay;
    using Managers;
    using Player;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UI;
    using Unity.VisualScripting;
    using UnityEngine;

    public class RandomJumper : MonoBehaviour
    {
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] Damageable idolDamageReceiver;
        [SerializeField] GameObject idolFace;
        [SerializeField] SpriteRenderer faceSprite;
        [SerializeField] GameObject[] positions;

        [SerializeField] float appearTime = 0.5f;
        [SerializeField] float hideTime = 0.5f;
        [SerializeField] float moveBy = 4f;

        bool runSequence = true;
        bool canHide = true;
        bool isHideSequence = false;

        void OnEnable()
        {
            this.idolEnergy.NoEnergyEvent += OnNoEnergyEvent;
            PlayerEvents.LandHitEvent += OnLandHitEvent;

            this.runSequence = true;
            //this.idolFace.transform.localScale = Vector3.zero;
            this.idolFace.SetActive(false);
            StartCoroutine(this.SwitchFaceOn());
        }

        void OnDisable()
        {
            this.idolEnergy.NoEnergyEvent -= OnNoEnergyEvent;
            PlayerEvents.LandHitEvent -= OnLandHitEvent;
        }

        IEnumerator SwitchFaceOn()
        {
            while (this.runSequence)
            {
                yield return new WaitForSeconds(DifficultySettingsSO.GetRandomAppearDelay());
                var posAndSort = GetRandomPositionAndSort();
                DOTween.Kill(this.idolFace.transform.parent);

                this.idolFace.transform.parent.SetPositionAndRotation(posAndSort.Item1.transform.position, posAndSort.Item1.transform.rotation);
                this.faceSprite.sortingOrder = posAndSort.Item2 - 1;
                this.idolDamageReceiver.SetProtection(false);
                this.idolFace.SetActive(true);
                this.idolFace.transform.parent.DOBlendableMoveBy(this.idolFace.transform.parent.up * this.moveBy, this.appearTime).SetEase(Ease.OutBounce);
                StartCoroutine(this.SwitchFaceOff());
            }
        }

        IEnumerator SwitchFaceOff()
        {
            yield return new WaitForSeconds(DifficultySettingsSO.GetRandomStayTime());
            DOTween.Kill(this.idolFace.transform.parent);
            this.idolFace.transform.parent.DOBlendableMoveBy(-this.idolFace.transform.parent.up * this.moveBy, this.hideTime).SetEase(Ease.OutBounce).OnComplete(() => this.idolFace.gameObject.SetActive(false));
            this.SwitchFaceOn();
        }

        IEnumerator HideAndRestart()
        {
            this.isHideSequence = true;
            yield return new WaitForSecondsRealtime(this.hideTime);

            if (canHide)
            {
                DOTween.Kill(this.idolFace.transform.parent);
                this.idolFace.transform.parent.DOBlendableMoveBy(-this.idolFace.transform.parent.up * this.moveBy, this.hideTime).SetEase(Ease.OutBounce).OnComplete(() => this.idolFace.gameObject.SetActive(false));
                StartCoroutine(this.SwitchFaceOn());
                this.isHideSequence = false;
            }
        }

        (GameObject, int) GetRandomPositionAndSort()
        {
            var rand = UnityEngine.Random.Range(0, this.positions.Length);
            var obj = this.positions[rand];
            var order = obj.transform.parent.GetComponent<SpriteRenderer>()?.sortingOrder ?? 0;
            return (obj, order);
        }

        void OnLandHitEvent()
        {
            if (!this.isHideSequence)
            {
                StopAllCoroutines();
                this.idolDamageReceiver.SetProtection(true);
                StartCoroutine(this.HideAndRestart());
            }
        }

        void OnNoEnergyEvent()
        {
            this.runSequence = false;
            this.canHide = false;
            StopAllCoroutines();
        }

    }
}