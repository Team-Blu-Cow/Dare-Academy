using System;

public class GameEventFlags : BitFlags_32
{
    public enum Flags : Int32
    {
        shoot_unlocked = 0x0000_0001,
        dash_unlocked = 0x0000_0002,
        block_unlocked = 0x0000_0004,
        movement_tutorial_complete = 0x0000_0008,
        first_enemy_encounter_complete = 0x0000_0010,
        hidden_room_enemy_hint_complete = 0x0000_0020,
        scream_cutscene_complete = 0x0000_0040,
        Flag_08 = 0x0000_0080,

        Flag_09 = 0x0000_0100,
        Flag_10 = 0x0000_0200,
        Flag_11 = 0x0000_0400,
        Flag_12 = 0x0000_0800,
        Flag_13 = 0x0000_1000,
        Flag_14 = 0x0000_2000,
        Flag_15 = 0x0000_4000,
        Flag_16 = 0x0000_8000,

        Flag_17 = 0x0001_0000,
        Flag_18 = 0x0002_0000,
        Flag_19 = 0x0004_0000,
        Flag_20 = 0x0008_0000,
        Flag_21 = 0x0010_0000,
        Flag_22 = 0x0020_0000,
        Flag_23 = 0x0040_0000,
        Flag_24 = 0x0080_0000,

        Flag_25 = 0x0100_0000,
        Flag_26 = 0x0200_0000,
        Flag_27 = 0x0400_0000,
        Flag_28 = 0x0800_0000,
        Flag_29 = 0x1000_0000,
        Flag_30 = 0x2000_0000,
        Flag_31 = 0x4000_0000,
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