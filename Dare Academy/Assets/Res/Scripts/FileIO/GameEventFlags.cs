using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

[System.Serializable]
public class GameEventFlags
{
    public enum Flags : long
    {
        _nullflag   = 0b0000000000000000000000000000000000000000000000000000000000000000,
    }

    [SerializeField] private long m_flagData = 0;

    public static int NumberOfFlags()
    {
        return System.Enum.GetNames(typeof(GridEntityFlags.Flags)).Length;
    }

    public void ZeroFlags()
    {
        m_flagData = 0;
    }

    public void SetFlags(Flags flags, bool value)
    {
        if (value)
        {
            m_flagData = m_flagData | (long)flags;
        }
        else
        {
            m_flagData = m_flagData & ~(long)flags;
        }
    }

    public void FlipFlags(Flags flags)
    {
        m_flagData = m_flagData ^ (long)flags;
    }

    public bool IsFlagsSet(Flags flags)
    {
        Flags set = flags & (Flags)m_flagData;
        if (set == flags)
            return true;

        return false;
    }
}