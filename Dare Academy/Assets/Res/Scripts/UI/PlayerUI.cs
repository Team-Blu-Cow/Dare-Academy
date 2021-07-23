using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private GameObject m_player; // Game object for getting player info

    private PlayerEntity m_playerInfo; // Infor of the player, such as health and energy
    private int m_energy; // Energy the player has
    private int m_health; // Health the player has
    [SerializeField] private List<GameObject> energyIcons; // Images for the amount of energy the player has
    [SerializeField] private List<GameObject> healthIcons; // Images for the amount of health the player has

    [SerializeField] private GameObject gunIcon; // Image for showing the gun icon
    [SerializeField] private GameObject dashIcon; // Image for showing the dash icon
    [SerializeField] private GameObject blockIcon; // Image for showing the block icon
    private Vector2[] iconPositions = { new Vector2(75, -100), new Vector2(75, -100), new Vector2(200, -100), new Vector2(75, -100), new Vector2(175, -100), new Vector2(275, -100) }; // Positions for where the icons should be
    private PlayerAbilities.AbilityEnum m_ability = PlayerAbilities.AbilityEnum.None; // Ability tracker for current ability
    private PlayerAbilities.AbilityEnum m_prevAbility = PlayerAbilities.AbilityEnum.None; // Tracker for previous ability
    [SerializeField] private int numOfAbilitiesUnlocked = 0; // Tracks the amount of abilities the player has unlocked
    private bool isIconsMoving = false; // Bool for if the icons are animating/moving
    private float m_timer; // Timer for timing how long it takes for the icons to animate

    private Vector3 blockPosition; // Position of the block icon
    private Vector3 dashPosition; // Position of the dash icon
    private Vector3 gunPosition; // Position of the gun icon

    private Vector3 blockScale; // Scale of the block icon
    private Vector3 dashScale; // Scale of the dash icon
    private Vector3 gunScale; // Scale of the gun icon

    private void Start()
    {
        m_player = PlayerEntity.Instance.gameObject;
        m_playerInfo = m_player.GetComponent<PlayerEntity>(); // Get the player's info

        m_energy = m_playerInfo.MaxEnergy; // Current energy is max on start
        m_health = m_playerInfo.MaxHealth; // Current health is max on start

        for (int i = 0; i < m_playerInfo.MaxHealth - 1; i++) // Loop for amount of health
        {
            AddHealth(); // Add an icon for the amount of health
        }

        for (int i = 0; i < m_playerInfo.MaxEnergy - 1; i++) // Do same for energy
        {
            AddEnergy();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_player == null) // If player doesn't exist
            return; // Don't update the player UI

        m_timer += Time.deltaTime; // The timer is timing

        if ((m_energy + 1) != m_playerInfo.Energy || m_energy != m_playerInfo.Energy) // If the player's energy has changed
        {
            UpdateEnergyUI(); // Update the energy UI
            m_energy = m_playerInfo.Energy; // Update the energy variable
        }

        if (m_health != m_playerInfo.Health) // If the player's health has changed
        {
            UpdateHealthUI(); // Update the health UI
            m_health = m_playerInfo.Health; // Update the health variable
        }

        if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash) // If the active ability is dash
        {
            dashIcon.transform.GetChild(0).gameObject.SetActive(true); // Enable the dash icon
        }
        else // If not
        {
            dashIcon.transform.GetChild(0).gameObject.SetActive(false); // Disable the dash icon
        }

        if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block) // If the active ability is block
        {
            blockIcon.transform.GetChild(0).gameObject.SetActive(true); // Enable the block icon
        }
        else // If not
        {
            blockIcon.transform.GetChild(0).gameObject.SetActive(false); // Disable the block icon
        }

        if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot) // IF the active ability is shoot
        {
            gunIcon.transform.GetChild(0).gameObject.SetActive(true); // Enable the shoot icon
        }
        else // If not
        {
            gunIcon.transform.GetChild(0).gameObject.SetActive(false); // Disable the shoot icon
        }

        if (m_ability != PlayerAbilities.AbilityEnum.None) // If the ability has not been set yet
        {
            if (m_ability != m_playerInfo.Abilities.GetActiveAbility()) // If the ability tracker of the UI is not the same as the ability of the players
            {
                isIconsMoving = true; // Animate the icons to show the current ability being used
            }
        }

        if (m_ability != m_playerInfo.Abilities.GetActiveAbility()) // If the ability tracker of the UI is not the same as the ability of the players
        {
            m_prevAbility = m_ability; // Set the previous ability to be the UI's ability
            m_ability = m_playerInfo.Abilities.GetActiveAbility(); // Set the UI's ability to be the player's active ability
        }

        if (isIconsMoving) // IF icons are being animated right now
        {
            MoveIconPositions(); // Animate the icons
        }

        if (numOfAbilitiesUnlocked == 0) // If the number of abilities has not been set yet
        {
            ///#todo #Sandy call this function when player unlocks new ability !!!!!!!!!!!!!!!!!!
            CheckAbilitiesUnlocked(); // Find the amount of abilities unlocked
            SetIconPositions(); // Set the icons to their right positions
        }
    }

    private void CheckAbilitiesUnlocked()
    {
        numOfAbilitiesUnlocked = 0; // Number of abilities equal 0

        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Dash)) // If the player has dash unlocked
        {
            dashIcon.SetActive(true); // Set the dash icon to be active
            numOfAbilitiesUnlocked++; // Increment the amount of abilities unlocked
        }

        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Block)) // If the player has block unlocked
        {
            blockIcon.SetActive(true); // Set the block icon to be active
            numOfAbilitiesUnlocked++; // Increment the amount of abilities unlocked
        }

        if (m_playerInfo.Abilities.IsUnlocked(PlayerAbilities.AbilityEnum.Shoot)) // If the player has gun unlocked
        {
            gunIcon.SetActive(true); // Set the gun icon to be active
            numOfAbilitiesUnlocked++; // Increment the amount of abilities unlocked
        }
    }

    private void SetIconPositions()
    {
        if (numOfAbilitiesUnlocked == 1) // If the player has one ability unlocked
        {
            if (dashIcon.activeSelf == true) // If the dash icon is active
            {
                dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0]; // Set its position to the first element of the icon position array
            }
            else if (gunIcon.activeSelf == true) // If the gun icon is active
            {
                gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0]; // Set its position to the first element of the icon position array
            }
            else if (blockIcon.activeSelf == true) // If the block icon is active
            {
                blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[0]; // Set its position to the first element of the icon position array
            }
        }
        else if (numOfAbilitiesUnlocked == 2) // If the player has two abilities unlocked
        {
            dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the icon position to the third element of the icon position array
            gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the icon position to the third element of the icon position array
            blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the icon position to the third element of the icon position array

            if (dashIcon.activeSelf == true) // If the dash icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash) // If the player is using the dash ability
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the icon position to the second element of the icon position array
                }
            }
            else if (gunIcon.activeSelf == true) // IF the gun icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot) // If the player is using the gun ability
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the icon position to the second element of the icon position array
                }
            }
            else if (blockIcon.activeSelf == true) // IF the block icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block) // If the player is using the block ability
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the icon position to the second element of the icon position array
                }
            }
        }
        else if (numOfAbilitiesUnlocked == 3) // IF the player has three abilities unlocked
        {
            if (dashIcon.activeSelf == true) // If the dash icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash) // If the player is using the dash ability
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the icon position to the fourth element of the icon position array
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the icon position to the fifth element of the icon position array
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the icon position to the sixth element of the icon position array
                }
            }
            else if (gunIcon.activeSelf == true) // If the gun icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot) // If the player is using the gun ability
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the icon position to the fourth element of the icon position array
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the icon position to the fifth element of the icon position array
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the icon position to the sixth element of the icon position array
                }
            }
            else if (blockIcon.activeSelf == true) // If the block icon is active
            {
                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block) // If the player is using the block ability
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the icon position to the fourth element of the icon position array
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the icon position to the fifth element of the icon position array
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the icon position to the sixth element of the icon position array
                }
            }
        }
    }

    private void MoveIconPositions()
    {
        float transitionSpeed = 0.5f; // How long it takes the icons to move

        CheckAnimationReady(transitionSpeed); // If we are still animating the icons moving then don't get new positions for the icons to  move to

        if (!isIconsMoving) // If the icons are not moving
        {
            if (numOfAbilitiesUnlocked == 2) // If the player has two abilities unlocked
            {
                Vector3 mainScale = new Vector3(1.5f, 1.5f, 1.5f); // Scale for the current ability being used

                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Dash) // If the player is using the dash ability
                {
                    if (blockIcon.activeSelf == false) // IF the block icon is not active
                    {
                        LeanTween.move(dashIcon, gunPosition, transitionSpeed); // Move the dash icon to the gun icon's position
                        LeanTween.move(gunIcon, dashPosition, transitionSpeed); // Move the gun icon to the dash icon's position
                        LeanTween.scale(dashIcon, mainScale, transitionSpeed); // Change the dash icon's scale to the main scale
                        LeanTween.scale(gunIcon, dashScale, transitionSpeed); // Change the gun icon's scale to the dash scale
                    }
                    else// IF the block icon is active
                    {
                        LeanTween.move(dashIcon, blockPosition, transitionSpeed); // Move the dash icon to the block icon's position
                        LeanTween.move(blockIcon, dashPosition, transitionSpeed); // Move the block icon to the dash icon's position
                        LeanTween.scale(dashIcon, mainScale, transitionSpeed); // Change the dash icon's scale to the main scale
                        LeanTween.scale(blockIcon, dashScale, transitionSpeed); // Change the block icon's scale to the dash scale
                    }
                }

                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Shoot) // If the player is using the gun ability
                {
                    if (blockIcon.activeSelf == false) // IF the block icon is not active
                    {
                        LeanTween.move(dashIcon, gunPosition, transitionSpeed); // Move the dash icon to the gun icon's position
                        LeanTween.move(gunIcon, dashPosition, transitionSpeed); // Move the gun icon to the dash icon's position
                        LeanTween.scale(dashIcon, gunScale, transitionSpeed); // Change the dash icon's scale to the gun scale
                        LeanTween.scale(gunIcon, mainScale, transitionSpeed); // Change the gun icon's scale to the main scale
                    }
                    else // IF the block icon is active
                    {
                        LeanTween.move(gunIcon, blockPosition, transitionSpeed); // Move the gun icon to the block icon's position
                        LeanTween.move(blockIcon, dashPosition, transitionSpeed); // Move the block icon to the dash icon's position
                        LeanTween.scale(gunIcon, mainScale, transitionSpeed); // Change the gun icon's scale to the main scale
                        LeanTween.scale(blockIcon, gunScale, transitionSpeed); // Change the block icon's scale to the gun scale
                    }
                }

                if (m_playerInfo.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block) // If the player is using the block ability
                {
                    if (dashIcon.activeSelf == false) // IF the dash icon is not active
                    {
                        LeanTween.move(blockIcon, gunPosition, transitionSpeed); // Move the block icon to the gun icon's position
                        LeanTween.move(gunIcon, blockPosition, transitionSpeed); // Move the gun icon to the block icon's position
                        LeanTween.scale(blockIcon, mainScale, transitionSpeed); // Change the block icon's scale to the main scale
                        LeanTween.scale(gunIcon, blockScale, transitionSpeed); // Change the gun icon's scale to the block scale
                    }
                    else // IF the dash icon is active
                    {
                        LeanTween.move(dashIcon, blockPosition, transitionSpeed); // Move the dash icon to the block icon's position
                        LeanTween.move(blockIcon, dashPosition, transitionSpeed); // Move the block icon to the dash icon's position
                        LeanTween.scale(dashIcon, blockScale, transitionSpeed); // Change the dash icon's scale to the block scale
                        LeanTween.scale(blockIcon, mainScale, transitionSpeed); // Change the block icon's scale to the main scale
                    }
                }
            }
            else if (numOfAbilitiesUnlocked == 3)
            {
                // Moving Left
                if (m_ability == PlayerAbilities.AbilityEnum.Dash && (m_prevAbility == PlayerAbilities.AbilityEnum.Block || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from dash to block
                {
                    MoveIconsLeft(transitionSpeed); // Move the icons left
                }
                else if (m_ability == PlayerAbilities.AbilityEnum.Block && (m_prevAbility == PlayerAbilities.AbilityEnum.Shoot || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from block to shoot
                {
                    MoveIconsLeft(transitionSpeed); // Move the icons left
                }
                else if (m_ability == PlayerAbilities.AbilityEnum.Shoot && (m_prevAbility == PlayerAbilities.AbilityEnum.Dash || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from shoot to dash
                {
                    MoveIconsLeft(transitionSpeed); // Move the icons left
                }

                // Moving Right
                if (m_ability == PlayerAbilities.AbilityEnum.Dash && (m_prevAbility == PlayerAbilities.AbilityEnum.Shoot || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from dash to shoot
                {
                    MoveIconsRight(transitionSpeed); // Move the icons right
                }
                else if (m_ability == PlayerAbilities.AbilityEnum.Shoot && (m_prevAbility == PlayerAbilities.AbilityEnum.Block || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from shoot to block
                {
                    MoveIconsRight(transitionSpeed); // Move the icons right
                }
                else if (m_ability == PlayerAbilities.AbilityEnum.Block && (m_prevAbility == PlayerAbilities.AbilityEnum.Dash || m_prevAbility == PlayerAbilities.AbilityEnum.None)) // If the ability has changed from block to dash
                {
                    MoveIconsRight(transitionSpeed); // Move the icons right
                }
            }
        }
    }

    private void MoveIconsLeft(float transitionSpeed)
    {
        LeanTween.move(dashIcon, blockPosition, transitionSpeed); // Move the dash icon positoin to the block position
        LeanTween.move(blockIcon, gunPosition, transitionSpeed); // Move the block icon positoin to the gun position
        LeanTween.move(gunIcon, dashPosition, transitionSpeed); // Move the gun icon positoin to the dash position
        LeanTween.scale(dashIcon, blockScale, transitionSpeed); // Chane the dash scale to the block scale
        LeanTween.scale(blockIcon, gunScale, transitionSpeed); // Chane the block scale to the gun scale
        LeanTween.scale(gunIcon, dashScale, transitionSpeed); // Chane the gun scale to the dash scale
    }

    private void MoveIconsRight(float transitionSpeed)
    {
        LeanTween.move(dashIcon, gunPosition, transitionSpeed); // Move the dash icon positoin to the gun position
        LeanTween.move(gunIcon, blockPosition, transitionSpeed); // Move the gun icon positoin to the block position
        LeanTween.move(blockIcon, dashPosition, transitionSpeed); // Move the block icon positoin to the dash position
        LeanTween.scale(dashIcon, gunScale, transitionSpeed); // Chane the dash scale to the gun scale
        LeanTween.scale(gunIcon, blockScale, transitionSpeed); // Chane the gun scale to the block scale
        LeanTween.scale(blockIcon, dashScale, transitionSpeed); // Chane the block scale to the dash scale
    }

    private void CheckAnimationReady(float transitionSpeed)
    {
        if (m_timer > transitionSpeed) // If the icon's have stopped animating
        {
            CheckIconsInPosition(); // Check where the icons are
            isIconsMoving = false; // Set animating to false

            // Reset position references
            blockPosition = blockIcon.GetComponent<RectTransform>().position;
            dashPosition = dashIcon.GetComponent<RectTransform>().position;
            gunPosition = gunIcon.GetComponent<RectTransform>().position;

            // Reset scale references
            blockScale = blockIcon.GetComponent<RectTransform>().localScale;
            dashScale = dashIcon.GetComponent<RectTransform>().localScale;
            gunScale = gunIcon.GetComponent<RectTransform>().localScale;

            m_timer = 0; // Reset timer
        }
    }

    private void CheckIconsInPosition()
    {
        if (numOfAbilitiesUnlocked == 2) // If the number of abilities unlocked is 2
        {
            if (m_prevAbility == PlayerAbilities.AbilityEnum.Dash && dashIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[2]) // If the previous icon was dash and the dash icon is not in the right position
            {
                if (blockIcon.activeSelf == false) // If the block icon is not active
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the dash icon to the proper position
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the gun icon to the proper position
                    dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the dash icon to be bigger
                    gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the gun icon to be normal size
                }
                else // If not
                {
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the dash icon to the proper position
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the block icon to the proper position
                    dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the dash icon to be bigger
                    blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the block icon to be normal size
                }
            }
            else if (m_prevAbility == PlayerAbilities.AbilityEnum.Shoot && gunIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[2])
            {
                if (blockIcon.activeSelf == false)
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the gun icon to the proper position
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the dash icon to the proper position
                    gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the gun icon to be bigger
                    dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the dash icon to be normal size
                }
                else
                {
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the gun icon to the proper position
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the block icon to the proper position
                    gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the gun icon to be bigger
                    blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the block icon to be normal size
                }
            }
            else if (m_prevAbility == PlayerAbilities.AbilityEnum.Block && blockIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[2])
            {
                if (dashIcon.activeSelf == false)
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the block icon to the proper position
                    gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the gun icon to the proper position
                    blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the block icon to be bigger
                    gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the gun icon to be normal size
                }
                else
                {
                    blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[2]; // Set the block icon to the proper position
                    dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[1]; // Set the dash icon to the proper position
                    blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the block icon to be bigger
                    dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the dash icon to be normal size
                }
            }
        }
        else if (numOfAbilitiesUnlocked == 3)
        {
            if (m_prevAbility == PlayerAbilities.AbilityEnum.Dash && dashIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[4])
            {
                blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the block icon to the proper position
                dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the dash icon to the proper position
                gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the gun icon to the proper position

                blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the block icon to be normal size
                dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the dash icon to be bigger
                gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the gun icon to be normal size
            }
            else if (m_prevAbility == PlayerAbilities.AbilityEnum.Block && blockIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[4])
            {
                gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the gun icon to the proper position
                blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the block icon to the proper position
                dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the dash icon to the proper position

                gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the gun icon to be normal size
                blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the block icon to be bigger
                dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the dash icon to be normal size
            }
            else if (m_prevAbility == PlayerAbilities.AbilityEnum.Shoot && gunIcon.GetComponent<RectTransform>().anchoredPosition != iconPositions[4])
            {
                dashIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[3]; // Set the dash icon to the proper position
                gunIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[4]; // Set the gun icon to the proper position
                blockIcon.GetComponent<RectTransform>().anchoredPosition = iconPositions[5]; // Set the block icon to the proper position

                dashIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the dash icon to be normal size
                gunIcon.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); // Set the gun icon to be bigger
                blockIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set the block icon to be normal size
            }
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthIcons.Count; i++) // For all the health images
        {
            healthIcons[i].SetActive(false); // Set active to false
        }

        for (int i = 0; i < m_playerInfo.Health; i++) // For the ones showing how much health the player has
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
        GameObject newHealth = new GameObject(); // Create new object
        newHealth.transform.parent = gameObject.transform.GetChild(0); // Set parent
        newHealth.AddComponent<RectTransform>(); // Add rect transform
        newHealth.AddComponent<CanvasRenderer>(); // Add canvas renderer
        newHealth.AddComponent<RawImage>(); // Add image

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f); // Set scale
        newHealth.GetComponent<RawImage>().color = new Color(1.0f, 0.0f, 0.6235f, 1.0f); // Set color
        healthIcons.Add(newHealth); // Add new game object to array of objects
    }

    public void AddEnergy()
    {
        GameObject newEnergy = new GameObject(); // Create new object
        newEnergy.transform.parent = gameObject.transform.GetChild(1); // Set parent
        newEnergy.AddComponent<RectTransform>(); // Add rect transform
        newEnergy.AddComponent<CanvasRenderer>(); // Add canvas renderer
        newEnergy.AddComponent<RawImage>(); // Add image

        newEnergy.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f); // Set scale
        newEnergy.GetComponent<RawImage>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f); // Set color
        energyIcons.Add(newEnergy); // Add new game object to array of objects
    }
}