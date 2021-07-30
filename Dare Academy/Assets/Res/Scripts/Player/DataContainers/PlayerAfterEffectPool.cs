using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterEffectPool : MonoBehaviour
{
    private GameObject m_afterImagePrefab;
    private GameObject m_afterImageContainer = null;

    private Queue<GameObject> m_availableObjects = new Queue<GameObject>();

    private int m_poolGowAmount = 10;

    public static PlayerAfterEffectPool Instance {get; private set;}

    private void OnValidate()
    {
        m_afterImagePrefab = Resources.Load<GameObject>("prefabs/GFX/PlayerDashAfterEffect");
    }

    private void Awake()
    {
        Instance = this;
        m_afterImageContainer = new GameObject("PlayerAfterImageContainer");
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < m_poolGowAmount; i++)
        {
            var instanceToAdd = Instantiate(m_afterImagePrefab);
            instanceToAdd.transform.SetParent(m_afterImageContainer.transform);

            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        m_availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if(m_availableObjects.Count <= 0)
            GrowPool();

        var instance = m_availableObjects.Dequeue();

        instance.SetActive(true);

        return instance;
    }
}
