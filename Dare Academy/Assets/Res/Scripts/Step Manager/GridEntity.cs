using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;
using JUtil;

public class GridEntity : MonoBehaviour
{
    // MEMBERS ************************************************************************************

    public Vector2 testDirection;

    private bool m_isAttack = false;

    private int m_mass;
    private int m_speed;
    private int m_health = 1;

    [SerializeField] private int m_roomIndex = 0;

    public bool IsAttack { get { return m_isAttack; } set { m_isAttack = value; } }
    public int Mass { get { return m_mass; } set { m_mass = value; } }
    public int Speed { get { return m_speed; } set { m_speed = value; } }
    public int RoomIndex { get { return m_roomIndex; } set { m_roomIndex = value; } }

    public bool RemoveFromList { get { return m_health <= 0; } }

    private StepController m_stepController;

    private GridNode m_currentNode = null;
    private GridNode m_targetNode = null;
    private GridNode m_previousNode = null;

    public GridNodePosition Position
    { get { return m_currentNode.position; } }

    // INITIALISATION METHODS *********************************************************************
    private void Start()
    {
        m_currentNode = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(transform.position);
        if (m_currentNode == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_roomIndex = m_currentNode.roomIndex;

        transform.position = Position.world;
        m_targetNode = m_currentNode;

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        if (m_roomIndex == m_stepController.m_currentRoomIndex)
        {
            m_stepController.AddEntity(this);
        }
    }

    // STEP FLOW METHODS **************************************************************************
    // step flow: [move] -> [resolve move] -> [attack] -> [damage] -> [end] -> [draw] -> [analyse]

    virtual public void MoveStep()
    {
        // set currentNode to targetNode
        // keep a record of where we came from
        if (m_targetNode != null && m_currentNode != null)
        {
            if (m_previousNode != null)
                m_previousNode.RemoveEntity(this);
            m_currentNode.RemoveEntity(this);
            m_targetNode.RemoveEntity(this);

            m_previousNode = m_currentNode;
            m_currentNode = m_targetNode;
            m_targetNode = null;

            m_currentNode.AddEntity(this);
        }

        Debug.Log("Move Step");
    }

    virtual public void ResolveMoveStep()
    {
        // check for conflict on current node
        Debug.Log("Resolve Move Step");
    }

    virtual public void AttackStep()
    {
        Debug.Log("Attack Step");
    }

    virtual public void DamageStep()
    {
        Debug.Log("Damage Step");
    }

    virtual public void EndStep()
    {
        if (m_health <= 0)
        {
            // TODO @matthew/@jay - dont remove immediately to allow for death animation
            // kill entity
            m_stepController.RemoveEntity(this);
            GameObject.Destroy(gameObject);
        }

        // Debug.Log("End Step");
    }

    virtual public void AnalyseStep()
    {
        // set m_targetPosition
        SetTargetNode(testDirection);
        Debug.Log("Analyse Step");
    }

    virtual public void DrawStep()
    {
        Debug.Log("Draw Step");
    }

    // HELPER METHODS *****************************************************************************

    public void Update()
    {
        if (RoomIndex == -1)
            return;
        if (m_currentNode != null)
        {
            float xx = Mathf.Lerp(transform.position.x, m_currentNode.position.world.x, 0.5f);
            float yy = Mathf.Lerp(transform.position.y, m_currentNode.position.world.y, 0.5f);
            float zz = Mathf.Lerp(transform.position.z, m_currentNode.position.world.z, 0.5f);
            transform.position = new Vector3(xx, yy, zz);
        }
    }

    public void SetTargetNode(Vector2 direction)
    {
        int dir;
        float angle = direction.GetRotation();

        dir = Mathf.RoundToInt(angle) / 45;

        SetTargetNode(dir);
    }

    virtual public void SetTargetNode(int direction)
    {
        Mathf.Clamp(direction, 0, 7);

        m_previousNode = null;
        m_targetNode = m_currentNode.Neighbors[direction].reference;
    }
}