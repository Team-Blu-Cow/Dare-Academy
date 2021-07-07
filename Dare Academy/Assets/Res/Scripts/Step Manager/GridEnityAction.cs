using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTypes
{
    MOVE,
    PASSTHROUGH,
    PUSHBACK
}

public class GridEnityAction
{
    public ActionTypes type;
    public JUtil.Grids.GridNodePosition position;
    public Vector3 targetPosition { get { return position.world; } }

    public GridEnityAction()
    {}

    public GridEnityAction(GridEnityAction action)
    {
        type = action.type;
        position = action.position;
    }
}
