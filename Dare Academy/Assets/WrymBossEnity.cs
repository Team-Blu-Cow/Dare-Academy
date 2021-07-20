using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class WrymBossEnity : GridEntity
{
    private PlayerEntity m_player;

    [SerializeField] private int m_moveSpeed;

    [SerializeField] private bool m_head;
    [SerializeField] private GridEntity m_followEntity;

    private bool m_attack = false;
    private Vector2 m_fireDirection;
    private Vector2Int m_burrowPos;
    List<Vector2Int> m_attackNodes = new List<Vector2Int>();


    [Header("Phase Two Variables")]
    [SerializeField] private int m_stepTimer = 0;

    [SerializeField] private bool m_firingPhaseTwo;
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private int m_fireCooldown = 3;

    private GameObject m_damageEntityPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_player = FindObjectOfType<PlayerEntity>();

        if (m_followEntity == null)
        {
            m_head = true;
        }

        m_damageEntityPrefab = Resources.Load<GameObject>("prefabs/Entities/DamageEntity");
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet"); // Find bullet prefab
    }

    public override void AnalyseStep()
    {
        if (m_player == null)
            return;

        if (m_burrowPos != Vector2Int.zero)
        {
            var currentRoom = App.GetModule<LevelModule>().CurrentRoom;

            RemoveFromCurrentNode();

            if (currentRoom[m_burrowPos].GetGridEntities().Count < 1)
            {
                m_currentNode = currentRoom[m_burrowPos];
                transform.position = currentRoom[m_burrowPos].position.world;
            }

            AddToCurrentNode();

            m_burrowPos = Vector2Int.zero;
        }

        Phase1();

        //if (Health > 5)
        //{
        //}
        //else if (m_stepTimer < 100) // TODO: add phase 2 timer
        //{
        //    Phase2();
        //}
        //else // Phase 3
        //{
        //}
    }

    public override void DamageStep()
    {
        base.DamageStep();
        if (m_attack)
        {
            m_attack = false;
            foreach (var node in m_attackNodes)
            {
                GameObject obj = Instantiate(m_damageEntityPrefab, App.GetModule<LevelModule>().CurrentRoom.ToWorld(node), Quaternion.identity);
                obj.GetComponent<DamageEntity>().Countdown = 1;
            }
        }
    }

    private void Phase1()
    {
        //PHASE 1
        Vector2 dir = new Vector2();

        Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, m_player.transform.position); // Find path to player

        if (Vector2Int.Distance(Position.grid, m_player.Position.grid) <= 1)
        {
            Vector2 temp = Position.grid - m_player.Position.grid;
            m_attack = true;
            FireAttack(temp.normalized);

            Burrow();
        }
        else if (path != null && path.Length > 1)
        {
            dir = path[1] - path[0]; // Set direction to be distance vector of the two closest pathfinding nodes
            SetMovementDirection(dir, m_moveSpeed); // Set movement
        }
    }

    private void Phase2()
    {
        Vector2 dir = new Vector2();

        if (m_head)
        {
            Vector2Int tempDir = new Vector2Int(0, 1);

            if (m_currentNode.GetNeighbour(tempDir) == null)
            {
                tempDir = new Vector2Int(-1, 0);
            }

            if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) == null)
            {
                tempDir = new Vector2Int(-1, 0);
                if (m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null && m_currentNode.GetNeighbour(new Vector2Int(0, 1)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(0, 1);
                }
            }

            if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null)
            {
                if (currentNode.GetNeighbour(new Vector2Int(0, -1)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(0, -1);
                }
            }

            dir = tempDir;
        }
        else
        {
            Vector3 path = m_followEntity.Position.world - Position.world;
            dir = Vector3.Normalize(path);
        }

        if (m_firingPhaseTwo && m_fireCooldown == 0)
        {
            if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null)
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                {
                    SpawnBullet(m_bulletPrefab, m_currentNode, new Vector2(1, 0));
                }
            }

            if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null)
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                {
                    SpawnBullet(m_bulletPrefab, m_currentNode, new Vector2(-1, 0));
                }
            }
        }

        if (m_fireCooldown <= 0)
        {
            m_firingPhaseTwo = !m_firingPhaseTwo;
            m_fireCooldown = 3;
        }

        m_fireCooldown--;
        m_stepTimer++;
        SetMovementDirection(dir, m_moveSpeed); // Set movement
    }

    private void FireAttack(Vector2 dir)
    {
        // Spawn one in direction
        m_attackNodes.Clear();

        m_attackNodes.Add(Position.grid - new Vector2Int((int)dir.x, (int)dir.y));

        if (Mathf.Abs(dir.x) > 0)
        {
            //facing right
            for (int i = -1; i < 2; i++)
            {
                for (int j = 2; j < 4; j++)
                {
                    if (App.GetModule<LevelModule>().CurrentRoom[Position.grid + new Vector2Int(j * -(int)Mathf.Sign(dir.x), i)] != null)
                        m_attackNodes.Add(Position.grid + new Vector2Int(j * -(int)Mathf.Sign(dir.x), i));
                }
            }
        }
        else if (Mathf.Abs(dir.y) > 0)
        {
            //facing Up
            for (int i = -1; i < 2; i++)
            {
                for (int j = 2; j < 4; j++)
                {
                    if (App.GetModule<LevelModule>().CurrentRoom[Position.grid + new Vector2Int(i, j * -(int)Mathf.Sign(dir.y))] != null)
                        m_attackNodes.Add(Position.grid + new Vector2Int(i, j * -(int)Mathf.Sign(dir.y)));
                }
            }
        }

        foreach (var node in m_attackNodes)
        {
            if (node != null && App.GetModule<LevelModule>().CurrentRoom[node] != null)
            {
                App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(App.GetModule<LevelModule>().CurrentRoom[node], TelegraphDrawer.Type.ATTACK);
                
            }
        }
    }

    private void Burrow()
    {
        int direction;
        var currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        int low1 = Mathf.Min(m_player.Position.grid.x, m_player.Position.grid.y);
        int low2 = Mathf.Min(currentRoom.Width - m_player.Position.grid.x, currentRoom.Height - m_player.Position.grid.y);

        if (low1 < low2)
        {
            if (low1 == m_player.Position.grid.x)
                direction = 0;
            else
                direction = 1;
        }
        else
        {
            if (low2 == currentRoom.Width - m_player.Position.grid.x)
                direction = 2;
            else
                direction = 3;
        }

        Vector2Int pos;

        do
        {
            pos = direction switch
            {
                0 => FindBurrowHoz(currentRoom.Width - 1, currentRoom),// Burrow Right /////////
                1 => FindBurrowVert(currentRoom.Height - 1, currentRoom),// Burrow Up /////////
                2 => FindBurrowHoz(0, currentRoom),// Burrow Left /////////
                3 => FindBurrowVert(0, currentRoom),// Burrow Down /////////
                _ => Vector2Int.zero,
            };
        } while (currentRoom[pos] == null);

        m_burrowPos = pos;
    }

    private Vector2Int FindBurrowHoz(int x, JUtil.Grids.Grid<GridNode> currentRoom)
    {
        List<Vector2Int> exitPos = new List<Vector2Int>();

        for (int i = 0; i < currentRoom.Height; i++)
        {
            if (currentRoom.NodeExists(x, i) && currentRoom[x, i].IsTraversable())
            {
                exitPos.Add(new Vector2Int(x, i));
            }
        }

        return exitPos[Random.Range(0, exitPos.Count)];
    }

    private Vector2Int FindBurrowVert(int y, JUtil.Grids.Grid<GridNode> currentRoom)
    {
        List<Vector2Int> exitPos = new List<Vector2Int>();

        for (int i = 0; i < currentRoom.Width; i++)
        {
            if (currentRoom.NodeExists(i, y) && currentRoom[i, y].IsTraversable())
            {
                exitPos.Add(new Vector2Int(i, y));
            }
        }

        return exitPos[Random.Range(0, exitPos.Count)];
    }

    // Update is called once per frame
    private void Update()
    {
    }
}