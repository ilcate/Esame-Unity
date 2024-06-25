using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<Transform> spawnPoints;

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points set.");
            return null;
        }
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
    }
}