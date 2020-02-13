using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
public class MainCameraStaticReference : MonoBehaviour
{
    public static Camera StaticMainCamera;

    void Awake()
    {
        Assert.IsTrue(StaticMainCamera==null);
        StaticMainCamera = GetComponent<Camera>();
    }

    void OnDestroy()
    {
        StaticMainCamera = null;
    }
}