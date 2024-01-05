using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int aliveEnemyCount = 0;
    public int waveEnemiesSpawned = 0;
    public int waveMaxEnemies = 10;
    public int maxEnemiesAllowed = 50;

    public GameObject prefabToSpawn = null;
    public float InitialSpawnDelay = 5.0f;
    private float currentSpawnDelay;
    private float lastSpawnTime;
    public Transform[] spawnPoints = null;

    private Transform playerTransform;

    bool waveStart = true;
    float nextWaveTimer = 10;
    float waveEndTime = 0;

    //--------------------------------------------------------

    private void Start()
    {
        playerTransform = FindObjectOfType<DecisionMakingEntity>().transform;
        DecisionMakingEntity.OnPlayerDead += () => { Destroy(gameObject); };
        SimpleEnemy.OnEnemyDeath += DecrementAliveEnemyCount;

        currentSpawnDelay = InitialSpawnDelay;

        Spawn();
        lastSpawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (waveStart && waveEnemiesSpawned < waveMaxEnemies && aliveEnemyCount < maxEnemiesAllowed)
        {
            if(Time.time > lastSpawnTime + currentSpawnDelay)
            {
                Spawn();
            }
        }

        if(!waveStart && Time.time > waveEndTime + nextWaveTimer)
        {
            waveStart = true;
        }
    }

    void DecrementAliveEnemyCount()
    { 
        aliveEnemyCount--;
        if(aliveEnemyCount == 0 && waveEnemiesSpawned == waveMaxEnemies)
        {
            InitNextWave();
        }
    }

    void InitNextWave()
    {
        waveStart = false;
        currentSpawnDelay *= 0.9f;
        if (currentSpawnDelay < 1)
        {
            currentSpawnDelay = 1;
        }
        waveMaxEnemies += 5;
        waveEndTime = Time.time;
        waveEnemiesSpawned = 0;
    }

    void Spawn()
    {
        lastSpawnTime = Time.time;

        aliveEnemyCount++;
        waveEnemiesSpawned++;

        int potentialSpawns = spawnPoints.Length;

        Transform selectedSpawn = spawnPoints[Random.Range(0, potentialSpawns - 1)];

        GameObject g = Instantiate(prefabToSpawn, selectedSpawn.position, Quaternion.identity);
        g.GetComponent<SimpleEnemy>().InitTarget(playerTransform);
    }
}
