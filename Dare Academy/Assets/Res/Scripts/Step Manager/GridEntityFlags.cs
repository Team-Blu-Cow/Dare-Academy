using System;

[System.Serializable]
public class GridEntityFlags : BitFlags_32
{
    public enum Flags : Int32
    {
        isPlayer            = 0b00000000000000000000000000000001,
        isAttack            = 0b00000000000000000000000000000010,
        isSolid             = 0b00000000000000000000000000000100,
        isPushable          = 0b00000000000000000000000000001000,
        isKillable          = 0b00000000000000000000000000010000,
        allowRoomSwitching  = 0b00000000000000000000000000100000,
        destroyOnReset      = 0b00000000000000000000000001000000,
        killOnRoomSwitch    = 0b00000000000000000000000010000000,
        refectAllBullets    = 0b00000000000000000000000100000000,
        allowedOffGrid      = 0b00000000000000000000001000000000,
        alwaysWinConflict   = 0b00000000000000000000010000000000,
        keepAwake           = 0b00000000000000000000100000000000,
        dontMoveOnReset     = 0b00000000000000000001000000000000,
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
        if (m_flagData == 0)
            return false;

        if (((Int32)flags & m_flagData) == (Int32)flags)
            return true;

        return false;
    }
}