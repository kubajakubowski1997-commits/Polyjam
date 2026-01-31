using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GoodThoughtSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform headFocalPoint;
    [SerializeField] private GameObject goodThoughtPrefab;

    [Header("Spawn Distance")]
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 3.5f;

    [Header("Timing")]
    [SerializeField] private float minSpawnTime = 0.8f;
    [SerializeField] private float maxSpawnTime = 2.5f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            Spawn();

            float wait = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(wait);
        }
    }

    private void Spawn()
    {
        if (headFocalPoint == null || goodThoughtPrefab == null)
            return;

        // losowy kierunek (sfera)
        Vector3 randomDir = Random.onUnitSphere;

        // opcjonalnie: nie spawnuj idealnie z dołu
        randomDir.y = Mathf.Abs(randomDir.y);

        float distance = Random.Range(minDistance, maxDistance);

        Vector3 spawnPos = headFocalPoint.position + randomDir * distance;

        Instantiate(goodThoughtPrefab, spawnPos, Quaternion.identity);
    }
}
