using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.OverlappingExecution
{
    public class DummyMaterialBlitter : MonoBehaviour
    {
        public int Repeats;
        public Material material;
        private RenderTexture _src;
        private RenderTexture _dst;

        void Awake()
        {
            _src = RenderTexture.GetTemporary(1024,1024);
            _dst = RenderTexture.GetTemporary(1024,1024);
        }

        void Update()
        {
            for (int i = 0; i < Repeats; i++)
            {
                Graphics.Blit(_src, _dst, material);
            }
        }
    }
}
