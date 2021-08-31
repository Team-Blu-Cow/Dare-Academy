using UnityEngine;
using UnityEditor;

namespace blu.EditorTools
{
    public static class EditorUtil
    {
        public static object AutoFieldHandleing(object field)
        {
            if (field is string)
            {
                return StringField(field as string);
            }

            if (field is bool)
            {
                return BoolField(field as bool?);
            }

            if (field is int)
            {
                return IntField(field as int?);
            }

            if (field is long)
            {
                return LongField(field as long?);
            }

            if (field is float)
            {
                return FloatField(field as float?);
            }

            if (field is double)
            {
                return DoubleField(field as double?);
            }

            if (field is Vector2)
            {
                return Vector2Field(field as Vector2?);
            }

            if (field is Vector2Int)
            {
                return Vector2IField(field as Vector2Int?);
            }

            if (field is Vector3)
            {
                return Vector3Field(field as Vector3?);
            }

            if (field is Vector3Int)
            {
                return Vector3IField(field as Vector3Int?);
            }

            if (field is Vector4)
            {
                return Vector4Field(field as Vector4?);
            }

            GUILayout.Label("DATA NOT READABLE IN EDITOR");
            return field;
        }

        public static string StringField(string s)
        {
            return EditorGUILayout.TextField(s ?? "");
        }

        public static bool BoolField(bool? b)
        {
            return EditorGUILayout.Toggle(b ?? false);
        }

        public static int IntField(int? i)
        {
            return EditorGUILayout.IntField(i ?? 0);
        }

        public static long LongField(long? l)
        {
            return EditorGUILayout.LongField(l ?? 0L);
        }

        public static float FloatField(float? f)
        {
            return EditorGUILayout.FloatField(f ?? 0F);
        }

        public static double DoubleField(double? d)
        {
            return EditorGUILayout.DoubleField(d ?? 0D);
        }

        public static Vector2 Vector2Field(Vector2? vec2)
        {
            return EditorGUILayout.Vector2Field("", vec2 ?? Vector2.zero);
        }

        public static Vector2Int Vector2IField(Vector2Int? vec2i)
        {
            return EditorGUILayout.Vector2IntField("", vec2i ?? Vector2Int.zero);
        }

        public static Vector3 Vector3Field(Vector3? vec3)
        {
            return EditorGUILayout.Vector3Field("", vec3 ?? Vector3.zero);
        }

        public static Vector3Int Vector3IField(Vector3Int? vec3i)
        {
            return EditorGUILayout.Vector3IntField("", vec3i ?? Vector3Int.zero);
        }

        public static Vector4 Vector4Field(Vector4? vec4)
        {
            return EditorGUILayout.Vector4Field("", vec4 ?? Vector4.zero);
        }

        public static GameObject GameObjectField(GameObject obj)
        {
            return UnityObjectField(obj, typeof(GameObject)) as GameObject;
        }

        public static ScriptableObject ScriptableObjectField(ScriptableObject obj)
        {
            return UnityObjectField(obj, typeof(ScriptableObject)) as ScriptableObject;
        }

        public static UnityEngine.Object UnityObjectField(UnityEngine.Object obj)
        {
            return EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), true);
        }

        public static UnityEngine.Object UnityObjectField(UnityEngine.Object obj, System.Type type)
        {
            return EditorGUILayout.ObjectField(obj, type, true);
        }
    }
}