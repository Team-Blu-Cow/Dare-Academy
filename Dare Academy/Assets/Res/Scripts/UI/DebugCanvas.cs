using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class DebugCanvas : MonoBehaviour
{
    public class OutputInfo<T> : Base
    {
        public string name;
        public T info;

        public virtual string DisplayInfo()
        {
            return info.ToString();
        }
    }

    public class Base { }

    private float delay;
    private TextMeshProUGUI text;

    private List<Base> lines = new List<Base>();

    public void AddList<T>(T line) where T : Base
    {
        lines.Add(line);
    }

    private void AddLine<T>(object test)
    {
        OutputInfo<T> display = (OutputInfo<T>)test;
        GetComponentInChildren<TextMeshProUGUI>().text += display.name + ": " + display.DisplayInfo() + ".\n";
    }

    private void ClearText()
    {
        text.text = "Debug Log:\n";
    }

    // Start is called before the first frame update
    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        ClearText();
    }

    // Update is called once per frame
    private void Update()
    {
        if (delay > 0.5f)
        {
            delay = 0;

            ClearText();
            foreach (var line in lines)
            {
                var con = Convert.ChangeType(line, line.GetType());

                if (line.GetType() == typeof(OutputInfo<Vector2>))
                {
                    AddLine<Vector2>(con);
                }
                else if (line.GetType() == typeof(OutputInfo<int>))
                {
                    AddLine<int>(con);
                }
                else if (line.GetType() == typeof(OutputInfo<float>))
                {
                    AddLine<float>(con);
                }
                else if (line.GetType() == typeof(OutputInfo<string>))
                {
                    AddLine<string>(con);
                }
                else
                {
                    Debug.LogWarning("Type not recognised, Add to Debug Canvas script");
                }
            }
        }
        else
        {
            delay += Time.deltaTime;
        }
    }
}