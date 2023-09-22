using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist
{
    public static class CoroutineUtil
    {
        public static IEnumerator OneTime(float duration, Action<float> action, Action start = null, Action finish = null)
        {
            start?.Invoke();

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                action(t / duration);
                yield return null;
            }

            finish?.Invoke();
        }


        public static IEnumerator Fade(float from, float to, float duration, Action<float> action, Action start = null, Action finish = null)
        {
            return OneTime(duration, (ratio) => action(Mathf.Lerp(from, to, ratio)), start, finish);
        }

        public static IEnumerator WaitForSeconds(float time, Action action, Action start = null)
        {
            start?.Invoke();
            yield return new WaitForSeconds(time);
            action.Invoke();
        }

        public static IEnumerator WaitForOneFrame(Action action)
        {
            yield return null;
            action.Invoke();
        }
    }
}