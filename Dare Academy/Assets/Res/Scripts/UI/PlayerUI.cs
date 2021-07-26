using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using blu;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerUI : MonoBehaviour
{
    private struct Icon
    {
        public GameObject GO;
        public Vector3 position;
        public Vector3 scale;
        public int index;

        public GameObject ActiveGameObject()
        { return GO.transform.GetChild(0).gameObject; }
    }

    private enum IconIndex
    {
        Dash,
        Gun,
        Block
    }

    private PlayerEntity m_player; // Info of the player, such as health and energy
    private int m_energy; // Energy the player has
    private int m_health; // Health the player has

    private List<GameObject> energyIcons = new List<GameObject>(); // Images for the amount of energy the player has
    private List<GameObject> healthIcons = new List<GameObject>(); // Images for the amount of health the player has

    private Icon[] m_Icons = new Icon[3];
    private Sprite[] controllIcons = new Sprite[4];

    private Vector2[] m_iconPositions;

    private int numOfAbilitiesUnlocked = 0; // Tracks the amount of abilities the player has unlocked

    private void Start()
    {
        m_player = PlayerEntity.Instance; // Get the player's info

        m_energy = m_player.Energy; // Current energy is max on start
        m_health = m_player.Health; // Current health is max on start

        for (int i = 0; i < m_player.MaxHealth; i++) // Loop for amount of health
        {
            AddHealth(); // Add an icon for the amount of health
        }

        for (int i = 0; i < m_player.MaxEnergy; i++) // Do same for energy
        {
            AddEnergy();
        }

        // Setup each icon
        for (int i = 0; i < m_Icons.Length; i++)
        {
            m_Icons[i].GO = transform.GetChild(2).GetChild(i).gameObject;
            var rect = (RectTransform)m_Icons[i].GO.transform;
            m_Icons[i].position = rect.anchoredPosition;
            m_Icons[i].index = i;
        }

        m_iconPositions = new Vector2[]{ // Positions for where the icons should be
        m_Icons[(int)IconIndex.Dash].position ,
        m_Icons[(int)IconIndex.Gun].position,
        m_Icons[(int)IconIndex.Block].position };

        CheckAbilitiesUnlocked();
        SetStartingPosition();

        UpdateSelected();

        InputModule input = App.GetModule<InputModule>();
        input.LastDeviceChanged += DeviceChanged;

        input.PlayerController.Player.SwapAbilityL.performed += ChangeAbilityL;
        input.PlayerController.Player.SwapAbilityR.performed += ChangeAbilityR;
    }

    private void OnValidate()
    {
        // Add button swaps sprites
        controllIcons[0] = Resources.Load<Sprite>("GFX/ButtonImages/1Button");
        controllIcons[1] = Resources.Load<Sprite>("GFX/ButtonImages/3Button");
        controllIcons[2] = Resources.Load<Sprite>("GFX/ButtonImages/LBButton");
        controllIcons[3] = Resources.Load<Sprite>("GFX/ButtonImages/RBButton");
    }

    private void OnDisable()
    {
        InputModule input = App.GetModule<InputModule>();
        input.LastDeviceChanged -= DeviceChanged;

        input.PlayerController.Player.SwapAbilityL.performed -= ChangeAbilityL;
        input.PlayerController.Player.SwapAbilityR.performed -= ChangeAbilityR;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_player == null) // If player doesn't exist
            return; // Don't update the player UI

        if ((m_energy + 1) != m_player.Energy || m_energy != m_player.Energy) // If the player's energy has changed
        {
            UpdateEnergyUI(); // Update the energy UI
            m_energy = m_player.Energy; // Update the energy variable
        }

        if (m_health != m_player.Health) // If the player's health has changed
        {
            UpdateHealthUI(); // Update the health UI
            m_health = m_player.Health; // Update the health variable
        }

        if (numOfAbilitiesUnlocked == 0) // If the number of abilities has not been set yet
        {
            ///#todo #Sandy call this function when player unlocks new ability !!!!!!!!!!!!!!!!!!
            CheckAbilitiesUnlocked(); // Find the amount of abilities unlocked
            SetStartingPosition();
        }
    }

    private void SetStartingPosition()
    {
        transform.GetChild(2).GetChild(3).gameObject.SetActive(true);

        m_Icons[(int)IconIndex.Dash].GO.SetActive(true);
        m_Icons[(int)IconIndex.Block].GO.SetActive(true);
        m_Icons[(int)IconIndex.Gun].GO.SetActive(true);

        switch (numOfAbilitiesUnlocked)
        {
            case 1:
                m_Icons[(int)IconIndex.Dash].GO.SetActive(false);
                m_Icons[(int)IconIndex.Block].GO.SetActive(false);
                transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
                break;

            case 2:
                m_Icons[(int)IconIndex.Dash].GO.SetActive(false);
                break;

            default:
                break;
        }
    }

    public void DeviceChanged()
    {
        Transform temp = transform.GetChild(2).GetChild(3);

        switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
        {
            case "Keyboard":
                temp.GetComponentsInChildren<Image>()[0].sprite = controllIcons[0];
                temp.GetComponentsInChildren<Image>()[1].sprite = controllIcons[1];
                break;

            case "Xbox Controller":
                temp.GetComponentsInChildren<Image>()[0].sprite = controllIcons[2];
                temp.GetComponentsInChildren<Image>()[1].sprite = controllIcons[3];
                break;
        }
    }

    private void UpdateSelected()
    {
        // Disable active icons
        for (int i = 0; i < m_Icons.Length; i++)
            m_Icons[i].ActiveGameObject().SetActive(false);

        // Enable active display for selected ability
        switch (m_player.Abilities.GetActiveAbility())
        {
            case PlayerAbilities.AbilityEnum.None:
                break;

            case PlayerAbilities.AbilityEnum.Shoot:
                m_Icons[(int)IconIndex.Gun].ActiveGameObject().SetActive(true);
                break;

            case PlayerAbilities.AbilityEnum.Dash:
                m_Icons[(int)IconIndex.Dash].ActiveGameObject().SetActive(true);
                break;

            case PlayerAbilities.AbilityEnum.Block:
                m_Icons[(int)IconIndex.Block].ActiveGameObject().SetActive(true);
                break;

            default:
                break;
        }
    }

    private void ChangeAbilityL(InputAction.CallbackContext ctx)
    {
        ChangeAbility(true);
    }

    private void ChangeAbilityR(InputAction.CallbackContext ctx)
    {
        ChangeAbility(false);
    }

    private void ChangeAbility(bool left)
    {
        UpdateSelected();
        MoveIconPositions(left);
    }

    private void CheckAbilitiesUnlocked()
    {
        numOfAbilitiesUnlocked = 0; // Number of abilities equal 0

        foreach (PlayerAbilities.AbilityEnum ability in (PlayerAbilities.AbilityEnum[])Enum.GetValues(typeof(PlayerAbilities.AbilityEnum)))
        {
            if (m_player.Abilities.IsUnlocked(ability))
                numOfAbilitiesUnlocked++;
        }
    }

    private void MoveIconPositions(bool left)
    {
        float transitionSpeed = 0.25f; // How long it takes the icons to move

        switch (numOfAbilitiesUnlocked)
        {
            case 2:
                for (int i = 1; i < m_Icons.Length; i++)
                {
                    if (m_Icons[i].index == 2)
                    {
                        LeanTween.move(m_Icons[i].GO, m_iconPositions[(int)IconIndex.Gun], transitionSpeed);
                        LeanTween.scale(m_Icons[i].GO, new Vector3(1.5f, 1.5f, 1.5f), transitionSpeed);
                        m_Icons[i].index--;
                    }
                    else
                    {
                        LeanTween.move(m_Icons[i].GO, m_iconPositions[(int)IconIndex.Block], transitionSpeed);
                        LeanTween.scale(m_Icons[i].GO, new Vector3(1, 1, 1), transitionSpeed);
                        m_Icons[i].GO.transform.SetAsFirstSibling();

                        m_Icons[i].index++;
                    }
                }
                break;

            case 3:
                for (int i = 0; i < m_Icons.Length; i++)
                {
                    if (left) // Cycle icons to the left
                    {
                        // Move Position
                        if (m_Icons[i].index > 0)
                        {
                            LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[m_Icons[i].index - 1], transitionSpeed);

                            m_Icons[i].index--;
                        }
                        else
                        {
                            LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[m_Icons.Length - 1], transitionSpeed);
                            m_Icons[i].GO.transform.SetAsFirstSibling();

                            m_Icons[i].index = m_Icons.Length - 1;
                        }

                        //Scale
                        if (m_Icons[i].index != 1)
                            LeanTween.scale(m_Icons[i].GO, new Vector3(1, 1, 1), transitionSpeed);
                        else
                            LeanTween.scale(m_Icons[i].GO, new Vector3(1.5f, 1.5f, 1.5f), transitionSpeed);
                    }
                    else // Cycle icons to the right
                    {
                        if (m_Icons[i].index < m_Icons.Length - 1)
                        {
                            LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[m_Icons[i].index + 1], transitionSpeed);
                            m_Icons[i].index++;
                        }
                        else
                        {
                            LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[0], transitionSpeed);
                            m_Icons[i].GO.transform.SetAsFirstSibling();

                            m_Icons[i].index = 0;
                        }

                        //Scale
                        if (m_Icons[i].index != 1)
                            LeanTween.scale(m_Icons[i].GO, new Vector3(1, 1, 1), transitionSpeed);
                        else
                            LeanTween.scale(m_Icons[i].GO, new Vector3(1.5f, 1.5f, 1.5f), transitionSpeed);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthIcons.Count; i++) // For all the health images
        {
            healthIcons[i].SetActive(false); // Set active to false
        }

        for (int i = 0; i < m_player.Health; i++) // For the ones showing how much health the player has
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
        GameObject newHealth = new GameObject("Health", typeof(RectTransform), typeof(Image)); // Create new object
        newHealth.transform.parent = gameObject.transform.GetChild(0); // Set parent

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // Set scale
        newHealth.GetComponent<Image>().sprite = Resources.Load<Sprite>("GFX/HeartFull"); // Set color
        healthIcons.Add(newHealth); // Add new game object to array of objects
    }

    public void AddEnergy()
    {
        GameObject newEnergy = new GameObject("Energy", typeof(RectTransform), typeof(Image)); // Create new object
        newEnergy.transform.parent = gameObject.transform.GetChild(1); // Set parent

        newEnergy.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f); // Set scale
        newEnergy.GetComponent<Image>().color = new Color(0.0f, 1.0f, 1.0f, 1.0f); // Set color
        energyIcons.Add(newEnergy); // Add new game object to array of objects
    }
}