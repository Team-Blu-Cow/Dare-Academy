using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobSprite : MonoBehaviour
{
    float startPos;

    public float multiplier = 1;
    public float bobSpeed = 1;
    public float offset = 0;
    private float currentOffset;
    private float angle;

    void Start()
    {
        startPos = transform.position.y;
        angle = 0;
    }

    private void Update()
    {
        angle += Time.deltaTime * bobSpeed;
        if (angle >= 360)
            angle = angle - 360;

        currentOffset = Mathf.Sin(Mathf.Deg2Rad * angle);

        transform.position = new Vector3(transform.position.x, startPos + offset + (currentOffset * multiplier), transform.position.z);
    }
}
