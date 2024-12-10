namespace UI.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using TMPro;
    using UnityEngine;

    public class TextWObble : MonoBehaviour
    {
        TMP_Text textMesh;
        [SerializeField] float a11 = 2f;
        [SerializeField] float a12 = 3f;
        [SerializeField] float b11 = 10f;
        [SerializeField] float b12 = 11f;
        [SerializeField] float c11 = 10f;
        [SerializeField] float c12 = 11f;

        [SerializeField] float a21 = 2f;
        [SerializeField] float a22 = 3f;
        [SerializeField] float b21 = 10f;
        [SerializeField] float b22 = 11f;
        [SerializeField] float c21 = 10f;
        [SerializeField] float c22 = 11f;

        List<float> a1 = new();
        List<float> b1 = new();
        List<float> c1 = new();
        List<float> a2 = new();
        List<float> b2 = new();
        List<float> c2 = new();
          Mesh mesh;

        Vector3[] vertices;

        void Awake()
        {
            textMesh = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            textMesh.ForceMeshUpdate();

            for (int i = 0; i < textMesh.textInfo.characterCount; i++)
            {
                this.a1.Insert(i,Random. Range(a11, a12));
                this.b1.Insert(i, Random.Range(b11, b12));
                this.c1.Insert(i, Random.Range(c11, c12));

                this.a2.Insert(i, Random.Range(a21, a22));
                this.b2.Insert(i, Random.Range(b21, b22));
                this.c2.Insert(i, Random.Range(c21, c22));
            }
        }

        void Update()
        {
            textMesh.ForceMeshUpdate();
            var textInfo = this.textMesh.textInfo;

            mesh = textMesh.mesh;
            vertices = mesh.vertices;

            for (int i = 0; i < textMesh.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo c = textInfo.characterInfo[i];

                if (!c.isVisible) continue;

                var verts = textInfo.meshInfo[c.materialReferenceIndex].vertices;
                for (var j = 0; j < 4; j++)
                {
                    var origin = verts[c.vertexIndex + j];
                    if(j==0 || j ==2) verts[c.vertexIndex + j] = origin + new Vector3(this.a1[i]*Mathf.Sin(Time.time * this.b1[i] + origin.x * 0.01f) * this.c1[i], this.a1[i] *Mathf.Cos(Time.time * this.b1[i] + origin.y * 0.01f) * this.c1[i], 0);
                    else                 verts[c.vertexIndex + j] = origin + new Vector3(this.a2[i] *Mathf.Sin(Time.time * this.b2[i] + origin.x * 0.01f) * this.c2[i], this.a2[i] *Mathf.Cos(Time.time * this.b2[i] + origin.y * 0.01f) * this.c2[i], 0);
                }

                /*int index = c.vertexIndex;

                Vector3 offset = Wobble(Time.time + i);
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;*/
            }

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textMesh.UpdateGeometry(meshInfo.mesh, i);
            }
    /*        mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);*/
        }

        Vector2 Wobble(float time)
        {
            return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));
        }
    }
}
