using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TelegraphDrawer
{
    [SerializeField] private List<GameObject> m_spriteList;
    [SerializeField] private GameObject m_telegraphParentGameobject;
    [SerializeField] private GameObject[] m_prefabList;

    public enum Type
    {
        MOVE,
        ATTACK,
        SHOOT,
        BLOCK
    }

    public void Initialise()
    {
        m_spriteList = new List<GameObject>();
    }

    public void OnValidate()
    {
        m_prefabList = Resources.LoadAll<GameObject>("prefabs/Telegraph Types");

        if(m_telegraphParentGameobject == null)
        {
            m_telegraphParentGameobject = new GameObject("Telegraph Container");
            m_telegraphParentGameobject.transform.position = Vector3.zero;
        }
    }

    public void CreateTelegraph(GridNode node, Type type)
    {
        GameObject newTelegraph =  GameObject.Instantiate(m_prefabList[GetPrefabIndex(type)], node.position.world, Quaternion.identity);

        newTelegraph.transform.parent = m_telegraphParentGameobject.transform;

        m_spriteList.Add(newTelegraph);
    }

    private int GetPrefabIndex(Type type)
    {
        switch (type)
        {
            case Type.MOVE:
                return 0;

            case Type.ATTACK:
                return 1;

            default:
                return 0;
        }

    }

    public void OnStep()
    {
        for(int i = m_spriteList.Count-1; i >= 0; i--)
        {
            GameObject.Destroy(m_spriteList[i]);
        }

        m_spriteList.Clear();
    }

}
