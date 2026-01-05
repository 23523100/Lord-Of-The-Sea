using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipHealth : MonoBehaviour
{
    [Header("Status Kapal")]
    public float maxHealth = 100f;
    public float currentHealth;

    // --- [FIX] INI YANG TADI HILANG ---
    [Tooltip("Level Upgrade HP Kapal (Default 1)")]
    public int hpLevel = 1;
    // ----------------------------------

    [Header("Regenerasi Darah (Auto-Heal)")]
    public bool canRegenerate = true;
    public float regenAmount = 7f;
    public float regenDelay = 2f;

    [HideInInspector]
    public bool isDead = false;

    [Header("Efek Kematian")]
    public GameObject damagePopupPrefab;
    public GameObject explosionPrefab;
    public AudioClip deathSound;
    public AudioSource audioSource;

    private float lastDamageTime;

    [Header("LOOT SYSTEM (Khusus Musuh)")]
    public GameObject coinPrefab;
    public int minDrop = 3;
    public int maxDrop = 6;
    public int enemyCoinValue = 50;

    void Start()
    {
        // Masukkan logic upgrade sebelum isi darah penuh
        ApplyUpgradeData();

        currentHealth = maxHealth;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void ApplyUpgradeData()
    {
        // Logic ini hanya berlaku untuk Player
        if (gameObject.CompareTag("Player"))
        {
            // Rumus: Tiap naik 1 level, darah nambah 50
            float bonusHealth = (hpLevel - 1) * 50f;
            maxHealth = 150f + bonusHealth;

            Debug.Log("Health Loaded. Level: " + hpLevel + " | Max HP: " + maxHealth);
        }
    }

    void Update()
    {
        if (isDead) return;

        // Logic Regenerasi Darah
        if (canRegenerate && currentHealth < maxHealth)
        {
            if (Time.time > lastDamageTime + regenDelay)
            {
                currentHealth += regenAmount * Time.deltaTime;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        lastDamageTime = Time.time;
        currentHealth -= damage;

        // Munculkan Angka Damage
        if (damagePopupPrefab != null)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
            GameObject popup = Instantiate(damagePopupPrefab, transform.position + randomOffset, Quaternion.identity);
            // Pastikan script DamagePopup ada public method Setup(float dmg)
            popup.GetComponent<DamagePopup>().Setup(damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        currentHealth = 0;

        // Efek Ledakan & Suara
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, transform.rotation);
        if (audioSource != null && deathSound != null) audioSource.PlayOneShot(deathSound);

        // LOGIKA KEMATIAN BERDASARKAN TAG
        if (gameObject.CompareTag("Enemy"))
        {
            // Lapor ke Manager
            if (GameLevelManager.Instance != null)
            {
                GameLevelManager.Instance.EnemyDefeated();
            }

            // Jatuhkan Koin
            if (coinPrefab != null)
            {
                int count = Random.Range(minDrop, maxDrop);
                for (int i = 0; i < count; i++)
                {
                    Vector3 spread = new Vector3(Random.Range(-2f, 2f), 1f, Random.Range(-2f, 2f));
                    GameObject loot = Instantiate(coinPrefab, transform.position + spread, Quaternion.identity);

                    CoinPickup coinScript = loot.GetComponent<CoinPickup>();
                    if (coinScript != null)
                    {
                        coinScript.coinValue = enemyCoinValue;
                    }
                }
            }
        }
        else if (gameObject.CompareTag("Player"))
        {
            // Player Mati
            if (GameLevelManager.Instance != null)
            {
                GameLevelManager.Instance.PlayerDied();
            }
            else
            {
                // Fallback kalau gak ada manager
                Invoke("ReloadScene", 3f);
            }
        }

        // MATIKAN KOMPONEN KAPAL BIAR GAK GERAK LAGI
        var playerMove = GetComponent<PlayerMovement>();
        if (playerMove) playerMove.enabled = false;

        var shipAttack = GetComponent<ShipAttack>();
        if (shipAttack) shipAttack.enabled = false;

        var enemyAI = GetComponent<EnemyShipAI>();
        if (enemyAI) enemyAI.enabled = false;

        var buoyancy = GetComponent<SimpleBuoyancy>();
        if (buoyancy) buoyancy.enabled = false;

        // FISIKA TENGGELAM
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            rb.useGravity = true;
            rb.mass = 3000f; // Biar berat
            rb.linearDamping = 2f; // Drag di air
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Biar tenggelam tegak (opsional)
        }

        // Hancurkan objek total setelah 8 detik
        Destroy(gameObject, 8f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}