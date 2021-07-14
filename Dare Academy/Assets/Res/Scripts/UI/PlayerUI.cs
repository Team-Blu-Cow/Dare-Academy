using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    GameObject m_player; // Game object for getting player info

    PlayerEntity m_playerInfo; // Infor of the player, such as health and energy
    private int m_energy; // Energy the player has
    private int m_health; // Health the player has
    [SerializeField] List<GameObject> energyIcons; // Images for the amount of energy the player has
    [SerializeField] List<GameObject> healthIcons; // Images for the amount of health the player has

    [SerializeField] GameObject gunIcon;
    [SerializeField] GameObject dashIcon;
    [SerializeField] GameObject blockIcon;
    Vector3[] iconPositions = { new Vector3(75, -100, 0), new Vector3(75, -100, 0), new Vector3(200, -100, 0), new Vector3(75, -100, 0), new Vector3(175, -100, 0), new Vector3(275, -100, 0) };
    PlayerAbilities.AbilityEnum m_ability = PlayerAbilities.AbilityEnum.None;
    [SerializeField] int numOfAbilitiesUnlocked = 0;
    bool isIconsMoving = false;

    private void OnValidate()
    {
        m_player = GameObject.Find("Green"); // Find the player
        m_playerInfo = m_player.GetComponent<PlayerEntity>(); // Get the player's info
    }

    private void Start()
    {
        m_energy = m_playerInfo.MaxEnergy;
        m_health = m_playerInfo.MaxHealth;

        for (int i = 0; i < m_playerInfo.MaxHealth - 1; i++)
        {
            AddHealth();
        }
        
        for(int i = 0; i < m_playerInfo.MaxEnergy - 1; i++)
        {
            AddEnergy();
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if(m_energy != m_playerInfo.Energy) // If the player's energy has changed
        {
            UpdateEnergyUI(); // Update the energy UI
            m_energy = m_playerInfo.Energy; // Update the energy variable 
        }
        
        if(m_health != m_playerInfo.Health) // If the player's health has changed
        {
            UpdateHealthUI(); // Update the health UI
            m_health = m_playerInfo.Health; // Update the health variable
        }



        if(m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash)
        {
            dashIcon.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            dashIcon.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block)
        {
            blockIcon.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            blockIcon.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot)
        {
            gunIcon.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            gunIcon.transform.GetChild(0).gameObject.SetActive(false);
        }

        if(m_ability != PlayerAbilities.AbilityEnum.None)
        {
            if(m_ability != m_playerInfo.Abilities.GetActiveAbility())
            {
                isIconsMoving = true;
            }
        }

        if(isIconsMoving)
        {
            MoveIconPositions();
        }

        if (numOfAbilitiesUnlocked == 0)
        {
            CheckAbilitiesUnlocked();
            SetIconPositions();
        }

        m_ability = m_playerInfo.Abilities.GetActiveAbility();
    }

    public void CheckAbilitiesUnlocked()
    {
        numOfAbilitiesUnlocked = 0;
        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Dash))
        {
            dashIcon.SetActive(true);
            numOfAbilitiesUnlocked++;
        }

        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Block))
        {
            blockIcon.SetActive(true);
            numOfAbilitiesUnlocked++;
        }

        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Shoot))
        {
            gunIcon.SetActive(true);
            numOfAbilitiesUnlocked++;
        }
    }

    private void SetIconPositions()
    {
        if (numOfAbilitiesUnlocked == 1)
        {
            if(dashIcon.activeSelf == true)
            {
                dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0];
            }
            else if (gunIcon.activeSelf == true)
            {
                gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0];
            }
            else if (blockIcon.activeSelf == true)
            {
                blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0];
            }
        }
        else if (numOfAbilitiesUnlocked == 2)
        {
            dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2];
            gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2];
            blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2];

            if (dashIcon.activeSelf == true)
            {
                if(m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash)
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1];
                }
            }
            else if (gunIcon.activeSelf == true)
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot)
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1];
                }
            }
            else if (blockIcon.activeSelf == true)
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block)
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1];
                }
            }
        }
        else if (numOfAbilitiesUnlocked == 3)
        {
            if (dashIcon.activeSelf == true)
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash)
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3];
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4];
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5];
                }
            }
            else if (gunIcon.activeSelf == true)
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot)
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3];
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4];
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5];
                }
            }
            else if (blockIcon.activeSelf == true)
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block)
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3];
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4];
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5];
                }
            }
        }

    }

    private void MoveIconPositions()
    {
        Vector3 blockPosition = blockIcon.GetComponent<RectTransform>().position;
        Vector3 dashPosition = dashIcon.GetComponent<RectTransform>().position;
        Vector3 gunPosition = gunIcon.GetComponent<RectTransform>().position;

        Vector3 blockScale = blockIcon.GetComponent<RectTransform>().localScale;
        Vector3 dashScale = dashIcon.GetComponent<RectTransform>().localScale;
        Vector3 gunScale = gunIcon.GetComponent<RectTransform>().localScale;

        float transitionSpeed = 0.5f;

        if (numOfAbilitiesUnlocked == 2)
        {
            if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash)
            {
                if(blockIcon.activeSelf == false)
                {
                    Debug.Log("Dash & gun moving");
                    LeanTween.move(dashIcon, gunPosition, transitionSpeed);
                    LeanTween.move(gunIcon, dashPosition, transitionSpeed);
                    LeanTween.scale(dashIcon, gunScale, transitionSpeed);
                    LeanTween.scale(gunIcon, dashScale, transitionSpeed);
                }
                else
                {
                    Debug.Log("Dash & block moving");
                    LeanTween.move(dashIcon, blockPosition, transitionSpeed);
                    LeanTween.move(blockIcon, dashPosition, transitionSpeed);
                    LeanTween.scale(dashIcon, blockScale, transitionSpeed);
                    LeanTween.scale(blockIcon, dashScale, transitionSpeed);
                }
            }
            
            if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot)
            {
                if (blockIcon.activeSelf == false)
                {
                    LeanTween.move(dashIcon, gunPosition, transitionSpeed);
                    LeanTween.move(gunIcon, dashPosition, transitionSpeed);
                    LeanTween.scale(dashIcon, gunScale, transitionSpeed);
                    LeanTween.scale(gunIcon, dashScale, transitionSpeed);
                }
                else
                {
                    LeanTween.move(dashIcon, blockPosition, transitionSpeed);
                    LeanTween.move(blockIcon, dashPosition, transitionSpeed);
                    LeanTween.scale(dashIcon, blockScale, transitionSpeed);
                    LeanTween.scale(blockIcon, dashScale, transitionSpeed);
                }
            }
            
            if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block)
            {
                if (dashIcon.activeSelf == false)
                {
                    LeanTween.move(blockIcon, gunPosition, transitionSpeed);
                    LeanTween.move(gunIcon, blockPosition, transitionSpeed);
                    LeanTween.scale(blockIcon, gunScale, transitionSpeed);
                    LeanTween.scale(gunIcon, blockScale, transitionSpeed);
                }
                else
                {
                    LeanTween.move(dashIcon, blockPosition, transitionSpeed);
                    LeanTween.move(blockIcon, dashPosition, transitionSpeed);
                    LeanTween.scale(dashIcon, blockScale, transitionSpeed);
                    LeanTween.scale(blockIcon, dashScale, transitionSpeed);
                }
            }
        }

        isIconsMoving = false;
    }

    public void UpdateHealthUI()
    {
        for(int i = 0; i < healthIcons.Count; i++) // For all the health images
        {
            healthIcons[i].SetActive(false); // Set active to false
        }
        
        for(int i = 0; i < m_playerInfo.Health; i++) // For the ones showing how much health the player has
        {
            healthIcons[i].SetActive(true); // Set active to true
        }
    }

    public void UpdateEnergyUI()
    {
        for (int i = 0; i < energyIcons.Count; i++) // For all the energy images
        {
            energyIcons[i].SetActive(false); // Set active to false
        }
        
        for (int i = 0; i < m_energy; i++) // For the ones showing how much energy the player has
        {
            if (i < energyIcons.Count)
            {
                energyIcons[i].SetActive(true); // Set active to true
            }
        }
    }

    public void AddHealth()
    {
        GameObject newHealth = new GameObject();
        newHealth.transform.parent = gameObject.transform.GetChild(0);
        newHealth.AddComponent<RectTransform>();
        newHealth.AddComponent<CanvasRenderer>();
        newHealth.AddComponent<RawImage>();

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        newHealth.GetComponent<RawImage>().color = new Color(1.0f, 0.0f, 0.6235f, 1.0f);
        healthIcons.Add(newHealth);
    }

    public void AddEnergy()
    {
        GameObject newEnergy = new GameObject();
        newEnergy.transform.parent = gameObject.transform.GetChild(1);
        newEnergy.AddComponent<RectTransform>();
        newEnergy.AddComponent<CanvasRenderer>();
        newEnergy.AddComponent<RawImage>();

        newEnergy.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        newEnergy.GetComponent<RawImage>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        energyIcons.Add(newEnergy);
    }
}
