using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdolAIMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] IdolInfoSO idolInfo;


    public bool IsWalking = false;

    //Transform targetAgent;
    //void OnEnable()  => this.vodKillers.VodKillerChanged += OnVodKillerChanged;
    //void OnDisable() => this.vodKillers.VodKillerChanged -= OnVodKillerChanged;

    void Start()
    {
        this.agent.updateUpAxis = false;
        this.agent.updateRotation = false;
        this.agent.speed = this.idolInfo.CurrentWalkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //if(this.targetAgent == null) return;

        //this.agent.SetDestination(this.targetAgent.position);

        var vel = agent.velocity;
        vel.z = 0;

        if (vel != Vector3.zero)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(Vector3.forward,vel), this.idolInfo.RotSpeed * Time.deltaTime);
        }

        this.IsWalking = this.agent.velocity != Vector3.zero;
    /*
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
*/

            /*var lookAt = this.transform.InverseTransformPoint(this.targetAgent.position);
            var angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90;
            if(Mathf.Abs(angle) > 4f) this.transform.Rotate(0, 0, Mathf.LerpAngle(this.transform.rotation.z, angle, this.rotSpeed * Time.deltaTime));*/
    }

   /* void OnVodKillerChanged(VodKillerSO vodKiller)
    {
        this.targetAgent = vodKiller.Killer;
    }*/
}
