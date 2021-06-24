using UnityEngine;
using UnityEditor;

namespace CanvasTool
{
    [CustomEditor(typeof(CanvasManager))]
    [CanEditMultipleObjects]
    public class CanvasEditor : Editor
    {
        private int indentSize = 20;
        [SerializeField] private bool showLayers;
        private string layerName = "";

        private static GUIContent
            moveDownButtonContent = new GUIContent("\u2193", "Move Down"),
            moveUpButtonContent = new GUIContent("\u2191", "Move Up"),
            addButtonContent = new GUIContent("+", "Add"),
            deleteButtonContent = new GUIContent("-", "Delete");

        private GameObject newCanvas;

        public override void OnInspectorGUI()
        {
            CanvasManager canvasContoller = (CanvasManager)target;

            serializedObject.Update();

            GUILayout.Label("Canvases", "BoldLabel");

            // Resolution settings
            using (var HorizontalScope = new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Default Resolution", GUILayout.Width(110));
                Indent();
                canvasContoller.refrenenceResolution = EditorGUILayout.Vector2Field("", canvasContoller.refrenenceResolution);
            }

            // What canvas to open on Start
            using (var VerticalScope = new GUILayout.VerticalScope())
            {
                SerializedProperty canvas = serializedObject.FindProperty("canvases");

                if (canvasContoller.startingCanvas == null)
                {
                    canvasContoller.startingCanvas = new System.Collections.Generic.List<CanvasContainer>();
                }

                for (int x = 0; x < canvasContoller.startingCanvas.Count; x++)
                {
                    using (var HorizontalScope = new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Starting Canvas", GUILayout.Width(100));
                        canvasContoller.startingCanvas[x].canvas = (Canvas)EditorGUILayout.ObjectField(canvasContoller.startingCanvas[x].canvas, typeof(Canvas), true);
                        if (GUILayout.Button(deleteButtonContent))
                        {
                            canvasContoller.startingCanvas.Remove(canvasContoller.startingCanvas[x]);
                        }
                    }
                }

                using (var HorizontalScope = new GUILayout.HorizontalScope())
                {
                    Indent();
                    GUILayout.Label("Add Starting Canvas", GUILayout.Width(120));

                    if (GUILayout.Button(addButtonContent))
                    {
                        canvasContoller.startingCanvas.Add(new CanvasContainer());
                    }
                }
            }

            Indent();

            // Looping each cavas to show
            for (int i = 0; i < canvasContoller.CanvasAmount(); i++)
            {
                CanvasContainer currentCanvas = canvasContoller.GetCanvasContainer(i);
                SerializedProperty canvas = serializedObject.FindProperty("canvases");
                canvas = canvas.GetArrayElementAtIndex(i);

                if (canvas != null)
                {
                    // Show canvas name and add/Remove buttons when its closed in inspector
                    using (var HorizontalScope = new GUILayout.HorizontalScope())
                    {
                        SerializedProperty showInEditor = canvas.FindPropertyRelative("showInEditor");
                        showInEditor.boolValue = EditorGUILayout.Foldout(showInEditor.boolValue, "Canvas " + (i + 1) + " (" + currentCanvas.name + ")", true);

                        if (!currentCanvas.showInEditor)
                        {
                            if (GUILayout.Button(moveDownButtonContent, EditorStyles.miniButtonLeft))
                            {
                                canvasContoller.MoveDown(i);
                            }

                            if (GUILayout.Button(moveUpButtonContent, EditorStyles.miniButtonMid))
                            {
                                canvasContoller.MoveUp(i);
                            }

                            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight))
                            {
                                canvasContoller.RemoveCanvasContainer(i);
                            }
                        }
                    }

                    if (currentCanvas.showInEditor)
                    {
                        using (var VerticleScope = new GUILayout.VerticalScope("HelpBox"))
                        {
                            GUILayout.Space(16);

                            // Layer settings
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Layer", GUILayout.Width(100));
                                SerializedProperty layer = canvas.FindPropertyRelative("layer");

                                layer.intValue = EditorGUILayout.Popup(layer.intValue, canvasContoller.layerNames.ToArray());
                            }

                            // Canvas name
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Name", GUILayout.Width(100));
                                SerializedProperty name = canvas.FindPropertyRelative("name");

                                GUI.changed = false;
                                name.stringValue = GUILayout.TextField(name.stringValue, 50);
                                if (GUI.changed)
                                {
                                    currentCanvas.gameObject.name = name.stringValue + " (Canvas)";
                                }
                            }

                            // Desciption of the canvas
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Description", GUILayout.Width(100));
                                SerializedProperty desc = canvas.FindPropertyRelative("desc");

                                desc.stringValue = GUILayout.TextField(desc.stringValue, 50);
                            }

