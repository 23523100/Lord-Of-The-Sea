using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 10f;

    
    [Tooltip("Berapa detik peluru hidup kalau meleset?")]
    public float lifeTime = 5f;

    [Header("Visual Effects")]
    public GameObject explosionPrefab;

    [Header("Audio Effects (YANG BARU)")]
    public AudioClip hitSound;
    [Range(0f, 1f)] public float hitVolume = 0.5f;

    [HideInInspector]
    public GameObject shooterObject;

    
    void Start()
    {
        // Kalau dalam 5 detik gak nabrak apa-apa, hancurkan diri sendiri!
        Destroy(gameObject, lifeTime);
    }
    

    void OnCollisionEnter(Collision collision)
    {
        HitTarget(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
    }

    void OnTriggerEnter(Collider other)
    {
        HitTarget(other.gameObject, transform.position, -transform.forward);
    }

    void HitTarget(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        
        if (hitObject == shooterObject) return;

       
        ShipHealth targetHealth = hitObject.GetComponent<ShipHealth>();
        if (targetHealth == null)
        {
            targetHealth = hitObject.GetComponentInParent<ShipHealth>();
        }

       
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, hitPoint, Quaternion.LookRotation(hitNormal));

            
            Destroy(explosion, 2f);
        }

        
        if (hitObject.CompareTag("Enemy") && hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, hitPoint, hitVolume);
        }

        // Hancurkan peluru ini karena sudah kena target
        Destroy(gameObject);
    }
}