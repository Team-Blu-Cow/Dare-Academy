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

        if (Health > 5)
        {
            Phase1();
        }
        else if (false) // TODO: add phase 2 timer
        {
        }
        else // Phase 3
        {
        }
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

    private void Burrow()
    {
        int direction;
        var currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        Mathf.Min(m_player.Position.grid.x, m_player.Position.grid.y);

        Mathf.Min(currentRoom.Width - m_player.Position.grid.x, currentRoom.Height - m_player.Position.grid.y);

        List<Vector2> exitPos = new List<Vector2>();

        // Exit 1 /////////
        int count = 0;

        while (exitPos.Count == 0 && count < currentRoom.Height)
        {
            for (int i = 0; i < grid.gridSize.x; i++)
            {
                if (grid.GetNode(new Vector3Int(i, count, 0)).IsTraversable())
                {
                    exitPos.Add(new Vector2(i, count));
                }
            }
            count++;
        }

        var pos = exitPos[Random.Range(0, exitPos.Count)];
    }

    // Update is called once per frame
    private void Update()
    {
    }
}