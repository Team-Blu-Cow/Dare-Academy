using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using System;
using JUtil;

public class WyrmHead : WyrmSection
{
    public enum WyrmState
    {
        NoState,
        UnderGround,
        Chasing,
        Circing,
        RandomMovement,
        Running,
        CrossScreenCharge,
        FireAttack,
    }

    private List<DamageEntity> m_activeWarningSymbols = new List<DamageEntity>();

    public WyrmState lastState
    { get; private set; }

    public WyrmState state
    { get; private set; }

    private LevelModule levelModule;
    private bool m_hasSplit = false;

    [SerializeField] private int m_length;

    [SerializeField, HideInInspector] private GameObject m_headPrefab;
    [SerializeField, HideInInspector] private GameObject m_bodyPrefab;
    [SerializeField, HideInInspector] private GameObject m_firePrefab;
    [SerializeField, HideInInspector] private GameObject m_warningPrefab;

    private GridNode m_chargeStartNode = null;
    private Vector2Int m_chargeDir = Vector2Int.zero;
    private int m_chargeCountdown = 0;
    private const int m_chargeCountdownTime = 0;

    private int m_stepsUntilResurface = 0;

    private int m_chasingNodeDirection = 0;

    protected override void OnValidate()
    {
        base.OnValidate();

        m_headPrefab = Resources.Load<GameObject>("prefabs/Entities/Wyrm/WyrmHead");
        m_bodyPrefab = Resources.Load<GameObject>("prefabs/Entities/Wyrm/WyrmBody");
        m_firePrefab = Resources.Load<GameObject>("prefabs/Entities/Fire");
        m_warningPrefab = Resources.Load<GameObject>("prefabs/Entities/WarningSymbol");
    }

    protected override void Start()
    {
        base.Start();
        levelModule = App.GetModule<LevelModule>();

        // we start underground
        RemoveFromCurrentNode();
        m_currentNode = null;
        state = WyrmState.UnderGround;

        m_stepsUntilResurface = 5;

        this.Head = this;

        // create the rest of the wyrm
        WyrmSection back = this;
        for (int i = 1; i < m_length; i++)
        {
            // slow but it happens on start so i couldn't give less of a fuck
            GameObject body = GameObject.Instantiate(m_bodyPrefab);
            body.transform.position = this.transform.position;

            WyrmSection section = body.GetComponent<WyrmSection>();
            section.Head = this;
            back.SectionBehind = section;
            section.SectionInfront = back;
            back = section;
        }
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
        AnalyseFSM();
    }

    public override void ReAnalyseStep()
    {
        base.ReAnalyseStep();
        AnalyseFSM();
        doReanalyse = false;
    }

    private void AnalyseFSM()
    {
        switch (state)
        {
            case WyrmState.NoState:
                State_NoState();
                break;

            case WyrmState.UnderGround:
                State_UnderGround();
                break;

            case WyrmState.Chasing:
                State_Chasing();
                break;

            case WyrmState.Circing:
                State_Circing();
                break;

            case WyrmState.RandomMovement:
                State_RandomMovement();
                break;

            case WyrmState.Running:
                State_Running();
                break;

            case WyrmState.CrossScreenCharge:
                State_CrossScreenCharge();
                break;

            case WyrmState.FireAttack:
                State_FireAttack();
                break;
        }
    }

    public override void AttackStep()
    {
        base.AttackStep();
    }

    public override void MoveStep()
    {
        base.MoveStep();
    }

    public override void EndStep()
    {
        base.EndStep();

        ResurfacedThisStep = false;
    }

    private GridNode GenerateBurrowLocation()
    {
        var currentRoom = levelModule.CurrentRoom;

        System.Random rnd = new System.Random();
        do
        {
            int new_x = rnd.Next(0, currentRoom.Width);
            int new_y = rnd.Next(0, currentRoom.Height);
            if (currentRoom[new_x, new_y] != null)
            {
                if (currentRoom[new_x, new_y].IsTraversable())
                {
                    if (currentRoom[new_x, new_y].GetGridEntities().Count == 0)
                    {
                        return currentRoom[new_x, new_y];
                    }
                }
            }
        }
        while (true);
    }

    private void Split()
    {
        List<WyrmSection> sections = new List<WyrmSection>();
        WyrmSection current = this;
        while (current)
        {
            sections.Add(current);
            current = current.SectionBehind;
        }

        if (sections.Count >= 4)
        {
            // point wyrm is split at
            int newHead = (sections.Count/2);

            // tail of front half will auto disconnect

            {
                // replace body script with head
                GameObject obj  = sections[newHead].gameObject;
                GameObject.Destroy(sections[newHead]);
                WyrmHead head = obj.AddComponent<WyrmHead>();
                head.m_hasSplit = true;

                //set health
                int h = Health/2;
                head.Health = h;

                Health = h;

                // add back to list and reset references
                sections[newHead] = head;
                sections[newHead].SectionBehind = sections[newHead + 1];
                sections[newHead + 1].SectionInfront = sections[newHead];
            }
        }
        else
        {
            Debug.LogWarning("Wyrm was to short to split");
        }
    }

