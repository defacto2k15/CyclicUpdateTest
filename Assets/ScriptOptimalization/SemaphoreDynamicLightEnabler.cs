using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(SemaphoreDynamicLight))]
public class SemaphoreDynamicLightEnabler : MonoBehaviour
{
    private SemaphoreDynamicLight _light;

    void Awake()
    {
        _light = GetComponent<SemaphoreDynamicLight>();

        CyclicUpdateRegistry.RegisterUpdateMethod(MyUpdate, this, 0.5f);
    }

    void OnDestroy()
    {
        CyclicUpdateRegistry.RemoveUpdateMethod(MyUpdate);
    }

    void MyUpdate()
    {
        if(MainCameraStaticReference.StaticMainCamera != null)
        {
            var distanceToPlayer = Vector3.Distance(MainCameraStaticReference.StaticMainCamera.transform.position,
                transform.position);
            _light.enabled = (distanceToPlayer * 1.25f < _light.maxDistance);
        }

    }
}