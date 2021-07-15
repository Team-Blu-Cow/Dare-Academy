using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private ScriptedActionQueue m_actionQueue;

    public int m_queuePos = 0;

    private SpriteRenderer sr;

    private Animator anim;

    private GameObject m_queuedDialoudeAction = null;

    protected override void OnValidate()
    {
        base.OnValidate();

        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (m_prefab == null)
        {
            Debug.LogWarning($"ScriptableEntity [{gameObject.name}] has no assigned prefab, killing self");
            Kill();
        }
        else
        {
            SpriteRenderer otherSr = m_prefab.GetComponentInChildren<SpriteRenderer>();

            if (otherSr)// TODO @matthew - log warning if failed
            {
                sr.sprite = otherSr.sprite;
            }

            anim = GetComponent<Animator>();
            Animator otherAnim = m_prefab.GetComponent<Animator>();

            // TODO @matthew - log this failing
            if (anim != null && otherAnim != null)
            {
                anim.runtimeAnimatorController = otherAnim.runtimeAnimatorController;
            }
        }

        if (m_actionQueue.m_actionList.Count == 0)
            ReplaceWithPrefab(); // TODO @matthew - log warning
    }

    public override void ResetAnimations()
    {
        base.ResetAnimations();

        if (m_queuedDialoudeAction != null)
        {
            blu.App.GetModule<blu.DialogueModule>().StartDialogue(m_queuedDialoudeAction);
            m_queuedDialoudeAction = null;
        }
    }

    public override void AnalyseStep()
    {
        bool stepQueue = true;

        if (m_actionQueue.m_actionList.Count == 0)
        {
            // TODO @matthew - log warning
            ReplaceWithPrefab();
            return;
        }

        if (m_queuePos == m_actionQueue.m_actionList.Count)
        {
            ReplaceWithPrefab();
            return;
        }

        ScriptedActionQueue.ActionWrapper currentAction = m_actionQueue.m_actionList[m_queuePos];

        switch (currentAction.type)
        {
            case ScriptedActionQueue.ActionType.None:
                break;

            case ScriptedActionQueue.ActionType.Kill:
                Kill();
                break;

            case ScriptedActionQueue.ActionType.Move:
                MoveAction(currentAction.moveData);
                break;

            case ScriptedActionQueue.ActionType.WaitTurns:
                stepQueue = WaitAction(currentAction);
                break;

            case ScriptedActionQueue.ActionType.Dialogue:
                m_queuedDialoudeAction = currentAction.gameObject;
                break;

            default:
                Debug.LogWarning($"ScriptedEntity [{gameObject.name}] could not resolve action [type = {currentAction.type.ToString()}]");
                break;
        }
        if (stepQueue)
            m_queuePos++;
    }

    protected void ReplaceWithPrefab()
    {
        if (m_prefab)
        {
            GameObject obj = GameObject.Instantiate(m_prefab, m_currentNode.position.world, Quaternion.identity);
            if (obj == null)
            {
                Debug.LogWarning($"ScriptableEntity [{gameObject.name}] could not instantiate prefab [prefab = {m_prefab.name}]");
            }
        }
        else
        {
            Debug.LogWarning($"ScriptableEntity [{gameObject.name}] prefab was null");
        }

        RemoveFromCurrentNode();
        m_currentNode = null;

        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Kill();
        return;
    }

    protected void MoveAction(ScriptedActionQueue.MoveData data)
    {
        if (data != null)
        {
            switch (data.direction)
            {
                case ScriptedActionQueue.MoveDirections.North:
                    SetMovementDirection(Vector2Int.up, data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.East:
                    SetMovementDirection(Vector2Int.right, data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.South:
                    SetMovementDirection(Vector2Int.down, data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.West:
                    SetMovementDirection(Vector2Int.left, data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.NorthEast:
                    SetMovementDirection(new Vector2Int(1, 1), data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.NorthWest:
                    SetMovementDirection(new Vector2Int(-1, 1), data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.SouthEast:
                    SetMovementDirection(new Vector2Int(1, -1), data.distance);
                    break;

                case ScriptedActionQueue.MoveDirections.SouthWest:
                    SetMovementDirection(new Vector2Int(-1, -1), data.distance);
                    break;

                default:
                    // TODO @matthew - log warning
                    break;
            }
        }
        else
        {
            // TODO @matthew - log warning
        }
    }

    protected bool WaitAction(ScriptedActionQueue.ActionWrapper data)
    {
        if (data == null)
            return true;

        data.intData--;

        if (data.intData > 0)
            return false;

        return true;
    }

    protected void DialogueAction(ScriptedActionQueue.ActionWrapper data)
    {
    }
}