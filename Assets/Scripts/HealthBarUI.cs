using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerHealth playerHealth;

    [Header("Text")]
    public TextMeshProUGUI healthText;  

    public float smoothSpeed = 10f;  // How fast the bar moves

    private float targetValue;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (slider == null)
            slider = GetComponent<Slider>();

        if (playerHealth == null)
        {
            Debug.LogError("HealthBarUI: No PlayerHealth found in the scene!");
            return;
        }

        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.CurrentHealth;
        targetValue = slider.value;

        UpdateHealthText(playerHealth.CurrentHealth, playerHealth.maxHealth);

        playerHealth.OnHealthChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= OnHealthChanged;
    }

    private void Update()
    {
        // Smoothly animate slider
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
    }

    private void OnHealthChanged(int current, int max)
    {
        slider.maxValue = max;
        targetValue = current;
        UpdateHealthText(current, max);
    }

    private void UpdateHealthText(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = current + " / " + max;
            
        }
    }
}
