using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace mj.gist.effect
{
    public class GhostEffect : MonoBehaviour
    {
        [SerializeField] private float strength = 0.1f;
        [SerializeField] private float decay = 0.01f;
        [SerializeField] private float period = 0.1f;
        [SerializeField] private bool debug;
        [SerializeField] Material mat;
        [SerializeField] private RenderTexture current;

        [SerializeField] private RawImage uiPrev;
        [SerializeField] private RawImage uiDiff;
        [SerializeField] private RawImage uiComposite;

        private RenderTexture previous;
        private RenderTexture diffTex;
        private RenderTexture composite;

        private Canvas canvas;
        private PingPongRenderTexture rt;


        private void Start()
        {
            previous = new RenderTexture(current);
            diffTex = new RenderTexture(current);
            composite = new RenderTexture(current);

            uiPrev.texture = previous;
            uiDiff.texture = diffTex;
            uiComposite.texture = composite;
            canvas = uiPrev.GetComponentInParent<Canvas>();


            rt = new PingPongRenderTexture(current);
            StartCoroutine(Step());
        }

        IEnumerator Step()
        {
            while (true)
            {
                yield return new WaitForSeconds(period);

                mat.SetTexture("_Prev", previous);
                mat.SetTexture("_Current", current);
                Graphics.Blit(Texture2D.blackTexture, diffTex, mat, 0);

                Graphics.Blit(current, previous);
            }
        }


        private void Update()
        {
            canvas.enabled = debug;

            mat.SetFloat("_Strength", strength);
            mat.SetFloat("_Decay", decay);
            mat.SetTexture("_Diff", diffTex);
            mat.SetTexture("_Accu", rt.Read);

            Graphics.Blit(Texture2D.blackTexture, rt.Write, mat, 1);
            rt.Swap();


            mat.SetTexture("_Accu", rt.Read);
            mat.SetTexture("_Composite", current);
            Graphics.Blit(Texture2D.blackTexture, composite, mat, 2);
        }

        private void OnDestroy()
        {
            rt.Dispose();
        }
    }
}