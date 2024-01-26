using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraRender : MonoBehaviour
{
    public Material material;

    void Start()
    {
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
 
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, material);
    }
}
