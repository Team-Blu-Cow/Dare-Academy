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
    [SerializeField] private int m_stepTimer = 40; // How many steps the wyrm stays in this phase
    [SerializeField] private bool m_firingPhaseTwo; // If the certain compartment of the wyrm has to fire a bullet this step
    [SerializeField] private GameObject m_bulletPrefab; // Bullet prefab for spawning the bullets
    [SerializeField] private int m_fireCooldown = 3; // The time after firing a bullet until it can fire another
    [SerializeField] private int m_phaseTwoDirection = 1; // Which way the wyrm is going 
    private Vector3 m_prevPosition = new Vector3(0, 0, 0); // Last position


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

        if(m_headEntity == null) // If the head the body is following is dead
        {
            Kill(); // Destroy the body
        }

        if (m_head) // If this is the head
        {
            if (Health > 5) // If health is greater than 5
            {
                Phase1(); // Boss is in phase 1
            }
            else if (m_stepTimer > 0) // If the timer is greater than 0 
            {
                Phase2(); // Boss is in phase 2
            }
            else // If past phases are done
            {
                Phase3(); // Boss is in phase 3
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
        Vector2 dir = new Vector2(); // Direction

        if (m_head) // If this is the front of the worm
        {
            Vector2Int tempDir = new Vector2Int(0, 1 * m_phaseTwoDirection); // Create temp direction which starts either going up or down

            if (m_currentNode.GetNeighbour(tempDir) == null) // If the wyrm can't go in its direction
            {
                tempDir = new Vector2Int(-1 * m_phaseTwoDirection, 0); // Change direction to go either left or right depending on its overall direction of movement (e.g. moving from right to left on screen or left to right)
            }
            
            if (m_currentNode.GetNeighbour(new Vector2Int(0, -1 * m_phaseTwoDirection)) == null) // If it can't go up or down
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(-1 * m_phaseTwoDirection, 0)) != null && m_currentNode.GetNeighbour(new Vector2Int(-1 * m_phaseTwoDirection, 0)).GetGridEntities().Count == 0) // Try to go left or right depending on movement 
                {
                    tempDir = new Vector2Int(-1 * m_phaseTwoDirection, 0); // If it can then set the direction
                }
                else if (m_currentNode.GetNeighbour(new Vector2Int(1 * m_phaseTwoDirection, 0)) != null && m_currentNode.GetNeighbour(new Vector2Int(1 * m_phaseTwoDirection, 0)).GetGridEntities().Count == 0) // Else try to go right or left depending on movement
                {
                    tempDir = new Vector2Int(1 * m_phaseTwoDirection, 0); // If it can then set the direction
                    m_phaseTwoDirection *= -1; // Also change direction of overall movement
                }

                if (m_currentNode.GetNeighbour(new Vector2Int(0, 1 * m_phaseTwoDirection)) != null && m_currentNode.GetNeighbour(new Vector2Int(0, 1 * m_phaseTwoDirection)).GetGridEntities().Count == 0) // If it can move up or down
                {
                    tempDir = new Vector2Int(0, 1 * m_phaseTwoDirection); // Set direction to do so
                }
            }
            
            if (m_currentNode.GetNeighbour(new Vector2Int(0, -1 * m_phaseTwoDirection)) != null) // If going down or up
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(0, -1 * m_phaseTwoDirection)).GetGridEntities().Count == 0) // If there are no entities in the square
                {
                    tempDir = new Vector2Int(0, -1 * m_phaseTwoDirection); // Move with the corresponding direction
                }
            }
            
            // These two if statements follow the same logic but make sure the wyrm does not get stuck when next to a room transition thing
            if (m_currentNode.GetNeighbour(new Vector2Int(1 * m_phaseTwoDirection, 0)) != null && m_prevPosition == m_currentNode.position.world && m_currentNode.GetNeighbour(new Vector2Int(1 * m_phaseTwoDirection, 0)).GetGridEntities().Count == 0)
            {
                tempDir = new Vector2Int(1 * m_phaseTwoDirection, 0);
            }
            if (m_currentNode.GetNeighbour(new Vector2Int(-1 * m_phaseTwoDirection, 0)) != null && m_prevPosition == m_currentNode.position.world && m_currentNode.GetNeighbour(new Vector2Int(-1 * m_phaseTwoDirection, 0)).GetGridEntities().Count == 0)
            {
                tempDir = new Vector2Int(-1 * m_phaseTwoDirection, 0);
            }
            

            m_prevPosition = m_currentNode.position.world; // Set previous position
            dir = tempDir; // Set direction to be the temporary direction
            m_previousMoves.Insert(0, dir); // Add this direction to the list of previous moves
           

            if (m_previousMoves.Count > m_body.Count) // If the previous moves are larger than the body count
            {
                for (int i = 0; i < m_body.Count; i++) // Start for loop for the amount of entities in the body
                {
                    m_body[i].SetMovementDirection(m_previousMoves[i + 1]); // Set direction of the corresponding body to the previous direction
                }
                m_previousMoves.RemoveAt(m_previousMoves.Count - 1); // Remove the oldest previous move
            }
            else // If not
            {
                for (int i = 0; i < m_body.Count; i++) // Start for loop for amount of entities in the body 
                {
                    if (m_previousMoves.Count > i + 1) // If the previous moves list is greater than the iterator + 1
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[i + 1]); // Set direction of movement to corresponding direction
                    }
                    else // If not
                    {
                        m_body[i].SetMovementDirection(m_previousMoves[m_previousMoves.Count - 1]); // Set direction to oldest value in list
                    }

                }
            }

            if (m_firingPhaseTwo && m_fireCooldown == 0) // If the head can fire and is meant to fire this step
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null) // If the neighbour to the left is not null
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0) // IF there are no entities in the left node
                    {
                        SpawnBullet(m_bulletPrefab, m_currentNode, new Vector2(1, 0)); // Spawn a bullet
                    }
                }

                if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null) // If the neighbour to the right is not null
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0) // If there are no entities in the right node
                    {
                        SpawnBullet(m_bulletPrefab, m_currentNode, new Vector2(-1, 0)); // Spawn a bullet
                    }
                }
            }
        }


        for (int i = 0; i < m_body.Count; i++) // Loop for body count
        {
            if (m_body[i].m_firingPhaseTwo && m_body[i].m_fireCooldown == 0) // If the body compartment can fire and is meant to fire this step
            {
                if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(1, 0)) != null) // Find its left neighbour and make sure it isnt null
                {
                    if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0) // Check if there are any entities on this node
                    {
                        SpawnBullet(m_bulletPrefab, m_body[i].m_currentNode, new Vector2(1, 0)); // Spawn bullet to the left
                    }
                }

                if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null) // Find its right neighbour and make sure it isnt null
                {
                    if (m_body[i].m_currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0) // Check if there are any entities on this node
                    {
                        SpawnBullet(m_bulletPrefab, m_body[i].m_currentNode, new Vector2(-1, 0)); // Spawn bullet to the right
                    }
                }
            }
        }

        if(m_fireCooldown == 1 && m_head) // If fire cooldown is 1, as in the body is meant to fire next step
        {
            m_attackNodes.Clear(); // Clear attack nodes for the worm

            if (m_firingPhaseTwo) // If the head is firing
            {
                if (m_currentNode.GetNeighbour(new Vector2Int(1, (int)dir.y)) != null) // If the next nodes isnt null
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(1, (int)dir.y)).GetGridEntities().Count == 0) // If there are no entities
                        m_attackNodes.Add(Position.grid + new Vector2Int(1, (int)dir.y)); // Telegrpah attack
                }

                if (m_currentNode.GetNeighbour(new Vector2Int(-1, (int)dir.y)) != null) // If the next node isnt null
                {
                    if (m_currentNode.GetNeighbour(new Vector2Int(-1, (int)dir.y)).GetGridEntities().Count == 0) // If there are no entities
                        m_attackNodes.Add(Position.grid + new Vector2Int(-1, (int)dir.y)); // Telegraph attack
                }

                TelegraphBullets(-1, 1, 3); // Telegraph attacks for the other body compartments
            }
            else // If the head isn't firing
            {
                TelegraphBullets(0,2,4); // Telegraph attacks for the corresponding body compartments
            }

            foreach (var node in m_attackNodes) // Loop for amount of attack nodes
            {
                if (node != null && App.GetModule<LevelModule>().CurrentRoom[node] != null) // If the node is not null
                {
                    App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(App.GetModule<LevelModule>().CurrentRoom[node], TelegraphDrawer.Type.ATTACK); // Draw attack telegraph
                }
            }
        }

        for (int i = 0; i < m_body.Count; i++) // Loop for amount of body entities
        {
            if (m_body[i].m_fireCooldown <= 0) // IF the body entity's fire cooldown is less than 1
            {
                m_body[i].m_firingPhaseTwo = !m_body[i].m_firingPhaseTwo; // Change the firing phase two variable, controls whether this body compartment is going to fire any bullets
                m_body[i].m_fireCooldown = 3; // Set its cooldown to  3
            }

            m_body[i].m_fireCooldown--; // Tick tock, it's bullet o'clock
        }

        if (m_fireCooldown <= 0) // If it is timer to fire bullets
        {
            m_firingPhaseTwo = !m_firingPhaseTwo; //  Change the firing phase two variable, controls whether the head is going to fire any bullets
            m_fireCooldown = 3; // Set its cooldown to 3
        }


        m_fireCooldown--; // Tick tock :]
        m_stepTimer--; // Decrease the time left in phase 2
        SetMovementDirection(dir, m_moveSpeed); // Set movement
    }

    private void Phase3()
    {
        if(m_split == true) // If the wyrm has not split yet
        {
            m_body[2].m_head = true; // Set the fourth entity of the worm to be a head

            m_body[2].m_body.Add(m_body[3]); // Add the fifth entity to the new head
            m_body[2].m_body.Add(m_body[4]); // Add the sixth entity to the new head

            m_body[2].m_headEntity = m_body[2]; // Set the head entity for the fourth entity to itself
            m_body[3].m_headEntity = m_body[2]; // Set the head entity for the fifth entity to the fourth entity
            m_body[4].m_headEntity = m_body[2];// Set the head entity for the sixth entity to the fourth entity

            m_body[2].Health = (int)Mathf.Ceil(Health / 2); // Set health of the 4th entity to be the current health divided by 2 rounded up
            Health = (int)Mathf.Ceil(Health / 2); // Do the same for the current head

            m_body.RemoveAt(4); // Remove the 6th entity from the body variable for the current head
            m_body.RemoveAt(3); // Remove the 5th entity from the body variable for the current head
            m_body.RemoveAt(2); // Remove the 4th entity from the body variable for the current head

            m_split = false; // Now that the split has complete we don't want to run this code again so set this variable to false
        }

        Phase1(); // Run phase 1
    }

    private void TelegraphBullets(int secOne, int secTwo, int secThree)
    {
        if (secOne == 0) // If a body compartment is trying to telegraph attacks, this is here because in the other case it is the head
        {
            if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))) != null) // If the node we are trying to telegraph is not null
            {
                if (m_body[secOne].m_currentNode.GetNeighbour(new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))).GetGridEntities().Count == 0) // If there are no entities in this node
                    m_attackNodes.Add(m_body[secOne].Position.grid + new Vector2Int(1, (int)(m_body[secOne].Position.world.y - m_body[secOne + 1].Position.world.y))); // Add this node to attack telegraph list
            }

            // THe previously commented if statement essentially explains the rest of this code. Attacks are telegraphed on the left and right side of the nodes which are passed into this function via there index in the list. :]

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