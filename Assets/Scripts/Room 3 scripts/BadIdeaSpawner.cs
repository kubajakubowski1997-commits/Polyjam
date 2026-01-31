using System.Collections;
using UnityEngine;

public class BadIdeaSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; // 2 punkty spawnów
    [SerializeField] private GameObject ballPrefab;

    [Header("Timing")]
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (spawnPoints.Length == 0 || ballPrefab == null)
                yield break; // brak ustawień → przerywamy

            // Losowy punkt spawn
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Tworzymy prefab – prefab sam ogarnia target
            Instantiate(ballPrefab, spawn.position, Quaternion.identity);

            // Losowy czas do następnego spawnu
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
