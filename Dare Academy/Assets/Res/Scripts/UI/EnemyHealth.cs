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
        if (m_entity == null)
            m_entity = GetComponentInParent<GridEntity>();

        m_storedHealth = m_entity.Health;
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
        tempGo.transform.localScale = Vector3.one * 0.4f;

        SpriteRenderer spriteRenderer = tempGo.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("GFX/HeartFull");
        spriteRenderer.sortingLayerName = "World Space UI";

        if (m_hearts.Count <= 0)
            // Shuffle to the side
            tempGo.transform.localPosition = new Vector3(0.2f, 1.2f, 0);
        else
            tempGo.transform.localPosition = new Vector3(m_hearts[m_hearts.Count - 1].transform.localPosition.x + 0.4f, 1.2f, 0);

        m_hearts.Add(tempGo);

        foreach (var heart in m_hearts)
        {
            heart.transform.localPosition = heart.transform.localPosition - new Vector3(0.2f, 0, 0);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_storedHealth > m_entity.Health)
        {
            if (m_hearts.Count == 0)
            {
                for (int i = 0; i < m_entity.Health + (m_storedHealth - m_entity.Health); i++)
                {
                    AddHeart();
                }
            }

            for (int i = 0; i < m_storedHealth - m_entity.Health; i++)
                OnHit();

            m_storedHealth = m_entity.Health;
        }
    }

    private void OnHit()
    {
        //Remove sprite
        if (m_entity.Health > 0)
            m_hearts[m_entity.Health].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("GFX/HeartEmpty");
    }
}