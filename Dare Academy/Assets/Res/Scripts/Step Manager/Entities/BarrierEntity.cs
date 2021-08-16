using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class BarrierEntity : GridEntity
{
    [SerializeField] private GameEventFlags.Flags m_barrierFlag;
    [SerializeField] private GameObject m_destructionVfxPrefab;
    private bool m_haveAnimated = false;

    public bool m_flipFlag = true;

    protected override void Start()
    {
        base.Start();
    }

    public override void AnalyseStep()
    {
        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet(m_barrierFlag))
            Kill();
    }

    public override void OnDeath()
    {
        base.OnDeath();

        if (m_flipFlag)
        {
            App.GetModule<LevelModule>().EventFlags.SetFlags(m_barrierFlag, true);
            App.GetModule<blu.LevelModule>().SaveGame();
        }

        var barriers = transform.parent.GetComponentsInChildren<BarrierEntity>();

        foreach (var barrier in barriers)
        {
            if (barrier != null)
                barrier.KillImmediate();
        }
    }

    public override void KillImmediate()
    {
        if (!m_haveAnimated)
            Instantiate(m_destructionVfxPrefab, transform.position, Quaternion.identity);
        m_haveAnimated = true;
        base.KillImmediate();
    }
}