using UnityEngine;

public class RandomCoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab; 
    public int totalCoins = 20;   
    public float areaSize = 100f; 
    public float waterLevel = 0f; 

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < totalCoins; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize, areaSize),
                waterLevel,
                Random.Range(-areaSize, areaSize)
            );

            Instantiate(coinPrefab, randomPos, Quaternion.identity);
        }
    }
}