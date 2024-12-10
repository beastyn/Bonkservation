namespace AI.Skills
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class StraightAttackerMovement : MonoBehaviour
    {
        [SerializeField] float timeToReach = 3f;
        [SerializeField] float impactScale = 4f;
        [SerializeField] bool rightDirection = true;

        Vector3 originalLocalScale;
        float speed;

        void OnEnable()
        {
            //this.pawToFollow = PlayerSOHolder.CurrentPaw;
            this.originalLocalScale = this.transform.localScale;
            this.transform.localScale = new Vector3(this.impactScale, this.impactScale, this.impactScale);
            //var direction = this.transform.up;
            var originalDistance =Screen.width;
            this.speed = originalDistance / this.timeToReach;

            //this.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }

        void OnDisable()
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localScale = this.originalLocalScale;

        }

        void Update()
        {
            this.transform.position += (this.rightDirection? transform.right : - transform.right) * Time.deltaTime * this.speed/100;

            //if (this.transform.localScale.x < this.impactScale) this.transform.localScale += (Vector3.right + Vector3.up) * (this.impactScale / this.timeToReach) * Time.deltaTime;
        }
    }
}