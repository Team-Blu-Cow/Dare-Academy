using System.Collections.Generic;
using UnityEngine;
using eventFlags = GameEventFlags.Flags;

[System.Serializable]
public class PlayerAbilities
{
    public enum AbilityEnum
    {
        None,
        Shoot,
        Dash,
        Block,
    }

    private List<AbilityEnum> m_avalibleAbilities = new List<AbilityEnum>();

    [SerializeField] protected AbilityEnum m_activeAbility = AbilityEnum.None;

    public void Refresh()
    {
        blu.LevelModule levelModule = blu.App.GetModule<blu.LevelModule>();
        if (levelModule.EventFlags.IsFlagsSet(eventFlags.shoot_unlocked))
            Unlock(AbilityEnum.Shoot);
        else
            Lock(AbilityEnum.Shoot);

        if (levelModule.EventFlags.IsFlagsSet(eventFlags.block_unlocked))
            Unlock(AbilityEnum.Block);
        else
            Lock(AbilityEnum.Block);

        if (levelModule.EventFlags.IsFlagsSet(eventFlags.dash_unlocked))
            Unlock(AbilityEnum.Dash);
        else
            Lock(AbilityEnum.Dash);

        WriteToSaveData();
    }

    public AbilityEnum GetActiveAbility()
    {
        return m_activeAbility;
    }

    public void SetActiveAbility(AbilityEnum ability)
    {
        m_activeAbility = ability;
    }

    public AbilityEnum LeftAbility()
    {
        if (m_avalibleAbilities.Count == 0)
            return AbilityEnum.None;

        int count = 0;
        for (int i = 0; i < m_avalibleAbilities.Count; i++)
        {
            if (m_avalibleAbilities[i] == m_activeAbility)
            {
                count = i;
                break;
            }
        }
        count--;

        if (count < 0)
        {
            count = m_avalibleAbilities.Count - 1;
        }

        return m_avalibleAbilities[count];
    }

    public AbilityEnum RightAbility()
    {
        if (m_avalibleAbilities.Count == 0)
            return AbilityEnum.None;

        int count = 0;
        for (int i = 0; i < m_avalibleAbilities.Count; i++)
        {
            if (m_avalibleAbilities[i] == m_activeAbility)
            {
                count = i;
                break;
            }
        }
        count++;

        if (count == m_avalibleAbilities.Count)
        {
            count = 0;
        }

        return m_avalibleAbilities[count];
    }

    public async void Initialise()
    {
        blu.LevelModule levelModule = blu.App.GetModule<blu.LevelModule>();

        await levelModule.AwaitSaveLoad();

        await levelModule.AwaitInitialised();

        if (levelModule.EventFlags.IsFlagsSet(eventFlags.shoot_unlocked))
        {
            Unlock(AbilityEnum.Shoot);
        }
        if (levelModule.EventFlags.IsFlagsSet(eventFlags.block_unlocked))
        {
            Unlock(AbilityEnum.Block);
        }

        if (levelModule.EventFlags.IsFlagsSet(eventFlags.dash_unlocked))
        {
            Unlock(AbilityEnum.Dash);
        }

        if (m_avalibleAbilities.Count > 0)
        {
            m_activeAbility = m_avalibleAbilities[0];
        }
    }

    public bool IsUnlocked(AbilityEnum ability)
    {
        foreach (AbilityEnum unlocked in m_avalibleAbilities)
        {
            if (ability == unlocked)
            {
                return true;
            }
        }

        return false;
    }

    private void Unlock(AbilityEnum ability)
    {
        bool contains = false;
        for (int i = m_avalibleAbilities.Count - 1; i >= 0; i--)
        {
            if (m_avalibleAbilities[i] == ability)
            {
                contains = true;
            }
        }

        if (!contains)
        {
            m_avalibleAbilities.Add(ability);
        }
    }

    private void Lock(AbilityEnum ability)
    {
        for (int i = m_avalibleAbilities.Count - 1; i >= 0; i--)
        {
            if (m_avalibleAbilities[i] == ability)
            {
                m_avalibleAbilities.RemoveAt(i);
                break;
            }
        }
    }

    private void WriteToSaveData()
    {
        blu.LevelModule levelModule = blu.App.GetModule<blu.LevelModule>();
        levelModule.AwaitInitialised();

        levelModule.EventFlags.SetFlags(eventFlags.shoot_unlocked | eventFlags.dash_unlocked | eventFlags.block_unlocked, false);

        eventFlags flag = 0;

        foreach (var ability in m_avalibleAbilities)
        {
            switch (ability)
            {
                case AbilityEnum.Shoot:
                    flag |= eventFlags.shoot_unlocked;
                    break;

                case AbilityEnum.Dash:
                    flag |= eventFlags.dash_unlocked;
                    break;

                case AbilityEnum.Block:
                    flag |= eventFlags.block_unlocked;
                    break;

                default:
                    break;
            }
        }

        levelModule.EventFlags.SetFlags(flag, true);
    }
}