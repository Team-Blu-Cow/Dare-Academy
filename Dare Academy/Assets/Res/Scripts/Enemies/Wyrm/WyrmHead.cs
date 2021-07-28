using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using System;

public class WyrmHead : WyrmSection
{
    private LevelModule levelModule;

    private BossPhase m_phase = BossPhase.Phase1;
    private int m_stepsUntilResurface = 0;

    private List<Vector2Int> m_attackNodes = new List<Vector2Int>();

    private GameObject m_damageEntityPrefab;

    private bool m_hasSplit = false;

    private Vector2Int m_prevPosition = Vector2Int.zero;
    private int m_phaseTwoDir = 1;
    private GameObject m_bulletPrefab = null; // Bullet prefab for spawning bullets
    private int m_fireCooldown = 3;
    private bool firingSide = false;
    public int m_stepTimer = 50;

    protected override void OnValidate()
    {
        base.OnValidate();
        m_damageEntityPrefab = Resources.Load<GameObject>("prefabs/Entities/DamageEntity");
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet"); // Find bullet prefab
    }

    protected override void Start()
    {
        levelModule = App.GetModule<LevelModule>();
        base.Start();

        if (m_hasSplit == false)
        {
            Health = 10;
        }
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();

        Debug.Log("Health - " + Health);

        if (m_currentNode == null)
        {
            m_stepsUntilResurface--;
            if (m_stepsUntilResurface <= 0)
            {
                Resurface();
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
            return;
        }

        if (Health > 5)
        {
            m_phase = BossPhase.Phase1;
        }
        else if (m_stepTimer > 0)
        {
            m_phase = BossPhase.Phase2;
        }
        else
        {
            m_phase = BossPhase.Phase3;
        }

        if(Health <= 0)
        {
            Kill();
            List<WyrmSection> sections = new List<WyrmSection>();
            WyrmSection current = this;
            while (current)
            {
                sections.Add(current);
                current = current.SectionBehind;
            }

            for(int i = (sections.Count - 1); i > 0; i--)
            {
                sections[i].Kill();
            }
        }

        switch (m_phase)
        {
            case BossPhase.Phase1:
                Phase1();
                break;

            case BossPhase.Phase2:
                Phase2();
                break;

            case BossPhase.Phase3:
                Phase3();
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

    private void Phase1()
    {
        GridNode target = PlayerEntity.Instance.currentNode;
        if (target == null)
            return;

        if (currentNode == null)
            return;

        Vector3[] path;
        if (SectionBehind.currentNode == null)
        {
            path = levelModule.MetaGrid.GetPath(currentNode, target);
        }
        else
        {
            path = levelModule.MetaGrid.GetPathWithAvoidance(currentNode.position.world, target.position.world, SectionBehind.Position.world, 1);
        }

        if (path == null)
            return;

        if (Vector2Int.Distance(Position.grid, PlayerEntity.Instance.Position.grid) <= 1)
        {
            Vector2 temp = Position.grid - PlayerEntity.Instance.Position.grid;
            FireAttack(temp.normalized);
            Burrow();
        }
        else
        {
            GridNode node = levelModule.MetaGrid.GetNodeFromWorld(path[0]);

            Vector2Int dir = node.position.grid - m_currentNode.position.grid;

            SetMovementDirection(dir);
        }
    }

    private void Phase2()
    {
        Vector2 dir = new Vector2(0, 1);

        if (m_currentNode.GetNeighbour(new Vector2Int((int)dir.x, (int)dir.y)) == null || m_prevPosition == m_currentNode.position.grid)
        {
            dir = new Vector2(-1 * m_phaseTwoDir, 0);
        }

        if (m_currentNode.GetNeighbour(new Vector2Int(0, -1)) != null && m_currentNode.GetNeighbour(new Vector2Int(0, -1)).GetGridEntities().Count == 0)
        {
            dir = new Vector2(0, -1);
        }

        if ((m_currentNode.GetNeighbour(new Vector2Int(0, 1)) != null && m_currentNode.GetNeighbour(new Vector2Int(0, 1)).GetGridEntities().Count != 0) && m_currentNode.GetNeighbour(new Vector2Int(0, -1)) == null)
        {
            dir = new Vector2(-1 * m_phaseTwoDir, 0);
        }

        if (m_currentNode.GetNeighbour(new Vector2Int(-1 * m_phaseTwoDir, 0)) == null)
        {
            m_phaseTwoDir *= -1;
        }

        FireBullets();

        m_stepTimer--;
        m_prevPosition = m_currentNode.position.grid;
        SetMovementDirection(dir);
    }

    private void Phase3()
    {
        if (!m_hasSplit)
        {
            m_hasSplit = true;
            Split();
        }

        Phase1();
    }

    private void FireBullets()
    {
        List<WyrmSection> sections = new List<WyrmSection>();
        WyrmSection current = this;
        while (current)
        {
            sections.Add(current);
            current = current.SectionBehind;
        }

        int temp = 0;

        if (firingSide == true)
        {
            temp = 1;
        }

        if (m_fireCooldown == 1)
            TelegraphBullets(sections, (0 + temp), (2 + temp), (4 + temp));

        if (m_fireCooldown <= 0)
        {
            if (sections[0 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)) != null && sections[0 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[0 + temp].currentNode, new Vector2(1, 0));

            if (sections[0 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null && sections[0 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[0 + temp].currentNode, new Vector2(-1, 0));

            if (sections[2 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)) != null && sections[2 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[2 + temp].currentNode, new Vector2(1, 0));

            if (sections[2 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null && sections[2 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[2 + temp].currentNode, new Vector2(-1, 0));

            if (sections[4 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)) != null && sections[4 + temp].currentNode.GetNeighbour(new Vector2Int(1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[4 + temp].currentNode, new Vector2(1, 0));

            if (sections[4 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)) != null && sections[4 + temp].currentNode.GetNeighbour(new Vector2Int(-1, 0)).GetGridEntities().Count == 0)
                SpawnBullet(m_bulletPrefab, sections[4 + temp].currentNode, new Vector2(-1, 0));

            m_fireCooldown = 3;
            firingSide = !firingSide;
        }

        m_fireCooldown--;
    }

    private void TelegraphBullets(List<WyrmSection> sections, int secOne, int secTwo, int secThree)
    {
        m_attackNodes.Clear(); // Clear attack nodes for the worm

        if (sections[secOne].currentNode.GetNeighbour(new Vector2Int(1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y))) != null) // If the node we are trying to telegraph is not null
        {
            if (sections[secOne].currentNode.GetNeighbour(new Vector2Int(1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y))).GetGridEntities().Count == 0) // If there are no entities in this node
                m_attackNodes.Add(sections[secOne].Position.grid + new Vector2Int(1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y))); // Add this node to attack telegraph list
        }

        // THe previously commented if statement essentially explains the rest of this code. Attacks are telegraphed on the left and right side of the nodes which are passed into this function via there index in the list. :]

        if (sections[secOne].currentNode.GetNeighbour(new Vector2Int(-1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y))) != null)
        {
            if (sections[secOne].currentNode.GetNeighbour(new Vector2Int(-1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(sections[secOne].Position.grid + new Vector2Int(-1, (int)(sections[secOne].Position.world.y - sections[secOne + 1].Position.world.y)));
        }

        if (sections[secTwo].currentNode.GetNeighbour(new Vector2Int(1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y))) != null)
        {
            if (sections[secTwo].currentNode.GetNeighbour(new Vector2Int(1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(sections[secTwo].Position.grid + new Vector2Int(1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y)));
        }

        if (sections[secTwo].currentNode.GetNeighbour(new Vector2Int(-1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y))) != null)
        {
            if (sections[secTwo].currentNode.GetNeighbour(new Vector2Int(-1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(sections[secTwo].Position.grid + new Vector2Int(-1, (int)(sections[secTwo].Position.world.y - sections[secTwo + 1].Position.world.y)));
        }

        if (sections[secThree].currentNode.GetNeighbour(new Vector2Int(1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y))) != null)
        {
            if (sections[secThree].currentNode.GetNeighbour(new Vector2Int(1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(sections[secThree].Position.grid + new Vector2Int(1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y)));
        }

        if (sections[secThree].currentNode.GetNeighbour(new Vector2Int(-1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y))) != null)
        {
            if (sections[secThree].currentNode.GetNeighbour(new Vector2Int(-1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y))).GetGridEntities().Count == 0)
                m_attackNodes.Add(sections[secThree].Position.grid + new Vector2Int(-1, -(int)(sections[secThree].Position.world.y - sections[secThree - 1].Position.world.y)));
        }

        foreach (var node in m_attackNodes) // Loop for amount of attack nodes
        {
            if (node != null && App.GetModule<LevelModule>().CurrentRoom[node] != null) // If the node is not null
            {
                App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(App.GetModule<LevelModule>().CurrentRoom[node], TelegraphDrawer.Type.ATTACK); // Draw attack telegraph
            }
        }
    }

    protected bool ShouldBurrow()
    {
        return false;
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

    protected override void Burrow()
    {
        base.Burrow();
        m_stepsUntilResurface = 3;
    }

    protected override void Resurface()
    {
        m_currentNode = GenerateBurrowLocation();
        base.Resurface();
    }

    private void FireAttack(Vector2 dir)
    {
        // #matthew cleanup

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
                    if (levelModule.CurrentRoom[Position.grid + new Vector2Int(j * -(int)Mathf.Sign(dir.x), i)] != null)
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
                    if (levelModule.CurrentRoom[Position.grid + new Vector2Int(i, j * -(int)Mathf.Sign(dir.y))] != null)
                        m_attackNodes.Add(Position.grid + new Vector2Int(i, j * -(int)Mathf.Sign(dir.y)));
                }
            }
        }

        foreach (var node in m_attackNodes)
        {
            if (node != null && App.GetModule<LevelModule>().CurrentRoom[node] != null)
            {
                levelModule.telegraphDrawer.CreateTelegraph(levelModule.CurrentRoom[node], TelegraphDrawer.Type.ATTACK);
            }
        }

        foreach (var node in m_attackNodes)
        {
            GameObject obj = Instantiate(m_damageEntityPrefab, App.GetModule<LevelModule>().CurrentRoom.ToWorld(node), Quaternion.identity);
            obj.GetComponent<DamageEntity>().Countdown = 0;
        }
        m_attackNodes.Clear();
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
                head.m_phase = BossPhase.Phase3;

                //set health
                int h = Health/2;
                head.Health = h;

                head.m_stepTimer = 0; // Don't make it go into phase two

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
}