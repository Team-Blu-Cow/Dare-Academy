using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class EightDirectionalEntity : GridEntity
{
    [Header("Attack attributes")]
    [SerializeField] private int m_attackSpeed = 3; // How often the entity fires bullets

    [SerializeField] private bool isFiringHorizontal = true; // Boolean for if the entity is firing horizontally or diagonally
    [SerializeField] private int agroRange = 5; // Variable which controls when the entity starts firing
    private int m_attackCounter = 0; // Cooldown timer for after firing bullets
    private bool isAttacking = false; // Boolean for whether the entity is firing or not
    private Vector2[] telegraphPos = { new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)}; // Positions for telegraphing where the entity is going to spawn bullets in the next step

    [Header("Resources needed")]
    [SerializeField] private GameObject m_bulletPrefab = null; // Bullet prefab for spawning

    [SerializeField] private GridEntity player; // Player to figure out how close they are to the entity (for agro range check)

    [SerializeField] private ParticleSystem m_sleepyParticles;

    protected override void Start()
    {
        m_health = 5; // Set health to 5
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true); // Set flag for killable to true
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true); // Set flag for if solid to true

        player = PlayerEntity.Instance; // Find the player
        m_bulletPrefab = Resources.Load<GameObject>("prefabs/Entities/Bullet"); // Find the bullet prefab
        base.Start(); // Run base start
        m_animationController.SetAnimationSpeed(0.5f);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        m_sleepyParticles = GetComponentInChildren<ParticleSystem>();
    }

    public override void AnalyseStep()
    {
        if (player == null)
            return;

        base.AnalyseStep(); // Run base function
        m_animationController.SetAnimationSpeed(1);
        float dist = Vector3.Distance(m_currentNode.position.world, player.currentNode.position.world);

        m_animationController.animator.SetBool("isAsleep", dist >= agroRange);

        if (dist < agroRange) // If the player is within agro range
        {

            if (m_attackCounter >= m_attackSpeed) //  If the cooldown has expired
            {
                isAttacking = true; // Set attacking to true
                TelegraphAttack(); // Telegraph the attacking position
            }
        }
    }

    public void StartSleeping()
    {
        if (!m_sleepyParticles.isPlaying)
            m_sleepyParticles.Play();
    }

    public void StopSleeping()
    {
        if (m_sleepyParticles.isPlaying)
            m_sleepyParticles.Stop();
    }

    public override void AttackStep()
    {
        m_attackCounter++; // Increment cooldown
        if (isAttacking == true) // If the entity is meant to be attacking
        {
            if (m_attackCounter >= m_attackSpeed) // IF the cooldown has expired
            {
                TelegraphAttack(); // Telegraph the attacking positions
                SpawnBullets(); // Spawn the bullets
                m_attackCounter = 0; // Reset cooldown
            }
            isAttacking = false; // Set attacking to false
        }
    }

    private void TelegraphAttack()
    {
        Vector2Int[] m_attackDirections = GetAttackDirections(); // Find the attack directions by calling a function

        for (int i = 0; i < 4; i++) // Loop four times (each direction)
        {
            if (m_currentNode.GetNeighbour(m_attackDirections[i]) != null) // If the grid neighbour is not a wall then it is fine to spawn a bullet
            {
                telegraphPos[i] = m_currentNode.GetNeighbour(m_attackDirections[i]).position.world; // Set the telegraph position to whatever the position of that node is
            }
        }
    }

    protected bool SpawnBullets()
    {
        if (m_bulletPrefab) // If the bullet prefab exists in the right location in resources
        {
            Vector2Int[] m_attackDirections = GetAttackDirections(); // Get the attacking directions

            for (int j = 0; j < 4; j++) // Loop for each direction
            {
                Vector3 spawnPosition; // Declare spawn position variable
                if (m_currentNode.GetNeighbour(m_attackDirections[j]) != null) // If the node where the bullet is meant to spawn is not a wall
                {
                    spawnPosition = m_currentNode.GetNeighbour(m_attackDirections[j]).position.world; // Set the bullet's spawn position to the current attacking directions's grid node
                    GameObject obj = GameObject.Instantiate(m_bulletPrefab, spawnPosition, Quaternion.identity); // Spawn the bullet

                    if (obj) // If the bullet exists
                    {
                        BulletEntity bullet = obj.GetComponent<BulletEntity>(); // Get the bullet entity script from the game object
                        if (bullet) // If the script exists within the bullet game object
                        {
                            bullet.m_bulletDirection = m_attackDirections[j]; // Set the bullet's direction to the corresponding attack direction

                            if (j == 3) // If this is the last bullet to be spawned
                            {
                                isFiringHorizontal = !isFiringHorizontal; // Change the firing direction
                                return true; // Return true (Don't think this is needed anymore)
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private Vector2Int[] GetAttackDirections()
    {
        Vector2Int[] m_attackDirections; // Declare array for attack directions
        if (isFiringHorizontal) // If the entity is meant to firing horizontally
        {
            m_attackDirections = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) }; // Set the attacking directions to the corresponding 2D vectors
            return m_attackDirections; // Return the newly made array
        }
        else // If firing diagonally
        {
            m_attackDirections = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(-1, -1) }; // Set the attacking directions to the corresponding 2D vectors
            return m_attackDirections; // Return the newly made array
        }
    }

    protected override void OnDrawGizmos()
    {
        for (int i = 0; i < 4; i++) // Loop for all four firing directions
        {
            if (telegraphPos[i] != new Vector2(0, 0)) // If the telegraph position is essentially not null
            {
                if (!isAttacking) // If the entity is not attacking
                {
                    Gizmos.color = new Color(0, 0, 0, 0); // Do not set a visible color
                }
                else // If the entity is attacking
                {
                    Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.25f); // Set the drawing color to be a transparent yellow
                }

                Gizmos.DrawCube(telegraphPos[i], new Vector3(1, 1, 1)); // Draw square at the correct coordinates using the telegraph positions vector array
            }
        }
    }
}