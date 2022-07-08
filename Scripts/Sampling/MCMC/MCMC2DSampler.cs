using mj.gist.math;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.sampling {
    /// <summary>
    /// 2D texture sampler
    /// </summary>
    public class MCMC2DSampler : MCMCSampler<Vector2> {

        private Texture2D texture;
        private float stdDev;
        private float aspect => (float)texture.width / texture.height;
        private Vector2 stddevAspect => new Vector2(stdDev, stdDev / aspect);

        public MCMC2DSampler(Texture2D texture, float stddev) {
            this.texture = texture;
            this.stdDev = stddev;
        }

        protected override Vector2 GetRandomCurrent() {
            return new Vector2(Random.value, Random.value);
        }

        protected override float ComputeDensity(Vector2 curr) {
            return texture.GetPixelBilinear(curr.x, curr.y).r;
        }

        protected override void Next() {
            var next = curr + Vector2.Scale(stddevAspect, RandomGenerator.RandGaussian());
            next.x -= Mathf.Floor(next.x);
            next.y -= Mathf.Floor(next.y);

            var nextDensity = ComputeDensity(next);

            if (IsValid(nextDensity)) {
                curr = next;
                currDensity = nextDensity;
            }
        }
    }
}