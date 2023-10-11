using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace mj.gist
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private readonly float normalizedTrasitionDuration = 0.2f;

        private Animator animator;
        private AnimatorStateInfo state => animator.GetCurrentAnimatorStateInfo(0);
        private List<Clip> clips;

        private Clip currentClip;
        private bool isFinished => state.shortNameHash == currentClip.hash && state.normalizedTime > 1;

        public string GetClipRandom() => clips.RandomPick().name;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            clips = animator.runtimeAnimatorController.animationClips.Select(c => new Clip(c)).ToList();
        }

        #region Play
        public void Play(Clip clip, int layer = 0, float normalizedTime = 0f)
        {
            animator.Play(clip.name, layer, normalizedTime);
            currentClip = clip;
        }

        public void PlayRandom(int layer = 0, float normalizedTime = 0f)
        {
            var clip = clips.RandomPick();
            Play(clip, layer, normalizedTime);
        }
        #endregion

        #region CrossFade
        public void CrossFade(Clip clip)
        {
            animator.CrossFade(clip.name, normalizedTrasitionDuration);
            currentClip = clip;
        }
        public void CrossFadeRandom()
        {
            var clip = clips.RandomPick();
            CrossFade(clip);
        }
        #endregion

        public void StartPlayRandomCourtine()
        {
            StartCoroutine(PlayRandomCourtine());
        }

        private IEnumerator PlayRandomCourtine()
        {
            while (true)
            {
                CrossFadeRandom();
                yield return null;
                yield return new WaitUntil(() => isFinished);
            }
        }

        private void OnDrawGizmos()
        {
            Handles.Label(transform.position, $"{currentClip.name} {state.normalizedTime.ToString("f2")}");
        }
    }

    [System.Serializable]
    public class Clip
    {
        public string name;
        public int hash;
        public float length;

        public Clip(AnimationClip clip)
        {
            this.name = clip.name;
            this.hash = Animator.StringToHash(clip.name);
            this.length = clip.length;
        }
    }
}
