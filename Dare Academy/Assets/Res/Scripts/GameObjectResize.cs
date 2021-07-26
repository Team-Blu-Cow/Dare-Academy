using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectResize : MonoBehaviour
{
    [SerializeField]private Camera m_cam;
    [SerializeField]private SpriteRenderer m_spriteRenderer;

    void OnValidate()
    {
        m_spriteRenderer        = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        m_cam = Camera.main;
    }

    void Update()
    {
        Bounds bounds = m_cam.OrthographicBounds();

        transform.localScale = Vector3.one;

        var width = m_spriteRenderer.sprite.bounds.size.x;
        var height = m_spriteRenderer.sprite.bounds.size.y;

        transform.localScale = new Vector3( bounds.size.x / width, bounds.size.y / height,1);
    }
}
