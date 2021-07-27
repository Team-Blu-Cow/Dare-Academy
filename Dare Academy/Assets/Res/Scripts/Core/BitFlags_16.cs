using System;

// holy fucking shit i hate c# sometimes
// the giga brains over at MSFT decided that operations on shorts should return ints
// i shit you not, thats in the fucking spec, like wtf???
// in short this means we have to cast everything back to a short after were done

public class BitFlags_16
{
    public enum Flags_16Base : Int16
    {
        Flag_01         = 0x0001,
        Flag_02         = 0x0002,
        Flag_03         = 0x0004,
        Flag_04         = 0x0008,
        Flag_05         = 0x0010,
        Flag_06         = 0x0020,
        Flag_07         = 0x0040,
        Flag_08         = 0x0080,

        Flag_09         = 0x0100,
        Flag_10         = 0x0200,
        Flag_11         = 0x0400,
        Flag_12         = 0x0800,
        Flag_13         = 0x1000,
        Flag_14         = 0x2000,
        Flag_15         = 0x4000,
    }

    [UnityEngine.SerializeField] protected Int16 m_flagData = 0;

    public Int16 _FlagData
    {
        get { return m_flagData; }
        set { m_flagData = value; }
    }

    public virtual int NumberOfFlags()
    {
        return System.Enum.GetNames(typeof(Flags_16Base)).Length;
    }

    public void ZeroFlags() => m_flagData = 0;

    public void SetFlags(Int16 flags, bool value)
    {
        if (value)
            m_flagData = (Int16)(m_flagData | flags);
        else
            m_flagData = (Int16)(m_flagData & ~flags);
    }

    public void FlipFlags(Int16 flags)
    {
        m_flagData = (Int16)(m_flagData ^ flags);
    }

    public bool IsFlagsSet(Int16 flags)
    {
        if (m_flagData == 0 || flags == 0)
            return false;

        if (((Int16)(flags & m_flagData)) == flags)
            return true;

        return false;
    }

    public static Int16 SetFlags(Int16 flags, Int16 flagData, bool value)
    {
        if (value)
            flagData = (Int16)(flagData | flags);
        else
            flagData = (Int16)(flagData & ~flags);

        return flagData;
    }

    public static bool IsFlagSet(Int16 flags, Int16 flagData)
    {
        if (flagData == 0 || flags == 0)
            return false;

        if (((Int16)(flags & flagData)) == flags)
            return true;

        return false;
    }
}