                            // Show what canvas is attached to it
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Canvas", GUILayout.Width(100));

                                EditorGUILayout.ObjectField(currentCanvas.canvas, typeof(Canvas), true);
                            }

                            // Set the resolution of the canvas
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Resolution", GUILayout.Width(100));
                                SerializedProperty canvasScaler = canvas.FindPropertyRelative("canvasScaler"); //???
                                SerializedProperty resolution = canvasScaler.FindPropertyRelative("referenceResolution");

                                currentCanvas.canvasScaler.referenceResolution = EditorGUILayout.Vector2Field("", currentCanvas.canvasScaler.referenceResolution);
                            }

                            // TODO:: How to transition to a new canvas
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                Indent();
                                GUILayout.Label("Transition", GUILayout.Width(100));
                                SerializedProperty transition = canvas.FindPropertyRelative("transition");

                                transition.intValue = EditorGUILayout.Popup(transition.intValue, System.Enum.GetNames(typeof(blu.TransitionType)));
                            }

                            Indent();

                            // Show a;ll the buttons on the canvas
                            GUILayout.Label("Buttons");
                            CreateEditor(currentCanvas.gameObject.GetComponent<ButtonWrapper>()).OnInspectorGUI();

                            Indent(10);

                            // Remove and move the canvas
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button(moveDownButtonContent, EditorStyles.miniButtonLeft))
                                {
                                    canvasContoller.MoveDown(i);
                                }

                                if (GUILayout.Button(moveUpButtonContent, EditorStyles.miniButtonMid))
                                {
                                    canvasContoller.MoveUp(i);
                                }

                                if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight))
                                {
                                    canvasContoller.RemoveCanvasContainer(i);
                                }
                            }

                            Indent(2);
                        }
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }

            // adding a new canvas
            using (var HorizontalScope = new GUILayout.HorizontalScope())
            {
                newCanvas = (GameObject)EditorGUILayout.ObjectField(newCanvas, typeof(GameObject), true);

                if (GUILayout.Button(addButtonContent))
                {
                    canvasContoller.AddCanvas(newCanvas);
                }
            }

            Indent(5);

            showLayers = EditorGUILayout.Foldout(showLayers, "Layers", true);

            if (showLayers)
            {
                int i = 0;
                foreach (string s in canvasContoller.layerNames)
                {
                    using (var VerticleScope = new GUILayout.VerticalScope("HelpBox"))
                    {
                        GUILayout.Label(s);

                        Indent(5);

                        // Kinda bad here, would liek to store them in a list or somethign but didnt seem to work
                        // This will work for now but coudl be umprved to be more efficiant
                        for (int j = 0; j < canvasContoller.CanvasAmount(); j++)
                        {
                            CanvasContainer currentCanvas = canvasContoller.GetCanvasContainer(j);

                            if (currentCanvas.layer == i)
                            {
                                using (var HorizontalScope = new GUILayout.HorizontalScope())
                                {
                                    Indent();
                                    GUILayout.Label(currentCanvas.name, GUILayout.Width(100));
                                    currentCanvas.layer = EditorGUILayout.Popup(currentCanvas.layer, canvasContoller.layerNames.ToArray());
                                }
                            }
                        }

                        if (s != "Default")
                        {
                            using (var HorizontalScope = new GUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button(moveDownButtonContent, EditorStyles.miniButtonLeft))
                                {
                                }

                                if (GUILayout.Button(moveUpButtonContent, EditorStyles.miniButtonMid))
                                {
                                }

                                if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight))
                                {
                                    canvasContoller.RemoveLayer(i);
                                    break;
                                }
                            }
                        }
                        i++;
                    }
                }

                Indent();

                using (var HorizontalScope = new GUILayout.HorizontalScope())
                {
                    layerName = GUILayout.TextField(layerName, 50, GUILayout.Width(300));

                    Indent();

                    if (GUILayout.Button(addButtonContent, EditorStyles.miniButtonMid))
                    {
                        canvasContoller.AddLayer(layerName);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Indent()
        {
            GUILayout.Space(indentSize);
        }

        private void Indent(int size)
        {
            GUILayout.Space(size);
        }
    }
}