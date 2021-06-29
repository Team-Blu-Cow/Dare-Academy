using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntityFlags
{
    public enum Flags : uint
    {
        isPlayer    = 0b00000000000000000000000000000001,
        isAttack    = 0b00000000000000000000000000000010,
        isSolid     = 0b00000000000000000000000000000100,
        isPushable  = 0b00000000000000000000000000001000,
        isKillable  = 0b00000000000000000000000000010000,
        Flag06      = 0b00000000000000000000000000100000,
        Flag07      = 0b00000000000000000000000001000000,
        Flag08      = 0b00000000000000000000000010000000,
        Flag09      = 0b00000000000000000000000100000000,
        Flag10      = 0b00000000000000000000001000000000,
        Flag11      = 0b00000000000000000000010000000000,
        Flag12      = 0b00000000000000000000100000000000,
        Flag13      = 0b00000000000000000001000000000000,
        Flag14      = 0b00000000000000000010000000000000,
        Flag15      = 0b00000000000000000100000000000000,
        Flag16      = 0b00000000000000001000000000000000,
        Flag17      = 0b00000000000000010000000000000000,
        Flag18      = 0b00000000000000100000000000000000,
        Flag19      = 0b00000000000001000000000000000000,
        Flag20      = 0b00000000000010000000000000000000,
        Flag21      = 0b00000000000100000000000000000000,
        Flag22      = 0b00000000001000000000000000000000,
        Flag23      = 0b00000000010000000000000000000000,
        Flag24      = 0b00000000100000000000000000000000,
        Flag25      = 0b00000001000000000000000000000000,
        Flag26      = 0b00000010000000000000000000000000,
        Flag27      = 0b00000100000000000000000000000000,
        Flag28      = 0b00001000000000000000000000000000,
        Flag29      = 0b00010000000000000000000000000000,
        Flag30      = 0b00100000000000000000000000000000,
        Flag31      = 0b01000000000000000000000000000000,
        Flag32      = 0b10000000000000000000000000000000
    }

    private uint m_flagData = 0;

    public void ZeroFlags()
    {
        m_flagData = 0;
    }

    public void Toggle(uint flags, bool value)
    {
        if (value)
        {
            m_flagData = m_flagData | flags;
        }
        else
        {
            m_flagData = m_flagData & ~flags;
        }
    }

    public void FlipFlags(uint flags)
    {
        m_flagData = m_flagData ^ flags;
    }

    public bool IsFlagsSet(uint flags)
    {
        uint set = flags & m_flagData;
        if (set == flags)
            return true;

        return false;
    }
}