using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] float[] spawnWeights;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] float spawnPadding = 3f;  // How far outside the screen to spawn
    [SerializeField] float spawnInterval = 0.05f; // Time between individual spawns in a wave

    [Header("Wave Settings")]
    [SerializeField] int baseEnemyCount = 1;
    [SerializeField] float waveGrowthFactor = 1.1f;
    [SerializeField] float timeBetweenWaves = 3f;
    [SerializeField] float initialHealthMultiplier = 0.1f; // Starts low (one-shot)
    [SerializeField] float healthGrowthFactor = 1.1f;

    // State
    int currentWave = 0;
    int enemiesAlive = 0;
    bool isSpawning = false;
    bool waveInProgress = false;

    // Calculated from camera
    Vector2 spawnAreaMin;
    Vector2 spawnAreaMax;

    public static event System.Action<int> OnWaveStart;

    void OnEnable()
    {
        Enemy.OnEnemyDeath += OnEnemyDied;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDeath -= OnEnemyDied;
    }

    void OnEnemyDied()
    {
        if (enemiesAlive > 0)
        {
            enemiesAlive--;
        }

        CheckWaveStatus();
    }

    void CheckWaveStatus()
    {
        if (enemiesAlive <= 0 && !isSpawning && waveInProgress)
        {
            waveInProgress = false;
            StartCoroutine(StartNextWaveDelay());
        }
    }

    IEnumerator StartNextWaveDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave();
    }

    Vector2 GetRandomSpawnPosition() {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(x, y);
    }

    int GetWeightedRandomIndex() {
        float totalWeight = 0f;
        foreach (float weight in spawnWeights)
            totalWeight += weight;

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < spawnWeights.Length; i++)
        {
            cumulative += spawnWeights[i];
            if (randomValue <= cumulative) return i;
        }

        return 0;
    }

    int CalculateEnemiesForWave(int wave)
    {
        // Formula: Base * (Growth ^ (Wave - 1))
        float count = baseEnemyCount * Mathf.Pow(waveGrowthFactor, wave - 1);
        
        // Every 5th wave is a big wave (1.5x multiplier)
        if (wave % 5 == 0 && wave != 0)
        {
            count *= 1.5f;
        }

        return (int) Mathf.Round(count);
    }

    void StartWave()
    {
        currentWave++;
        OnWaveStart?.Invoke(currentWave);
        int count = CalculateEnemiesForWave(currentWave);
        StartCoroutine(SpawnWaveRoutine(count));
    }

    IEnumerator SpawnWaveRoutine(int count) {
        isSpawning = true;
        waveInProgress = true;
        enemiesAlive = count; // Set expectation immediately so we don't trigger "wave done" early
        
        Debug.Log($"Starting Wave {currentWave} with {count} enemies.");

        float waveHealthMultiplier = initialHealthMultiplier * Mathf.Pow(healthGrowthFactor, currentWave - 1);

        for (int i = 0; i < count; i++) {
            int weightedIndex = GetWeightedRandomIndex();
            Vector2 spawnPosition = GetRandomSpawnPosition();
            GameObject enemyObj = Instantiate(enemyPrefabs[weightedIndex], spawnPosition, Quaternion.identity);
            
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.SetHealthMultiplier(waveHealthMultiplier);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
        
        isSpawning = false;
        CheckWaveStatus();
    }

    // Start is called before the first frame update
    void Start()
    {
        CalculateSpawnBounds();
        StartWave();
    }
    
    void CalculateSpawnBounds()
    {
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        
        Vector3 camPos = cam.transform.position;
        
        // Calculate screen bounds + padding (spawn just outside screen)
        spawnAreaMin = new Vector2(
            camPos.x - camWidth - spawnPadding,
            camPos.y - camHeight - spawnPadding
        );
        spawnAreaMax = new Vector2(
            camPos.x + camWidth + spawnPadding,
            camPos.y + camHeight + spawnPadding
        );
    }

    // Update is called once per frame
    void Update()
    {
        // Logic handled via events and coroutines
    }
}
