using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyAI : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject _litterPrefab;

    private EnemyData _currentData;
    private Transform _target;
    private BoxCollider2D _roamArea;
    private Rigidbody2D _rb;
    private CharacterStats _stats;
    private SpriteRenderer _sr;
    private Vector2 _currentRoamTarget;
    private Vector2 _fleeTarget;

    private bool _isFleeing = false;

    // ------------------------------------

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<CharacterStats>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Configures the AI with data and a target
    /// Called from the WaveSpawner after pulling the enemy from the pool.
    /// </summary>
    /// <param name="data">EnemyData</param>
    /// <param name="rabbitHole">Target the enemy moves towards</param>
    public void InitialiseEnemy(EnemyData data, Transform rabbitHole, BoxCollider2D roamArea)
    {
        // Reset fleeing state when pulled from the pool
        _isFleeing = false;

        // Reset scale for object pool
        transform.localScale = Vector3.one;
        if (_sr != null)
        {
            _sr.transform.localPosition = Vector3.zero;
        }

        _currentData = data;
        _target = rabbitHole;
        _roamArea = roamArea;

        // Pass the data down to the stats component to configure health
        _stats.InitialiseEnemyHealth(data);

        // Choose a random sprite from the array
        if (_sr != null && data.EnemySprites != null && data.EnemySprites.Length > 0)
        {
            int randomIndex = Random.Range(0, data.EnemySprites.Length);
            _sr.sprite = data.EnemySprites[randomIndex];

            // Force the colour to pure white
            _sr.color = Color.white;
        }

        // If this is a roaming enemy, pick their first destination
        if (_currentData.Type != EnemyType.CityPlanner)
        {
            PickNewRoamTarget();
        }
    }

    private void FixedUpdate()
    {
        if (_target == null || _currentData == null)
        {
            return;
        }

        // Override the state machine if enemy is annoyed and leaving
        if (_isFleeing)
        {
            // Sprint away at 3x speed
            MoveTowardsDestination(_fleeTarget, 3f);

            // Check if enemy has reached off-screen coordinate
            if (Vector2.Distance(transform.position, _fleeTarget) < 2f)
            {
                WaveSpawner.Instance.OnEnemyDefeated();
                PoolManager.Instance.ReturnToPool(gameObject);
            }

            return;
        }

        switch (_currentData.Type)
        {
            case EnemyType.CityPlanner:
                // Boss strictly targets the rabbit hole
                MoveTowardsDestination(_target.position);
                break;

            case EnemyType.ConfusedTourist:           
            case EnemyType.LitteringTeenager:
            case EnemyType.ConstructionWorker:
                // All standard enemies share the roaming logic for now
                HandleRoaming();
                break;
        }
    }

    private void MoveTowardsDestination(Vector2 destination, float speedMultiplier = 1f)
    {
        // Simple vector locomotion to chase the destination
        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        // Push the Rigidbody2D towards the destination using the ScriptableObject's speed value
        _rb.MovePosition(_rb.position + direction * (_currentData.MoveSpeed * speedMultiplier) * Time.fixedDeltaTime);
    }

    private void HandleRoaming()
    {
        MoveTowardsDestination(_currentRoamTarget);

        // If the enemy is close enough to their random spot, pick a new one
        if (Vector2.Distance(transform.position, _currentRoamTarget) < 0.5f)
        {
            // Spawn pooled litter if this is a teenager
            if (_currentData.Type == EnemyType.LitteringTeenager && _litterPrefab != null)
            {
                PoolManager.Instance.SpawnFromPool(_litterPrefab, transform.position, Quaternion.identity);    
            }

            PickNewRoamTarget();

            // TODO: Trigger specific abilities here (take picture, drop flag, throw litter)
        }
    }

    private void PickNewRoamTarget()
    {
        if (_roamArea == null)
        {
            return;
        }

        Bounds bounds = _roamArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        _currentRoamTarget = new Vector2(randomX, randomY);
    }

    public void StartFleeing()
    {
        _isFleeing = true;

        // Pick a coordinate far off-screen by calculating the direction away from the center
        Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)_target.position).normalized;
        _fleeTarget = (Vector2)transform.position + fleeDirection * 15f;
    }

    public void UpdateAnnoyanceColour(float currentHealth, float maxHealth)
    {
        if (_sr != null && _currentData != null)
        {
            // Calculate a percentage from 1.0 (Full) to 0.0 (Empty)
            float healthPercent = currentHealth / maxHealth;

            // Lerp to blend between colours
            _sr.color = Color.Lerp(Color.red, Color.white, healthPercent);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger damage if the city planner hits the specific rabbit hole target
        if (collision.CompareTag("RabbitHole") && _currentData.Type == EnemyType.CityPlanner)
        {
            VictoryConditionsManager.Instance.DamageForest(25f);

            // Tell the spawner the boss has been removed
            WaveSpawner.Instance.OnEnemyDefeated();
            PoolManager.Instance.ReturnToPool(gameObject);
        }
    }

    // The enemy stays in the roam area and applies damage over time
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Only tri
        if (!_isFleeing && collision.CompareTag("Forest"))
        {
            // Drain a tiny amount of health every frame
            // Damage the forest
            VictoryConditionsManager.Instance.DamageForest(2f * Time.fixedDeltaTime);
        }     
    }

    // Detect when the enemy physically walks outside the box
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isFleeing && collision.CompareTag("Forest"))
        {
            // Restore forest health and finally return to the pool
            VictoryConditionsManager.Instance.HealForest(10f);
        }     
    }
}
