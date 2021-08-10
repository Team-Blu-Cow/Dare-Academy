using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MortarShotLand : MonoBehaviour
{
    public float disapearDistance = 3;
    public float speedMod = 2;
    private float currentYScale;

    private float speedModifier = 0;
    // private float alphaModifier = 0;

    private float initY;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color colour;

    [SerializeField] private GameObject explosionPrefab;

    // Start is called before the first frame update
    private void Start()
    {
        initY = transform.position.y;
        transform.position += Vector3.up * disapearDistance;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        colour = sr.color;
        sr.sortingOrder = Mathf.RoundToInt( disapearDistance);
        rb.velocity = Vector2.down*20;
        currentYScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        speedModifier += Time.deltaTime * speedMod;
        rb.velocity += Vector2.down * (speedModifier * 2);

        currentYScale += speedModifier * 0.01f;

        transform.localScale = new Vector3(transform.localScale.x, currentYScale, transform.localScale.z);

        sr.sortingOrder = Mathf.RoundToInt(transform.position.y - initY);

        if (transform.position.y <= initY)
        {
            if(explosionPrefab != null)
                GameObject.Instantiate(explosionPrefab, new Vector3(transform.position.x,initY-0.75f,transform.position.z), Quaternion.identity);

            App.CameraController.CameraShake(6f, 0.3f);
            Destroy(gameObject);
        }
    }
}