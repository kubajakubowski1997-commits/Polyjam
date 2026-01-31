using System.Collections;
using UnityEngine;

public class GoodThoughtSpawner : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform headFocalPoint;

    [SerializeField] private GameObject goodThoughtPrefab;

    [Header("Spawn Distance")] [SerializeField]
    private float minDistance = 2.5f;

    [SerializeField] private float maxDistance = 6f;

    [Header("Timing (slow & calm)")] [SerializeField]
    private float minSpawnTime = 8f;

    [SerializeField] private float maxSpawnTime = 16f;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        // zabezpieczenie przed wieloma coroutine
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // losowe opóźnienie pierwszego spawnu
        //yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

        while (true)
        {
            Spawn();

            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void Spawn()
    {
        if (headFocalPoint == null || goodThoughtPrefab == null)
            return;

        // naturalne rozproszenie wokół głowy
        Vector3 randomDir = Random.insideUnitSphere.normalized;
        randomDir.y = Mathf.Clamp(randomDir.y, 0.2f, 0.9f);

        float distance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPos = headFocalPoint.position + randomDir * distance;

        Instantiate(goodThoughtPrefab, spawnPos, Quaternion.identity);
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
}

