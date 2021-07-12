using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    GameObject m_player; // Game object for getting player info

    PlayerEntity m_playerInfo; // Infor of the player, such as health and energy
    [SerializeField] private int m_energy = 5; // Energy the player has
    [SerializeField] private int m_health = 5; // Health the player has
    [SerializeField] List<GameObject> energyIcons; // Images for the amount of energy the player has
    [SerializeField] List<GameObject> healthIcons; // Images for the amount of health the player has

    private void OnValidate()
    {
        m_player = GameObject.Find("Green"); // Find the player
        m_playerInfo = m_player.GetComponent<PlayerEntity>(); // Get the player's info
    }

    private void Start()
    {
        // Update the UI on start
        UpdateEnergyUI(); 
        UpdateHealthUI(); 
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
            energyIcons[i].SetActive(true); // Set active to true
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
