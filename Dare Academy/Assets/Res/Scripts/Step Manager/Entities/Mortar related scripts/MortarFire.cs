using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarFire : MonoBehaviour
{
    public float disapearDistance = 3;
    public float shotSpeed = 1.1f;
    private float currentAlpha = 1;

    private float speedModifier = 0;
    private float alphaModifier = 0;

    private float initY;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color colour;

    void Start()
    {
        initY = transform.position.y;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        colour = sr.color;
        rb.velocity = Vector2.up * 30;
    }
    
    void Update()
    {
        speedModifier += Time.deltaTime*shotSpeed;
        rb.velocity -= Vector2.up * (speedModifier);

        sr.color = new Color(colour.r, colour.g, colour.b, currentAlpha);

        sr.sortingOrder = Mathf.RoundToInt(transform.position.y - initY);

        if (transform.position.y > initY + disapearDistance)
        {
            alphaModifier += Time.deltaTime;
            currentAlpha -= (alphaModifier * 0.1f);
        }

        if (currentAlpha < 0)
            Destroy(gameObject);
    }
}
