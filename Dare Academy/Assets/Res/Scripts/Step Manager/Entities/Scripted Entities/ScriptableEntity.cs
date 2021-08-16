using UnityEngine;

public class ScriptableEntity : GridEntity
{
    [SerializeField] private GameObject m_prefab = null;
    [SerializeField] private ScriptedActionQueue m_actionQueue = null;

    [SerializeField] private int m_flagValue = 0; // so custom editor can access this

    public int m_queuePos = 0;

    private bool m_dialogueHasStarted = false;

    [SerializeField, HideInInspector] private SpriteRenderer sr;

    [SerializeField, HideInInspector] private Animator anim;

    private bool m_awaitingDialogueComplete = false;
    private bool m_hasBeenReplaced = false;

    protected override void OnValidate()
    {
        base.OnValidate();

        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (m_prefab != null)
        {
            Sprite prefabSprite = m_prefab.GetComponentInChildren<SpriteRenderer>().sprite;

            if (prefabSprite != null)
                sr.sprite = prefabSprite;
        }
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
            // Debug.LogWarning($"[ScriptableEntity] [{gameObject.name}] has no assigned prefab, killing self");
            // Kill();
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

    protected void LateUpdate()
    {
        if (m_queuePos >= m_actionQueue.m_actionList.Count)
            return;

        ScriptedActionQueue.ActionType type = m_actionQueue.m_actionList[m_queuePos].type;

        if (type == ScriptedActionQueue.ActionType.WaitPlayerEnterTrigger || type == ScriptedActionQueue.ActionType.WaitPlayerExitTrigger)
        {
            while (true)
            {
                if (m_queuePos == m_actionQueue.m_actionList.Count)
                {
                    ReplaceWithPrefab();
                    return;
                }

                ScriptedActionQueue.ActionWrapper currentAction = m_actionQueue.m_actionList[m_queuePos];

                ProcessActionQueue(out bool runAgain, out bool stepQueue, currentAction);

                if (stepQueue)
                {
                    m_queuePos++;
                }

                if (!runAgain)
                {
                    break;
                }
            }
        }
    }

    public override void ResetAnimations()
    {
        base.ResetAnimations();
    }

    public override async void AnalyseStep()
    {
        await blu.App.GetModule<blu.LevelModule>().AwaitSaveLoad();
        await blu.App.GetModule<blu.LevelModule>().AwaitInitialised();

        if (m_actionQueue.m_actionList.Count == 0)
        {
            Debug.LogWarning($"[ScriptableEntity] [name = {gameObject.name}] action queue has no content");
            ReplaceWithPrefab();
        }

        while (true)
        {
            if (m_queuePos == m_actionQueue.m_actionList.Count)
            {
                ReplaceWithPrefab();
                return;
            }

            ScriptedActionQueue.ActionWrapper currentAction = m_actionQueue.m_actionList[m_queuePos];

            ProcessActionQueue(out bool runAgain, out bool stepQueue, currentAction);

            if (stepQueue)
            {
                m_queuePos++;
            }

            if (!runAgain)
            {
                break;
            }
        }
    }

    protected void ProcessActionQueue(out bool runAgain, out bool stepQueue, ScriptedActionQueue.ActionWrapper currentAction)
    {
        bool b;

        switch (currentAction.type)
        {
            case ScriptedActionQueue.ActionType.None:
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.Kill:
                Kill();
                runAgain = false;
                stepQueue = false;
                return;

            case ScriptedActionQueue.ActionType.Move:
                MoveAction(currentAction.moveData);
                runAgain = false;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.WaitTurns:
                stepQueue = WaitAction(currentAction);
                runAgain = false;
                return;

            case ScriptedActionQueue.ActionType.WaitPlayerEnterTrigger:
                b = currentAction.gameObject.GetComponent<ScriptableEntityTrigger>().HasPlayer;
                stepQueue = b;
                runAgain = b;
                return;

            case ScriptedActionQueue.ActionType.WaitPlayerExitTrigger:
                b = !currentAction.gameObject.GetComponent<ScriptableEntityTrigger>().HasPlayer;
                stepQueue = b;
                runAgain = b;
                return;

            case ScriptedActionQueue.ActionType.SetFlagEntityValue:

                m_flags.SetFlags(currentAction.int32Data, currentAction.boolData);
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.SetEventFlagValue:

                blu.App.GetModule<blu.LevelModule>().EventFlags.SetFlags(currentAction.int32Data, currentAction.boolData);
                blu.App.GetModule<blu.LevelModule>().SaveGame();
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.Dialogue:
                if (currentAction.gameObject != null)
                {
                    blu.App.GetModule<blu.DialogueModule>().StartDialogue(currentAction.gameObject);
                }
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.AwaitDialogueComplete:
                runAgain = AwaitDialogueAction();
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.SetCameraPosition:
                CameraPosAction(currentAction);
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.SetCameraToPlayer:
                CameraToPlayerAction();
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.ExecuteSteps:
                ExecuteSteps(currentAction);
                runAgain = true;
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.KillIfEventFlagSet:
                b = KillIfEventFlagSet(currentAction);
                runAgain = b;
                stepQueue = b;
                return;

            case ScriptedActionQueue.ActionType.KillIfEventFlagNotSet:
                runAgain = KillIfEventFlagNotSet(currentAction);
                stepQueue = true;
                return;

            case ScriptedActionQueue.ActionType.AddQuest:
                AddQuest(currentAction);
                runAgain = true;
                stepQueue = true;
                break;

            case ScriptedActionQueue.ActionType.CompleteQuest:
                CompleteQuest(currentAction);
                runAgain = true;
                stepQueue = true;
                break;

            case ScriptedActionQueue.ActionType.AwaitEventFlagSet:
                b = AwaitEventFlagSet(currentAction);
                runAgain = b;
                stepQueue = b;
                break;

            case ScriptedActionQueue.ActionType.WaitForever:
                runAgain = false;
                stepQueue = false;
                break;

            default:
                Debug.LogWarning($"[ScriptedEntity] [{gameObject.name}] could not resolve action [type = {currentAction.type.ToString()}]");
                runAgain = true;
                stepQueue = true;
                return;
        }
    }

    protected void ReplaceWithPrefab()
    {
        if (m_hasBeenReplaced)
            return;

        if (m_prefab)
        {
            GameObject obj = GameObject.Instantiate(m_prefab, m_currentNode.position.world, Quaternion.identity);
            if (obj == null)
            {
                Debug.LogWarning($"[ScriptableEntity] [{gameObject.name}] could not instantiate prefab [prefab = {m_prefab.name}]");
            }

            m_hasBeenReplaced = true;
            GridEntity entity = obj.GetComponent<GridEntity>();
            entity.Flags.SetFlags(this.Flags._FlagData, true);
            entity.m_deathEventFlags = m_deathEventFlags;
            m_deathEventFlags = 0;
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
        bool active = blu.App.GetModule<blu.DialogueModule>().DialogueActive;

        if (!m_dialogueHasStarted)
        {
            if (active)
            {
                m_dialogueHasStarted = true;
            }
            else
            {
                return false;
            }
        }

        if (active)
        {
            m_awaitingDialogueComplete = true;
            m_dialogueHasStarted = false;
            return false;
        }

        return true;
    }

    protected bool AwaitEventFlagSet(ScriptedActionQueue.ActionWrapper data)
    {
        if (blu.App.GetModule<blu.LevelModule>().EventFlags.IsFlagsSet(data.int32Data))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected void ExecuteSteps(ScriptedActionQueue.ActionWrapper data)
    {
        LevelManager.Instance.ForceStepsCount = data.int32Data;
        LevelManager.Instance.ForceStepTime = data.doubleData;
    }

    protected bool KillIfEventFlagSet(ScriptedActionQueue.ActionWrapper data)
    {
        if (data.boolData)
        {
            // all
            if (blu.App.GetModule<blu.LevelModule>().EventFlags.IsFlagsSet(data.int32Data))
            {
                Kill();
                return false;
            }
        }
        else
        {
            // any
            for (int i = 0; i < 31; i++)
            {
                if (blu.App.GetModule<blu.LevelModule>().EventFlags.IsFlagsSet(data.int32Data & (1 << i)))
                {
                    Kill();
                    return false;
                }
            }
        }

        return true;
    }

    protected bool KillIfEventFlagNotSet(ScriptedActionQueue.ActionWrapper data)
    {
        if (data.boolData)
        {
            // all
            if (!blu.App.GetModule<blu.LevelModule>().EventFlags.IsFlagsSet(data.int32Data))
            {
                Kill();
                return false;
            }
        }
        else
        {
            // any
            for (int i = 0; i < 31; i++)
            {
                if (!blu.App.GetModule<blu.LevelModule>().EventFlags.IsFlagsSet(data.int32Data & (1 << i)))
                {
                    Kill();
                    return false;
                }
            }
        }

        return true;
    }

    protected void AddQuest(ScriptedActionQueue.ActionWrapper data)
    {
        if (data.unityObject && data.unityObject is Quest)
        {
            blu.App.GetModule<blu.QuestModule>().AddQuest(data.unityObject as Quest, true);
        }
    }

    protected void CompleteQuest(ScriptedActionQueue.ActionWrapper data)
    {
        if (data.unityObject && data.unityObject is Quest)
        {
            blu.App.GetModule<blu.QuestModule>().SetComplete(data.unityObject as Quest);
        }
    }
}