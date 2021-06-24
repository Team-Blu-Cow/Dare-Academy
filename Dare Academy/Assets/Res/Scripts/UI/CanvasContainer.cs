using UnityEngine;
using UnityEngine.UI;
using System;

namespace CanvasTool
{
    [Serializable]
    public class CanvasContainer
    {
        public GameObject gameObject;
        public Canvas canvas;
        public CanvasScaler canvasScaler;

        public string name;
        public string desc;
        public int layer;
        public int transition = 0;
        public bool showInEditor = false;

        virtual public void CloseCanvas()
        {
            canvas.enabled = false;
        }

        virtual public void OpenCanvas()
        {
            if (canvas)
                canvas.enabled = true;
        }
    }
}