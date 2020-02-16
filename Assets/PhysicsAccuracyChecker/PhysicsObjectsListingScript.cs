using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PhysicsAccuracyChecker
{
    public class PhysicsObjectsListingScript : MonoBehaviour
    {
        public bool ListOnUpdate;

        void Awake()
        {
            ListColliders("OnAwake");
        }

        void Start()
        {
            ListColliders("OnStart");
        }

        void Update()
        {
            if (ListOnUpdate)
            {
                ListColliders($"OnUpdate:{Time.frameCount}");
            }
        }

        private static void ListColliders(string description)
        {
            var colliders = FindObjectsOfType<Collider>();

            var sb = new StringBuilder();
            sb.AppendLine($"BartekPhysics:{description}: there are {colliders.Length} colliders on the scene");
            foreach (var aCollider in colliders)
            {
                sb.AppendLine($"Collider: {GetGameObjectPath(aCollider.gameObject)}" +
                              $" type:{aCollider.GetType()} isTrigger:{aCollider.isTrigger} position {aCollider.transform.position}");
            }

            Debug.Log(sb);
        }

        private static string GetGameObjectPath(GameObject go)
        {
            string path = go.transform.name;
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                path = go.transform.name + "/" + path;
            }

            return $"Scene:{go.scene.name} path:{path}";
        }
    }
}
