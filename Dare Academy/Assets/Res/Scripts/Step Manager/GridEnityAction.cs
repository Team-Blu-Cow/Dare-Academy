using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionTypes
{
    MOVE,
    PASSTHROUGH,
    PUSHBACK,
    STATIC_ACTION
}

public class GridEnityAction
{
    public string animationName;
    public ActionTypes type;
    public JUtil.Grids.GridNodePosition position;
    public Vector3 targetPosition { get { return position.world; } }
    public int animationLayer;

    public GridEnityAction()
    {}

    public GridEnityAction(GridEnityAction action)
    {
        type            = action.type;
        position        = action.position;
        animationName   = action.animationName;
        animationLayer = action.animationLayer;
    }
}
