using System;

[System.Serializable]
public class GridEntityInternalFlags : BitFlags_32
{
    public enum Flags : Int32
    {
        isDead              = 0b00000000000000000000000000000001,
        refectBullets_N     = 0b00000000000000000000000000000010,
        refectBullets_E     = 0b00000000000000000000000000000100,
        refectBullets_S     = 0b00000000000000000000000000001000,
        refectBullets_W     = 0b00000000000000000000000000010000,
        refectBullets_NE    = 0b00000000000000000000000000100000,
        refectBullets_NW    = 0b00000000000000000000000001000000,
        refectBullets_SE    = 0b00000000000000000000000010000000,
        refectBullets_SW    = 0b00000000000000000000000100000000,
    }

    public void SetFlags(Flags flags, bool value)
    {
        if (value)
            m_flagData = m_flagData | (Int32)flags;
        else
            m_flagData = m_flagData & ~(Int32)flags;
    }

    public void FlipFlags(Flags flags)
    {
        m_flagData = m_flagData ^ (Int32)flags;
    }

    public bool IsFlagsSet(Flags flags)
    {
        if (((Int32)flags & m_flagData) == (Int32)flags)
            return true;

        return false;
    }
}