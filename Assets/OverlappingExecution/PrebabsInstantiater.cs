using System.Linq;
using UnityEngine;

public class PrebabsInstantiater : MonoBehaviour
{
    public int InstancesCount;
    public GameObject Prefab;

    void Start()
    {
        foreach (var i in Enumerable.Range(0, InstancesCount))
        {
            Instantiate(Prefab, new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.identity, transform);
        }
    }
}