using UnityEngine;

namespace Utils
{
    public class SoftBodyData : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] Material squashMaterial;
        [SerializeField] float deltaTime = 3f;

        float startTime;
        float currentTime;
       

        void Awake()
        {
            squashMaterial.SetVector("_ContactPoint", new Vector3(900f, 900f, 900f));
            squashMaterial.SetFloat("_ContactTime", float.PositiveInfinity);
            this.startTime = Time.realtimeSinceStartup;
        }

        void Update()
        {
            Vector3 velocity = rb.linearVelocity;
            squashMaterial.SetVector("_Velocity", velocity);
        }

        void OnCollisionEnter(Collision collision)
        {
            this.currentTime = Time.realtimeSinceStartup;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (this.currentTime - this.startTime > this.deltaTime)
                {                    
                    squashMaterial.SetVector("_ContactPoint", this.transform.InverseTransformPoint(contact.point));
                    squashMaterial.SetFloat("_ContactTime", Time.time);
                    Debug.Log($"{contact.otherCollider.gameObject}");
                    this.startTime = Time.realtimeSinceStartup;
                }               

                
            }
        }

   /*      void OnCollisionExit(Collision collision)
        {
            squashMaterial.SetVector("_ContactPoint", new Vector3(900f,900f,900f));
            squashMaterial.SetFloat("_ContactTime", Time.time);

        }*/
    }
}