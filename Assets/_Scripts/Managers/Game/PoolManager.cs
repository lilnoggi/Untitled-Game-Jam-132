using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A global object pool. Uses a dictionary of queues to recycle GameObjects
/// and eliminate performance spikes from runtime instantiation.
/// </summary>
public class PoolManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static PoolManager Instance { get; private set; }

    // Dictionary to hold different queues for different prefabs
    private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Retreives an inactive GameObject from the pool, or creates a new one if the queue is empty.
    /// </summary>
    /// <param name="prefab">The original prefab to spawn.</param>
    /// <param name="position">World position to place the object.</param>
    /// <param name="rotation">World rotation to apply to the object.</param>
    /// <returns>The activated GameObject.</returns>
    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string poolKey = prefab.name;

        // If this is the first time this prefab was requested, create a new queue
        if (!_poolDictionary.ContainsKey(poolKey))
        {
            _poolDictionary.Add(poolKey, new Queue<GameObject>());
        }

        GameObject objectToSpawn;

        // Pull from the queue if there are inactive objects, otherwise instantiate a new one
        if (_poolDictionary[poolKey].Count > 0)
        {
            objectToSpawn = _poolDictionary[poolKey].Dequeue();
        }
        else
        {
            objectToSpawn = Instantiate(prefab);
            objectToSpawn.name = prefab.name;
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    /// <summary>
    /// Disables a GameObject and returns it to its respective queue for future use.
    /// </summary>
    /// <param name="objectToReturn">The disabled GameObject.</param>
    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        _poolDictionary[objectToReturn.name].Enqueue(objectToReturn);
    }
}
