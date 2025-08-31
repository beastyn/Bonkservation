using UnityEngine;

namespace Utils
{
    public class AgentOutliner : MonoBehaviour
    {
        [SerializeField] uint outlineBitIndex = 8; // use the same bit you set in Render Objects
        [SerializeField] Color outlineColor;
        [SerializeField] float outlineThickness = 0.1f;
        Renderer renderer;
        MaterialPropertyBlock mpb;

        public string normalLayer = "Idol";
        public string outlineLayer = "IdolWithOutline";
        uint OutlineBit => 1u << (int)outlineBitIndex;

        static readonly int ColorID = Shader.PropertyToID("_OutlineColor");
        static readonly int ThickID = Shader.PropertyToID("_OutlineThickness");

        void Awake() { renderer = GetComponent<Renderer>(); mpb = new MaterialPropertyBlock(); }

        void Start()
        {
            if (renderer == null)
            {
                Debug.LogError("AgentOutliner requires a Renderer component.");
                return;
            }
            SetEnabled(false);
            SetStyle(this.outlineColor, outlineThickness);
        }

        /* public void SetEnabled(bool on)
         {
             if (on) r.renderingLayerMask |= OutlineBit;
             else r.renderingLayerMask &= ~OutlineBit;
         }*/

        public void SetEnabled(bool on)
        {
            int layer = LayerMask.NameToLayer(on ? outlineLayer : normalLayer);
            renderer.gameObject.layer = layer; // moves ONLY this renderer GO
        }

        public void SetStyle(Color color, float thickness)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetColor(ColorID, color);
            mpb.SetFloat(ThickID, thickness);
            renderer.SetPropertyBlock(mpb);
        }
    }
}
