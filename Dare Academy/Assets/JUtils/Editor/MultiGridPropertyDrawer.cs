using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace JUtil.Grids
{
    [CustomPropertyDrawer(typeof(PathfindingMultiGrid<>))]
    public class MultiGridPropertyDrawer : PropertyDrawer
    {
        private bool initialised = false;

        private bool gridsFoldout = false;
        private bool overridesFoldout = false;

        private GUIContent
            addButtonContent = new GUIContent("+", "add group"),
            removeButtonContent = new GUIContent("-", "remove group");

        [SerializeField] private List<bool> gridDropdowns;
        [SerializeField] private List<bool> overrideDropdowns;

        private float lineHeight;
        private float padding;

        private float lineCount;
        private Rect m_position;

        private const float overflowWidth = 332;

        private SerializedProperty[] extralists;

        private Color backgroundColour = new Color(0.5f, 0.5f, 0.5f, 0.2f);

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

        private void Initialise(SerializedProperty property)
        {
            if (initialised)
                return;

            SerializedProperty gridProperty = property.FindPropertyRelative("gridInfo");
            SerializedProperty overrideProperty = property.FindPropertyRelative("nodeOverrides").FindPropertyRelative("gridLinks");

            if (overrideDropdowns == null || overrideDropdowns.Count != overrideProperty.arraySize)
            {
                overrideDropdowns = new List<bool>();

                for (int i = 0; i < overrideProperty.arraySize; i++)
                {
                    overrideDropdowns.Add(false);
                }
            }

            if (gridDropdowns == null || gridDropdowns.Count != gridProperty.arraySize)
            {
                gridDropdowns = new List<bool>();

                for (int i = 0; i < gridProperty.arraySize; i++)
                {
                    gridDropdowns.Add(false);
                }
            }

            extralists = new SerializedProperty[]
            {
                property.FindPropertyRelative("gridNames")
            };

            initialised = true;
        }

        private void NewLine() => lineCount += lineHeight + padding;

        private void NewLine(float val) => lineCount += (lineHeight + padding) * val;

        private float IndentOffset()
        {
            return 10f * EditorGUI.indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialise(property);
            int lines = 1;
            int paddingLines = 1;

            if (property.isExpanded)
            {
                lines++;
                if (gridsFoldout)
                {
                    int i = 0;
                    foreach (var item in gridDropdowns)
                    {
                        lines++;
                        if (item)
                        {
                            lines += 4;
                            if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                lines++;
                            paddingLines += 6;
                        }
                        i++;
                    }
                }

                lines++;
                if (property.FindPropertyRelative("nodeOverrides").isExpanded)
                {
                    lines++;

                    SerializedProperty overrideProp = property.FindPropertyRelative("nodeOverrides");

                    if (overridesFoldout)
                    {
                        /*for (int i = 0; i < overrideProp.FindPropertyRelative("gridLinks").arraySize; i++)
                        {
                            lines++;
                            if (overrideProp.FindPropertyRelative("gridLinks").GetArrayElementAtIndex(i).isExpanded)
                            {
                                SerializedProperty linkProp = overrideProp.FindPropertyRelative("gridLinks").GetArrayElementAtIndex(i);

                                lines += 2;
                                if(linkProp.FindPropertyRelative("grid1").isExpanded)
                                {
                                    lines += 3;
                                    if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                        lines++;
                                }

                                if (linkProp.FindPropertyRelative("grid2").isExpanded)
                                {
                                   lines += 3;
                                    if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                        lines++;
                                }
                            }
                        }*/
                        //lines+= overrideProp.FindPropertyRelative("gridLinks").arraySize;
                        for (int i = 0; i < overrideProp.FindPropertyRelative("gridLinks").arraySize; i++)
                        {
                            lines++;
                            SerializedProperty linkProp = overrideProp.FindPropertyRelative("gridLinks").GetArrayElementAtIndex(i);

                            if (linkProp.isExpanded)
                            {
                                lines += 2;
                                if (linkProp.FindPropertyRelative("grid1").isExpanded)
                                {
                                    lines += 3;
                                    if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                        lines++;
                                }
                                if (linkProp.FindPropertyRelative("grid2").isExpanded)
                                {
                                    lines += 3;
                                    if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                        lines++;
                                }
                            }
                        }
                    }
                }

                lines++;
                if (property.FindPropertyRelative("tileData").isExpanded)
                {
                    lines += 2;

                    if (property.FindPropertyRelative("tileData").FindPropertyRelative("tileData").isExpanded)
                    {
                        lines += property.FindPropertyRelative("tileData").FindPropertyRelative("tileData").arraySize + 1;
                        paddingLines += 5;
                    }

                    if (property.FindPropertyRelative("tileData").FindPropertyRelative("tilemaps").isExpanded)
                    {
                        lines += property.FindPropertyRelative("tileData").FindPropertyRelative("tilemaps").arraySize + 1;
                        paddingLines += 5;
                    }
                }

                lines++;
                if (property.FindPropertyRelative("debugSettings").isExpanded)
                {
                    lines += property.FindPropertyRelative("debugSettings").CountInProperty() + 2;
                }
            }
            return ((lineHeight + padding) * lines) + (padding * paddingLines);
        }

        private Rect GetSingleLineRect()
        {
            Rect rect = new Rect(m_position.position.x, lineCount, m_position.size.x, EditorGUIUtility.singleLineHeight);
            return rect;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialise(property);

            lineCount = position.position.y;
            m_position = position;

            lineHeight = EditorGUIUtility.singleLineHeight;
            padding = EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.BeginProperty(position, label, property);

            Rect dropdown = GetSingleLineRect();
            property.isExpanded = EditorGUI.Foldout(dropdown, property.isExpanded, new GUIContent("Grids"), true);
            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            NewLine();
            EditorGUI.indentLevel++;

            DrawPropertyArray(property, ref gridsFoldout);

            DrawOverrides(property);

            DrawTileData(property);

            DrawDebugSettings(property);

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        private void DrawOverrides(SerializedProperty property)
        {
            NewLine();

            SerializedProperty overrideProp = property.FindPropertyRelative("nodeOverrides");

            Rect rect = GetSingleLineRect();
            EditorGUI.PropertyField(rect, overrideProp, false);

            if (!overrideProp.isExpanded)
                return;

            EditorGUI.indentLevel++;

            NewLine();

            SerializedProperty gridLinksProp = overrideProp.FindPropertyRelative("gridLinks");

            DrawArrayDropdown(gridLinksProp, ref overridesFoldout, null, overrideDropdowns);

            if (!overridesFoldout)
            {
                EditorGUI.indentLevel--;
                return;
            }

            EditorGUI.indentLevel++;

            for (int i = 0; i < gridLinksProp.arraySize; i++)
            {
                SerializedProperty linkProp = gridLinksProp.GetArrayElementAtIndex(i);

                NewLine();
                rect = GetSingleLineRect();
                string[] names = Supyrb.SerializedPropertyExtensions.GetValue<string[]>(property.FindPropertyRelative("gridNames"));
                string linkLabel = "Link " + i.ToString() + "\t( "
                    + " " + names[linkProp.FindPropertyRelative("grid1").FindPropertyRelative("index").intValue]
                    + " ⟷"
                    + " " + names[linkProp.FindPropertyRelative("grid2").FindPropertyRelative("index").intValue]
                    + " )";

                linkProp.isExpanded = EditorGUI.Foldout(rect, linkProp.isExpanded, linkLabel);

                if (linkProp.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    NewLine();

                    rect = GetSingleLineRect();

                    linkProp.FindPropertyRelative("grid1").isExpanded = EditorGUI.Foldout(rect, linkProp.FindPropertyRelative("grid1").isExpanded, linkProp.FindPropertyRelative("grid1").displayName);

                    if (linkProp.FindPropertyRelative("grid1").isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        NewLine();

                        //string[] names = Supyrb.SerializedPropertyExtensions.GetValue<string[]>(property.FindPropertyRelative("gridNames"));

                        rect = GetSingleLineRect();
                        linkProp.FindPropertyRelative("grid1").FindPropertyRelative("index").intValue = EditorGUI.Popup(rect, "Grid", linkProp.FindPropertyRelative("grid1").FindPropertyRelative("index").intValue, names);

                        NewLine();
                        rect = GetSingleLineRect();
                        EditorGUI.PropertyField(rect, linkProp.FindPropertyRelative("grid1.position"));

                        if (EditorGUIUtility.currentViewWidth < overflowWidth)
                            NewLine();

                        NewLine();
                        rect = GetSingleLineRect();

                        rect.x = EditorGUIUtility.labelWidth + IndentOffset() / 2 - 5;
                        rect.width = EditorGUIUtility.fieldWidth;
                        if (EditorGUIUtility.currentViewWidth < overflowWidth)
                            rect.x += 15;

                        if (GUI.Button(rect, arrows[linkProp.FindPropertyRelative("grid1.direction").intValue]))
                        {
                            linkProp.FindPropertyRelative("grid1.direction").intValue++;
                            if (linkProp.FindPropertyRelative("grid1.direction").intValue > 7)
                                linkProp.FindPropertyRelative("grid1.direction").intValue = 0;
                        }

                        rect.x += rect.width;
                        rect.width = 100;
                        EditorGUI.IntField(rect, GUIContent.none, linkProp.FindPropertyRelative("grid1.direction").intValue);

                        rect = GetSingleLineRect();
                        EditorGUI.LabelField(rect, "Direction");

                        EditorGUI.indentLevel--;
                    }

                    NewLine();

                    rect = GetSingleLineRect();

                    linkProp.FindPropertyRelative("grid2").isExpanded = EditorGUI.Foldout(rect, linkProp.FindPropertyRelative("grid2").isExpanded, linkProp.FindPropertyRelative("grid2").displayName);

                    if (linkProp.FindPropertyRelative("grid2").isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        NewLine();
                        //string[] names = Supyrb.SerializedPropertyExtensions.GetValue<string[]>(property.FindPropertyRelative("gridNames"));
                        rect = GetSingleLineRect();
                        linkProp.FindPropertyRelative("grid2").FindPropertyRelative("index").intValue = EditorGUI.Popup(rect, "Grid", linkProp.FindPropertyRelative("grid2").FindPropertyRelative("index").intValue, names);

                        NewLine();
                        rect = GetSingleLineRect();
                        EditorGUI.PropertyField(rect, linkProp.FindPropertyRelative("grid2.position"));

                        if (EditorGUIUtility.currentViewWidth < overflowWidth)
                            NewLine();

                        NewLine();
                        rect = GetSingleLineRect();

                        rect.x = EditorGUIUtility.labelWidth + IndentOffset() / 2 - 5;
                        rect.width = EditorGUIUtility.fieldWidth;
                        if (EditorGUIUtility.currentViewWidth < overflowWidth)
                            rect.x += 15;

                        if (GUI.Button(rect, arrows[linkProp.FindPropertyRelative("grid2.direction").intValue]))
                        {
                            linkProp.FindPropertyRelative("grid2.direction").intValue++;
                            if (linkProp.FindPropertyRelative("grid2.direction").intValue > 7)
                                linkProp.FindPropertyRelative("grid2.direction").intValue = 0;
                        }

                        rect.x += rect.width;
                        rect.width = 100;
                        EditorGUI.IntField(rect, GUIContent.none, linkProp.FindPropertyRelative("grid2.direction").intValue);

                        rect = GetSingleLineRect();
                        EditorGUI.LabelField(rect, "Direction");

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        private void DrawTileData(SerializedProperty property)
        {
            NewLine();
            Rect rect = GetSingleLineRect();
            if (property.FindPropertyRelative("tileData").isExpanded)
            {
                //rect.height += (lineHeight + padding) * 9;

                NewLine(2);

                if (property.FindPropertyRelative("tileData").FindPropertyRelative("tileData").isExpanded)
                {
                    NewLine(property.FindPropertyRelative("tileData").FindPropertyRelative("tileData").arraySize + 1);
                    lineCount += padding * 5;
                }

                if (property.FindPropertyRelative("tileData").FindPropertyRelative("tilemaps").isExpanded)
                {
                    NewLine(property.FindPropertyRelative("tileData").FindPropertyRelative("tilemaps").arraySize + 1);
                    lineCount += padding * 5;
                }
            }

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tileData"), true);
        }

        private void DrawDebugSettings(SerializedProperty property)
        {
            NewLine();
            Rect rect = GetSingleLineRect();
            if (property.FindPropertyRelative("debugSettings").isExpanded)
            {
                rect.height += (lineHeight + padding) * 9;
                rect.y += lineHeight + (padding * 3);
                rect.x += IndentOffset() * 2;
                rect.width -= IndentOffset() * 2;

                EditorGUI.DrawRect(rect, backgroundColour);

                //rect.height += (lineHeight + padding);
                rect.y -= lineHeight + (padding * 3);
                rect.x -= IndentOffset() * 2;
                rect.width += IndentOffset();
            }

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("debugSettings"), true);
        }

        private void DrawArrayDropdown(SerializedProperty property, ref bool fold, SerializedProperty[] otherLists, List<bool> boolList)
        {
            Rect dropdown = GetSingleLineRect();

            fold = EditorGUI.Foldout(dropdown, fold, property.displayName);

            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");

            int indentlevelPrev = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 0;

            //dropdown.width += IndentOffset();
            dropdown.x = EditorGUIUtility.currentViewWidth - 53;// - IndentOffset();
            dropdown.width = 49;// + IndentOffset();

            EditorGUI.PropertyField(dropdown, arraySizeProp, GUIContent.none);
            //EditorGUI.FloatField(dropdown, m_position.width);

            dropdown.x = EditorGUIUtility.currentViewWidth - 53 - 50;
            dropdown.width = 50;

            //DrawListButtons(dropdown, property, extralists);
            DrawGridInfoListButtons(dropdown, property, boolList, otherLists);

            EditorGUI.indentLevel = indentlevelPrev;
        }

        private void DrawPropertyArray(SerializedProperty property, ref bool fold)
        {
            SerializedProperty gridInfoProperty = property.FindPropertyRelative("gridInfo");
            SerializedProperty namesProp = property.FindPropertyRelative("gridNames");

            DrawArrayDropdown(gridInfoProperty, ref fold, extralists, gridDropdowns);

            SerializedProperty arraySizeProp = gridInfoProperty.FindPropertyRelative("Array.size");

            if (fold)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < arraySizeProp.intValue; i++)
                {
                    DrawGridProperty(property, i);
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawGridProperty(SerializedProperty property, int i)
        {
            NewLine();
            bool fold = gridDropdowns[i];

            SerializedProperty GridInfoProperty = property.FindPropertyRelative("gridInfo").GetArrayElementAtIndex(i);
            SerializedProperty namesProp = property.FindPropertyRelative("gridNames").GetArrayElementAtIndex(i);

            // draw background colour
            Rect background = GetSingleLineRect();
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;
            if (fold)
                background.height += (lineHeight + padding) * ((EditorGUIUtility.currentViewWidth < overflowWidth) ? 4.5f : 3.5f);
            background.x += IndentOffset();
            background.width -= IndentOffset();

            EditorGUI.DrawRect(background, backgroundColour);
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;

            // draw dropdown
            Rect dropdown = GetSingleLineRect();
            fold = EditorGUI.Foldout(dropdown, fold, "");

            //SerializedProperty gridNameProp = rootProperty.FindPropertyRelative("gridNames").GetArrayElementAtIndex(i);
            dropdown = GetSingleLineRect();
            dropdown.x += 10;
            dropdown.width -= 10;
            EditorGUI.PropertyField(dropdown, namesProp, GUIContent.none);

            gridDropdowns[i] = fold;

            if (fold)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;

                NewLine();
                lineCount += padding * 3;

                // draw size fields
                Vector2Int dimensions = new Vector2Int(
                    GridInfoProperty.FindPropertyRelative("width").intValue,
                    GridInfoProperty.FindPropertyRelative("height").intValue
                    );

                Rect rect = GetSingleLineRect();
                rect.width -= IndentOffset() / 2;

                lineCount += padding;

                dimensions = EditorGUI.Vector2IntField(rect, GUIContent.none, dimensions);

                GridInfoProperty.FindPropertyRelative("width").intValue = dimensions.x;
                GridInfoProperty.FindPropertyRelative("height").intValue = dimensions.y;

                // draw cell size field
                NewLine();
                rect.y += lineHeight + padding;
                EditorGUI.PropertyField(rect, GridInfoProperty.FindPropertyRelative("cellSize"));

                // draw position field
                NewLine();
                rect.y += lineHeight + padding;
                EditorGUI.PropertyField(rect, GridInfoProperty.FindPropertyRelative("originPosition"), new GUIContent("Origin"));

                if (EditorGUIUtility.currentViewWidth < overflowWidth)
                    NewLine();

                lineCount += padding * 3;

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }

        private void DrawGridInfoListButtons(Rect rect, SerializedProperty list, List<bool> boolList, SerializedProperty[] extraLists = null)
        {
            rect.width /= 2;

            if (GUI.Button(rect, addButtonContent))
            {
                list.arraySize += 1;
                if (extraLists != null)
                {
                    foreach (var array in extraLists)
                    {
                        array.arraySize += 1;
                        if (array.name == "gridNames")
                            array.GetArrayElementAtIndex(list.arraySize - 1).stringValue = "unnamed grid";
                    }
                }

                boolList.Add(true);
            }

            rect.x += rect.width;

            if (GUI.Button(rect, removeButtonContent) && list.arraySize > 0)
            {
                int oldSize = list.arraySize;
                if (extraLists != null)
                {
                    foreach (var array in extraLists)
                    {
                        array.DeleteArrayElementAtIndex(list.arraySize - 1);
                    }
                }

                list.DeleteArrayElementAtIndex(list.arraySize - 1);
                if (list.arraySize == oldSize)
                {
                    list.DeleteArrayElementAtIndex(list.arraySize - 1);
                }

                boolList.RemoveAt(list.arraySize - 1);
            }
        }
    }//*/
}