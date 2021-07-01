using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnitity : GridEntity
{
    [SerializeField] private Vector2 testDirection = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    public override void AnalyseStep()
    {
        SetTargetNode(testDirection, moveSpeed);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, m_movementDirection);
        }
    }
}