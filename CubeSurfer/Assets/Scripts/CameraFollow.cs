using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform[] targetTransforms;
   
    private CinemachineVirtualCamera camera;
    float minZoom = 0;
    float maxZoom = 120;
    public float zoomDamp = 25;
    private float smooth = 2;
    private void Awake()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
        minZoom = camera.m_Lens.FieldOfView;
    }
    private void LateUpdate()
    {
        Zoom();
    }
    private void Zoom()
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, GetDistance() / zoomDamp);
        camera.m_Lens.FieldOfView = Mathf.Lerp(camera.m_Lens.FieldOfView, zoom, Time.deltaTime* smooth);
    }

    float GetDistance()
    {
        var targetBounds = new Bounds(targetTransforms[0].position, Vector3.zero);
        for (int i = 0; i < targetTransforms.Length; i++)
        {
            targetBounds.Encapsulate(targetTransforms[i].position);
        }
        return targetBounds.size.y;
    }
}
