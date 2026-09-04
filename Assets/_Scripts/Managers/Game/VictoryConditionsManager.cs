using System;
using UnityEngine;

public class VictoryConditionsManager : MonoBehaviour
{
    // Singleton Pattern
    public static VictoryConditionsManager Instance { get; private set; }

    [Header("Forest Defense")]
    [SerializeField] private float _forestMaxHealth = 100f;
    private float _currentForestHealth;
    [SerializeField] private float _passiveRegenRate = 1f;
    private bool _isGameOver = false;

    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _currentForestHealth = _forestMaxHealth;
    }

    private void Start()
    {
        // Push initial health values after UIManager finishes Awake setup
        UIManager.Instance.UpdateForestBar(_currentForestHealth, _forestMaxHealth);
    }

    private void Update()
    {
        // Automatically regenerate health over time if the forest is damaged
        if (!_isGameOver && _currentForestHealth < _forestMaxHealth)
        {
            HealForest(_passiveRegenRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// Reduces the forest's health when enemies reach the center.
    /// </summary>
    /// <param name="damageAmount">Amount of damage an enemy does</param>
    public void DamageForest(float damageAmount)
    {
        if (_isGameOver)
        {
            return;
        }

        _currentForestHealth -= damageAmount;

        // Update the UI
        UIManager.Instance.UpdateForestBar(_currentForestHealth, _forestMaxHealth);

        if (_currentForestHealth <= 0)
        {
            TriggerDefeat();
        }
    }

    public void HealForest(float healAmount)
    {
        if (_isGameOver)
        {
            return;
        }

        // Prevent health from exceeding the maximum
        _currentForestHealth = MathF.Min(_currentForestHealth + healAmount, _forestMaxHealth);

        // Update UI
        UIManager.Instance.UpdateForestBar(_currentForestHealth, _forestMaxHealth);
    }

    /// <summary>
    /// Called by WaveSpawner when all waves are successfully cleared
    /// </summary>
    public void TriggerVictory()
    {
        if (_isGameOver)
        {
            return;
        }

        _isGameOver = true;

        Debug.Log("<color=yellow>VICTORY!</color> The forest is safe from the City Planners.");

        // Show Victory Screen
        UIManager.Instance.ShowVictoryScreen();

        // TODO: Trigger Victory Music
    }

    /// <summary>
    /// If the forest takes too much damage
    /// </summary>
    private void TriggerDefeat()
    {
        _isGameOver = true;

        Debug.Log("<color=red>DEFEAT!</color> The forest has been paved over.");

        // Show Defeat Panel
        UIManager.Instance.ShowDefeatScreen();
        
        // Trigger Game Over Music
    }
}
