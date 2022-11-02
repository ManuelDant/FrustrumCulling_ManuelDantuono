using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustrum;
    Collider collider;


    //Calcular el frustrum de la camara con la posicion, la direccion normalizada, el aspect ratio y la distancia
    public static Plane[] CalculateFrustum(Vector3 origin, Vector3 direction,
     float fovRadians, float viewRatio, float distance)
    {
        //Calculo de la distancia y lados del frustrum
        Vector3 nearCenter = origin + direction * 0.3f;
        Vector3 farCenter = origin + direction * distance;
        Vector3 camRight = Vector3.Cross(direction, Vector3.up) * -1;
        Vector3 camUp = Vector3.Cross(direction, camRight);

        //Calculo del ancho y largo del frustrum.
        float nearHeight = 2 * Mathf.Tan(fovRadians / 2) * 0.3f;
        float farHeight = 2 * Mathf.Tan(fovRadians / 2) * distance;
        float nearWidth = nearHeight * viewRatio;
        float farWidth = farHeight * viewRatio;

        //Calculo de los 6 planos
        Vector3 farTopLeft = farCenter + camUp * (farHeight * 0.5f) - camRight * (farWidth * 0.5f);
        Vector3 farBottomLeft = farCenter - camUp * (farHeight * 0.5f) - camRight * (farWidth * 0.5f);
        Vector3 farBottomRight = farCenter - camUp * (farHeight * 0.5f) + camRight * (farWidth * 0.5f);
        Vector3 nearTopLeft = nearCenter + camUp * (nearHeight * 0.5f) - camRight * (nearWidth * 0.5f);
        Vector3 nearTopRight = nearCenter + camUp * (nearHeight * 0.5f) + camRight * (nearWidth * 0.5f);
        Vector3 nearBottomRight = nearCenter - camUp * (nearHeight * 0.5f) + camRight * (nearWidth * 0.5f);

        Plane[] planes = {
             new Plane(nearTopLeft,farTopLeft,farBottomLeft),
             new Plane(nearTopRight,nearBottomRight,farBottomRight),
             new Plane(farBottomLeft,farBottomRight,nearBottomRight),
             new Plane(farTopLeft,nearTopLeft,nearTopRight),
             new Plane(nearBottomRight,nearTopRight,nearTopLeft),
             new Plane(farBottomRight,farBottomLeft,farTopLeft)};
        return planes;
    }


    //Enum para identificar si el objeto esta dentro o fuera del Frustrum
    public enum TestPlanesResults
    {
        Inside = 0,
        Outside
    }
    private static TestPlanesResults testResult;

    //Calculo del minimo y maximo del objeto
    public static TestPlanesResults TestPlanesAABBInternalFast(Plane[] planes, ref Bounds bounds)
    {

        var min = bounds.min;
        var max = bounds.max;

        return TestPlanesAABBInternalFast(planes, ref min, ref max);
    }

    //Retorna si el objeto esta dentro o fuera de los planos calculando el minimo y maximo de los lados del objeto en cada eje.
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

        //Fov de la camara en radianes
        float cameraFovRadians = camera.fieldOfView * Mathf.Deg2Rad;

        //Calcular Frustrum
        cameraFrustrum = CalculateFrustum(camera.transform.position, camera.transform.forward.normalized, cameraFovRadians, camera.aspect, camera.farClipPlane );    

        //Calcular los collider del objeto con los planos del Frustrum.
        TestPlanesAABBInternalFast(cameraFrustrum, ref bounds);

        //Si el objeto esta dentro del frustrum sera del color verde, de lo contrario sera transparente (negro).
        if (testResult == TestPlanesResults.Inside)
        {
            renderer.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            renderer.GetComponent<MeshRenderer>().enabled = false;
        }

    }
}
