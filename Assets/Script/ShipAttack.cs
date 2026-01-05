using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ShipAttack : MonoBehaviour
{
    [Header("Identitas (Wajib Centang untuk Player)")]
    public bool isPlayer = false; 

    [Header("Pengaturan Meriam")]
    public Transform firePoint;
    public GameObject cannonBallPrefab;
    public float shootForce = 170f;
    public float baseDamage = 10f;

    [Header("Pengaturan Suara")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    private ShipStamina shipStamina;

    void Start()
    {
        shipStamina = GetComponent<ShipStamina>();
    }

    
    void Update()
    {
       
        if (!isPlayer) return;

  
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !EventSystem.current.IsPointerOverGameObject())
        {
            TryToShoot();
        }
    }

    
    public void OnFire(InputValue value)
    {
        if (!isPlayer) return; 
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (value.isPressed)
        {
            TryToShoot();
        }
    }

    public void TryToShoot()
    {
        // Cek Stamina
        if (shipStamina != null)
        {
            if (shipStamina.TryShoot() == false) return;
        }

        Shoot();
    }

    void Shoot()
    {
        if (cannonBallPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(cannonBallPrefab, firePoint.position, firePoint.rotation);
            BulletDamage bulletScript = bullet.GetComponent<BulletDamage>();

            if (bulletScript != null)
            {
                bulletScript.shooterObject = this.gameObject;

                // --- LOGIKA DAMAGE ---
                float finalDamage = baseDamage; 

                
                if (isPlayer)
                {
                    int currentDmgLevel = PlayerPrefs.GetInt("DamageLevel", 1);
                    finalDamage = baseDamage + ((currentDmgLevel - 1) * 5f);
                }

                bulletScript.damage = finalDamage;
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
            }

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            Destroy(bullet, 4f);
        }
    }
}