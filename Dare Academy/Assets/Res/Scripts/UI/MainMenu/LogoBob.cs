using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoBob : MonoBehaviour
{
    float startPos;

    public float multiplier = 1;
    public float bobSpeed = 1;
    public float offset = 0;
    private float currentOffset;
    private float angle;

    RectTransform rectTransform;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        startPos = rectTransform.anchoredPosition.y;
        angle = 0;
    }

    private void Update()
    {
        angle += Time.deltaTime*bobSpeed;
        if (angle >= 360)
            angle = angle - 360;

        currentOffset = Mathf.Sin(Mathf.Deg2Rad*angle);

        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPos + offset + (currentOffset * multiplier));
    }

}
