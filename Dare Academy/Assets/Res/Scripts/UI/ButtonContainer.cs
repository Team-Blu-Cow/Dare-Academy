using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

[Serializable]
public class ButtonContainer
{
    public List<Canvas> canvas;

    // Button settings
    public Button button;

    public Image image;
    public TextMeshProUGUI textMeshPro;
    public GameObject gameObject;
    public string text;
    public string name;

    // Canvas Swaps

    public bool open;
    public bool closeAll;
    public bool stack;
    public bool quit;

    // Scene swap

    public bool test;
    public int transition;
    public int loadingBar;
    public bool swapScene;

    public bool showInEditor = false;
}