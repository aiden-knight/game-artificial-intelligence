using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static Action<Vector3,Vector3> OnPickUpSpawned;

    [SerializeField]
    Transform[] spawnPoints;

    [SerializeField]
    GameObject healthPickUpPrefab;

    [SerializeField]
    GameObject ammoPickUpPrefab;

    float minWaitTime = 15.0f;
    float maxWaitTime = 30.0f;

    float timeSpawnAllowed = 0;
    float currentSpawnDelay = 0;
    bool spawnAllowed = true;

    private void Start()
    {
        Pickup.PickUpCollected += OnPickupCollected;

        currentSpawnDelay = minWaitTime;
    }

    private void FixedUpdate()
    {
        if(spawnAllowed && Time.time > timeSpawnAllowed + currentSpawnDelay)
        {
            Spawn();
        }
    }

    void OnPickupCollected()
    {
        spawnAllowed = true;
        timeSpawnAllowed = Time.time;
    }
    

    void Spawn()
    {
        int rand1 = UnityEngine.Random.Range(0, spawnPoints.Length - 1);
        int rand2 = UnityEngine.Random.Range(0, spawnPoints.Length - 1);

        if(rand1 == rand2) // Only one try
        {
            rand2 = UnityEngine.Random.Range(0, spawnPoints.Length - 1);
        }

        Transform selectedSpawn1 = spawnPoints[rand1];
        Transform selectedSpawn2 = spawnPoints[rand2];

        GameObject g1 = Instantiate(healthPickUpPrefab, selectedSpawn1.position, Quaternion.identity);
        GameObject g2 = Instantiate(ammoPickUpPrefab, selectedSpawn2.position, Quaternion.identity);

        OnPickUpSpawned.Invoke(selectedSpawn1.position, selectedSpawn2.position);

        currentSpawnDelay = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        spawnAllowed = false;
    }
}
