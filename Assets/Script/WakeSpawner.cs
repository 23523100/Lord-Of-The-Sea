using UnityEngine;

public class WakeSpawner : MonoBehaviour
{
    [Header("Aset Splash")]
    public GameObject splashPrefab; 

    [Header("Posisi Muncul")]
    public Transform spawnPoint; 
    [Header("Pengaturan")]
    public float spawnRate = 0.2f; 
    public float minSpeed = 2.0f;  
    public float lifeTime = 2.0f;  

    private float timer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        if (rb != null && rb.linearVelocity.magnitude > minSpeed)
        {
            timer += Time.deltaTime;

            
            if (timer > spawnRate)
            {
                SpawnSplash();
                timer = 0;
            }
        }
    }

    void SpawnSplash()
    {
        if (spawnPoint != null && splashPrefab != null)
        {
            
            GameObject newSplash = Instantiate(splashPrefab, spawnPoint.position, spawnPoint.rotation);

            
            Destroy(newSplash, lifeTime);
        }
    }
}