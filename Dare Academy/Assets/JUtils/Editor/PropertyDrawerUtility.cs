using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JUtil
{
    public class PropertyDrawerUtility
    {
        public float lineHeight;
        public float padding;

        public float lineCount;

        public Rect m_position;

        public PropertyDrawerUtility(Rect position)
        {
            lineCount = position.position.y;
            m_position = position;

            lineHeight = EditorGUIUtility.singleLineHeight;
            padding = EditorGUIUtility.standardVerticalSpacing;
        }

        static public int IndentLevel
        {
            get => EditorGUI.indentLevel;
            set => EditorGUI.indentLevel = value;
        }

        public void NewLine() => lineCount += lineHeight + padding;

        public void NewLine(float val) => lineCount += (lineHeight + padding) * val;

        public float IndentOffset()
        {
            return 10f * EditorGUI.indentLevel;
        }

        public Rect GetSingleLineRect()
        {
            Rect rect = new Rect(m_position.position.x, lineCount, m_position.size.x, EditorGUIUtility.singleLineHeight);
            rect.x += IndentOffset();
            return rect;
        }
    }
}