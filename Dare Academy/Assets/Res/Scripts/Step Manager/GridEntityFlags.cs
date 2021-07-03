using UnityEngine;

[System.Serializable]
public class GridEntityFlags
{
    public enum Flags : uint
    {
        isPlayer    = 0b00000000000000000000000000000001,
        isAttack    = 0b00000000000000000000000000000010,
        isSolid     = 0b00000000000000000000000000000100,
        isPushable  = 0b00000000000000000000000000001000,
        isKillable  = 0b00000000000000000000000000010000,
        isDead      = 0b00000000000000000000000000100000,
    }

    [SerializeField] private Flags m_flagData = 0;

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
            m_flagData = m_flagData | flags;
        }
        else
        {
            m_flagData = m_flagData & ~flags;
        }
    }

    public static uint SetFlags(Flags flags, uint flagData, bool value)
    {
        if (value)
        {
            flagData = flagData | (uint)flags;
        }
        else
        {
            flagData = flagData & (uint)~flags;
        }

        return flagData;
    }

    public void FlipFlags(Flags flags)
    {
        m_flagData = m_flagData ^ flags;
    }

    public bool IsFlagsSet(Flags flags)
    {
        Flags set = flags & m_flagData;
        if (set == flags)
            return true;

        return false;
    }

    public static bool IsFlagSet(Flags flags, uint flagData)
    {
        uint set = (uint)flags & flagData;
        if (set == (uint)flags)
            return true;

        return false;
    }
}