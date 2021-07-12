using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace JUtil.Grids
{
    [CustomPropertyDrawer(typeof(PathfindingMultiGrid))]
    public class MultiGridPropertyDrawer : PropertyDrawer
    {
        private bool initialised = false;

        private bool gridsFoldout = false;
        private bool gridLinksOverridesFoldout = false;
        private bool sceneLinksOverridesFoldout = false;

        private GUIContent
            addButtonContent = new GUIContent("+", "add group"),
            removeButtonContent = new GUIContent("-", "remove group");

        [SerializeField] private List<bool> gridDropdowns;
        [SerializeField] private List<bool> gridLinksOverrideDropdowns;
        [SerializeField] private List<bool> sceneLinksOverrideDropdowns;

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

        // DEFAULT PROPERTY DRAWER METHODS **********************************************************************************************************

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
                            paddingLines += 7;
                        }
                        i++;
                    }
                }

                lines++;
                if (property.FindPropertyRelative("nodeOverrides").isExpanded)
                {
                    lines++;

                    SerializedProperty overrideProp = property.FindPropertyRelative("nodeOverrides");

                    if (gridLinksOverridesFoldout)
                    {
                        for (int i = 0; i < overrideProp.FindPropertyRelative("gridLinks").arraySize; i++)
                        {
                            lines++;
                            SerializedProperty linkProp = overrideProp.FindPropertyRelative("gridLinks").GetArrayElementAtIndex(i);

                            if (linkProp.isExpanded)
                            {
                                lines += 3;
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

                    lines++;
                    // HERE IS THE SCENE OVERRIDES AREA

                    SerializedProperty sceneLinksProp = overrideProp.FindPropertyRelative("sceneLinks");

                    if (sceneLinksProp.isExpanded)
                    {
                        if (sceneLinksProp.arraySize > 0)
                        {
                            for (int i = 0; i < sceneLinksProp.arraySize; i++)
                            {
                                lines++;
                                if (sceneLinksProp.GetArrayElementAtIndex(i).isExpanded)
                                {
                                    if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                        lines += 2;
                                    lines += 13;
                                }
                            }

                            lines++;
                        }
                        else
                        {
                            lines += 2;
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

            DrawGridInfoEditor(property, ref gridsFoldout);

            DrawOverrides(property);

            DrawTileData(property);

            DrawDebugSettings(property);

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        // GRID OVERRIDE METHODS ********************************************************************************************************************
        private void DrawOverrides(SerializedProperty property)
        {
            NewLine();

            SerializedProperty overrideProp = property.FindPropertyRelative("nodeOverrides");

            Rect rect = GetSingleLineRect();
            EditorGUI.PropertyField(rect, overrideProp, false);

            if (!overrideProp.isExpanded)
                return;

            EditorGUI.indentLevel++;

            // draw single scene grid links *******************************************************
            NewLine();

            SerializedProperty gridLinksProp = overrideProp.FindPropertyRelative("gridLinks");

            DrawArrayDropdown(gridLinksProp, ref gridLinksOverridesFoldout, null, gridLinksOverrideDropdowns);

            if (gridLinksOverridesFoldout)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < gridLinksProp.arraySize; i++)
                {
                    DrawGridLinks(property, gridLinksProp, i);
                }

                EditorGUI.indentLevel--;
            }

            // draw multi scene grid links ********************************************************

            NewLine();

            SerializedProperty sceneLinksProp = overrideProp.FindPropertyRelative("sceneLinks");

            DrawSceneLinks(sceneLinksProp);

            //DrawArrayDropdown(sceneLinksProp, ref sceneLinksOverridesFoldout, null, sceneLinksOverrideDropdowns);

            /*if (sceneLinksOverridesFoldout)
            {
                EditorGUI.indentLevel++;

                NewLine();

                for (int i = 0; i < sceneLinksProp.arraySize; i++)
                {
                    DrawGridLinks(property, sceneLinksProp, i);
                }

                EditorGUI.indentLevel--;
            }*/

            EditorGUI.indentLevel--;
        }

        private void DrawGridLinks(SerializedProperty property, SerializedProperty gridLinksProp, int i)
        {
            SerializedProperty linkProp = gridLinksProp.GetArrayElementAtIndex(i);

            NewLine();
            Rect rect = GetSingleLineRect();
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
                EditorGUI.PropertyField(rect, linkProp.FindPropertyRelative("width"));

                DrawGridLink(linkProp, 1, names);
                DrawGridLink(linkProp, 2, names);

                EditorGUI.indentLevel--;
            }
        }

        public void DrawGridLink(SerializedProperty linkProp, int num, string[] names)
        {
            if (num != 1 && num != 2)
                return;

            NewLine();
            Rect rect = GetSingleLineRect();
            linkProp.FindPropertyRelative("grid" + num.ToString()).isExpanded = EditorGUI.Foldout(rect, linkProp.FindPropertyRelative("grid" + num.ToString()).isExpanded, linkProp.FindPropertyRelative("grid" + num.ToString()).displayName);

            if (linkProp.FindPropertyRelative("grid" + num.ToString()).isExpanded)
            {
                EditorGUI.indentLevel++;
                NewLine();

                rect = GetSingleLineRect();
                linkProp.FindPropertyRelative("grid" + num.ToString()).FindPropertyRelative("index").intValue = EditorGUI.Popup(rect, "Grid", linkProp.FindPropertyRelative("grid" + num.ToString()).FindPropertyRelative("index").intValue, names);

                NewLine();
                rect = GetSingleLineRect();
                EditorGUI.PropertyField(rect, linkProp.FindPropertyRelative("grid" + num.ToString() + ".position"));

                if (EditorGUIUtility.currentViewWidth < overflowWidth)
                    NewLine();

                NewLine();
                rect = GetSingleLineRect();

                rect.x = EditorGUIUtility.labelWidth + IndentOffset() / 2 - 5;
                rect.width = EditorGUIUtility.fieldWidth;
                if (EditorGUIUtility.currentViewWidth < overflowWidth)
                    rect.x += 15;

                if (GUI.Button(rect, arrows[linkProp.FindPropertyRelative("grid" + num.ToString() + ".direction").intValue]))
                {
                    linkProp.FindPropertyRelative("grid" + num.ToString() + ".direction").intValue++;
                    if (linkProp.FindPropertyRelative("grid" + num.ToString() + ".direction").intValue > 7)
                        linkProp.FindPropertyRelative("grid" + num.ToString() + ".direction").intValue = 0;
                }

                rect.x += rect.width;
                rect.width = 100;
                EditorGUI.IntField(rect, GUIContent.none, linkProp.FindPropertyRelative("grid" + num.ToString() + ".direction").intValue);

                rect = GetSingleLineRect();
                EditorGUI.LabelField(rect, "Direction");

                EditorGUI.indentLevel--;
            }
        }

        public void DrawSceneLinks(SerializedProperty sceneLinksProp)
        {
            Rect rect = GetSingleLineRect();

            //EditorGUIUtility.labelWidth = 10f;
            EditorGUIUtility.fieldWidth -= 100f;

            EditorGUI.PropertyField(rect, sceneLinksProp, true);

            if (sceneLinksProp.isExpanded)
            {
                if (sceneLinksProp.arraySize > 0)
                {
                    for (int i = 0; i < sceneLinksProp.arraySize; i++)
                    {
                        NewLine();
                        if (sceneLinksProp.GetArrayElementAtIndex(i).isExpanded)
                        {
                            if (EditorGUIUtility.currentViewWidth < overflowWidth)
                                NewLine(2);
                            NewLine(13);
                        }
                    }

                    NewLine();
                }
                else
                {
                    NewLine(2);
                }
            }

            EditorGUIUtility.fieldWidth += 100f;

            // EditorGUIUtility.labelWidth = 0;
        }

        // TILE DATA METHODS ************************************************************************************************************************
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

        // DEBUG SETTINGS METHODS *******************************************************************************************************************
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

        // GRID INFO METHODS ************************************************************************************************************************
        private void DrawGridInfoEditor(SerializedProperty property, ref bool fold)
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
                background.height += (lineHeight + padding) * ((EditorGUIUtility.currentViewWidth < overflowWidth) ? 5.5f : 4.5f);
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
            dropdown.width -= 42;
            EditorGUI.PropertyField(dropdown, namesProp, GUIContent.none);

            dropdown = GetSingleLineRect();
            dropdown.x += m_position.size.x - 60;
            dropdown.width = 60;
            EditorGUI.IntField(dropdown, i);

            

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

                NewLine();
                rect.y += lineHeight + padding;
                EditorGUI.PropertyField(rect, GridInfoProperty.FindPropertyRelative("cameraPadding"));

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

                if (list.name == "gridLinks")
                    list.GetArrayElementAtIndex(list.arraySize - 1).FindPropertyRelative("width").intValue = 1;

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

                boolList.RemoveAt(list.arraySize);
            }
        }

        // HELPER METHODS ***************************************************************************************************************************
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

        private Rect GetSingleLineRect()
        {
            Rect rect = new Rect(m_position.position.x, lineCount, m_position.size.x, EditorGUIUtility.singleLineHeight);
            return rect;
        }

        private float IndentOffset()
        {
            return 10f * EditorGUI.indentLevel;
        }

        private void NewLine() => lineCount += lineHeight + padding;

        private void NewLine(float val) => lineCount += (lineHeight + padding) * val;

        private void Initialise(SerializedProperty property)
        {
            if (initialised)
                return;

            SerializedProperty gridProperty = property.FindPropertyRelative("gridInfo");
            SerializedProperty gridLinksOverrideProperty = property.FindPropertyRelative("nodeOverrides").FindPropertyRelative("gridLinks");
            SerializedProperty sceneLinksOverrideProperty = property.FindPropertyRelative("nodeOverrides").FindPropertyRelative("sceneLinks");

            if (gridLinksOverrideDropdowns == null || gridLinksOverrideDropdowns.Count != gridLinksOverrideProperty.arraySize)
            {
                gridLinksOverrideDropdowns = new List<bool>();

                for (int i = 0; i < gridLinksOverrideProperty.arraySize; i++)
                {
                    gridLinksOverrideDropdowns.Add(false);
                }
            }

            if (sceneLinksOverrideDropdowns == null || sceneLinksOverrideDropdowns.Count != sceneLinksOverrideProperty.arraySize)
            {
                sceneLinksOverrideDropdowns = new List<bool>();

                for (int i = 0; i < sceneLinksOverrideProperty.arraySize; i++)
                {
                    sceneLinksOverrideDropdowns.Add(false);
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
    }//*/
}