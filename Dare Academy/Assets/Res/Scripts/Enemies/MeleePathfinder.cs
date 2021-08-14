using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil;

public class MeleePathfinder : GridEntity
{
    private Vector2 m_dir = new Vector2();
    [SerializeField] private int moveSpeed = 1;

    [SerializeField] private GridEntity m_player;

    private Vector3[] m_path;

    [SerializeField] private GameObject m_attackPrefab;
    [SerializeField] private GridNode m_attackNode;

    [SerializeField] private GameObject m_attackVFXPrefab;

    [FMODUnity.EventRef]
    [SerializeField]
    private string m_deathSFX = null;

    [FMODUnity.EventRef]
    [SerializeField]
    private string m_attackSFX = null;

    private Vector3 m_attackVfxSpawnPos;

    public bool showPath = false;

    public enum State
    {
        MOVE,
        ATTACK
    }

    [SerializeField] private State m_state;

    protected override void OnValidate()
    {
        base.OnValidate();
        m_attackPrefab = Resources.Load<GameObject>("prefabs/Entities/MeleeAttackEntity");
    }

    protected override void Start()
    {
        base.Start();
        m_player = PlayerEntity.Instance;
        m_animationController.animator.speed = 1;
        m_animationController.m_overwriteAnimSpeed = false;
    }

    public override void AnalyseStep()
    {
        if (isDead)
            return;

        m_animationController.animator.speed = 1;

        if (m_player == null)
            return;

        if (m_player.currentNode == null)
            return;

        DecideState();

        switch (m_state)
        {
            case State.MOVE:
                MoveState();
                break;

            case State.ATTACK:
                m_animationController.animator.SetBool("IsAttacking", true);
                m_animationController.animator.SetBool("IsMoving", false);
                break;
        }
    }

    private void DecideState()
    {
        m_state = State.MOVE;

        float angle = 0;

        for (int i = 0; i < 4; i++)
        {
            Vector2 v = Vector2.up.Rotate(angle);
            GridNode node = m_currentNode.Neighbors[v].reference;
            if (node != null && node.roomIndex == m_currentNode.roomIndex)
            {
                foreach (var e in node.GetGridEntities())
                {
                    if (e.Flags.IsFlagsSet(GridEntityFlags.Flags.isPlayer))
                    {
                        m_state = State.ATTACK;
                        m_attackNode = node;
                        App.GetModule<LevelModule>().telegraphDrawer.CreateTelegraph(node, TelegraphDrawer.Type.ATTACK);
                        m_animationController.SetDirection(-v.x, 1);
                        return;
                    }
                }
            }
            angle += 90;
        }
    }

    private void MoveState()
    {
        m_path = App.GetModule<LevelModule>().MetaGrid.GetPath(m_currentNode, m_player.currentNode, m_flags.IsFlagsSet(GridEntityFlags.Flags.isAirBorn));

        Vector3 dir = Vector3.zero;

        if (m_path == null)
        {
            SetMovementDirection(Vector2Int.zero, 0);
            return;
        }

        if (m_path.Length > 1)
            dir = m_path[0] - m_currentNode.position.world;

        if (Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.y) > 0)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                dir = new Vector3(0, Mathf.Sign(dir.y), 0);
        }

        m_dir = new Vector2Int((int)dir.x, (int)dir.y);

        SetMovementDirection(m_dir, moveSpeed);

        LevelManager.Instance.TelegraphDrawer.CreateTelegraph(m_currentNode.Neighbors[new Vector2(dir.x, dir.y)].reference, TelegraphDrawer.Type.MOVE);

        m_animationController.SetDirection(-m_dir.x, 1);
    }

    public override void AttackStep()
    {
        if (isDead)
            return;

        if (m_state == State.ATTACK && m_attackNode != null)
        {
            GameObject gobj = Instantiate(m_attackPrefab, m_attackNode.position.world, Quaternion.identity);

            MeleeAttackEntity ent = gobj.GetComponent<MeleeAttackEntity>();
            ent.m_damageTimeOffset = 0.15f;

            m_attackVfxSpawnPos = m_attackNode.position.world + new Vector3(0, 0.4f, 0);

            ent.Init(m_attackNode, m_attackVFXPrefab);

            App.GetModule<AudioModule>().PlayAudioEvent(m_attackSFX);
        }
    }

    protected override void OnDrawGizmos()
    {
        if (showPath)
            JUtil.JUtils.DrawPath(m_path, Position.world);
    }

    public override void DrawStep()
    {
        m_animationController.animator.speed = 1;

        switch (m_state)
        {
            case State.MOVE:
                m_animationController.animator.SetBool("IsMoving", true);
                m_animationController.animator.SetBool("IsAttacking", false);
                m_animationController.animator.SetBool("HasAttacked", false);
                LeanTween.move(gameObject, m_currentNode.position.world, m_stepController.stepTime)
                    .setOnComplete(() => { m_animationController.animator.SetBool("IsMoving", false); });
                break;

            case State.ATTACK:
                m_animationController.animator.SetBool("IsAttacking", false);
                m_animationController.animator.SetBool("HasAttacked", true);
                m_animationController.animator.SetBool("IsMoving", false);
                break;
        }
    }

    public void EndAttackAnimation()
    {
        m_animationController.animator.SetBool("HasAttacked", false);
        Instantiate(m_attackVFXPrefab, m_attackVfxSpawnPos, Quaternion.identity);
    }

    public void Tween(float tweenVal, float targetVal, float time, System.Action<float> setMethod)
    {
        LeanTween.value(tweenVal, targetVal, time)
            .setOnUpdate(setMethod);
    }

    public override void OnHit(int damage, float offsetTime = 0f)
    {
        base.OnHit(damage);

        m_animationController.DamageFlash();
    }

    public override void CleanUp()
    {
        m_animationController.SpawnDeathPoof(transform.position);
        base.CleanUp();
    }

    public override void OnDeath()
    {
        // #TODO #sound play specific death sound once impl App.GetModule<AudioModule>().PlayAudioEvent(m_deathSFX);
        m_animationController.PlayAnimation("Die", 1);
    }
}