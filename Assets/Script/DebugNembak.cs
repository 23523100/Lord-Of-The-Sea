using UnityEngine;
using UnityEngine.EventSystems;

public class DebugNembak : MonoBehaviour
{
    void Update()
    {
        // Cek Klik Kiri
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("🖱️ MOUSE DIKLIK!");

            // 1. Cek Apakah Terhalang UI?
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("❌ DIBLOKIR UI! Ada Panel/Tombol yang menghalangi layar.");
                return;
            }

            // 2. Cek Raycast ke Dunia Game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("✅ RAYCAST KENA: " + hit.collider.gameObject.name);
                Debug.Log("🌊 LAYER OBJEK: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            }
            else
            {
                Debug.Log("💨 RAYCAST TIDAK KENA APAPUN (Tembus ke Infinity)");
            }
        }
    }
}