using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{
    private EnemyData _enemyData;
    private float _currentHealth;

    private EnemyAI _enemyAI;

    // --------------------------------

    private void Awake()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    /// <summary>
    /// COnfigures the stats based on the provided data.
    /// Called from the WaveSpawner or EnemyAI immediately after getting the prefab from the pool
    /// </summary>
    public void InitialiseEnemyHealth(EnemyData data)
    {
        _enemyData = data;

        // Reset health to max when pulled from the pool
        _currentHealth = _enemyData.MaxHealth;
    }

    /// <summary>
    /// Implements the IDamageable interface method.
    /// </summary>
    /// <param name="amount">Amount of damage.</param>
    public void TakeDamage(float amount)
    {
        // Ignore damage if the enemy is already fleeing
        if (_currentHealth <= 0)
        {
            return;
        }
        
        _currentHealth -= amount;

        // TODO: Trigger hit animation

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // If this is the boss, trigger the phase 2 swarm
        if (_enemyData.Type == EnemyType.CityPlanner)
        {
            WaveSpawner.Instance.TriggerPhaseTwo(transform.position);
        }
        
        // TODO: EconomyManager will call this _enemyData.PointsAwarded

        // Trigger fleeing state
        if (_enemyAI != null)
        {
            _enemyAI.StartFleeing();
        }
    }
}