    protected override void Burrow()
    {
        base.Burrow();
        m_stepsUntilResurface = 5;
    }

    protected override void Resurface()
    {
        m_currentNode = GenerateBurrowLocation();
        base.Resurface();
    }

    private void State_NoState()
    {
        // #wyrm implement state selection
        System.Random rnd = new System.Random();

        state = WyrmState.CrossScreenCharge;
        return;

    tryAgain:

        int num = rnd.Next(0,4);

        if (num == 0)
        { state = WyrmState.UnderGround; }
        else if (num == 1)
        { state = WyrmState.Chasing; }
        else if (num == 2)
        { state = WyrmState.Circing; }
        else if (num == 3)
        { state = WyrmState.CrossScreenCharge; }

        // { state = WyrmState.RandomMovement; }
        // else if (num == 4)
        // { state = WyrmState.Running; }
        // else if (num == 5)

        if (state == lastState)
            goto tryAgain;

        lastState = state;
    }

    private void State_UnderGround()
    {
        if (currentNode != null)
        {
            Burrow();
            return;
        }

        spriteRenderer.enabled = false;
        m_stepsUntilResurface--;

        if (m_stepsUntilResurface < 0)
        {
            Resurface();
            state = WyrmState.NoState;
        }
    }

    private void State_Chasing()
    {
        List<WyrmSection> sections = GetAboveGroundSections();
        Vector3[] avoidNodes = new Vector3[sections.Count];
        for (int i = 0; i < sections.Count; i++)
        {
            avoidNodes[i] = sections[i].currentNode.position.world;
        }

        Vector3[] path = levelModule.MetaGrid.GetPathWithAvoidance(currentNode.position.world, PlayerEntity.Instance.currentNode.position.world, avoidNodes, 1);

        if (path == null)
            return;

        if (Vector2Int.Distance(Position.grid, PlayerEntity.Instance.Position.grid) <= 1)
        {
            // Vector2 temp = Position.grid - PlayerEntity.Instance.Position.grid;
            // attack
            state = WyrmState.NoState;
            return;
        }
        else
        {
            // idk why but path[0].z is 2 so it cant be resolved to the grid
            Vector3 pos = new Vector3(path[0].x, path[0].y, 0);

            GridNode node = levelModule.MetaGrid.GetNodeFromWorld(pos);
            Vector2Int dir = node.position.grid - m_currentNode.position.grid;

            SetMovementDirection(dir);
            doReanalyse = true;
        }
    }

    private void State_Circing()
    {
    restartCirclingFunc:

        Vector3[] path = CirclingGetPath();
        if (path == null)
        {
            state = WyrmState.Chasing;
            m_chasingNodeDirection = 0;
            return;
        }

        if (path.Length == 0)
        {
            m_chasingNodeDirection++;
            goto restartCirclingFunc; // I'm sorry
        }

        // idk why but path[0].z is 2 so it cant be resolved to the grid
        Vector3 pos = new Vector3(path[0].x, path[0].y, 0);

        GridNode targetNode = levelModule.MetaGrid.GetNodeFromWorld(pos);
        Vector2Int targetDir = targetNode.position.grid - m_currentNode.position.grid;

        SetMovementDirection(targetDir);
        doReanalyse = true;
    }

    private void State_RandomMovement()
    {
        // #wyrm implement random movement
        state = WyrmState.NoState;
    }

    private void State_Running()
    {
        // #wyrm implement running
        state = WyrmState.NoState;
    }

    private void State_CrossScreenCharge()
    {
        if (m_chargeStartNode == null)
        {
            for (int i = 0; i < 10; i++)
            {
                if (GenerateChargePath())
                {
                    SpawnWarningSymbol(m_chargeStartNode);
                    m_chargeCountdown = m_chargeCountdownTime;
                    Burrow();
                    return;
                }
            }
            return;
        }
        else
        {
            m_chargeCountdown--;
            if (m_chargeCountdown <= 0)
            {
                // #wyrm do charge
                state = WyrmState.UnderGround;
                m_chargeStartNode = null;
                m_chargeDir = Vector2Int.zero;
                ClearWarningsSymbols();
            }
        }
    }

    private void State_FireAttack()
    {
        // #wyrm implement fire attack
        state = WyrmState.NoState;
    }

    // HELPER FUNCTIONS

