using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace mj.gist.postprocessing {
    [RequireComponent(typeof(Camera))]
    public class StencilMaskGenerator : MonoBehaviour {
        [SerializeField] private Shader shader;
        [SerializeField] private List<StencilMask> masks;

        private Camera cam;
        private CommandBuffer buffer;
        private int cameraRenderingTextureID;

        private void OnEnable() {
            cam = GetComponent<Camera>();

            SetupBuffer();
            cam.AddCommandBuffer(CameraEvent.AfterForwardAlpha, buffer);
        }

        private void OnDisable() {
            cam.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, buffer);
        }

        private void SetupBuffer() {
            if (buffer == null) {
                buffer = new CommandBuffer { name = "StencilGenerator" };

                if (cameraRenderingTextureID == 0)
                    cameraRenderingTextureID = Shader.PropertyToID("_temp");

                // store current render result to tmp
                buffer.GetTemporaryRT(cameraRenderingTextureID, -1, -1, 0);
                buffer.Blit(BuiltinRenderTextureType.CameraTarget, cameraRenderingTextureID);


                foreach (var mask in masks) {
                    var name = mask.name;
                    var stencil = mask.stencil;

                    var mat = new Material(shader);

                    mat.SetInt("_Stencil", stencil);
                    var outputId = Shader.PropertyToID($"_output_{name}");
                    buffer.GetTemporaryRT(outputId, -1, -1, 0);
                    buffer.Blit(cameraRenderingTextureID, BuiltinRenderTextureType.CameraTarget, mat);
                    buffer.Blit(BuiltinRenderTextureType.CameraTarget, outputId);
                    buffer.SetGlobalTexture(name, outputId);
                }

                // set original rendering back
                buffer.Blit(cameraRenderingTextureID, BuiltinRenderTextureType.CameraTarget);
            }
        }

        [System.Serializable]
        public class StencilMask {
            public string name;
            public int stencil;
        }
    }
}