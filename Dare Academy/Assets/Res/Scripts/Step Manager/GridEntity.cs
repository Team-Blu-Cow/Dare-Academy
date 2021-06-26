using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;
using JUtil;

public class GridEntity : MonoBehaviour
{
    // MEMBERS ************************************************************************************

    private bool _isAttack = false;

    private int _mass;
    private int _speed;

    [SerializeField] private int _roomIndex = 0;

    public bool IsAttack    { get { return _isAttack; } set { _isAttack = value; } }
    public int Mass         { get { return _mass; } set { _mass = value; } }
    public int Speed        { get { return _speed; } set { _speed = value; } }
    public int RoomIndex    { get { return _roomIndex; } set { _speed = _roomIndex; } }

    private StepController _stepController;

    JUtil.Grids.GridNodePosition _position;
    public JUtil.Grids.GridNodePosition Position
    { get { return _position; } set { _position = value; } }

    // INITIALISATION METHODS *********************************************************************
    private void Start()
    {
        _stepController = App.GetModule<LevelModule>().LevelManager.StepController;

        if (_roomIndex == _stepController.currentRoomIndex)
        {
            _stepController.AddEntity(this);
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
        GridNode node = App.GetModule<LevelModule>().Grid(_roomIndex)[Position];

        GridNode targetNode = node.Neighbors[direction].reference;
    }
}
