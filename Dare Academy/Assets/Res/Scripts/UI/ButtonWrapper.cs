using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using blu;

namespace CanvasTool
{
    public class ButtonWrapper : MonoBehaviour
    {
        [SerializeField]
        public List<ButtonContainer> buttons = new List<ButtonContainer>();

        private void Start()
        {
            CanvasManager canvasManager = FindObjectOfType<CanvasManager>();

            foreach (ButtonContainer button in buttons)
            {
                if (button.open)
                    button.button.onClick.AddListener(delegate { canvasManager.OpenCanvas(canvasManager.GetCanvasContainers(button.canvas), button.stack); });
                else if (button.quit)
                    button.button.onClick.AddListener(delegate { App.GetModule<SceneModule>().Quit(); });
                else if (!button.swapScene)
                    button.button.onClick.AddListener(delegate { canvasManager.CloseCanvas(true); });
                else
                    button.button.onClick.AddListener(delegate
                    {
                        App.GetModule<SceneModule>().SwitchScene(button.sceneName, (blu.TransitionType)button.transition, (blu.LoadingBarType)button.loadingBar, button.test);
                    });

                //button.button.onClick.AddListener(delegate { blu.Application.ModuleManager.GetModule<AudioModule>().PlayAudioEvent("event:/UI/buttons/on click"); });
            }
        }

        public void AddButton()
        {
            GameObject Go = new GameObject("Button");
            Go.transform.SetParent(transform);

            GameObject btnGo = new GameObject("Text");
            btnGo.transform.SetParent(Go.transform);

            ButtonContainer button = new ButtonContainer();

            Button btn = Go.AddComponent<Button>();
            Image image = Go.AddComponent<Image>();
            TextMeshProUGUI text = btnGo.AddComponent<TextMeshProUGUI>();

            button.name = "Button";
            button.text = "Button";
            button.button = btn;
            button.textMeshPro = text;
            button.image = image;
            button.gameObject = Go;

            text.text = button.text;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 24;

            image.type = Image.Type.Sliced;
            image.SetNativeSize();

            Go.transform.localScale = new Vector3(1, 1, 1);
            Go.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);
            Go.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);

            btnGo.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            btnGo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            btnGo.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            buttons.Add(button);
        }

        public void RemoveButton(ButtonContainer container)
        {
            DestroyImmediate(container.button.gameObject);
            buttons.Remove(container);
        }
    }
}