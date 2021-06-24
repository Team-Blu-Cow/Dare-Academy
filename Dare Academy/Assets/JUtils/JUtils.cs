using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace JUtil
{
    public static class JUtils
    {
        // courtesy of http://answers.unity.com/answers/893984/view.html
        public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
        {
            Transform t = parent.transform;
            foreach (Transform tr in t)
            {
                if (tr.tag == tag)
                {
                    return tr.GetComponent<T>();
                }
            }
            return null;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;

            return new Vector2(-1*(cos * tx - sin * ty), sin * tx + cos * ty);
        }

        public static float BetterSign(float value)
        {
            int temp = Mathf.RoundToInt(value);
            if (temp == 0)
                return 0f;

            return Mathf.Sign(value);
        }

        public static int BetterSign(int value)
        {
            if (value == 0)
                return 0;

            return (int)Mathf.Sign(value);
        }

        public static void ShowTime(double ticks, string message)
        {
            double milliseconds = (ticks / Stopwatch.Frequency) * 1000;
            double nanoseconds = (ticks / Stopwatch.Frequency) * 1000000000;
            UnityEngine.Debug.Log(message + "\n " + milliseconds + "ms" + " [" + nanoseconds + "ns]");
        }
    }
}
