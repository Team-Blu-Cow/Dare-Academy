using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CanvasTool
{
    [System.Serializable]
    public class CanvasManager : MonoBehaviour
    {
        // List of all the canvases in the scene

        public List<CanvasContainer> canvases;
        public List<string> layerNames;

        private int sortingBoost = 20;

        private CanvasContainer overlay = new CanvasContainer();
        [SerializeField] public List<CanvasContainer> startingCanvas = new List<CanvasContainer>();

        // Stack of open canvases
        private Stack<CanvasContainer> openCanvases = new Stack<CanvasContainer>();

        public CanvasContainer topCanvas => openCanvases.Peek();

        public Vector2 refrenenceResolution = new Vector2(1600, 900);

        private void OnValidate()
        {
            if (canvases == null)
            {
                canvases = new List<CanvasContainer>();
            }

            if (layerNames == null)
            {
                layerNames = new List<string>();
                layerNames.Add("Default");
            }
        }

        private void Awake()
        {
            if (overlay.gameObject == null)
            {
                GameObject Go = new GameObject("Overlay");

                Go.transform.SetParent(transform);

                // Add compenents to the game Object
                Canvas canvas = Go.AddComponent<Canvas>();
                CanvasScaler canvasScaler = Go.AddComponent<CanvasScaler>();
                Go.AddComponent<GraphicRaycaster>();
                Image image = Go.AddComponent<Image>();

                // Set up added components
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.enabled = false;

                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = refrenenceResolution;

                image.color = new Color(0, 0, 0, 0.7f);

                overlay.canvas = canvas;
                overlay.canvasScaler = canvasScaler;
                overlay.gameObject = Go;
            }

            CloseCanvas(true);

            if (startingCanvas.Count > 0)
            {
                OpenCanvas(startingCanvas, true);
            }
            else if (canvases.Count > 0)
            {
                OpenCanvas(canvases[0]);
            }
        }

        public void OpenCanvas(List<CanvasContainer> containers, bool stack = false)
        {
            foreach (CanvasContainer container in containers)
            {
                if (container != null)
                {
                    // If the canvas is already open
                    if (openCanvases.Contains(container))
                    {
                        // Close untill at desired canvas
                        while (openCanvases.Peek() != container)
                        {
                            // Close canvases
                            CanvasContainer top = openCanvases.Pop();
                            top.CloseCanvas();
                        }
                        overlay.canvas.sortingOrder = openCanvases.Count + sortingBoost;
                        break;
                    }

                    if (stack)
                    {
                        //close on same layer
                        if (container.layer != 0)
                        {
                            foreach (CanvasContainer canvas in canvases)
                            {
                                if (container.layer == canvas.layer)
                                {
                                    canvas.CloseCanvas();
                                }
                            }
                        }

                        openCanvases.Push(container);
                    }
                    else
                    {
                        // Close canvases
                        foreach (CanvasContainer canvas in canvases)
                        {
                            canvas.CloseCanvas();
                        }
                        openCanvases.Clear();
                    }
                }
            }

            foreach (CanvasContainer container in containers)
            {
                if (container != null)
                {
                    container.OpenCanvas();
                }
            }
            overlay.canvas.sortingOrder = openCanvases.Count + sortingBoost;
        }

        public void OpenCanvas(string container, bool stack = false)
        {
            OpenCanvas(GetCanvasContainer(container), stack);
        }

        public void OpenCanvas(CanvasContainer container, bool stack = false)
        {
            //overlay.canvas.enabled = true;
            if (container == null)
            {
                Debug.LogWarning("Tring to open a non-exiting container");
                return;
            }

            // If the canvas is already open
            if (openCanvases.Contains(container))
            {
                // Close untill at desired canvas
                while (openCanvases.Peek() != container)
                {
                    // Close canvases
                    CanvasContainer top = openCanvases.Pop();
                    top.CloseCanvas();
                }
                overlay.canvas.sortingOrder = openCanvases.Count + sortingBoost;
                return;
            }

            if (stack)
            {
                //close on same layer
                if (container.layer != 0)
                {
                    foreach (CanvasContainer canvas in canvases)
                    {
                        if (container.layer == canvas.layer)
                        {
                            canvas.CloseCanvas();
                        }
                    }
                }

                openCanvases.Push(container);
            }
            else
            {
                // Close canvases
                foreach (CanvasContainer canvas in canvases)
                {
                    canvas.CloseCanvas();
                }
                openCanvases.Clear();
            }

            container.OpenCanvas();

            if (overlay.canvas)
                overlay.canvas.sortingOrder = openCanvases.Count + sortingBoost;
        }

        public void CloseCanvas(bool all = false)
        {
            if (all)
            {
                foreach (CanvasContainer canvas in canvases)
                {
                    canvas.CloseCanvas();
                }
            }
            else
            {
                CanvasContainer top = openCanvases.Pop();
                top.CloseCanvas();
                overlay.canvas.sortingOrder = openCanvases.Count - 1;
            }
        }

        public void MoveUp(int index)
        {
            if (index - 1 >= 0)
            {
                CanvasContainer temp = GetCanvasContainer(index);
                canvases.RemoveAt(index);
                canvases.Insert(index - 1, temp);
                SetSortingOrder();
            }
        }

        public void MoveDown(int index)
        {
            if (index + 1 < canvases.Count)
            {
                CanvasContainer temp = GetCanvasContainer(index);
                canvases.RemoveAt(index);
                canvases.Insert(index + 1, temp);
                SetSortingOrder();
            }
        }

        private void SetSortingOrder()
        {
            int i = 0;
            foreach (CanvasContainer container in canvases)
            {
                container.canvas.sortingOrder = i;
                i++;
            }
        }

        public Canvas GetCanvasIndex(int in_index)
        {
            if (in_index <= canvases.Count)
                return canvases[in_index].canvas;
            else
                return null;
        }

        #region GetContainer

        public CanvasContainer GetCanvasContainer(string in_name)
        {
            return canvases.Find(i => i.name == in_name);
        }

        public CanvasContainer GetCanvasContainer(Canvas in_canvas)
        {
            return canvases.Find(i => i.canvas == in_canvas);
        }

        public List<CanvasContainer> GetCanvasContainers(List<Canvas> in_canvas)
        {
            List<CanvasContainer> temp = new List<CanvasContainer>();
            foreach (Canvas canvas in in_canvas)
            {
                temp.Add(canvases.Find(i => i.canvas == canvas));
            }
            return temp;
        }

        public CanvasContainer GetCanvasContainer(int in_index)
        {
            if (in_index < canvases.Count)
                return canvases[in_index];
            else
                return null;
        }

        #endregion GetContainer

        #region RemoveCanvas

        public bool RemoveCanvasContainer(string name)
        {
            CanvasContainer temp = GetCanvasContainer(name);
            if (temp != null)
            {
                CleanUpContainer(temp);
            }
            else
            {
                Debug.LogWarning("[App/CanvasManager]: Could not find canvas: " + name);
                return false;
            }

            bool x = canvases.Remove(temp);
            SetSortingOrder();
            return x;
        }

        public bool RemoveCanvasContainer(Canvas canvas)
        {
            CanvasContainer temp = GetCanvasContainer(canvas);
            CleanUpContainer(temp);
            bool x = canvases.Remove(temp);
            SetSortingOrder();
            return x;
        }

        public bool RemoveCanvasContainer(int index)
        {
            CleanUpContainer(GetCanvasContainer(index));
            if (index <= canvases.Count)
            {
                canvases.RemoveAt(index);
                SetSortingOrder();
                return true;
            }
            return false;
        }

        #endregion RemoveCanvas

        public int CanvasAmount()
        {
            return canvases.Count;
        }

        // Add a new canvas to the scene
        public void AddCanvas()
        {
            CanvasContainer canvasContainer = new CanvasContainer();

            // make a new game object and setup
            GameObject Go = new GameObject("Canvas");
            Go.transform.SetParent(transform);

            // Add compenents to the game Object
            Canvas canvas = Go.AddComponent<Canvas>();
            CanvasScaler canvasScaler = Go.AddComponent<CanvasScaler>();
            Go.AddComponent<GraphicRaycaster>();
            Go.AddComponent<ButtonWrapper>();

            // Set up added components
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = refrenenceResolution;

            canvasContainer.canvas = canvas;
            canvasContainer.canvasScaler = canvasScaler;
            canvasContainer.gameObject = Go;

            canvases.Add(canvasContainer);
            SetSortingOrder();
        }

        public void AddCanvas(GameObject canvasGO)
        {
            CanvasContainer canvasContainer = new CanvasContainer();

            if (canvasGO == null)
            {
                canvasGO = new GameObject("Canvas");
            }
            else
            {
                canvasContainer.name = canvasGO.name;
            }

            // make a new game object and setup
            canvasGO.transform.SetParent(transform);

            // Add compenents to the game Object
            if (!canvasGO.TryGetComponent(out Canvas Canvas))
            {
                Canvas canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasContainer.canvas = canvas;
            }
            else
            {
                canvasContainer.canvas = Canvas;
                canvasContainer.canvas.sortingOrder = sortingBoost * 2;
            }

            if (!canvasGO.TryGetComponent(out CanvasScaler CanvasScaler))
            {
                CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = refrenenceResolution;
                canvasContainer.canvasScaler = canvasScaler;
            }
            else
            {
                canvasContainer.canvasScaler = CanvasScaler;
            }

            if (!canvasGO.TryGetComponent(out GraphicRaycaster GraphicRaycaster))
            {
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            if (!canvasGO.TryGetComponent(out ButtonWrapper ButtonWrapper))
            {
                canvasGO.AddComponent<ButtonWrapper>();
            }

            canvasContainer.gameObject = canvasGO;

            if (Application.isPlaying)
                canvasContainer.CloseCanvas();

            canvases.Add(canvasContainer);
            SetSortingOrder();
        }

        // Deletes the game object of the canvas
        private void CleanUpContainer(CanvasContainer container)
        {
            if (container.canvas)
                DestroyImmediate(container.canvas.gameObject);
        }

        public void AddLayer(string s)
        {
            if (!layerNames.Contains(s))
            {
                layerNames.Add(s);
            }
            else
                Debug.LogWarning("Cant add a layer with the same name");
        }

        public void RemoveLayer(int index)
        {
            layerNames.RemoveAt(index);
        }
    }
}