using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.OverlappingExecution
{
    [RequireComponent(typeof(Camera))]
    public class DummyImageEffectScript : MonoBehaviour
    {
        public int Repeats;
        public Material EffectMaterial;
        private Camera _camera;

        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            for (int i = 0; i < Repeats; i++)
            {
                Graphics.Blit(src, dest, EffectMaterial);
            }
        }
    }
}
