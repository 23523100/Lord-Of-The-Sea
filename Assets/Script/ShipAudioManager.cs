using UnityEngine;

public class ShipAudioManager : MonoBehaviour
{
    [Header("Komponen Kapal")]
    public Rigidbody shipRb;

    [Header("Speaker Normal (Loop)")]
    public AudioSource woodSource;
    public AudioSource sailSource;

    [Header("Speaker Turbo")]
    public AudioSource windLoopSource; // Suara angin (Loop)
    public AudioSource boosterStartSource; // Suara ledakan awal (One Shot)

    [Header("Settings")]
    public float maxSpeed = 15f; // Kecepatan kapal saat ngebut full
    private bool isTurbo = false;

    void Start()
    {
        if (shipRb == null) shipRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Cek Kecepatan & Input
        float currentSpeed = shipRb.linearVelocity.magnitude;
        bool turboInput = Input.GetKey(KeyCode.LeftShift); // Asumsi pakai Shift

        // --- A. LOGIKA SUARA JALAN BIASA (Wood & Sail) ---
        // Volumenya 0 kalau diam, dan pelan-pelan naik sampai 1 kalau jalan
        float targetVol = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Pakai Lerp biar perubahan volumenya halus (fade in/out)
        woodSource.volume = Mathf.Lerp(woodSource.volume, targetVol, Time.deltaTime * 2);
        sailSource.volume = Mathf.Lerp(sailSource.volume, targetVol, Time.deltaTime * 2);


        // --- B. LOGIKA SUARA TURBO ---
        if (turboInput)
        {
            // Kalau baru banget pencet turbo (sebelumnya mati), bunyikan Booster sekali
            if (!isTurbo)
            {
                boosterStartSource.Play();
                isTurbo = true;
            }

            // Suara Angin: Fade In (Makin kencang)
            windLoopSource.volume = Mathf.Lerp(windLoopSource.volume, 1f, Time.deltaTime * 5);

            // Opsional: Naikkan pitch angin biar makin ngebut
            windLoopSource.pitch = Mathf.Lerp(windLoopSource.pitch, 1.2f, Time.deltaTime * 2);
        }
        else
        {
            isTurbo = false;

            // Suara Angin: Fade Out (Makin pelan sampai mati)
            windLoopSource.volume = Mathf.Lerp(windLoopSource.volume, 0f, Time.deltaTime * 5);
            windLoopSource.pitch = 1f;
        }
    }
}