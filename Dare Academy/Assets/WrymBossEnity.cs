using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class WrymBossEnity : GridEntity
{
    private PlayerEntity m_player;

    [SerializeField]
    private int m_moveSpeed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_player = FindObjectOfType<PlayerEntity>();
    }

    public override void AnalyseStep()
    {
        if (m_player == null)
            return;
        Phase1();
        if (Health > 5)
        {
        }
        else if (false) // TODO: add phase 2 timer
        {
        }
        else // Phase 3
        {
        }
    }

    public override void DamageStep()
    {
        base.DamageStep();
    }

    private void Phase1()
    {
        //PHASE 1
        Vector2 dir = new Vector2();

        Vector3[] path = App.GetModule<LevelModule>().MetaGrid.GetPath(Position.world, m_player.transform.position); // Find path to player

        if (path.Length > 1) // If path is greater than 1
        {
            dir = path[1] - path[0]; // Set direction to be distance vector of the two closest pathfinding nodes
        }
        else if (path.Length <= 1)
        {
            //attack
            Debug.Log("Wyrm attacking");
            Burrow();
        }

        SetMovementDirection(dir, m_moveSpeed); // Set movement
    }

    private void FireAttack()
    {
        for (int i = 0; i < 3; i++)
        {
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

        Vector2 pos;

        switch (direction)
        {
            case 0:
                // Burrow Right /////////
                pos = FindBurrowHoz(currentRoom.Width - 1, currentRoom);

                break;

            case 1:
                // Burrow Up /////////
                pos = FindBurrowVert(0, currentRoom);
                break;

            case 2:
                // Burrow Left /////////
                pos = FindBurrowHoz(0, currentRoom);
                break;

            case 3:
                // Burrow Down /////////
                pos = FindBurrowVert(currentRoom.Height - 1, currentRoom);

                break;

            default:
                pos = Vector2.zero;
                break;
        }

        Debug.Log(pos);
    }

    private Vector2 FindBurrowHoz(int x, JUtil.Grids.Grid<GridNode> currentRoom)
    {
        List<Vector2> exitPos = new List<Vector2>();

        for (int i = 0; i < currentRoom.Height; i++)
        {
            if (currentRoom.NodeExists(x, i))
            {
                exitPos.Add(new Vector2(x, i));
            }
        }

        return exitPos[Random.Range(0, exitPos.Count)];
    }

    private Vector2 FindBurrowVert(int y, JUtil.Grids.Grid<GridNode> currentRoom)
    {
        List<Vector2> exitPos = new List<Vector2>();

        for (int i = 0; i < currentRoom.Width; i++)
        {
            if (currentRoom.NodeExists(i, y))
            {
                exitPos.Add(new Vector2(i, y));
            }
        }

        return exitPos[Random.Range(0, exitPos.Count)];
    }

    // Update is called once per frame
    private void Update()
    {
    }
}