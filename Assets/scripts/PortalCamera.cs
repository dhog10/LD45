using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PortalCamera : MonoBehaviour
{
    private Portal portal;

    private void Start()
    {
        portal = transform.parent.GetComponent<Portal>();

        RenderPipeline.beginFrameRendering += PreCameraRender;
    }

    private void PreCameraRender(Camera[] cameras)
    {
        portal.UpdatePortal();
    }
}
