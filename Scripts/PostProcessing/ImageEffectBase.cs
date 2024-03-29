﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.postprocessing {
    [RequireComponent(typeof(Camera))]
    public class ImageEffectBase : MonoBehaviour {

        [SerializeField] protected bool enable = true;
        [SerializeField] protected Material material;

        protected virtual void Start() {
        }

        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (IsSupportAndEnable())
                Graphics.Blit(src, dst, material);
            else
                Graphics.Blit(src, dst);
        }

        protected bool IsSupportAndEnable () {
            return material != null && material.shader.isSupported && enable;
        }
    }
}