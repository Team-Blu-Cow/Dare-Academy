using UnityEditor;
using UnityEngine;

namespace JUtil.Grids
{
    [CustomPropertyDrawer(typeof(NeighbourGraph))]
    public class NeighbourGraphPropertyDrawer : PropertyDrawer
    {

        private GUIStyle BlockColour = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 30,
            
            normal = new GUIStyleState()
            {
                background = Texture2D.blackTexture,
                textColor = Color.white
            },

            hover = new GUIStyleState()
            {
                background = Texture2D.blackTexture,
                textColor = Color.white
            },

            active = new GUIStyleState()
            {
                background = Texture2D.blackTexture,
                textColor = Color.white
            }


        };

        private Color falseColour = new Color(0.25f, 0.25f, 0.25f, 1);
        private Color trueColour = new Color(0.5f, 0.5f, 0.5f, 1);
        private Color onewayColour = new Color(0.5f, 0.4f, 0.4f, 1);


        private const float offset = 20;

        private string[] arrows = new string[8]
        {
            "↑",
            "↗",
            "→",
            "↘",
            "↓",
            "↙",
            "←",
            "↖"
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.currentViewWidth + 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position,label,property);

            Rect fieldRect = new Rect(position.x, position.y, position.width, offset);

            property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, new GUIContent("Neighbours"), true);
            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel = (int)offset;

            int margin = 2;

            BlockColour.fontSize = Mathf.RoundToInt(EditorGUIUtility.currentViewWidth / 6);

            float angle = 0;
            float addition = 360 / 8;

            float blockWidth = (position.width / 3);
            float blockHeight = (position.height - offset) / 3;

            float blockPosx = position.x + blockWidth - (margin/2);
            float blockPosy = position.y + (position.height / 2) + (offset / 2) - (blockWidth / 2) - (margin * 1.5f);

            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = Vector2.up.Rotate(angle);

                float x_distance = blockPosx + (JUtils.BetterSign(direction.x)*blockWidth);
                float y_distance = blockPosy + (JUtils.BetterSign(-direction.y) * blockHeight); 

                var rect = new Rect(
                        x_distance,
                        y_distance,
                        blockWidth - margin * 2,
                        blockHeight - margin * 2
                        );

                STATE thisValue = (STATE)property.FindPropertyRelative("neighbors").GetArrayElementAtIndex(i).intValue;

                EditorGUI.DrawRect(rect, (thisValue == STATE.OFF) ? falseColour : (thisValue == STATE.ONEWAY)? onewayColour : trueColour);

                if (GUI.Button(rect, arrows[i], BlockColour))
                {
                    //property.FindPropertyRelative("neighbors").GetArrayElementAtIndex(i).boolValue =
                    //   !thisValue;

                    property.FindPropertyRelative("neighbors").GetArrayElementAtIndex(i).intValue++;
                    if (property.FindPropertyRelative("neighbors").GetArrayElementAtIndex(i).intValue >= (int)STATE.NumberOfStates)
                        property.FindPropertyRelative("neighbors").GetArrayElementAtIndex(i).intValue = 0;

                }

                angle += addition;
            }

            EditorGUI.EndProperty();
        }
    }
}