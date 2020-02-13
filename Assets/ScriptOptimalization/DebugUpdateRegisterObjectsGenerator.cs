using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BartekC.ScriptOptimalization
{
    public class DebugUpdateRegisterObjectsGenerator : MonoBehaviour
    {
        public int ObjectsToGenerateCount;
        public float TimeBetweenUpdates;
        public float TimeBetweenLateUpdates;
        public DebugUpdateRegisterObject Prefab;

        

        private void Start()
        {
            Enumerable.Range(0, ObjectsToGenerateCount).ToList().ForEach(i =>
            {
                var o = GameObject.Instantiate<DebugUpdateRegisterObject>(Prefab, transform);
                o.TimeBetweenLateUpdates = TimeBetweenLateUpdates;
                o.TimeBetweenUpdates = TimeBetweenUpdates;
            });
        }
    }
}
