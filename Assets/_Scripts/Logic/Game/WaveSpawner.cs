using UnityEngine;

[System.Serializable]
public class WaveConfig
{
    public string waveName;
    public int enemyCount;
    public float spawnRate;
    public EnemyData[] allowedEnemyTypes;
}

public class WaveSpawner : MonoBehaviour
{
    // Singleton Pattern
    public static WaveSpawner Instance { get; private set; }

    [Header("Wave Configuration")]
    [SerializeField] private WaveConfig[] _waves;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    [Header("Targeting")]
    [SerializeField] private Transform _targetToDefend;

    private int _currentWaveIndex = 0;
    private int _enemiesAlive = 0;
    private int _enemiesSpawnedThisWave = 0;
    private bool _isSpawning = false;
    private float _spawnTimer = 0f;

    // --------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartWave();
    }

    private void Update()
    {
        if (_isSpawning)
        {
            _spawnTimer += Time.deltaTime;

            // Check if enough time has passed based on the current wave's spawn rate
            if (_spawnTimer >= _waves[_currentWaveIndex].spawnRate)
            {
                SpawnEnemy();
                _spawnTimer =0f;
            }
        }
    }

    private void StartWave()
    {
        _isSpawning = true;
        _enemiesSpawnedThisWave = 0;
        _spawnTimer = 0f;
    }

    private void SpawnEnemy()
    {
        WaveConfig currentWave = _waves[_currentWaveIndex];

        // Pick a random spawn point and a random enemy type from this wave's allowed enemy types
        Transform randomSpawn = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        EnemyData randomData = currentWave.allowedEnemyTypes[Random.Range(0, currentWave.allowedEnemyTypes.Length)];

        // Get a blank enemy prefab from the pool
        GameObject enemyObj = PoolManager.Instance.SpawnFromPool(_enemyPrefab, randomSpawn.position, Quaternion.identity);

        // Put the specific EnemyData and target into the blank prefab
        EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.InitialiseEnemy(randomData, _targetToDefend);
        }

        _enemiesAlive++;
        _enemiesSpawnedThisWave++;

        // Stop spawning enemies if the max count has been reached for this wave
        if (_enemiesSpawnedThisWave >= currentWave.enemyCount)
        {
            _isSpawning = false;
        }
    }

    /// <summary>
    /// Called from CharacterStats when an enemy's health reaches 0
    /// </summary>
    public void OnEnemyDefeated()
    {
        _enemiesAlive--;

        // If all enemies are spawned and all are dead, the wave is complete
        if (!_isSpawning && _enemiesAlive <= 0)
        {
            CompleteWave();
        }
    }

    private void CompleteWave()
    {
        _currentWaveIndex++;

        if (_currentWaveIndex < _waves.Length)
        {
            // TODO: Open ShopManager here instead
            
            // Automatically start the next wave after a 3-second delay
            Invoke(nameof(StartWave), 3f);
        }
        else
        {
            // TODO: Trigger VictoryConditionsManager
        }
    }
}
