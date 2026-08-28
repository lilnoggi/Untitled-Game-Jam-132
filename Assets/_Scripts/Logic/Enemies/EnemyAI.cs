using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyAI : MonoBehaviour
{
    private EnemyData _currentData;
    private Transform _target;
    private Rigidbody2D _rb;
    private CharacterStats _stats;
    private SpriteRenderer _sr;

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
    /// <param name="target">Target the enemy moves towards</param>
    public void InitialiseEnemy(EnemyData data, Transform target)
    {
        _currentData = data;
        _target = target;

        // Pass the data down to the stats component to configure health
        _stats.InitialiseEnemyHealth(data);

        // Apply the colour from the data
        if (_sr != null)
        {
            _sr.color = data.EnemyColour;
        }
    }

    private void FixedUpdate()
    {
        if (_target == null || _currentData == null)
        {
            return;
        }

        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        // Simple vector locomotion to chase the target
        Vector2 direction = (_target.position - transform.position).normalized;

        // Push the Rigidbody2D towards the target using the ScriptableObject's speed value
        _rb.MovePosition(_rb.position + direction * _currentData.MoveSpeed * Time.fixedDeltaTime);
    }
}
