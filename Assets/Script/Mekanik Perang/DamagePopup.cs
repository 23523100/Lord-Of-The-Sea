using UnityEngine;
using TMPro; // Wajib ada biar bisa baca TextMeshPro

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    public float moveSpeed = 3f;
    public float lifeTime = 1f; // Berapa detik sebelum hilang

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Fungsi ini dipanggil untuk setting angkanya
    public void Setup(float damageAmount)
    {
        textMesh.text = damageAmount.ToString("0"); // "0" biar gak ada koma (bulat)

        // Opsional: Ubah warna kalau damage besar (Kritis)
        if (damageAmount > 20)
        {
            textMesh.color = Color.red;
            textMesh.fontSize += 2; // Makin besar
        }
    }

    void Update()
    {
        // 1. Gerak Naik ke Atas
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. Selalu Menghadap Kamera (Biar gak gepeng dilihat dari samping)
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }

        // 3. Hitung mundur kehancuran
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }

        // (Opsional) Efek Fade Out pelan-pelan
        if (lifeTime < 0.5f)
        {
            textMesh.alpha = lifeTime * 2; // Makin transparan
        }
    }
}