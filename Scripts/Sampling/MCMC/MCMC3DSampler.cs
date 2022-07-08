using mj.gist.math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.sampling {
    /// <summary>
    /// 3D distribution sampler
    /// </summary>
    public class MCMC3DSampler : MCMCSampler<Vector3> {
        private static readonly int WEIGHT_REFERENCE_LOOP_COUNT = 500;

        private Vector3 scale;
        private Vector4[] data;
        private float threshold;

        public MCMC3DSampler(Vector4[] data, Vector3 scale, float threshold) {
            this.data = data;
            this.scale = scale;
            this.threshold = threshold;
        }

        protected override Vector3 GetRandomCurrent() {
            return Vector3.Scale(scale, new Vector3(Random.value, Random.value, Random.value));
        }

        protected override float ComputeDensity(Vector3 pos) {
            var weight = 0f;

            for (var i = 0; i < WEIGHT_REFERENCE_LOOP_COUNT; i++) {
                var id = Mathf.FloorToInt(Random.value * (data.Length - 1));
                Vector3 posi = data[id];
                var mag = Vector3.SqrMagnitude(posi - pos);
                weight += Mathf.Exp(-mag) * data[id].w;
            }
            return weight;
        }


        protected override void Next() {
            var next = curr + GaussianDistribution.GenerateRandomPointStandard();

            var nextDensity = ComputeDensity(next);
            var f1 = nextDensity > threshold;
            var f2 = currDensity <= 0f || Mathf.Min(1f, nextDensity / currDensity) >= Random.value;
            if (f1 && f2) {
                curr = next;
                currDensity = nextDensity;
            }
        }

    }
}