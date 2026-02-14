using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansanitoSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] availableFruititas;

    [SerializeField] float minSpeed = 3f;
    [SerializeField] float maxSpeed = 10f;

    [SerializeField] float minSize = 0.5f;
    [SerializeField] float maxSize = 1.5f;

    private class FruitData
    {
        public GameObject go;
        public float speed;

        public FruitData(GameObject go, float speed)
        {
            this.go = go;
            this.speed = speed;
        }
    }

    private List<FruitData> currentFrutitas = new List<FruitData>();

    private void Start()
    {
        StartCoroutine(StartFruitSpawnTimer());
    }

    IEnumerator StartFruitSpawnTimer()
    {
        while (true)
        {
            float newTimeToWait = Random.Range(5f, 10f);
            yield return new WaitForSeconds(newTimeToWait);

            int fruitIndex = Random.Range(0, availableFruititas.Length);

            GameObject fruit = Instantiate(
                availableFruititas[fruitIndex],
                transform.position,
                Quaternion.identity,
                transform
            );

            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            float randomScale = Random.Range(minSize, maxSize);
            fruit.transform.localScale = Vector3.one * randomScale;

            currentFrutitas.Add(new FruitData(fruit, randomSpeed));

            Destroy(fruit, 10f);
        }
    }

    private void Update()
    {
        for (int i = currentFrutitas.Count - 1; i >= 0; i--)
        {
            if (currentFrutitas[i].go == null)
            {
                currentFrutitas.RemoveAt(i);
                continue;
            }

            currentFrutitas[i].go.transform.position +=
                Vector3.left * currentFrutitas[i].speed * Time.deltaTime;
        }
    }
}
