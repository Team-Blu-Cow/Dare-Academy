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

    [SerializeField] private int m_roomIndex = 0;

    public bool IsAttack    { get { return m_isAttack; } set { m_isAttack = value; } }
    public int Mass         { get { return m_mass; } set { m_mass = value; } }
    public int Speed        { get { return m_speed; } set { m_speed = value; } }
    public int RoomIndex    { get { return m_roomIndex; } set { m_roomIndex = value; } }

    private StepController m_stepController;

    GridNodePosition m_position;
    GridNodePosition m_targetPosition;
    GridNode m_node;

    public GridNodePosition Position
    { get { return m_position; } set { m_position = value; } }

    // INITIALISATION METHODS *********************************************************************
    private void Start()
    {
        m_node = App.GetModule<LevelModule>().MetaGrid.GetNodeFromWorld(transform.position);
        if (m_node == null)
        {
            m_roomIndex = -1;
            return;
        }

        m_position = m_node.position;
        m_roomIndex = m_node.roomIndex;

        transform.position = m_position.world;
        m_targetPosition = m_position;

        m_stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        if (m_roomIndex == m_stepController.m_currentRoomIndex)
        {
            m_stepController.AddEntity(this);
        }
    }

    // STEP FLOW METHODS **************************************************************************
    // step flow: [move] -> [resolve move] -> [attack] -> [damage] -> [end] -> [draw] -> [analyse]
    virtual public void AnalyseStep()
    {
        Debug.Log("Analyse Step");
    }
    
    virtual public void MoveStep()
    {
        Debug.Log("Move Step");
        MoveTile(testDirection);
    }

    virtual public void ResolveMoveStep()
    {
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
        Debug.Log("End Step");
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

        float xx = Mathf.Lerp(transform.position.x, m_position.world.x, 0.5f);
        float yy = Mathf.Lerp(transform.position.y, m_position.world.y, 0.5f);
        float zz = Mathf.Lerp(transform.position.z, m_position.world.z, 0.5f);

        transform.position = new Vector3(xx, yy, zz);
    }

    public void MoveTile(Vector2 direction)
    {
        int dir;
        float angle = direction.GetRotation();

        dir = Mathf.RoundToInt(angle) / 45;

        MoveTile(dir);
    }

    virtual public void MoveTile(int direction)
    {
        Mathf.Clamp(direction, 0, 7);
        GridNode node = App.GetModule<LevelModule>().Grid(m_roomIndex)[Position];

        GridNode targetNode = node.Neighbors[direction].reference;

        if (targetNode == null)
            return;

        m_position = targetNode.position;
    }
}
