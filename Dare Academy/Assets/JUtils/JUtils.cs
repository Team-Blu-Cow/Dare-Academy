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

        // VECTOR2 HELPER METHODS *****************************************************************
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;

            return new Vector2(-1*(cos * tx - sin * ty), sin * tx + cos * ty);
        }

        public static float GetRotation(this Vector2 v)
        {
            Vector2 v_up = Vector2.up;
            var sign = BetterSign(v_up.x * v.y - v_up.y * v.x);

            if(sign <= 0)
                return Vector2.Angle(v_up, v);
            else
                return 360 - Vector2.Angle(v_up, v);
        }

        public static float GetRotation(this Vector2Int v)
        {
            Vector2Int v_up = Vector2Int.up;
            var sign = BetterSign(v_up.x * v.y - v_up.y * v.x);

            if (sign <= 0)
                return Vector2.Angle(v_up, v);
            else
                return 360 - Vector2.Angle(v_up, v);
        }

        public static int RotationToIndex(this Vector2 v, int sliceSize = 45)
        {
            float angle = v.GetRotation();

            return Mathf.RoundToInt(angle) / sliceSize;
        }

        public static int RotationToIndex(this Vector2Int v, int sliceSize = 45)
        {
            float angle = v.GetRotation();

            return Mathf.RoundToInt(angle) / sliceSize;
        }

        public static Vector2 IndexToRotation(this int i, int sliceSize = 45)
        {
            float angle = sliceSize * i;

            return Vector2.up.Rotate(angle);
        }

        // BETTER SIGN MENTHODS *******************************************************************
        // Returns -1, 0 or 1 depending on the numbers sign or if it is zero 

        public static float BetterSign(float value)
        {
            //int temp = Mathf.RoundToInt(value);
            if (value < 0.00001f && value > -0.00001f)
                return 0f;

            return Mathf.Sign(value);
        }

        public static int BetterSign(int value)
        {
            if (value == 0)
                return 0;

            return (int)Mathf.Sign(value);
        }

        // PERFORMANCE TESTING METHODS ************************************************************

        public static void ShowTime(double ticks, string message)
        {
            double milliseconds = (ticks / Stopwatch.Frequency) * 1000;
            double nanoseconds = (ticks / Stopwatch.Frequency) * 1000000000;
            UnityEngine.Debug.Log(message + "\n " + milliseconds + "ms" + " [" + nanoseconds + "ns]");
        }
    }
}
