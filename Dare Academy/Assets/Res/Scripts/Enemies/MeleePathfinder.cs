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

    public bool showPath = false;
    public enum State
    {
        MOVE,
        ATTACK
    }

    public enum AnimationState : int 
    { 
        IDLE,
        IDLE_ATTACKING,
        MOVE
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
        m_flags.SetFlags(GridEntityFlags.Flags.isKillable, true);
        m_flags.SetFlags(GridEntityFlags.Flags.isSolid, true);
        m_player = PlayerEntity.Instance;
        m_animationController.animator.speed = 1;
        m_animationController.m_overwriteAnimSpeed = false;
    }

    public override void AnalyseStep()
    {
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
                        return;
                    }
                }
            }
            angle += 90;
        }
    }

    private void MoveState()
    {
        m_path = App.GetModule<LevelModule>().MetaGrid.GetPath(m_currentNode, m_player.currentNode);

        Vector3 dir = Vector3.zero;

        if (m_path.Length > 1)
            dir = m_path[0] - m_currentNode.position.world;

        if(Mathf.Abs(dir.x) > 0 && Mathf.Abs(dir.y) > 0)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                dir = new Vector3(0, Mathf.Sign(dir.y), 0);
        }

        m_dir = new Vector2Int((int)dir.x, (int)dir.y);

        SetMovementDirection(m_dir, moveSpeed);

        m_animationController.SetDirection(-m_dir.x, 1);
    }

    public override void AttackStep()
    {
        if (m_state == State.ATTACK && m_attackNode != null)
        {
            GameObject gobj = Instantiate(m_attackPrefab, m_attackNode.position.world, Quaternion.identity);

            MeleeAttackEntity ent = gobj.GetComponent<MeleeAttackEntity>();

            ent.Init(m_attackNode, m_attackVFXPrefab);
        }
    }

    public void OnDrawGizmos()
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
    }

    public void Tween(float tweenVal, float targetVal, float time, System.Action<float> setMethod)
    {
        LeanTween.value(tweenVal, targetVal, time)
            .setOnUpdate(setMethod);
    }
}