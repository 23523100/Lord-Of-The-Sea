using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 15;

   
    public float rotateSpeed = 100f;

    void Update()
    {
       
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            Collect();
        }
    }

    void Collect()
    {
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
        }

        
        Destroy(gameObject);
    }
}