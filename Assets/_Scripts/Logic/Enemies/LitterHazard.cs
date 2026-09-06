using System;
using UnityEngine;

public class LitterHazard : MonoBehaviour, IDamageable
{
    [Header("Litter Stats")]
    [SerializeField] private float _maxLitterHealth = 30f;
    [SerializeField] private float _baseDrainRate = 1f;
    [SerializeField] private float _drainEscalationRate = 0.5f;
    [SerializeField] private float _pickupHealAmount = 5f;

    [Header("Visuals")]
    [SerializeField] private Sprite[] _litterSprites;

    private float _currentLitterHealth;
    private float _currentDrainRate;
    private SpriteRenderer _sr;

    // ----------------------------------------------------------------

    private void Awake()
    {
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Reset stats back to default when pulled from the PoolManager
        _currentLitterHealth = _maxLitterHealth;
        _currentDrainRate = _baseDrainRate;
        transform.localScale = Vector3.one;

        // Randomise the rubbish visual every time one is pulled from the pool
        if (_sr != null && _litterSprites != null && _litterSprites.Length > 0)
        {
            _sr.sprite = _litterSprites[UnityEngine.Random.Range(0, _litterSprites.Length)];
        }
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
