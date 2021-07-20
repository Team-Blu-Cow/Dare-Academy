using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab = null;
    [SerializeField] private ScriptedActionQueue m_actionQueue = null;

    public int m_queuePos = 0;

    private SpriteRenderer sr;

    private Animator anim;

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

            if (otherSr)//#todo #matthew - log warning if failed
            {
                sr.sprite = otherSr.sprite;
            }

            anim = GetComponent<Animator>();
            Animator otherAnim = m_prefab.GetComponent<Animator>();

            //#todo #matthew - log this failing
            if (anim != null && otherAnim != null)
            {
                anim.runtimeAnimatorController = otherAnim.runtimeAnimatorController;
            }
        }

        if (m_actionQueue.m_actionList.Count == 0)
            ReplaceWithPrefab(); //#todo #matthew - log warning
    }

    public override void ResetAnimations()
    {
        base.ResetAnimations();
    }

    public override void AnalyseStep()
    {
        bool stepQueue = true;
        bool runAgain = false;

        if (m_actionQueue.m_actionList.Count == 0)
        {
            //#todo #matthew - log warning
            ReplaceWithPrefab();
            return;
        }

    RunAgainLabel:

        if (m_queuePos == m_actionQueue.m_actionList.Count)
        {
            ReplaceWithPrefab();
            return;
        }

        ScriptedActionQueue.ActionWrapper currentAction = m_actionQueue.m_actionList[m_queuePos];

        bool b;
        switch (currentAction.type)
        {
            case ScriptedActionQueue.ActionType.None:
                runAgain = true;
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

            case ScriptedActionQueue.ActionType.WaitPlayerEnterTrigger:
                b = currentAction.gameObject.GetComponent<ScriptableEntityTrigger>().HasPlayer;
                stepQueue = b;
                runAgain = b;

                break;

            case ScriptedActionQueue.ActionType.WaitPlayerExitTrigger:
                b = !currentAction.gameObject.GetComponent<ScriptableEntityTrigger>().HasPlayer;
                stepQueue = b;
                runAgain = b;

                break;

            case ScriptedActionQueue.ActionType.SetFlagEntityValue:

                m_flags.SetFlags(currentAction.int32Data, currentAction.boolData);
                runAgain = true;
                break;

            case ScriptedActionQueue.ActionType.SetEventFlagValue:

                blu.App.GetModule<blu.LevelModule>().EventFlags.SetFlags(currentAction.int32Data, currentAction.boolData);
                runAgain = true;
                break;

            case ScriptedActionQueue.ActionType.Dialogue:
                if (currentAction.gameObject != null)
                {
                    blu.App.GetModule<blu.DialogueModule>().StartDialogue(currentAction.gameObject);
                }
                runAgain = true;
                break;

            default:
                Debug.LogWarning($"ScriptedEntity [{gameObject.name}] could not resolve action [type = {currentAction.type.ToString()}]");
                break;
        }
        if (stepQueue)
        {
            m_queuePos++;
        }
        if (runAgain)
        {
            runAgain = false;
            goto RunAgainLabel;
        }
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
                    //#todo #matthew - log warning
                    break;
            }
        }
        else
        {
            //#todo #matthew - log warning
        }
    }

    protected bool WaitAction(ScriptedActionQueue.ActionWrapper data)
    {
        if (data == null)
            return true;

        data.int32Data--;

        if (data.int32Data > 0)
            return false;

        return true;
    }

    protected void DialogueAction(ScriptedActionQueue.ActionWrapper data)
    {
    }
}