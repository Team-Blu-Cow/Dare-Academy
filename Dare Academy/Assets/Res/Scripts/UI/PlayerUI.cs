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

    private List<Image> energyIcons = new List<Image>(); // Images for the amount of energy the player has
    private List<Image> healthIcons = new List<Image>(); // Images for the amount of health the player has

    private Icon[] m_Icons = new Icon[3];
    [SerializeField, HideInInspector] private Sprite[] m_controlSprites = new Sprite[4];
    [SerializeField, HideInInspector] private Sprite[] m_healthSprites = new Sprite[2];
    [SerializeField, HideInInspector] private Sprite[] m_energySprites = new Sprite[2];

    [SerializeField] private GameObject m_heartPrefab;
    [SerializeField] private GameObject m_energyPrefab;

    [SerializeField] private float m_abilityIconSizeLarge = 1.5f;
    [SerializeField] private float m_abilityIconSizeSmall = 1f;

    [SerializeField] private GameObject[] m_iconObjects;

    private Vector2[] m_iconPositions;

    private int numOfAbilitiesUnlocked = 0; // Tracks the amount of abilities the player has unlocked

    private async void Start()
    {
        m_player = PlayerEntity.Instance; // Get the player's info

        // Setup each icon
        for (int i = 0; i < m_Icons.Length; i++)
        {
            m_Icons[i].GO = transform.GetChild(4).GetChild(i).gameObject;
            var rect = (RectTransform)m_Icons[i].GO.transform;
            m_Icons[i].position = rect.anchoredPosition;
            m_Icons[i].index = i;
        }

        m_iconPositions = new Vector2[]{ // Positions for where the icons should be
        m_Icons[(int)IconIndex.Dash].position ,
        m_Icons[(int)IconIndex.Gun].position,
        m_Icons[(int)IconIndex.Block].position };

        InputModule input = App.GetModule<InputModule>();
        input.LastDeviceChanged += DeviceChanged;

        input.PlayerController.Player.SwapAbilityL.performed += ChangeAbilityL;
        input.PlayerController.Player.SwapAbilityR.performed += ChangeAbilityR;

        await App.GetModule<LevelModule>().AwaitInitialised();
        await App.GetModule<LevelModule>().AwaitSaveLoad();

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

        UpdateUI();
        UpdateHealthUI();
        UpdateEnergyUI();
    }

    private void OnValidate()
    {
        // Add button swaps sprites
        m_controlSprites[0] = Resources.Load<Sprite>("GFX/ButtonImages/Keyboard/1_Key_Dark");
        m_controlSprites[1] = Resources.Load<Sprite>("GFX/ButtonImages/Keyboard/3_Key_Dark");
        m_controlSprites[2] = Resources.Load<Sprite>("GFX/ButtonImages/Controller/LBButton");
        m_controlSprites[3] = Resources.Load<Sprite>("GFX/ButtonImages/Controller/RBButton");

        m_healthSprites[0] = Resources.Load<Sprite>("GFX/UI/heart");
        m_healthSprites[1] = Resources.Load<Sprite>("GFX/UI/heart_empty");

        m_energySprites[0] = Resources.Load<Sprite>("GFX/UI/energy");
        m_energySprites[1] = Resources.Load<Sprite>("GFX/UI/energy_empty");

        if (m_iconObjects == null)
            return;

        for (int i = 0; i < m_iconObjects.Length; i++)
        {
            if (i == 1)
            {
                m_iconObjects[i].GetComponent<RectTransform>().localScale = new Vector3(m_abilityIconSizeLarge, m_abilityIconSizeLarge, m_abilityIconSizeLarge);
            }
            else
            {
                m_iconObjects[i].GetComponent<RectTransform>().localScale = new Vector3(m_abilityIconSizeSmall, m_abilityIconSizeSmall, m_abilityIconSizeSmall);
            }
        }
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
    }

    public void UpdateUI()
    {
        CheckAbilitiesUnlocked(); // Find the amount of abilities unlocked
        SetStartingPosition();
        UpdateSelected();
    }

    private void SetStartingPosition()
    {
        transform.GetChild(4).GetChild(3).gameObject.SetActive(true);

        m_Icons[(int)IconIndex.Dash].GO.SetActive(true);
        m_Icons[(int)IconIndex.Block].GO.SetActive(true);
        m_Icons[(int)IconIndex.Gun].GO.SetActive(true);

        for (int i = 0; i < m_Icons.Length; i++)
        {
            if (m_Icons[i].index == 1)
            {
                m_Icons[i].GO.GetComponent<RectTransform>().localScale = new Vector3(m_abilityIconSizeLarge, m_abilityIconSizeLarge, m_abilityIconSizeLarge);
            }
            else
            {
                m_Icons[i].GO.GetComponent<RectTransform>().localScale = new Vector3(m_abilityIconSizeSmall, m_abilityIconSizeSmall, m_abilityIconSizeSmall);
            }
        }

        switch (numOfAbilitiesUnlocked)
        {
            case 0:
                m_Icons[(int)IconIndex.Dash].GO.SetActive(false);
                m_Icons[(int)IconIndex.Block].GO.SetActive(false);
                m_Icons[(int)IconIndex.Gun].GO.SetActive(false);
                transform.GetChild(4).GetChild(3).gameObject.SetActive(false);

                break;

            case 1:
                m_Icons[(int)IconIndex.Dash].GO.SetActive(false);
                m_Icons[(int)IconIndex.Block].GO.SetActive(false);

                transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
                break;

            case 2:
                m_Icons[(int)IconIndex.Dash].GO.SetActive(false);
                break;

            case 3:
                if (m_player.Abilities.GetActiveAbility() == PlayerAbilities.AbilityEnum.Block)
                {
                    m_Icons[(int)IconIndex.Dash].GO.transform.localPosition = m_iconPositions[(int)IconIndex.Block];
                    m_Icons[(int)IconIndex.Dash].index = 2;

                    m_Icons[(int)IconIndex.Gun].GO.transform.localPosition = m_iconPositions[(int)IconIndex.Dash];
                    m_Icons[(int)IconIndex.Gun].index = 0;
                }

                break;

            default:
                //uhhh ohhh wtf are you doing
                break;
        }
    }

    public void DeviceChanged()
    {
        Transform temp = transform.GetChild(4);
        temp = temp.GetChild(3);

        switch (App.GetModule<InputModule>().LastUsedDevice.displayName)
        {
            case "Keyboard":
                temp.GetComponentsInChildren<Image>()[0].sprite = m_controlSprites[0];
                temp.GetComponentsInChildren<Image>()[1].sprite = m_controlSprites[1];
                break;

            case "Mouse":
                temp.GetComponentsInChildren<Image>()[0].sprite = m_controlSprites[0];
                temp.GetComponentsInChildren<Image>()[1].sprite = m_controlSprites[1];
                break;

            case "Xbox Controller":
                temp.GetComponentsInChildren<Image>()[0].sprite = m_controlSprites[2];
                temp.GetComponentsInChildren<Image>()[1].sprite = m_controlSprites[3];
                break;

            case "Wireless Controller":
                temp.GetComponentsInChildren<Image>()[0].sprite = m_controlSprites[2];
                temp.GetComponentsInChildren<Image>()[1].sprite = m_controlSprites[3];
                break;

            default:
                temp.GetComponentsInChildren<Image>()[0].sprite = m_controlSprites[0];
                temp.GetComponentsInChildren<Image>()[1].sprite = m_controlSprites[1];
                break;
        }
    }

    private void UpdateSelected()
    {
        // Disable active icons
        for (int i = 0; i < m_Icons.Length; i++)
            m_Icons[i].ActiveGameObject().SetActive(false);

        // Enable active display for selected ability
        /*switch (m_player.Abilities.GetActiveAbility())
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
        }*/
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
                        LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[(int)IconIndex.Gun], transitionSpeed);
                        LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeLarge, m_abilityIconSizeLarge, m_abilityIconSizeLarge), transitionSpeed);
                        m_Icons[i].index--;
                    }
                    else
                    {
                        LeanTween.moveLocal(m_Icons[i].GO, m_iconPositions[(int)IconIndex.Block], transitionSpeed);
                        LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeSmall, m_abilityIconSizeSmall, m_abilityIconSizeSmall), transitionSpeed);
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
                            LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeSmall, m_abilityIconSizeSmall, m_abilityIconSizeSmall), transitionSpeed);
                        else
                            LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeLarge, m_abilityIconSizeLarge, m_abilityIconSizeLarge), transitionSpeed);
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
                            LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeSmall, m_abilityIconSizeSmall, m_abilityIconSizeSmall), transitionSpeed);
                        else
                            LeanTween.scale(m_Icons[i].GO, new Vector3(m_abilityIconSizeLarge, m_abilityIconSizeLarge, m_abilityIconSizeLarge), transitionSpeed);
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
            healthIcons[i].sprite = m_healthSprites[1]; // Set active to false
        }

        for (int i = 0; i < m_player.Health; i++) // For the ones showing how much health the player has
        {
            if (i >= healthIcons.Count)
                break;

            healthIcons[i].sprite = m_healthSprites[0]; // Set active to true
        }
    }

    public void UpdateEnergyUI()
    {
        for (int i = 0; i < energyIcons.Count; i++) // For all the energy images
        {
            energyIcons[i].sprite = m_energySprites[1]; // Set active to false
        }

        for (int i = 0; i < m_energy; i++) // For the ones showing how much energy the player has
        {
            if (i >= energyIcons.Count)
                break;

            energyIcons[i].sprite = m_energySprites[0]; // Set active to true
        }
    }

    public void AddHealth()
    {
        GameObject newHealth = new GameObject("Health", typeof(RectTransform), typeof(Image)); // Create new object
        newHealth.transform.SetParent(gameObject.transform.GetChild(0)); // Set parent

        newHealth.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f); // Set scale
        Image image = newHealth.GetComponent<Image>();
        image.sprite = m_healthSprites[0]; // Set color

        healthIcons.Add(image); // Add new game object to array of objects
    }

    public void AddEnergy()
    {
        GameObject newEnergy = new GameObject("Energy", typeof(RectTransform), typeof(Image)); // Create new object
        newEnergy.transform.SetParent(gameObject.transform.GetChild(1)); // Set parent

        newEnergy.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f); // Set scale
        Image image = newEnergy.GetComponent<Image>();
        image.sprite = m_energySprites[0]; // Set color

        energyIcons.Add(image); // Add new game object to array of objects
    }
}