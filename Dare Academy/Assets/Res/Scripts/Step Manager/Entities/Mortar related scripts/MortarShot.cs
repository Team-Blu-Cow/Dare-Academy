using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MortarShot : GridEntity
{
    [SerializeField] public int m_damage = 1;
    public int counter = 0;
    public int m_landTime = 3;
    bool m_landed = false;

    public float m_initialScale = 0.5f;
    public float m_scale = 0.5f;

    GameObject m_mortarLandPrefab;

    public void Start()
    {
        base.Start();

        m_scale = m_initialScale;
        transform.localScale = Vector3.one * m_scale;

        m_mortarLandPrefab = Resources.Load<GameObject>("prefabs/Entities/MortarLand");
    }

    public void Init()
    {
        m_currentNode = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(transform.position);

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;
        m_stepController.RoomChangeEvent += RoomChange;


        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        transform.position = Position.world;

        m_stepController.AddEntity(this);

        m_landed = false;

        m_scale = m_initialScale;
        transform.localScale = Vector3.one * m_scale;
    }

    public void IncreaseScale()
    {
        float increment = (1f / (m_landTime)) * counter;

        m_scale = Mathf.Lerp(m_initialScale, 1.1f, increment);

        //transform.localScale 
        LeanTween.scale(gameObject, Vector3.one * m_scale, 0.1f);
    }

    public void Impact()
    {
        m_currentNode.AddEntity(this);
        m_roomIndex = m_currentNode.roomIndex;

        m_health = int.MinValue;

        m_landed = true;

        
    }

    public override void AnalyseStep()
    {
        counter++;

        IncreaseScale();

        if (counter >= m_landTime)
        {
            Impact();
        }
    }

    public override void DamageStep()
    {
        if (!m_landed)
            return;

        Instantiate(m_mortarLandPrefab, m_currentNode.position.world, Quaternion.identity);

        foreach (var e in m_currentNode.GetGridEntities())
        {
            if (e.Flags.IsFlagsSet(GridEntityFlags.Flags.isKillable))
            {
                //e.Health -= m_damage;
                e.OnHit(m_damage);
            }
        }
    }

    public override void EndStep()
    {
        if (m_landed)
            OnDeath();
    }

    public override void RoomChange()
    {
        OnDeath();
    }

}
