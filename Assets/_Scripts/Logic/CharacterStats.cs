using UnityEngine;

public class CharacterStats : MonoBehaviour, IDamageable
{
    private EnemyData _enemyData;
    private float _currentHealth;

    // --------------------------------

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
        _currentHealth -= amount;

        // TODO: Trigger hit animation

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // TODO: EconomyManager will call this _enemyData.PointsAwarded

        // WaveSpawner needs to know an enemy was defeated
        WaveSpawner.Instance.OnEnemyDefeated();

        // Return prefab to pool
        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
