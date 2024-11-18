using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FlipCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
    }

    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == mainCamera)
        {
            camera = mainCamera;
            camera.ResetWorldToCameraMatrix();
            camera.ResetProjectionMatrix();
            camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3(1, 1, 1));
            Debug.Log("End Camera : " + camera.name);
        }

    }

    private void OnEndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == mainCamera)
            {
                GL.invertCulling = false;
                Debug.Log("End Frame " + cameras[i].name);
                
            }
        }
    }

    private void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == mainCamera)
            {
                GL.invertCulling = true;
                Debug.Log("Begin Frame " + cameras[i].name);
            }
        }
    }


    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == mainCamera)
        {
            camera = mainCamera;
            camera.ResetWorldToCameraMatrix();
            camera.ResetProjectionMatrix();
            camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
            Debug.Log("Begin Camera : " + camera.name);
        }

    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
        RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
    }
}
