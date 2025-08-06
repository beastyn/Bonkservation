using UnityEngine;
using UnityEngine.AI;

namespace Agent
{
    using BrainDesigner.Scripts;
    using Gameplay;

    public class AgentAnimatorController : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] BrainDesigner brainDesigner;
        [SerializeField] float turnThreshold;
        [SerializeField] float scale;

        SOEnergy soEnergy;
        float velocity = 0.0f;
        int velocityHash;
        int turnHash;
        int isBonkedHash;
        void Awake()
        {
            this.animator ??= this.GetComponent<Animator>();
            this.animator ??= this.GetComponentInChildren<Animator>();
            this.agent ??= this.GetComponent<NavMeshAgent>();
            this.soEnergy = this.GetComponent<AgentManagersAndData>().AgentEnergy;
            this.velocityHash = Animator.StringToHash("Velocity");
            this.turnHash = Animator.StringToHash("Turn");
            this.isBonkedHash = Animator.StringToHash("IsBonked");
        }

        void OnEnable()
        {
            this.brainDesigner.ActionChangeEvent += OnActionChangeEvent;
            this.soEnergy.EnergyChangeEvent += OnEnergyChangeEvent;
        }

        void OnDisable()
        {
            this.brainDesigner.ActionChangeEvent -= OnActionChangeEvent;
            this.soEnergy.EnergyChangeEvent -= OnEnergyChangeEvent;
        }

        void Start()
        {
            this.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
        }


        // Update is called once per frame
        void Update()
        {
            //forward
            this.velocity = this.agent.velocity.magnitude;
            animator.SetFloat(velocityHash, velocity);

            //rotation
            Vector3 desiredDirection = this.agent.steeringTarget - transform.position;
            var angleDifference = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            if (angleDifference > this.turnThreshold)
                this.animator.SetFloat(this.turnHash, 1);
            else if (angleDifference < -this.turnThreshold)
                this.animator.SetFloat(this.turnHash, -1);
            else
                this.animator.SetFloat(this.turnHash, 0);


        }

        void OnActionChangeEvent(string previousActionName, string newActionName)
        {
            if (!string.IsNullOrEmpty(previousActionName)) 
                this.animator.SetBool(previousActionName, false);
            if (!string.IsNullOrEmpty(newActionName))
                this.animator.SetBool(newActionName, true);
        }

        void OnEnergyChangeEvent(float energy, bool isRestoring)
        {
            if (!isRestoring) 
                this.animator.SetTrigger(isBonkedHash);
            //this.animator.ResetTrigger(isBonkedHash);
        }

    }
}