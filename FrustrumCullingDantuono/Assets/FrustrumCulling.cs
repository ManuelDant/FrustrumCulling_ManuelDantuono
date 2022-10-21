using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustum;
    Collider collider;

    void Start()
    {
        camera = Camera.main;
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }


    void Update()
    {
        var bounds = collider.bounds;

        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            renderer.sharedMaterial.color = Color.green;
        }
        else
        {
            renderer.sharedMaterial.color = Color.red;
        }
    }
}
