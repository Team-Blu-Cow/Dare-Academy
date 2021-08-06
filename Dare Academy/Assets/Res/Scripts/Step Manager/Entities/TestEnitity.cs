using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnitity : GridEntity
{
    [SerializeField] private Vector2Int testDirection = new Vector2Int();
    [SerializeField] private int moveSpeed = 1;

    public override void AnalyseStep()
    {
        SetMovementDirection(testDirection, moveSpeed);

        m_animationController.SetDirection(-testDirection.x, 1);
    }

    protected override void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.white;
            // Gizmos.DrawRay(transform.position, m_movementDirection);
        }
    }
}