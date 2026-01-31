using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float[] spawnWeights;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] int enemiesPerSpawn = 1;
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] int bigWaveEnemyCount = 10;
    [SerializeField] float bigWaveInterval = 1f;
    [SerializeField] float spawnPadding = 1f;  // How far outside the screen to spawn
    
    // Calculated from camera
    Vector2 spawnAreaMin;
    Vector2 spawnAreaMax;



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



    void SpawnEnemies(int count) {
        for (int i = 0; i < count; i++) {
            int weightedIndex = GetWeightedRandomIndex();
            Vector2 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefabs[weightedIndex], spawnPosition, Quaternion.identity);
        }
    }



    void SpawnBigWave() {
        SpawnEnemies(bigWaveEnemyCount);
    }



    IEnumerator SpawnRoutine() {
        float timeSinceLastBigWave = 0f;
        while (true) {
            timeSinceLastBigWave += spawnInterval;

            if (timeSinceLastBigWave >= bigWaveInterval) {
                SpawnBigWave();
                timeSinceLastBigWave = 0f;
            }
            else {
                SpawnEnemies(enemiesPerSpawn);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        CalculateSpawnBounds();
        StartCoroutine(SpawnRoutine());
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
        
    }
}
