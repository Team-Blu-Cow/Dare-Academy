using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class BarrierEntity : GridEntity
{
    [SerializeField] private GameEventFlags.Flags m_flagToFlip;

    protected override void Start()
    {
        base.Start();

        if (App.GetModule<LevelModule>().EventFlags.IsFlagsSet(m_flagToFlip))        
            DestroyAll();
        
    }

    private void OnDestroy()
    {
        App.GetModule<LevelModule>().EventFlags.SetFlags(m_flagToFlip, true);
        DestroyAll();
    }

    void DestroyAll()
    {
        foreach (BarrierEntity barrier in transform.parent.GetComponentsInChildren<BarrierEntity>())
        {
            barrier.CleanUp();
        }
    }

}
