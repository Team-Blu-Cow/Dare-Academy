using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class BarrierEntity : GridEntity
{
    [SerializeField] private GameEventFlags.Flags m_barrierFlag;

    public override void AnalyseStep()
    {
        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet(m_barrierFlag))
            Kill();
    }

    public override void OnDeath()
    {
        base.OnDeath();

        App.GetModule<LevelModule>().EventFlags.SetFlags(m_barrierFlag, true);

        var barriers = transform.parent.GetComponentsInChildren<BarrierEntity>();

        foreach (var barrier in barriers)
        {
            if (barrier != null)
                barrier.CleanUp();
        }
    }
}