    private Vector3[] CirclingGetPath()
    {
        if (m_chasingNodeDirection >= 8)
            m_chasingNodeDirection = 0;

        const int chasingDistance = 3;

        GridNode playerNode = PlayerEntity.Instance.currentNode;

        Vector2 dir = m_chasingNodeDirection.IndexToRotation();
        Vector2Int dirI = Vector2Int.RoundToInt(dir);
        GridNode node  = playerNode;

        for (int i = 0; i < chasingDistance; i++)
        {
            if (node != null)
                node = node.GetNodeRelative(dirI);
            else
                break;
        }

        if (node == null)
        {
            return null;
        }

        List<WyrmSection> sections = GetAboveGroundSections();

        Vector3[] avoidNodes = new Vector3[sections.Count + 1];
        for (int i = 0; i < sections.Count; i++)
        {
            avoidNodes[i] = sections[i].currentNode.position.world;
        }

        // avoid player
        avoidNodes[avoidNodes.Length - 1] = PlayerEntity.Instance.currentNode.position.world;

        Vector3[] path = levelModule.MetaGrid.GetPathWithAvoidance(currentNode.position.world, node.position.world, avoidNodes, 1);

        return path;
    }

    private List<WyrmSection> GetAboveGroundSections()
    {
        List<WyrmSection> sections = new List<WyrmSection>();
        sections.Add(this);
        while (true)
        {
            if (sections[sections.Count - 1].SectionBehind == null)
                break;

            if (sections[sections.Count - 1].SectionBehind.currentNode == null)
                break;

            sections.Add(sections[sections.Count - 1].SectionBehind);
        }

        return sections;
    }

    private bool SpawnFire(GridNode spawnNode)
    {
        const int fireTime = 3;

        if (spawnNode == null)
            return false;

        List<GridEntity> entities = spawnNode.GetGridEntities();

        // if a fire is already present extend its life
        foreach (var entity in entities)
        {
            if (entity is DamageEntity && (entity as DamageEntity).DamageType == DamageEntity.DamageEntityType.Fire)
            {
                if ((entity as DamageEntity).Linger < fireTime)
                    (entity as DamageEntity).Linger = fireTime;
                return true;
            }
        }

        // spawn a fire
        GameObject obj = GameObject.Instantiate(m_firePrefab, spawnNode.position.world, Quaternion.identity);
        if (obj.TryGetComponent(out DamageEntity fire))
        {
            fire.Linger = fireTime;
            return true;
        }

        // couldnt get component, spawn failed, destroy to be safe
        if (obj)
            GameObject.Destroy(obj);

        return false;
    }

    private bool SpawnWarningSymbol(GridNode spawnNode)
    {
        if (spawnNode == null)
            return false;

        List<GridEntity> entities = spawnNode.GetGridEntities();

        // if a fire is already present extend its life
        foreach (var entity in entities)
        {
            if (entity is DamageEntity && (entity as DamageEntity).DamageType == DamageEntity.DamageEntityType.Warning)
            {
                return true;
            }
        }

        // spawn warning
        GameObject obj = GameObject.Instantiate(m_warningPrefab, spawnNode.position.world, Quaternion.identity);
        if (obj.TryGetComponent(out DamageEntity warning))
        {
            warning.Linger = int.MaxValue;
            m_activeWarningSymbols.Add(warning);
            return true;
        }

        // couldn't get component, spawn failed, destroy to be safe
        if (obj)
            GameObject.Destroy(obj);

        return false;
    }

    private void ClearWarningsSymbols()
    {
        for (int i = 0; i < m_activeWarningSymbols.Count; i++)
        {
            if (m_activeWarningSymbols[i])
            {
                m_activeWarningSymbols[i].KillImmediate();
            }
        }

        m_activeWarningSymbols.Clear();
    }

    private bool GenerateChargePath()
    {
        m_chargeStartNode = null;
        m_chargeDir = Vector2Int.zero;

        JUtil.Grids.Grid<GridNode> grid = levelModule.MetaGrid.Grid(RoomIndex);

        System.Random rand = new System.Random();

        int r = rand.Next(0,4);

        Vector2Int dir = Vector2Int.zero;
        GridNode node = null;

        switch (r)
        {
            case 0:
                dir = Vector2Int.up;
                node = grid[rand.Next(0, grid.Width), 0];
                break;

            case 1:
                dir = Vector2Int.down;
                node = grid[rand.Next(0, grid.Width), grid.Height - 1];
                break;

            case 2:
                dir = Vector2Int.left;
                node = grid[grid.Width - 1, rand.Next(0, grid.Height)];
                break;

            case 3:
                dir = Vector2Int.right;
                node = grid[0, rand.Next(0, grid.Height)];
                break;
        }

        if (node == null)
            return false;

        int dist = node.GetDistanceInDirection(dir);

        if (dist < 4)
            return false;

        GridNode n = node;
        while (true)
        {
            if (n == null)
                break;

            if (n.overridden)
                return false;

            n = n.GetNeighbour(dir);
        }

        m_chargeStartNode = node;
        m_chargeDir = dir;

        return true;
    }
}