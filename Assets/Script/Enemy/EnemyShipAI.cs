using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyShipAI : MonoBehaviour
{
    // --- VARIABEL BARU (PENGGANTI SHIP ATTACK) ---
    [Header("Stats Musuh (Diatur Level Manager)")]
    public float damage = 10f; // Damage bawaan musuh
    // ---------------------------------------------

    [Header("Movement Settings")]
    public float moveSpeed = 150f;
    public float rotateSpeed = 5f;
    public float wanderRadius = 100f;

    [Header("AI Intelligence")]
    public float separationRadius = 40f;
    public float separationForce = 80f;
    public float tooCloseDistance = 30f;
    public float idealCombatRange = 80f;

    public float stopChaseDistance = 400f;
    public float fleeHealthThreshold = 0.3f;
    public float resumeFightThreshold = 0.9f;

    private bool isFleeing = false;

    [Header("Obstacle Sensors")]
    public float detectionRange = 300f;
    public LayerMask obstacleLayer;

    [Header("Combat Settings")]
    public float attackRange = 200f;
    public float fireRate = 2f;
    public float shootForce = 170f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float aimHeightOffset = 2.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    // References
    private ShipStamina shipStamina;
    private ShipHealth myHealth;

    // HAPUS: private ShipAttack myAttackStats; (Sudah tidak dipakai)

    private bool isReloadingStamina = false;
    private Collider[] neighbors;

    // Logic Vars
    private bool isUnstucking = false;
    private float unstuckTimer = 0f;
    private float stuckCheckTimer = 0f;
    private float randomTurnDir = 0f;
    private Transform currentTarget;
    private Vector3 wanderTarget;
    private float nextFireTime;
    private float timerWander;
    private float timerSearch;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        shipStamina = GetComponent<ShipStamina>();
        myHealth = GetComponent<ShipHealth>();

        // HAPUS: myAttackStats = GetComponent<ShipAttack>(); (Sudah tidak dipakai)

        neighbors = new Collider[15];
        PickNewWanderPoint();
    }

    void FixedUpdate()
    {
        // 1. Anti-Panjat & Gravitasi Ekstra
        if (transform.position.y > 0.5f) rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);

        // 2. PRIORITAS UTAMA: MUNDUR DULU KALAU NYANGKUT
        if (isUnstucking)
        {
            UnstuckManeuver();
            return;
        }

        CheckIfStuck();
        CheckHealthStatus();

        if (!isFleeing)
        {
            timerSearch += Time.fixedDeltaTime;
            if (timerSearch > 0.5f) { FindClosestTarget(); timerSearch = 0; }
        }

        // --- STATE MACHINE ---
        if (isFleeing)
        {
            if (currentTarget != null) PerformFlee(currentTarget.position);
            else Wander();
        }
        else if (currentTarget != null)
        {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            if (dist > stopChaseDistance)
            {
                currentTarget = null;
                PickNewWanderPoint();
                return;
            }

            // Cek Obstacle
            if (CheckForObstacles())
            {
                AvoidObstacleBehavior();
            }
            else
            {
                // Combat Movement
                if (dist < tooCloseDistance)
                {
                    Vector3 dirAway = (transform.position - currentTarget.position).normalized;
                    MoveShip(dirAway, moveSpeed * 0.8f, true);
                }
                else
                {
                    Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
                    Vector3 separationVector = CalculateSeparation();
                    Vector3 finalDirection = (dirToTarget + separationVector).normalized;

                    if (dist <= attackRange)
                    {
                        float combatSpeed = (dist > idealCombatRange) ? moveSpeed * 0.5f : 0f;
                        MoveShip(finalDirection, combatSpeed, false);
                        RotateTowards(currentTarget.position);
                        AttackTarget();
                    }
                    else
                    {
                        MoveShip(finalDirection, moveSpeed, true);
                    }
                }
            }
        }
        else
        {
            if (CheckForObstacles()) AvoidObstacleBehavior();
            else Wander();
        }
    }

    // --- LOGIKA MUNDUR (UNSTUCK) ---

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            StartUnstuck();
        }
    }

    void StartUnstuck()
    {
        if (isUnstucking) return;
        isUnstucking = true;
        unstuckTimer = 2.0f;
        stuckCheckTimer = 0f;
        randomTurnDir = (Random.value > 0.5f) ? 1f : -1f;
    }

    void UnstuckManeuver()
    {
        unstuckTimer -= Time.fixedDeltaTime;
        rb.AddForce(-transform.forward * moveSpeed * 0.8f, ForceMode.Acceleration);
        float turnAmount = randomTurnDir * rotateSpeed * 2f * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnAmount, 0));

        if (unstuckTimer <= 0)
        {
            isUnstucking = false;
            PickNewWanderPoint();
        }
    }

    bool CheckForObstacles()
    {
        RaycastHit hit;
        Vector3 sensorOrigin = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(sensorOrigin, transform.forward, out hit, 40f, obstacleLayer))
        {
            if (hit.distance < 10f)
            {
                StartUnstuck();
                return true;
            }
            wanderTarget = transform.position + Vector3.Reflect(transform.forward, hit.normal) * 30f;
            return true;
        }
        return false;
    }

    // --- MOVEMENT ---
    void MoveShip(Vector3 targetDir, float speed, bool enableRotation = true)
    {
        if (transform.position.y > 2.0f) return;
        targetDir.y = 0;
        if (enableRotation && targetDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
        if (speed > 0.1f) rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
    }

    void AvoidObstacleBehavior()
    {
        Vector3 dir = (wanderTarget - transform.position).normalized;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(dir), rotateSpeed * 3f * Time.fixedDeltaTime));
        rb.linearVelocity = transform.forward * (moveSpeed * 0.4f);
    }

    void CheckIfStuck()
    {
        if (rb.linearVelocity.magnitude < 2f)
        {
            stuckCheckTimer += Time.fixedDeltaTime;
            if (stuckCheckTimer > 2f) StartUnstuck();
        }
        else stuckCheckTimer = 0f;
    }

    // --- UTILS ---
    void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0; if (dir != Vector3.zero) { Quaternion targetRot = Quaternion.LookRotation(dir); rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime)); }
    }
    void FindClosestTarget()
    {
        ShipHealth[] allShips = FindObjectsByType<ShipHealth>(FindObjectsSortMode.None);
        float closestDist = detectionRange; Transform bestTarget = null;
        foreach (ShipHealth ship in allShips)
        {
            if (ship.gameObject == this.gameObject || ship.currentHealth <= 0) continue;
            float dist = Vector3.Distance(transform.position, ship.transform.position);
            if (dist < closestDist) { closestDist = dist; bestTarget = ship.transform; }
        }
        currentTarget = bestTarget;
    }
    void CheckHealthStatus()
    {
        if (myHealth == null) return;
        float healthPercent = myHealth.currentHealth / myHealth.maxHealth;
        if (isFleeing) { if (healthPercent >= resumeFightThreshold) isFleeing = false; }
        else { if (healthPercent < fleeHealthThreshold) isFleeing = true; }
    }
    void PerformFlee(Vector3 threatPos)
    {
        Vector3 dirAway = (transform.position - threatPos).normalized;
        if (CheckForObstacles()) AvoidObstacleBehavior();
        else MoveShip(dirAway, moveSpeed * 1.2f, true);
    }
    void AttackTarget()
    {
        if (Time.time >= nextFireTime && currentTarget != null)
        {
            Vector3 aimPos = currentTarget.position + Vector3.up * aimHeightOffset;
            Vector3 sensorStartPos = firePoint.position + transform.forward * 2f;
            Vector3 fireDir = (aimPos - sensorStartPos).normalized;
            float distToTarget = Vector3.Distance(sensorStartPos, currentTarget.position);
            if (Physics.Raycast(sensorStartPos, fireDir, out RaycastHit hit, distToTarget, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject != currentTarget.gameObject) return;
            }
            if (shipStamina != null && shipStamina.enabled)
            {
                if (isReloadingStamina) { if (shipStamina.currentStamina >= shipStamina.maxStamina * 0.95f) isReloadingStamina = false; else return; }
                if (shipStamina.TryShoot()) { Shoot(); nextFireTime = Time.time + fireRate; } else isReloadingStamina = true;
            }
            else { Shoot(); nextFireTime = Time.time + fireRate; }
        }
    }

    
    void Shoot()
    {
        if (bulletPrefab && firePoint && currentTarget != null)
        {
            Vector3 targetPos = currentTarget.position + Vector3.up * aimHeightOffset;
            Vector3 accuracySpread = Random.insideUnitSphere * 1.5f;
            Vector3 shootDir = ((targetPos + accuracySpread) - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDir));

            Collider[] myCols = GetComponentsInChildren<Collider>();
            Collider bullCol = bullet.GetComponent<Collider>();
            if (bullCol) foreach (Collider c in myCols) Physics.IgnoreCollision(c, bullCol);

            BulletDamage bDamage = bullet.GetComponent<BulletDamage>();
            if (bDamage)
            {
                bDamage.shooterObject = this.gameObject;

               
                bDamage.damage = this.damage;
            }

            Rigidbody bRb = bullet.GetComponent<Rigidbody>();
            if (bRb) { bRb.linearVelocity = Vector3.zero; bRb.AddForce(shootDir * shootForce, ForceMode.Impulse); }
            if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);
            Destroy(bullet, 5f);
        }
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero; int count = Physics.OverlapSphereNonAlloc(transform.position, separationRadius, neighbors);
        for (int i = 0; i < count; i++)
        {
            Collider col = neighbors[i];
            if (col != null && col.gameObject != this.gameObject && col.attachedRigidbody != null)
            {
                Vector3 pushDir = transform.position - col.transform.position; pushDir.y = 0; float dist = pushDir.magnitude;
                if (dist > 0.1f && dist < separationRadius) { separation += (pushDir.normalized / dist) * separationForce; }
            }
        }
        return separation;
    }

    void Wander()
    {
        Vector3 dir = (wanderTarget - transform.position).normalized;
        Vector3 sep = CalculateSeparation();
        Vector3 finalWander = (dir + sep * 0.5f).normalized;

        MoveShip(finalWander, moveSpeed * 0.6f, true);

        timerWander += Time.fixedDeltaTime;
        if (Vector3.Distance(transform.position, wanderTarget) < 20f || timerWander > 8f)
        {
            PickNewWanderPoint();
            timerWander = 0;
        }
    }

    void PickNewWanderPoint()
    {
        float x = Random.Range(-wanderRadius, wanderRadius); float z = Random.Range(-wanderRadius, wanderRadius);
        wanderTarget = new Vector3(transform.position.x + x, 0, transform.position.z + z);
    }
}