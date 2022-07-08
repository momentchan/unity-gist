using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.sampling {
    public abstract class MCMCSampler<T> : MonoBehaviour {
        protected static readonly int LIMIT_RESET_LOOP_COUNT = 100;

        protected T curr;
        protected float currDensity;
        protected bool IsValid(float nextDensity) => currDensity <= 0f || Mathf.Min(1f, nextDensity / currDensity) >= Random.value;

        protected abstract T GetRandomCurrent();

        protected virtual void Reset() {
            for (var i = 0; currDensity <= 0f && i < LIMIT_RESET_LOOP_COUNT; i++) {
                curr = GetRandomCurrent();
                currDensity = ComputeDensity(curr);
            }
        }

        protected abstract void Next();
        protected abstract float ComputeDensity(T curr);

        public virtual IEnumerable<T> Chain(int nInitials, int nSamples) {
            Reset();

            // Burn-in
            for (var i = 0; i < nInitials; i++)
                Next();

            for (var i = 0; i < nSamples; i++) {
                yield return curr;
                Next();
            }
        }

    }
}