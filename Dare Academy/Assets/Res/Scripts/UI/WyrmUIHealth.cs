using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;

public class WyrmUIHealth : MonoBehaviour
{
    [SerializeField, HideInInspector] private Sprite[] m_healthSprites = new Sprite[2];

    private List<Image> m_addHealth = new List<Image>();
    private List<Image> m_addHealthSplit = new List<Image>();

    [SerializeField] private WyrmHead m_startHead;
    [HideInInspector] public WyrmHead m_splitHead;

    private int m_deaths = 0;

    private bool m_split = false;

    private void OnValidate()
    {
        m_healthSprites[0] = Resources.Load<Sprite>("GFX/UI/heart");
        m_healthSprites[1] = Resources.Load<Sprite>("GFX/UI/heart_empty");
    }

    public void Hit(int health, WyrmSection head)
    {
        if (m_split)
        {
            if (head == m_startHead)
                m_addHealth[health].sprite = m_healthSprites[1];
            else
                m_addHealthSplit[health].sprite = m_healthSprites[1];
        }
        else
            m_addHealth[health].sprite = m_healthSprites[1];
    }

    public void FightStart()
    {
        for (int i = 0; i < m_startHead.Health; i++)
        {
            AddHealth(transform.GetChild(0), m_addHealth);
        }
        GetComponent<Canvas>().enabled = true;
    }

    public void FightEnd()
    {
        m_deaths++;

        if (m_deaths > 1)
            GetComponent<Canvas>().enabled = false;
    }

    public void FightSplit()
    {
        m_split = true;

        foreach (Image image in m_addHealth)
            Destroy(image.gameObject);

        m_addHealth.Clear();

        for (int i = 0; i < m_startHead.Health; i++)
            AddHealth(transform.GetChild(2).GetChild(0), m_addHealth);

        for (int i = 0; i < m_splitHead.Health; i++)
            AddHealth(transform.GetChild(2).GetChild(1), m_addHealthSplit);
    }

    public void AddHealth(Transform parent, List<Image> list)
    {
        GameObject newHealth = new GameObject("Health", typeof(RectTransform), typeof(Image)); // Create new object
        newHealth.transform.SetParent(parent); // Set parent

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f); // Set scale
        Image image = newHealth.GetComponent<Image>();
        image.sprite = m_healthSprites[0];

        list.Add(image);
    }
}