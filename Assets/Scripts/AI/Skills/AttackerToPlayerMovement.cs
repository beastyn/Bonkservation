namespace AI.Skills
{
    using Gameplay;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AttackerToPlayerMovement : MonoBehaviour
    {

        [SerializeField] float timeToReach = 1f;
        [SerializeField] float impactScale = 4f;
        [SerializeField] float compensation = 0.8f;

        GameObject pawToFollow;

        //Vector3 originalPawPosition;
        Vector3 originalLocalScale;
        float speed;

        void OnEnable()
        {
            this.pawToFollow = PlayerSOHolder.CurrentPaw;
            this.originalLocalScale = this.transform.localScale;

            var direction = this.pawToFollow.transform.position - this.transform.position;
            var originalDistance = direction.magnitude;
            this.speed = originalDistance / this.timeToReach;

            this.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }

        void OnDisable()
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localScale = this.originalLocalScale;

        }

        void Update()
        {
            this.transform.position += transform.up * Time.deltaTime * this.speed;

            if (this.transform.localScale.x < this.impactScale) this.transform.localScale += (Vector3.right + Vector3.up) * (this.impactScale / this.timeToReach) * Time.deltaTime * this.compensation;
        }
    }
}