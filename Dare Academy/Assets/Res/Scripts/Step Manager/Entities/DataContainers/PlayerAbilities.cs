using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (levelModule.ActiveSaveSata.shootUnlocked)
        {
            Unlock(AbilityEnum.Shoot, false);
        }
        if (levelModule.ActiveSaveSata.dashUnlocked)
        {
            Unlock(AbilityEnum.Dash, false);
        }
        if (levelModule.ActiveSaveSata.blockUnlocked)
        {
            Unlock(AbilityEnum.Block, false);
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

    public void Unlock(AbilityEnum ability, bool writeToSavedata = true)
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

        if (writeToSavedata)
        {
            WriteToSaveData(ability, true);
        }
    }

    public void Lock(AbilityEnum ability, bool writeToSavedata = true)
    {
        for (int i = m_avalibleAbilities.Count - 1; i >= 0; i--)
        {
            if (m_avalibleAbilities[i] == ability)
            {
                m_avalibleAbilities.RemoveAt(i);
                break;
            }
        }

        if (writeToSavedata)
        {
            WriteToSaveData(ability, false);
        }
    }

    private void WriteToSaveData(AbilityEnum ability, bool unlocked)
    {
        blu.SaveData savedata =  blu.App.GetModule<blu.LevelModule>().ActiveSaveSata;

        switch (ability)
        {
            case AbilityEnum.None:
                break;

            case AbilityEnum.Shoot:
                savedata.shootUnlocked = unlocked;
                break;

            case AbilityEnum.Dash:
                savedata.dashUnlocked = unlocked;
                break;

            case AbilityEnum.Block:
                savedata.blockUnlocked = unlocked;
                break;

            default:
                Debug.LogWarning("[Player Abilities] - unexpected value");
                break;
        }
    }
}