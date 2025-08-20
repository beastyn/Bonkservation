using UnityEngine;

namespace Utils
{
    public class SoftBodyData : MonoBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] Material squashMaterial;
        [SerializeField] Material softMaterialOutline;
        [SerializeField] float deltaTime = 3f;
        [SerializeField] Renderer renderer;

        float startTime;
        float currentTime;
        MaterialPropertyBlock materialPropertyBlock;

        static readonly int contactPoint = Shader.PropertyToID("_ContactPoint");
        static readonly int contactTime = Shader.PropertyToID("_ContactTime");
        static readonly int bonkTrigger = Shader.PropertyToID("_BonkTrigger");

        void Awake()
        {
            this.renderer ??= GetComponent<Renderer>();
            this.materialPropertyBlock = new MaterialPropertyBlock();

            this.renderer.GetPropertyBlock(this.materialPropertyBlock);

            this.materialPropertyBlock.SetVector(contactPoint, new Vector3(900f, 900f, 900f));
            this.materialPropertyBlock.SetFloat(contactTime, float.PositiveInfinity);
            this.materialPropertyBlock.SetFloat(bonkTrigger, 0f);

            this.renderer.SetPropertyBlock(this.materialPropertyBlock);

            this.startTime = Time.realtimeSinceStartup;
        }

        void OnCollisionEnter(Collision collision)
        {
            this.currentTime = Time.realtimeSinceStartup;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (this.currentTime - this.startTime > this.deltaTime)
                {        
                    this.renderer.GetPropertyBlock(this.materialPropertyBlock);
                    this.materialPropertyBlock.SetVector(contactPoint, this.transform.InverseTransformPoint(contact.point));
                    this.materialPropertyBlock.SetFloat(contactTime, Time.time); 

                    Debug.Log($"{contact.otherCollider.gameObject}");

                    this.renderer.SetPropertyBlock(this.materialPropertyBlock);
                    this.startTime = Time.realtimeSinceStartup;
                }                               
            }
        }

        public void ApplyUpToDownForce()
        {
            this.renderer.GetPropertyBlock(this.materialPropertyBlock);

            this.materialPropertyBlock.SetFloat(bonkTrigger, 1f);
            this.materialPropertyBlock.SetVector(contactPoint, new Vector3(0, 1, 0));
            this.materialPropertyBlock.SetFloat(contactTime, Time.time);

            this.renderer.SetPropertyBlock(this.materialPropertyBlock);
        }
    }
}