// MansanitoSpawner.cs
using System.Collections;
using UnityEngine;

public class MansanitoSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] availableFruititas;
    [SerializeField] private GameObject grabbableObjectPrefab;
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 1.5f;
    [SerializeField] private float minSpawnDelay = 5f;
    [SerializeField] private float maxSpawnDelay = 10f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            SpawnFruit();
        }
    }

    private void SpawnFruit()
    {
        // Spawn the grabbable container
        GameObject grabbed = Instantiate(
            grabbableObjectPrefab,
            transform.position,
            Quaternion.identity
        );

        // Attach the random fruit visual as a child of the grabbable
        int index = Random.Range(0, availableFruititas.Length);
        Transform pivot = grabbed.transform.Find("Pivot");
        Instantiate(availableFruititas[index], pivot.position, Quaternion.identity, pivot);

        // Configure speed and size directly on the DraggableObject component
        float speed = Random.Range(minSpeed, maxSpeed);
        float scale = Random.Range(minSize, maxSize);

        pivot.localScale = Vector3.one * scale;
        grabbed.GetComponent<DraggableObject>().Init(speed);
    }
}