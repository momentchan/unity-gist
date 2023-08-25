using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist
{
    public static class CoroutineUtil
    {
        public static IEnumerator OneTime(float duration, Action<float> action, Action onStart = null, Action onFinished = null)
        {
            onStart?.Invoke();

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                action(t / duration);
                yield return null;
            }

            onFinished?.Invoke();
        }


        public static IEnumerator Fade(float from, float to, float duration, Action<float> action, Action onStart = null, Action onFinished = null)
        {
            return OneTime(duration, (ratio) => action(Mathf.Lerp(from, to, ratio)), onStart, onFinished);
        }
    }
}