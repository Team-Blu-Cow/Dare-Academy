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
        Splitting,
        Chasing,
        Circing,
        CrossScreenCharge,
        FireAttack,
        Wait
    }

    private List<DamageEntity> m_activeWarningSymbols = new List<DamageEntity>();

    public WyrmState lastState
    { get; private set; }

    public WyrmState state
    { get; private set; }

    private LevelModule levelModule;

    public bool HasSplit
    { get; private set; }

    public int StartingHealth
    { get; private set; }

    private int SplitHealth
    { get; set; }

    private bool HasAttacked
    { get; set; }

    private List<GridNode> m_fireAttackNodes = new List<GridNode>();

    private Vector3 m_animationMidNode = Vector3.zero;

    [SerializeField] private int m_length;

    [SerializeField, HideInInspector] private GameObject m_headPrefab;
    [SerializeField, HideInInspector] private GameObject m_bodyPrefab;
    [SerializeField, HideInInspector] private GameObject m_firePrefab;
    [SerializeField, HideInInspector] private GameObject m_warningPrefab;
    [SerializeField] private Sprite m_warningSprite;
    [SerializeField] private ParticleSystem[] m_fireParticles;

    private GridNode m_chargeStartNode = null;
    private Vector2Int m_chargeDir = Vector2Int.zero;
    private int m_chargeCountdown = 0;
    [SerializeField] private int m_chargeCountdownTime = 5;
    private List<GridNode> m_chargeDamageNodes = new List<GridNode>();

    private Vector2Int m_attackDirection;

    private const int CirclingMinTime = 30;
    private const int CirclingMaxTime = 40;

    private int CirclingCountToChase
    { get; set; }

    public WyrmHead other
    { get; private set; }

    private bool DoChargeAttack
    { get; set; }

    private int m_chargeDamage = 1;

    private bool hasResurfaced;

    private GridNode m_randomMovementTargetNode = null;

    private int m_stepsUntilResurface = 0;

    private int m_chasingNodeDirection = 0;

    private int started = 0;

    GridNode m_resurfaceNode;

    public GridNode resurfaceNode
    {get { return m_resurfaceNode; }}


    [SerializeField] private GameObject m_endPickup;
    [SerializeField] private BarrierEntity m_endBarrier;
    private GameObject m_startBarrier;

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
        hasResurfaced = false;

        state = WyrmState.UnderGround;
        lastState = state;

        base.Start();
        levelModule = App.GetModule<LevelModule>();

        ResetChaseCounter();

        StartingHealth = Health;
        SplitHealth = StartingHealth / 2;

        // we start underground
        RemoveFromCurrentNode();
        m_currentNode = null;

        m_stepsUntilResurface = 5;

        this.Head = this;

        if (!HasSplit)
        {
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

        {
            WyrmSection section = SectionBehind;
            bool leg = true;
            while (section)
            {
                if (leg)
                {
                    ((WyrmBody)section).isLegs = true;
                }
                leg = !leg;

                section = section.SectionBehind;
            }
        }
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();
        AnalyseFSM();

        if (started <= 1 && !HasSplit)
        {
            if (started == 1)
            {
                m_uiHealth.FightStart();

                m_startBarrier = Resources.Load<GameObject>("prefabs/BarrierEntrance");
                m_startBarrier = Instantiate(m_startBarrier);
            }

            started++;
        }
    }

    public override void ResetAnimations()
    {
        base.ResetAnimations();

        if(state != WyrmState.UnderGround && state != WyrmState.CrossScreenCharge)
        {
            spriteRenderer.enabled = true;
        }
    }

    public override void OnHit(int damage, float offsetTime = 0)
    {
        base.OnHit(damage, offsetTime);
    }

    public override void ReAnalyseStep()
    {
        base.ReAnalyseStep();
        AnalyseFSM();
        doReanalyse = false;
        if (m_currentNode != null)
        {
            m_animationMidNode = m_currentNode.position.world;
        }
    }

    protected override void OnRoomEnter()
    {
        //spawn barrier

    }

    protected override void OnRoomExit()
    {
        //spawn barrier

        if (m_startBarrier != null)
        {
            m_startBarrier.GetComponentInChildren<BarrierEntity>().KillImmediate();
            m_startBarrier = null;

            LeanTween.delayedCall(0.1f, () =>
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags(GameEventFlags.Flags.wyrm_fight_start, false);
        });
        }
    }

    private void AnalyseFSM()
    {
        if (state != WyrmState.Splitting && !HasSplit && Health <= SplitHealth)
        {
            state = WyrmState.Splitting;
        }

        switch (state)
        {
            case WyrmState.NoState:
                State_NoState();
                break;

            case WyrmState.UnderGround:
                State_UnderGround();
                break;

            case WyrmState.Splitting:
                State_Splitting();
                break;

            case WyrmState.Chasing:
                State_Chasing();
                break;

            case WyrmState.Circing:
                State_Circing();
                break;

            //             case WyrmState.RandomMovement:
            //                 State_RandomMovement();
            //                 break;

            case WyrmState.CrossScreenCharge:
                State_CrossScreenCharge();
                break;

            case WyrmState.FireAttack:
                State_FireAttack();
                break;

            case WyrmState.Wait:
                state = WyrmState.UnderGround;
                break;
        }
    }

    public override void AttackStep()
    {
        base.AttackStep();

        if (m_fireAttackNodes.Count > 0)
        {
            foreach (var node in m_fireAttackNodes)
            {
                SpawnFire(node);
            }
            m_fireAttackNodes.Clear();
            state = WyrmState.Wait;
            HasAttacked = true;
        }

        if(hasResurfaced && m_currentNode != null)
        {
            hasResurfaced = false;

            if (m_currentNode.Neighbors[Vector2Int.up].reference != null
                && m_currentNode.Neighbors[Vector2Int.up].reference.IsTraversable(false))
            {
                GameObject obj = GameObject.Instantiate(m_firePrefab, m_currentNode.Neighbors[Vector2Int.up].reference.position.world, Quaternion.identity);
                if (obj.TryGetComponent(out DamageEntity fire))
                {
                    fire.Init(m_currentNode.Neighbors[Vector2Int.up].reference);
                    fire.Linger = 2;
                }
            }

            if (m_currentNode.Neighbors[Vector2Int.down].reference != null
                && m_currentNode.Neighbors[Vector2Int.down].reference.IsTraversable(false))
            {
                GameObject obj = GameObject.Instantiate(m_firePrefab, m_currentNode.Neighbors[Vector2Int.down].reference.position.world, Quaternion.identity);
                if (obj.TryGetComponent(out DamageEntity fire))
                {
                    fire.Init(m_currentNode.Neighbors[Vector2Int.down].reference);
                    fire.Linger = 2;
                }
            }

            if (m_currentNode.Neighbors[Vector2Int.left].reference != null
                && m_currentNode.Neighbors[Vector2Int.left].reference.IsTraversable(false))
            {
                GameObject obj = GameObject.Instantiate(m_firePrefab, m_currentNode.Neighbors[Vector2Int.left].reference.position.world, Quaternion.identity);
                if (obj.TryGetComponent(out DamageEntity fire))
                {
                    fire.Init(m_currentNode.Neighbors[Vector2Int.left].reference);
                    fire.Linger = 2;
                }
            }

            if (m_currentNode.Neighbors[Vector2Int.right].reference != null
                && m_currentNode.Neighbors[Vector2Int.right].reference.IsTraversable(false))
            {
                GameObject obj = GameObject.Instantiate(m_firePrefab, m_currentNode.Neighbors[Vector2Int.right].reference.position.world, Quaternion.identity);
                if (obj.TryGetComponent(out DamageEntity fire))
                {
                    fire.Init(m_currentNode.Neighbors[Vector2Int.right].reference);
                    fire.Linger = 2;
                }
            }

        }

        if (state == WyrmState.Circing || state == WyrmState.Chasing)
        {
            WyrmSection section = this;

            while(section.SectionBehind != null)
            {
                section = section.SectionBehind;
            }

            if (section.currentNode != null)
            {
                for(int i = 0; i < ((WyrmBody)section).m_nodesVisited.Count; i++)
                {
                    if (((WyrmBody)section).m_nodesVisited[i] == null)
                        continue;
                    GameObject obj = GameObject.Instantiate(m_firePrefab, ((WyrmBody)section).m_nodesVisited[i].position.world, Quaternion.identity);
                    if (obj.TryGetComponent(out DamageEntity fire))
                    {
                        fire.Linger = 5;

                    }
                }
            }
        }
    }

    public override void PreMoveStep()
    {
        base.PreMoveStep();
        if (m_resurfaceNode != null)
            Resurface();
    }

    public override void MoveStep()
    {
        base.MoveStep();
    }

    public override void EndStep()
    {
        base.EndStep();
    }

    public override void DamageStep()
    {
        base.DamageStep();
        if (DoChargeAttack)
        {
            //DoChargeAttack = false;
            foreach (var node in m_chargeDamageNodes)
            {
                if (node != null)
                {
                    var entities = node.GetGridEntities();
                    foreach (var entity in entities)
                    {
                        if (entity is WyrmSection)
                            continue;
                        if (entity is BarrierEntity)
                            continue;

                        entity.OnHit(m_chargeDamage);
                    }
                }
            }
        }
    }

    public override void OnDeath()
    {
        if (other == null)
        {
            m_uiHealth.FightEnd();
            m_endPickup.SetActive(true);

            //Kill barriers
            if (m_startBarrier != null)
                Destroy(m_startBarrier);

            if (m_endBarrier != null)
                m_endBarrier.KillImmediate();
        }

        WyrmSection section = SectionBehind;
        while (section)
        {
            section.KillImmediate();
            section = section.SectionBehind;
        }

        base.OnDeath();
    }

    public void EndAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

    public override void DrawStep()
    {
        // base.DrawStep();

        if(DoChargeAttack)
        {
            DoChargeAttack = false;

            // do stuff here
            Vector3 startPos = m_chargeStartNode.position.world;
            Vector3 endPos = m_chargeDamageNodes[m_chargeDamageNodes.Count - 1].position.world;

            Vector3 dir = (endPos - startPos).normalized;

            animator.SetFloat("WalkDirX", dir.x);
            animator.SetFloat("WalkDirY", dir.y);

            SetAnimationFlags(transform.position, m_chargeDamageNodes[m_chargeDamageNodes.Count - 1].position.world);

            StartCoroutine(AnimateCharge(startPos, endPos, m_length));

            state = WyrmState.UnderGround;
            m_chargeStartNode = null;
            m_chargeDir = Vector2Int.zero;

            ClearWarningsSymbols();
            m_chargeDamageNodes.Clear();

            return;
        }


        animator.SetBool("IsAttacking", HasAttacked);
        if (HasAttacked)
        {
            animator.SetFloat("WalkDirX", m_attackDirection.x);
            animator.SetFloat("WalkDirY", m_attackDirection.y);

            HasAttacked = false;
        }
        else if (MovedThisStep && m_currentNode != null && m_animationMidNode != Vector3.zero)
        {
            // m_animationController.animator.SetBool("IsMoving", true);
            //SetAnimationFlags(transform.position, m_animationMidNode);
            Vector3 dir = (m_animationMidNode - transform.position).normalized;

            animator.SetFloat("WalkDirX", dir.x);
            animator.SetFloat("WalkDirY", dir.y);
            LeanTween.move(gameObject, m_animationMidNode, m_stepController.stepTime / 2).setOnComplete(() =>
            {
                if (m_currentNode is null)
                {
                    return;
                }

                //SetAnimationFlags(transform.position, m_currentNode.position.world);
                Vector3 dir = (m_currentNode.position.world - transform.position).normalized;

                animator.SetFloat("WalkDirX", dir.x);
                animator.SetFloat("WalkDirY", dir.y);
                LeanTween.move(gameObject, m_currentNode.position.world, m_stepController.stepTime / 2).setOnComplete(() =>
                {
                    // m_animationController.animator.SetBool("IsMoving", false);
                }
                );
            }
            );

            m_animationMidNode = Vector3.zero;
        }
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
                if (currentRoom[new_x, new_y].IsTraversable(false))
                {
                    if (currentRoom[new_x, new_y].GetGridEntities().Count == 0)
                    {
                        bool anyValidNeighbours = false;
                        foreach (var neighbor in currentRoom[new_x, new_y].Neighbors)
                        {
                            if (neighbor != null && neighbor.reference != null && neighbor.reference.IsTraversable(false))
                            {
                                anyValidNeighbours = true;
                                break;
                            }
                        }
                        if (anyValidNeighbours == false)
                        {
                            continue;
                        }

                        return currentRoom[new_x, new_y];
                    }
                }
            }
        }
        while (true);
    }

    public GridNode GetClosestPoint()
    {
        int start_x, start_y;
        start_x = PlayerEntity.Instance.Position.grid.x;
        start_y = PlayerEntity.Instance.Position.grid.y;

        var currentRoom = levelModule.CurrentRoom;
        int X, Y;
        X = currentRoom.Width;
        Y = currentRoom.Height;
        int x,y,dx,dy;
        x = y = dx = 0;
        dy = -1;
        int t = Mathf.Max(X,Y);
        int maxI = t*t;
        for (int i = 0; i < maxI; i++)
        {
            if ((-X / 2 <= x) && (x <= X / 2) && (-Y / 2 <= y) && (y <= Y / 2))
            {
                // DO STUFF...
                if (currentRoom[start_x+x, start_y+y] != null)
                {
                    if (currentRoom[start_x + x, start_y + y].IsTraversable(false))
                    {
                        if (currentRoom[start_x + x, start_y + y].GetGridEntities().Count == 0)
                        {
                            bool anyValidNeighbours = false;
                            foreach (var neighbor in currentRoom[start_x + x, start_y + y].Neighbors)
                            {
                                if (neighbor != null && neighbor.reference != null && neighbor.reference.IsTraversable(false))
                                {
                                    anyValidNeighbours = true;
                                    break;
                                }
                            }
                            if (anyValidNeighbours == false)
                            {
                                continue;
                            }

                            return currentRoom[start_x + x, start_y + y];
                        }
                    }
                }


            }
            if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
            {
                t = dx;
                dx = -dy;
                dy = t;
            }
            x += dx;
            y += dy;
        }

        //Debug.LogError("Wyrm could not find a valid tile");
        return GenerateBurrowLocation();
    }

    private void Split()
    {
        HasSplit = true;

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
            int newHead = (sections.Count / 2);

            // tail of front half will auto disconnect

            {
                Vector3 startPos = sections[newHead].transform.position;

                if(m_currentNode != null && startPos == m_currentNode.position.world)
                {
                    startPos = GetClosestPoint().position.world;
                }

                GameObject.Destroy(sections[newHead]);
                GameObject obj = GameObject.Instantiate(m_headPrefab, startPos, Quaternion.identity);

                WyrmHead head = obj.GetComponent<WyrmHead>();

                head.HasSplit = true;

                head.other = this;
                this.other = head;

                other.m_endPickup = m_endPickup;
                other.m_endBarrier = m_endBarrier;
                other.m_startBarrier = m_startBarrier;

                other.Flags._FlagData = Flags._FlagData;

                //set health
                int h = Mathf.CeilToInt( (float)StartingHealth / 4f);
                head.Health = h;
                Health = h;

                // add back to list and reset references
                sections[newHead] = head;
                sections[newHead].SectionBehind = sections[newHead + 1];
                sections[newHead + 1].SectionInfront = sections[newHead];

                // reassign other variable
                WyrmSection section = this.other;
                while (section)
                {
                    section.Head = this.other;
                    section = section.SectionBehind;
                }

                // Update ui so it know its split
                m_uiHealth.m_splitHead = head;
                m_uiHealth.FightSplit();

                // wyrm head start function will assign all other references
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
        m_stepsUntilResurface = RandWrapper(3,8);
    }

    protected override void Resurface()
    {
        // #todo #jay spawn damage here
        base.Resurface();
        lastState = WyrmState.UnderGround;
        state = WyrmState.NoState;
        m_resurfaceNode = null;
        hasResurfaced = true;
    }

    private void State_NoState()
    {
        // #wyrm implement state selection
        System.Random rnd = new System.Random();

    tryAgain:

        int num = rnd.Next(0, 100);

        if (num < 25) // 25% chance
        { state = WyrmState.UnderGround; }
        else if (num < 35) // 10% chance
        { state = WyrmState.Chasing; }
        else if (num < 80) // 45% chance
        { state = WyrmState.Circing; }
        else // 20% chance
        { state = WyrmState.UnderGround; }

        if (state == lastState)
            goto tryAgain;

        if (lastState == WyrmState.UnderGround && state == WyrmState.CrossScreenCharge)
            goto tryAgain;

        lastState = state;

        AnalyseFSM();
    }

    private void State_UnderGround()
    {
        if (currentNode != null)
        {
            Burrow();
            return;
        }

        if (!HasSplit && Health <= StartingHealth / 2)
        {
            state = WyrmState.Splitting;
            return;
        }

        spriteRenderer.enabled = false;
        m_stepsUntilResurface--;

        if (m_stepsUntilResurface < 0)
        {
            System.Random rnd = new System.Random();

            int num = rnd.Next(0, 100);

            if (num < 33)
            { 
                state = WyrmState.CrossScreenCharge;
                AnalyseFSM();
            }
            else
            {
                PrepareToResurface();
            }
            //m_resurfaceNode = null;
        }
    }

    public void PrepareToResurface()
    {
        m_resurfaceNode = GetClosestPoint();

        levelModule.telegraphDrawer.CreateTelegraph(m_resurfaceNode, TelegraphDrawer.Type.ATTACK);
        if (m_resurfaceNode.Neighbors[Vector2Int.up].reference != null
            && m_resurfaceNode.Neighbors[Vector2Int.up].reference.IsTraversable(false))
            levelModule.telegraphDrawer.CreateTelegraph(m_resurfaceNode.Neighbors[Vector2Int.up].reference, TelegraphDrawer.Type.ATTACK);

        if (m_resurfaceNode.Neighbors[Vector2Int.down].reference != null
            && m_resurfaceNode.Neighbors[Vector2Int.down].reference.IsTraversable(false))
            levelModule.telegraphDrawer.CreateTelegraph(m_resurfaceNode.Neighbors[Vector2Int.down].reference, TelegraphDrawer.Type.ATTACK);

        if (m_resurfaceNode.Neighbors[Vector2Int.left].reference != null
            && m_resurfaceNode.Neighbors[Vector2Int.left].reference.IsTraversable(false))
            levelModule.telegraphDrawer.CreateTelegraph(m_resurfaceNode.Neighbors[Vector2Int.left].reference, TelegraphDrawer.Type.ATTACK);

        if (m_resurfaceNode.Neighbors[Vector2Int.right].reference != null
            && m_resurfaceNode.Neighbors[Vector2Int.right].reference.IsTraversable(false))
            levelModule.telegraphDrawer.CreateTelegraph(m_resurfaceNode.Neighbors[Vector2Int.right].reference, TelegraphDrawer.Type.ATTACK);

        m_currentNode = m_resurfaceNode;
    }

    private void State_Splitting()
    {
        if (HasSplit)
        {
            state = WyrmState.UnderGround;
            return;
        }

        if (currentNode != null)
        {
            Burrow();
            return;
        }

        if (GetAboveGroundSections().Count > 0)
            return;

        Split();
        state = WyrmState.UnderGround;
    }

    private void State_Chasing()
    {
        Vector3[] path = PathToPlayer();

        if (path == null)
        {
            // couldnt find a path, burrow
            state = WyrmState.UnderGround;
            return;
        }

        // if (Vector2Int.Distance(Position.grid, PlayerEntity.Instance.Position.grid) <= 1)
        if (path.Length <= 1)
        {
            state = WyrmState.FireAttack;
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
        if(m_currentNode == null)
        {
            state = WyrmState.UnderGround;
            AnalyseFSM();
            return;
        }

        if (CirclingCountToChase < 0)
        {
            ResetChaseCounter();
            state = WyrmState.Chasing;
            doReanalyse = true;
            return;
        }

    restartCirclingFunc:

        Vector3[] path = CirclingGetPath();
        if (path == null)
        {
            ResetChaseCounter();
            state = WyrmState.Chasing;
            m_chasingNodeDirection = 0;
            doReanalyse = true;
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

        CirclingCountToChase--;
        SetMovementDirection(targetDir);
        doReanalyse = true;
    }

    //     private void State_RandomMovement()
    //     {
    //         state = WyrmState.NoState;
    //         if (m_randomMovementTargetNode is null)
    //         {
    //             m_randomMovementTargetNode = GetRandomNode();
    //         }
    //
    //         if (m_randomMovementTargetNode is null)
    //         {
    //             state = WyrmState.UnderGround;
    //             return;
    //         }
    //
    //         List<WyrmSection> sections = GetAboveGroundSections();
    //
    //         Vector3[] avoidNodes = new Vector3[sections.Count + 1];
    //         for (int i = 0; i < sections.Count; i++)
    //         {
    //             avoidNodes[i] = sections[i].currentNode.position.world;
    //         }
    //
    //         // avoid player
    //         avoidNodes[avoidNodes.Length - 1] = PlayerEntity.Instance.currentNode.position.world;
    //
    //         Vector3[] path = levelModule.MetaGrid.GetPathWithAvoidance(currentNode.position.world, m_randomMovementTargetNode.position.world, avoidNodes, 1);
    //
    //         if (path is null)
    //         {
    //             state = WyrmState.UnderGround;
    //             m_randomMovementTargetNode = null;
    //             return;
    //         }
    //
    //         if (path.Length == 0)
    //         {
    //             state = WyrmState.UnderGround;
    //             m_randomMovementTargetNode = null;
    //             return;
    //         }
    //
    //         // idk why but path[0].z is 2 so it cant be resolved to the grid
    //         Vector3 pos = new Vector3(path[0].x, path[0].y, 0);
    //
    //         GridNode targetNode = levelModule.MetaGrid.GetNodeFromWorld(pos);
    //         Vector2Int targetDir = targetNode.position.grid - m_currentNode.position.grid;
    //
    //         SetMovementDirection(targetDir);
    //         doReanalyse = true;
    //     }

    private void State_CrossScreenCharge()
    {
        if (m_chargeStartNode == null)
        {
            for (int i = 0; i < 30; i++)
            {
                if (GenerateChargePath())
                {
                    TelegraphChargeNodes();
                    // SpawnWarningSymbol(m_chargeStartNode);
                    m_chargeCountdown = m_chargeCountdownTime;
                    //Burrow();
                    return;
                }
            }

            state = WyrmState.UnderGround;

            return;
        }
        else
        {
            m_chargeCountdown--;
            if (m_chargeCountdown <= 0)
            {
                // #wyrm do charge
                DoChargeAttack = true;
                TelegraphChargeNodes();
                CreateWaringSymbols();
            }
            else
            {
                TelegraphChargeNodes();
            }
        }
    }

    private void State_FireAttack()
    {
        if (m_fireAttackNodes.Count > 0)
            return;

        Vector3[] path = PathToPlayer();

        if(path == null)
        {
            state = WyrmState.UnderGround;
            AnalyseFSM();
            return;
        }

        Vector3 pos = new Vector3(path[0].x, path[0].y, 0);
        GridNode n = levelModule.MetaGrid.GetNodeFromWorld(pos);
        Vector2Int dir = n.position.grid - m_currentNode.position.grid;

        m_attackDirection = dir;

        if (dir.x != 0)
        {
            int x = dir.x;

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(x, 0));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(2 * x, 0));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(2 * x, 1));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(2 * x, -1));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(3 * x, 0));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(3 * x, 1));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(3 * x, -1));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(4 * x, 0));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(4 * x, 1));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(4 * x, -1));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(5 * x, 0));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(5 * x, 1));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(5 * x, -1));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(6 * x, 0));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(6 * x, 1));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(6 * x, -1));
        }
        else if (dir.y != 0)
        {
            int y = dir.y;

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, y));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, 2 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(1, 2 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(-1, 2 * y));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, 3 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(1, 3 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(-1, 3 * y));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, 4 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(1, 4 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(-1, 4 * y));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, 5 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(1, 5 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(-1, 5 * y));

            m_fireAttackNodes.Add(currentNode.GetNodeRelative(0, 6 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(1, 6 * y));
            m_fireAttackNodes.Add(currentNode.GetNodeRelative(-1, 6 * y));
        }

        foreach(var node in m_fireAttackNodes)
        {
            levelModule.telegraphDrawer.CreateTelegraph(node, TelegraphDrawer.Type.ATTACK);
        }
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
        GridNode node = playerNode;

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

        WyrmSection sect = this;

        while (sect)
        {
            if (sect.currentNode != null)
                sections.Add(sect);

            sect = sect.SectionBehind;
        }

        sect = other;
        while (sect)
        {
            if (sect.currentNode != null)
                sections.Add(sect);
            sect = sect.SectionBehind;
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

            if (entity is BarrierEntity)
            {
                return true;
            }
        }

        // spawn a fire
        GameObject obj = GameObject.Instantiate(m_firePrefab, spawnNode.position.world, Quaternion.identity);
        if (obj.TryGetComponent(out DamageEntity fire))
        {
            fire.Init(spawnNode);
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

        // if a warning is already present return
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
        m_chargeDamageNodes.Clear();
        m_chargeDir = Vector2Int.zero;
        JUtil.Grids.Grid<GridNode> grid = levelModule.MetaGrid.Grid(RoomIndex);
        System.Random rand = new System.Random();

        List<GridNode> nodes = new List<GridNode>();
        GridNode playerNode = PlayerEntity.Instance.currentNode;

        // double chance of spawn
        nodes.Add(playerNode);
        nodes.Add(playerNode);

        GridNode node;

        node = playerNode.GetNeighbour(Vector2Int.up);
        if (node != null) nodes.Add(node);
        node = playerNode.GetNeighbour(Vector2Int.down);
        if (node != null) nodes.Add(node);
        node = playerNode.GetNeighbour(Vector2Int.left);
        if (node != null) nodes.Add(node);
        node = playerNode.GetNeighbour(Vector2Int.right);
        if (node != null) nodes.Add(node);

        node = nodes[rand.Next(0, nodes.Count)];

        Vector2Int dir = Vector2Int.zero;
        switch (rand.Next(0, 4))
        {
            case 0:
                dir = Vector2Int.up;
                break;

            case 1:
                dir = Vector2Int.down;
                break;

            case 2:
                dir = Vector2Int.left;
                break;

            case 3:
                dir = Vector2Int.right;
                break;
        }

        m_chargeStartNode = FindLastNodeInDirection(node, -dir);
        if (m_chargeStartNode == null)
            return false;

        m_chargeDir = dir;
        GridNode damageNode = m_chargeStartNode;
        while (damageNode != null)
        {
            m_chargeDamageNodes.Add(damageNode);
            damageNode = damageNode.GetNeighbour(dir);
        }

        return true;
    }

    private GridNode FindLastNodeInDirection(GridNode node, Vector2Int dir)
    {
        if (dir == Vector2Int.zero)
            return node;
        if (node == null)
            return null;

        GridNode n = node;
        while (true)
        {
            if (n.overridden)
            {
                return n;
            }

            GridNode neighbor = n.GetNeighbour(dir);
            if (neighbor == null)
            {
                return n;
            }
            else
            {
                n = neighbor;
            }
        }
    }

    private GridNode GetRandomNode()
    {
        System.Random rand = new System.Random();
        JUtil.Grids.Grid<GridNode> grid = levelModule.MetaGrid.Grid(RoomIndex);

        GridNode node = null;
        for (int i = 0; i < 100; i++)
        {
            node = grid[rand.Next(0, grid.Width), rand.Next(0, grid.Height)];
            if (node != null)
                break;
        }

        if (node is null)
            Debug.LogWarning("[WyrmHead] could not generate node");

        return node;
    }

    private Vector3[] PathToPlayer()
    {
        List<WyrmSection> sections = GetAboveGroundSections();
        Vector3[] avoidNodes = new Vector3[sections.Count];
        for (int i = 0; i < sections.Count; i++)
        {
            avoidNodes[i] = sections[i].currentNode.position.world;
        }

        if (m_currentNode == null)
            return null;

        Vector3[] path = levelModule.MetaGrid.GetPathWithAvoidance(currentNode.position.world, PlayerEntity.Instance.currentNode.position.world, avoidNodes, 1);

        return path;

    }

    private void CreateWaringSymbols()
    {
        /*foreach (var node in m_chargeDamageNodes)
        {
            bool hasConflict = false;
            var entities = node.GetGridEntities();
            foreach (var entity in entities)
            {
                if (entity is BarrierEntity)
                {
                    hasConflict = true;
                    break;
                }
            }

            if (!hasConflict)
            {
                SpawnWarningSymbol(node);
            }
        }*/

        foreach (var node in m_chargeDamageNodes)
        {
            LevelManager.Instance.TelegraphDrawer.CreateTelegraph(node, TelegraphDrawer.Type.ATTACK, m_warningSprite);
        }
    }

    private void TelegraphChargeNodes()
    {
        foreach (var node in m_chargeDamageNodes)
        {
            LevelManager.Instance.TelegraphDrawer.CreateTelegraph(node, TelegraphDrawer.Type.MOVE);
        }
    }

    private static int RandWrapper(int min, int max)
    {
        return new System.Random().Next(min, max);
    }

    private void ResetChaseCounter()
    {
        CirclingCountToChase = RandWrapper(CirclingMinTime, CirclingMaxTime);
    }

    public void BreatheFire(int index)
    {
        m_fireParticles[index].Play();
    }
}