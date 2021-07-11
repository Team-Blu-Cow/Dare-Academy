using System;

[System.Serializable]
public class GridEntityInternalFlags : BitFlags_32
{
    public enum Flags : Int32
    {
        isDead              = 0b00000000000000000000000000000001,
        refectUpBullets     = 0b00000000000000000000000000000010,
        refectRightBullets  = 0b00000000000000000000000000000100,
        refectLeftBullets   = 0b00000000000000000000000000001000,
        refectDownBullets   = 0b00000000000000000000000000010000,
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