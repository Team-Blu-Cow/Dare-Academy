using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;

public class WyrmUIHealth : MonoBehaviour
{
    [SerializeField, HideInInspector] private Sprite[] m_healthSprites = new Sprite[2];

    [SerializeField] private WyrmHead m_startHead;
    [HideInInspector] public WyrmHead m_splitHead;

    private void OnValidate()
    {
        m_healthSprites[0] = Resources.Load<Sprite>("GFX/UI/heart");
        m_healthSprites[1] = Resources.Load<Sprite>("GFX/UI/heart_empty");
    }

    public void FightStart()
    {
        for (int i = 0; i < m_startHead.Health; i++)
        {
            AddHealth();
        }
        GetComponent<Canvas>().enabled = true;
    }

    public void FightEnd()
    {
        GetComponent<Canvas>().enabled = false;
    }

    public void FightSplit()
    {
    }

    public void AddHealth()
    {
        GameObject newHealth = new GameObject("Health", typeof(RectTransform), typeof(Image)); // Create new object
        newHealth.transform.SetParent(transform.GetChild(0)); // Set parent

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f); // Set scale
        Image image = newHealth.GetComponent<Image>();
        image.sprite = m_healthSprites[0];
    }
}