using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Kena Tembak!");
            
        }

        
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}