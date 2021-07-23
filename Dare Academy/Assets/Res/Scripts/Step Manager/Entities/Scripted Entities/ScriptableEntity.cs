using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab = null;
    [SerializeField] private ScriptedActionQueue m_actionQueue = null;

    [SerializeField] private int m_flagValue; // so custom editor can access this

    public int m_queuePos = 0;

    private SpriteRenderer sr;

    private Animator anim;

    private bool m_awaitingDialogueComplete = false;

    protected override void OnValidate()
    {
        base.OnValidate();

        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected void Awake()
    {
        m_flags._FlagData = m_flagValue;
    }

    protected override void Start()
    {
        base.Start();

        if (m_prefab == null)
        {
            Debug.LogWarning($"[ScriptableEntity] [{gameObject.name}] has no assigned prefab, killing self");
            Kill();
        }
        else
        {
            SpriteRenderer otherSr = m_prefab.GetComponentInChildren<SpriteRenderer>();

            if (otherSr)
            {
                sr.sprite = otherSr.sprite;
            }
            else
            {
                Debug.LogWarning($"[ScriptableEntity] [name = {gameObject.name}] could not get sprite renderer of prefab");
            }

            anim = GetComponent<Animator>();
            Animator otherAnim = m_prefab.GetComponent<Animator>();

            if (anim != null && otherAnim != null)
            {
                anim.runtimeAnimatorController = otherAnim.runtimeAnimatorController;
            }
            else
            {
                Debug.LogWarning($"[ScriptableEntity] [name = {gameObject.name}] could not get animator of prefab");
            }
        }
    }

    protected void Update()
    {
        if (m_awaitingDialogueComplete)
        {
            if (!blu.App.GetModule<blu.DialogueModule>().DialogueActive)
            {
                m_awaitingDialogueComplete = false;
                this.AnalyseStep();
            }
        }
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
            Debug.LogWarning($"[ScriptableEntity] [name = {gameObject.name}] action queue has no content");
            ReplaceWithPrefab();
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

            case ScriptedActionQueue.ActionType.AwaitDialogueComplete:
                runAgain = AwaitDialogueAction();
                break;

            case ScriptedActionQueue.ActionType.SetCameraPosition:
                CameraPosAction(currentAction);
                runAgain = true;
                break;

            case ScriptedActionQueue.ActionType.SetCameraToPlayer:
                CameraToPlayerAction();
                runAgain = true;
                break;

            case ScriptedActionQueue.ActionType.ExecuteSteps:
                ExecuteSteps(currentAction);
                runAgain = true;
                break;

            default:
                Debug.LogWarning($"[ScriptedEntity] [{gameObject.name}] could not resolve action [type = {currentAction.type.ToString()}]");
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
                Debug.LogWarning($"[ScriptableEntity] [{gameObject.name}] could not instantiate prefab [prefab = {m_prefab.name}]");
            }

            GridEntity entity = obj.GetComponent<GridEntity>();
            entity.Flags.SetFlags(this.Flags._FlagData, true);
        }
        else
        {
            Debug.LogWarning($"[ScriptableEntity] [{gameObject.name}] prefab was null");
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
                    Debug.LogWarning($"[ScriptedEntity] [name = {gameObject.name}] could not execute move action");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[ScriptedEntity] [name = {gameObject.name}] move action data was null");
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

    protected void CameraPosAction(ScriptedActionQueue.ActionWrapper data)
    {
        if (data.boolData)
        {
            blu.App.CameraController.MoveToPosition(data.gameObject.transform);
        }
        else
        {
            blu.App.CameraController.MoveToPosition(data.vec3data);
        }
    }

    protected void CameraToPlayerAction()
    {
        blu.App.CameraController.KeepPlayerInFrame();
    }

    protected bool AwaitDialogueAction()
    {
        if (blu.App.GetModule<blu.DialogueModule>().DialogueActive)
        {
            m_awaitingDialogueComplete = true;
            return false;
        }
        return true;
    }

    protected void ExecuteSteps(ScriptedActionQueue.ActionWrapper data)
    {
        LevelManager.Instance.ForceStepsCount = data.int32Data;
        LevelManager.Instance.ForceStepTime = data.doubleData;
    }
}