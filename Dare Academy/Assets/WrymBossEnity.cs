using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

// #TODO #Jack - Clean Up The Movement Code

public class WrymBossEnity : GridEntity
{
    private PlayerEntity m_player;

    [SerializeField] private int m_moveSpeed;

    [SerializeField] private bool m_head;
    [SerializeField] private WrymBossEnity m_headEntity;
    [SerializeField] private bool m_split;

    private bool m_attack = false;
    private Vector2Int m_burrowPos;
    private Vector2Int m_lastBurrowPos;
    private List<Vector2Int> m_attackNodes = new List<Vector2Int>();

    [Header("Following Variables")]
    [SerializeField]
    private List<WrymBossEnity> m_body;

    private List<Vector2> m_previousMoves = new List<Vector2>();

    [Header("Phase Two Variables")]
    [SerializeField] private int m_stepTimer = 40;
    [SerializeField] private Vector3 m_prevPosition = new Vector3(0, 0, 0);
    [SerializeField] private bool m_firingPhaseTwo;
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private int m_fireCooldown = 3;

    private GameObject m_damageEntityPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_player = PlayerEntity.Instance;
        Health = 10;
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


        if (!m_head && Health < 5)
        {
            m_headEntity.Health -= (5 - Health);
            Health = 5;
        }

        if(m_headEntity == null)
        {
            Kill();
        }

