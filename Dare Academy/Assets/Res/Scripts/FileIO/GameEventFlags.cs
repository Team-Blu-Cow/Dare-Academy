using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

[System.Serializable]
public class GameEventFlags
{
    public enum Flags : long
    {
        Flag_01         = 0x0000_0000_0000_0001,
        Flag_02         = 0x0000_0000_0000_0002,
        Flag_03         = 0x0000_0000_0000_0004,
        Flag_04         = 0x0000_0000_0000_0008,
        Flag_05         = 0x0000_0000_0000_0010,
        Flag_06         = 0x0000_0000_0000_0020,
        Flag_07         = 0x0000_0000_0000_0040,
        Flag_08         = 0x0000_0000_0000_0080,

        Flag_09         = 0x0000_0000_0000_0100,
        Flag_10         = 0x0000_0000_0000_0200,
        Flag_11         = 0x0000_0000_0000_0400,
        Flag_12         = 0x0000_0000_0000_0800,
        Flag_13         = 0x0000_0000_0000_1000,
        Flag_14         = 0x0000_0000_0000_2000,
        Flag_15         = 0x0000_0000_0000_4000,
        Flag_16         = 0x0000_0000_0000_8000,

        Flag_17         = 0x0000_0000_0001_0000,
        Flag_18         = 0x0000_0000_0002_0000,
        Flag_19         = 0x0000_0000_0004_0000,
        Flag_20         = 0x0000_0000_0008_0000,
        Flag_21         = 0x0000_0000_0010_0000,
        Flag_22         = 0x0000_0000_0020_0000,
        Flag_23         = 0x0000_0000_0040_0000,
        Flag_24         = 0x0000_0000_0080_0000,

        Flag_25         = 0x0000_0000_0100_0000,
        Flag_26         = 0x0000_0000_0200_0000,
        Flag_27         = 0x0000_0000_0400_0000,
        Flag_28         = 0x0000_0000_0800_0000,
        Flag_29         = 0x0000_0000_1000_0000,
        Flag_30         = 0x0000_0000_2000_0000,
        Flag_31         = 0x0000_0000_4000_0000,
        Flag_32         = 0x0000_0000_8000_0000,

        Flag_33         = 0x0000_0001_0000_0000,
        Flag_34         = 0x0000_0002_0000_0000,
        Flag_35         = 0x0000_0004_0000_0000,
        Flag_36         = 0x0000_0008_0000_0000,
        Flag_37         = 0x0000_0010_0000_0000,
        Flag_38         = 0x0000_0020_0000_0000,
        Flag_39         = 0x0000_0040_0000_0000,
        Flag_40         = 0x0000_0080_0000_0000,

        Flag_41         = 0x0000_0100_0000_0000,
        Flag_42         = 0x0000_0200_0000_0000,
        Flag_43         = 0x0000_0400_0000_0000,
        Flag_44         = 0x0000_0800_0000_0000,
        Flag_45         = 0x0000_1000_0000_0000,
        Flag_46         = 0x0000_2000_0000_0000,
        Flag_47         = 0x0000_4000_0000_0000,
        Flag_48         = 0x0000_8000_0000_0000,

        Flag_49         = 0x0001_0000_0000_0000,
        Flag_50         = 0x0002_0000_0000_0000,
        Flag_51         = 0x0004_0000_0000_0000,
        Flag_52         = 0x0008_0000_0000_0000,
        Flag_53         = 0x0010_0000_0000_0000,
        Flag_54         = 0x0020_0000_0000_0000,
        Flag_55         = 0x0040_0000_0000_0000,
        Flag_56         = 0x0080_0000_0000_0000,

        Flag_57         = 0x0100_0000_0000_0000,
        Flag_58         = 0x0200_0000_0000_0000,
        Flag_59         = 0x0400_0000_0000_0000,
        Flag_60         = 0x0800_0000_0000_0000,
        Flag_61         = 0x1000_0000_0000_0000,
        Flag_62         = 0x2000_0000_0000_0000,
        Flag_63         = 0x4000_0000_0000_0000,
    }

    [SerializeField] private long m_flagData = 0;

    public long FlagData
    {
        get { return m_flagData; }
        set { m_flagData = value; }
    }

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

    public static long SetFlags(Flags flags, long flagData, bool value)
    {
        if (value)
        {
            flagData = flagData | (long)flags;
        }
        else
        {
            flagData = flagData & (long)~flags;
        }

        return flagData;
    }

    public static bool IsFlagSet(Flags flags, long flagData)
    {
        long set = (long)flags & flagData;
        if (set == (long)flags)
            return true;

        return false;
    }
}