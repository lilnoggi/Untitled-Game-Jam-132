using System;
using Unity.VisualScripting;
using UnityEngine;

public class LitterHazard : MonoBehaviour, IDamageable
{
    [Header("Litter Stats")]
    [SerializeField] private float _maxLitterHealth = 30f;
    [SerializeField] private float _baseDrainRate = 1f;
    [SerializeField] private float _drainEscalationRate = 0.5f;
    [SerializeField] private float _pickupHealAmount = 5f;

    private float _currentLitterHealth;
    private float _currentDrainRate;

    // ----------------------------------------------------------------

    private void OnEnable()
    {
        // Reset stats back to default when pulled from the PoolManager
        _currentLitterHealth = _maxLitterHealth;
        _currentDrainRate = _baseDrainRate;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        // The longer it sits on the grass, the more it damages the forest
        _currentDrainRate += _drainEscalationRate * Time.deltaTime;
    }

    public void TakeDamage(float amount)
    {
        _currentLitterHealth -= amount;

        // Shrink the sprite 
        transform.localScale = Vector3.one * (_currentLitterHealth / _maxLitterHealth);

        if (_currentLitterHealth <= 0)
        {
            // Recycle from pool manager
            PoolManager.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player must clean it up for a morale boost
        if (collision.CompareTag("Player"))
        {
            VictoryConditionsManager.Instance.HealForest(_pickupHealAmount);
            PoolManager.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Continously drain the forest's health using the escalating rate
        if (collision.CompareTag("Forest"))
        {
            VictoryConditionsManager.Instance.DamageForest(_currentDrainRate * Time.fixedDeltaTime);
        }
    }
}