        if (m_head)
        {
            if (Health > 5)
            {
                Phase1();
            }
            else if (m_stepTimer > 0) // TODO: add phase 2 timer
            {
                Phase2();
            }
            else // Phase 3
            {
                Phase3();
            }
        }
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
                obj.GetComponent<DamageEntity>().Countdown = 0;
            }
        }
    }

    private void Phase1()
    {
        //PHASE 1
        if (m_head)
        {
            MoveToBurrow();
            for (int i = 0; i < m_body.Count; i++)
            {
                if (m_previousMoves.Count > i + 1 && m_previousMoves[i + 1] == Vector2.zero)
                {
                    m_body[i].m_burrowPos = m_lastBurrowPos;
                    m_body[i].MoveToBurrow();
                }
            }

            Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, m_player.transform.position); // Find path to player

            if (Vector2Int.Distance(Position.grid, m_player.Position.grid) <= 1)
            {
                Vector2 temp = Position.grid - m_player.Position.grid;
                m_attack = true;
                FireAttack(temp.normalized);

                Burrow();
                m_previousMoves.Insert(0, Vector2.zero);
            }
            else if (path != null && path.Length > 1)
            {
                Vector2 dir = path[1] - path[0]; // Set direction to be distance vector of the two closest path finding nodes
                SetMovementDirection(dir, m_moveSpeed); // Set movement
                m_previousMoves.Insert(0, dir);
            }

            int count = 0;

            for (int i = 0; i < 8; i += 2)
            {
                if (m_currentNode.Neighbors[i].reference != null && m_currentNode.Neighbors[i].reference.GetGridEntities().Count > 0)
                {
                    count++;
                }
            }

            if (count == 4)
            {
                Burrow();
                SetMovementDirection(Vector2.zero, m_moveSpeed); // Set movement
            }

            if (m_previousMoves.Count > m_body.Count + 1)
            {
                for (int i = 0; i < m_body.Count; i++)
                {
                    m_body[i].SetMovementDirection(m_previousMoves[i + 1]);
                }
                m_previousMoves.RemoveAt(m_previousMoves.Count - 1);
            }
            else
            {
                for (int i = 0; i < m_body.Count; i++)
                {
                    if (m_previousMoves.Count > i + 1)
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[i + 1]);
                    }
                    else
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[m_previousMoves.Count - 1]);
                    }
                }
            }
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
                if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null && m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(-1, 0);
                }
                else if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null && m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(1, 0);
                }

                if (m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null && m_currentNode.GetNeighbour(new Vector2Int(0, 1)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(0, 1);
                }
            }
            
            if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null)
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)).GetGridEntities().Count == 0)
                {
                    tempDir = new Vector2Int(0, -1);
                }
            }
            
            if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null && m_prevPosition == m_currentNode.position.world && m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
            {
                tempDir = new Vector2Int(1, 0);
            }
            if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null && m_prevPosition == m_currentNode.position.world && m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
            {
                tempDir = new Vector2Int(-1, 0);
            }
            
            m_prevPosition = m_currentNode.position.world;
            dir = tempDir;
            m_previousMoves.Insert(0, dir);   
           

            if (m_previousMoves.Count > m_body.Count)
            {
                for (int i = 0; i < m_body.Count; i++)
                {
                    m_body[i].SetMovementDirection(m_previousMoves[i + 1]);
                }
                m_previousMoves.RemoveAt(m_previousMoves.Count - 1);
            }
            else
            {
                for (int i = 0; i < m_body.Count; i++)
                {
                    if (m_previousMoves.Count > i + 1)
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[i + 1]);
                    }
                    else
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[m_previousMoves.Count - 1]);
                    }

                }
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
        }


        for (int i = 0; i < m_body.Count; i++)
        {
            if (m_body[i].m_firingPhaseTwo && m_body[i].m_fireCooldown == 0)
            {
                if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null)
                {
                    if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                    {
                        SpawnBullet(m_bulletPrefab, m_body[i].m_currentNode, new Vector2(1, 0));
                    }
                }

                if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null)
                {
                    if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                    {
                        SpawnBullet(m_bulletPrefab, m_body[i].m_currentNode, new Vector2(-1, 0));
                    }
                }
            }
        }

        if(m_fireCooldown == 1 && m_head)
        {
            m_attackNodes.Clear();

            if (m_firingPhaseTwo)
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(1, (int)dir.y)) != null)
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(1, (int)dir.y)).GetGridEntities().Count == 0)
                        m_attackNodes.Add(Position.grid + new Vector2Int(1, (int)dir.y));
                }

                if (m_currentNode.GetNeighbour(new Vector2Int(-1, (int)dir.y)) != null)
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(-1, (int)dir.y)).GetGridEntities().Count == 0)
                        m_attackNodes.Add(Position.grid + new Vector2Int(-1, (int)dir.y));
                }

                TelegraphBullets(-1, 1, 3);
            }
            else
            {
                TelegraphBullets(0,2,4);
            }

            foreach (var node in m_attackNodes)
            {
                if (node != null && App.GetModule<LevelModule>().CurrentRoom[node] != null)
                {
                    App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(App.GetModule<LevelModule>().CurrentRoom[node], TelegraphDrawer.Type.ATTACK);
                }
            }
        }

        for (int i = 0; i < m_body.Count; i++)
        {
            if (m_body[i].m_fireCooldown <= 0)
            {
                m_body[i].m_firingPhaseTwo = !m_body[i].m_firingPhaseTwo;
                m_body[i].m_fireCooldown = 3;
            }

            m_body[i].m_fireCooldown--;
        }

        if (m_fireCooldown <= 0)
        {
            m_firingPhaseTwo = !m_firingPhaseTwo;
            m_fireCooldown = 3;
        }


        m_fireCooldown--;
        m_stepTimer--;
        SetMovementDirection(dir, m_moveSpeed); // Set movement
    }

    private void Phase3()
    {
        if(m_split == true)
        {
            m_body[2].m_head = true;

            m_body[2].m_body.Add(m_body[3]);
            m_body[2].m_body.Add(m_body[4]);

            m_body[2].m_headEntity = m_body[2];
            m_body[3].m_headEntity = m_body[2];
            m_body[4].m_headEntity = m_body[2];

            m_body[2].Health = (int)Mathf.Ceil(Health / 2);
            Health = (int)Mathf.Ceil(Health / 2);

            m_body.RemoveAt(4);
            m_body.RemoveAt(3);
            m_body.RemoveAt(2);

            m_split = false;
        }

        Phase1();
    }

    private void TelegraphBullets(int secOne, int secTwo, int secThree)
    {
        if (secOne == 0)
        {
            if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))) != null)
            {
                if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))).GetGridEntities().Count == 0)
                    m_attackNodes.Add(m_body[secOne].Position.grid + new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y)));
            }

            if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(-1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))) != null)
            {
                if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(-1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))).GetGridEntities().Count == 0)
                    m_attackNodes.Add(m_body[secOne].Position.grid + new Vector2Int(-1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y)));
            }
        }

        if (m_body[secTwo].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y))) != null)
        {
            if (m_body[secTwo].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(m_body[secTwo].Position.grid + new Vector2Int(1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y)));
        }

        if (m_body[secTwo].m_currentNode.GetNeighbour(new Vector2Int(-1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y))) != null)
        {
            if (m_body[secTwo].m_currentNode.GetNeighbour(new Vector2Int(-1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(m_body[secTwo].Position.grid + new Vector2Int(-1, (int)(m_body[secTwo].Position.world.y - m_body[secTwo + 1].Position.world.y)));
        }

        if (m_body[secThree].m_currentNode.GetNeighbour(new Vector2Int(1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y))) != null)
        {
            if (m_body[secThree].m_currentNode.GetNeighbour(new Vector2Int(1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(m_body[secThree].Position.grid + new Vector2Int(1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y)));
        }

        if (m_body[secThree].m_currentNode.GetNeighbour(new Vector2Int(-1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y))) != null)
        {
            if (m_body[secThree].m_currentNode.GetNeighbour(new Vector2Int(-1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(m_body[secThree].Position.grid + new Vector2Int(-1, -(int)(m_body[secThree].Position.world.y - m_body[secThree - 1].Position.world.y)));
        }
    }

    public void MoveToBurrow()
    {
        if (m_burrowPos != Vector2Int.zero)
        {
            var currentRoom = App.GetModule<LevelModule>().CurrentRoom;

            SetMovementDirection(new Vector2(1, 0));
            RemoveFromCurrentNode();

            if (currentRoom[m_burrowPos].GetGridEntities().Count < 1)
            {
                m_currentNode = currentRoom[m_burrowPos];
                transform.position = currentRoom[m_burrowPos].position.world;
            }

            AddToCurrentNode();

            m_lastBurrowPos = m_burrowPos;
            m_burrowPos = Vector2Int.zero;
        }
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