using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class EnemyHealth : MonoBehaviour
{
    private int m_storedHealth;
    private GridEntity m_entity;

    private List<GameObject> m_hearts = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        m_storedHealth = m_entity.Health;

        for (int i = 0; i < m_entity.Health; i++)
        {
            AddHeart();
        }
    }

    private void OnValidate()
    {
        m_entity = GetComponentInParent<GridEntity>();
    }

    private void AddHeart()
    {
        //Add sprite
        GameObject tempGo = new GameObject("Health");

        tempGo.transform.SetParent(transform);
        tempGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        SpriteRenderer spriteRenderer = tempGo.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("GFX/HeartFull"); ;
        spriteRenderer.sortingLayerName = "World Space UI";

        if (m_hearts.Count <= 0)
            // Shuffle to the side
            tempGo.transform.localPosition = new Vector3(0.3f, 1.2f, 0);
        else
            tempGo.transform.localPosition = new Vector3(m_hearts[m_hearts.Count - 1].transform.localPosition.x + 0.6f, 1.2f, 0);

        m_hearts.Add(tempGo);

        foreach (var heart in m_hearts)
        {
            heart.transform.localPosition = heart.transform.localPosition - new Vector3(0.3f, 0, 0);
            // LeanTween.moveLocalX(heart, heart.transform.localPosition.x - 0.6f, 0.0f);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_storedHealth > m_entity.Health)
        {
            for (int i = 0; i < m_storedHealth - m_entity.Health; i++)
                OnHit();
        }
    }

    private void OnHit()
    {
        //Remove sprite

        //Move across
    }
}