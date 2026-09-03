using UnityEngine;
using UnityEngine.UI;

public class BossAI : MonoBehaviour, IDamageable
{
    [Header("Boss Settings")]
    [SerializeField] private EnemyData _bossData;
    [SerializeField] private float _coffeeHealthDuration = 2f;
    [SerializeField] private float _minionSpawnRate = 3f;
    [SerializeField] private float _dashSpeedMultiplier = 3f;
    [SerializeField] private float _dashDuration = 1.5f;

    private Transform _rabbitHoleTarget;
    private BoxCollider2D _roamArea;
    private Rigidbody2D _rb;

    private float _currentHealth;
    private float _minionTimer = 0f;
    private float _stateTimer = 0f;

    // State Tracking
    private bool _isPhaseTwo = false;
    private bool _isDrinkingCoffee = false;
    private bool _isDefeated = false;
    private bool _hasDrunkCoffee = false;

    // Dash Tracking
    private bool _isWindingUp = true;
    private bool _isDashing = false;
    private Vector2 _cornerTarget;
    private Vector2 _dashDirection;

    // ---------------------------------------------------------------

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseBoss(Transform target, BoxCollider2D roamArea)
    {
        _rabbitHoleTarget = target;
        _roamArea = roamArea;
        _currentHealth = _bossData.MaxHealth;

        _isPhaseTwo = false;
        _isDrinkingCoffee = false;
        _isDefeated = false;
        _hasDrunkCoffee = false;

        UIManager.Instance.ToggleBossHealthBar(true);
        UIManager.Instance.UpdateBossHealthBar(_currentHealth, _bossData.MaxHealth);

        PickNewCorner();
    }

    private void FixedUpdate()
    {
        if (_rabbitHoleTarget == null || _isDefeated || _isDrinkingCoffee)
        {
            return;
        }

        // Phase 2: Spawn minions while moving toward center
        if (_isPhaseTwo)
        {
            _minionTimer += Time.fixedDeltaTime;
            if (_minionTimer >= _minionSpawnRate)
            {
                SpawnConstructionMinion();
                _minionTimer = 0f;
            }
        }

        // --- DASH STATE MACHINE ---
        if (_isWindingUp)
        {
            // Move toward the selected corner
            Vector2 direction = (_cornerTarget - (Vector2)transform.position).normalized;
            _rb.MovePosition(_rb.position + direction * _bossData.MoveSpeed * Time.fixedDeltaTime);

            // Once reached corner, trigger dash
            if (Vector2.Distance(transform.position, _cornerTarget) < 0.5f)
            {
                StartDash();
            }
        }
        else if (_isDashing)
        {
            // Move along the dash direction at high speed
            _rb.MovePosition(_rb.position + _dashDirection * (_bossData.MoveSpeed * _dashSpeedMultiplier) * Time.fixedDeltaTime);

            _stateTimer -= Time.fixedDeltaTime;
            if (_stateTimer <= 0)
            {
                PickNewCorner();
            }
        }
    }

    private void PickNewCorner()
    {
        if (_roamArea == null)
        {
            return;
        }

        Bounds bounds = _roamArea.bounds;

        // Randomly snap to either far left/right and far top/bottom
        float x = Random.value > 0.5f ? bounds.min.x : bounds.max.x;
        float y = Random.value > 0.5f ? bounds.min.y : bounds.max.y;

        _cornerTarget = new Vector2(x, y);
        _isWindingUp = true;
        _isDashing = false;
    }

    private void StartDash()
    {
        _isWindingUp = false;
        _isDashing = true;

        _dashDirection = ((Vector2)_rabbitHoleTarget.position - (Vector2)transform.position).normalized;
        _stateTimer = _dashDuration;
    }

    public void TakeDamage(float amount)
    {
        if (_isDefeated || _isDrinkingCoffee)
        {
            return;
        }

        _currentHealth -= amount;

        // Update health bar
        UIManager.Instance.UpdateBossHealthBar(_currentHealth, _bossData.MaxHealth);

        // Check health 0
        if (_currentHealth <= 0)
        {
            if (!_hasDrunkCoffee)
            {
                TriggerCoffeeRevive();
            }
            else
            {
                Die();
            }
        }
        // Check for Phase 2
        // Trigger Phase 2 at 50% health
        else if (!_isPhaseTwo && !_hasDrunkCoffee && _currentHealth <= (_bossData.MaxHealth / 2))
        {
            _isPhaseTwo = true;

            // Force a miunion to spawn instantly
            SpawnConstructionMinion();
            _minionTimer = 0f;

            // TODO: Trigger dramatic screen shake here
        }
    }

    private void TriggerCoffeeRevive()
    {
        _isDrinkingCoffee = true;

        // Temporarily stop spawning minions while drinking
        _isPhaseTwo = false;
        _hasDrunkCoffee = true;

        // Trigger the heal sequence, and wait 2 seconds before resuming movement
        Invoke(nameof(FinishCoffee), _coffeeHealthDuration);
    }

    private void FinishCoffee()
    {
        _currentHealth = _bossData.MaxHealth;
        _isDrinkingCoffee = false;
        _isPhaseTwo = true;

        // Update UIManager Boss health bar
        UIManager.Instance.UpdateBossHealthBar(_currentHealth, _bossData.MaxHealth);
        PickNewCorner();
    }

    private void Die()
    {
        _isDefeated = true;
        UIManager.Instance.ToggleBossHealthBar(false);

        // Award points 
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.AddScore(_bossData.PointsAwarded);
        }

        WaveSpawner.Instance.OnEnemyDefeated();
        Destroy(gameObject);
    }

    private void SpawnConstructionMinion()
    {
        // TTrigger phase two swarm
        WaveSpawner.Instance.TriggerPhaseTwo(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RabbitHole") && !_isDefeated)
        {
            VictoryConditionsManager.Instance.DamageForest(20f);
        }   
    }
}
