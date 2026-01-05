using UnityEngine;

public class SimpleBuoyancy : MonoBehaviour
{
    [Header("Water Settings")]
    public float waterLevel = 0.5f;
    public float floatThreshold = 2f;
    public float waterDensity = 1.5f;
    public float downForce = 2f;

    [Header("Physics Settings")]
    public float waterDrag = 3f;      // Berat saat di air (Stabil)
    public float airDrag = 0.05f;     // Ringan saat di udara (Cepat Jatuh)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Gravitasi buatan biar kapal gak melayang aneh
        rb.AddForce(Vector3.down * downForce, ForceMode.Acceleration);

        // Cek ketinggian kapal
        float diff = waterLevel - transform.position.y;

        // --- LOGIKA BARU: Ganti Drag Otomatis ---
        if (diff > 0) // Jika Kapal DI DALAM AIR
        {
            rb.linearDamping = waterDrag; // Pakai rem air (3)

            // Hitung gaya apung
            float forceFactor = Mathf.Clamp01(diff / floatThreshold);
            Vector3 floatForce = Vector3.up * Physics.gravity.magnitude * rb.mass * (waterDensity - rb.mass / 1000f) * forceFactor; // Rumus disederhanakan
            rb.AddForce(Vector3.up * Physics.gravity.magnitude * rb.mass * waterDensity * forceFactor, ForceMode.Force);
        }
        else // Jika Kapal TERBANG DI UDARA
        {
            rb.linearDamping = airDrag; // Lepas rem (0.05) biar jatuh!
        }
    }
}