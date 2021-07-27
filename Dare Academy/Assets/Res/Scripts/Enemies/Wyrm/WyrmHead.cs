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

    protected override void OnValidate()
    {
        base.OnValidate();
        m_damageEntityPrefab = Resources.Load<GameObject>("prefabs/Entities/DamageEntity");
    }

    protected override void Start()
    {
        levelModule = App.GetModule<LevelModule>();
        base.Start();
    }

    public override void AnalyseStep()
    {
        base.AnalyseStep();

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