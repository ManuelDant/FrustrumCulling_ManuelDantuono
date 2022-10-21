using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustrum;
    Collider collider;

    public enum TestPlanesResults
    {
        Inside = 0,
        Outside
    }
    private static TestPlanesResults testResult;

    public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Bounds bounds)
    {

        var min = bounds.min;
        var max = bounds.max;

        return TestPlanesAABBInternalFast(planes, ref min, ref max);
    }

    public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Vector3 boundsMin, ref Vector3 boundsMax)
    {
        Vector3 vmin, vmax;

        testResult = TestPlanesResults.Inside;

        for (int planeIndex = 0; planeIndex < planes.Length; planeIndex++)
        {
            var normal = planes[planeIndex].normal;
            var planeDistance = planes[planeIndex].distance;

            // Eje X
            if (normal.x < 0)
            {
                vmin.x = boundsMin.x;
                vmax.x = boundsMax.x;
            }
            else
            {
                vmin.x = boundsMax.x;
                vmax.x = boundsMin.x;
            }

            // Eje Y
            if (normal.y < 0)
            {
                vmin.y = boundsMin.y;
                vmax.y = boundsMax.y;
            }
            else
            {
                vmin.y = boundsMax.y;
                vmax.y = boundsMin.y;
            }

            // Eje Z
            if (normal.z < 0)
            {
                vmin.z = boundsMin.z;
                vmax.z = boundsMax.z;
            }
            else
            {
                vmin.z = boundsMax.z;
                vmax.z = boundsMin.z;
            }

            var dot1 = normal.x * vmin.x + normal.y * vmin.y + normal.z * vmin.z;
            if (dot1 + planeDistance < 0)
                return testResult = TestPlanesResults.Outside;

          
        }

        return testResult;
    }

    void Start()
    {
        camera = Camera.main;
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    
    void Update()
    {
        var bounds = collider.bounds;

        cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(camera);
        TestPlanesAABBInternalFast(cameraFrustrum, ref bounds);

        if (testResult == TestPlanesResults.Inside)
        {
            renderer.sharedMaterial.color = Color.green;
        }
        else
        {
            renderer.sharedMaterial.color = Color.clear;
        }
        
    }
}
