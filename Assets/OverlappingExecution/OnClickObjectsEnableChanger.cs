using UnityEngine;

public class OnClickObjectsEnableChanger : MonoBehaviour
{
    public GameObject Object;
    public KeyCode Code;

    void Update()
    {
        if (Input.GetKeyDown(Code))
        {
            Object.SetActive(!Object.activeInHierarchy);
        }
    }

}