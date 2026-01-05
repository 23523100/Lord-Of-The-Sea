using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 10f;
    public float attackRange = 50f;

    [Header("Visual")]
    public LineRenderer aimLine;
    public Transform firePoint;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        AimAtMouse();
        DrawLaser();
    }

    void AimAtMouse()
    {
        // 1. Tembakkan sinar ghaib dari kamera ke arah kursor mouse
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        // 2. Buat bidang datar matematika (Invisible Plane) setinggi posisi turret
        // Ini fungsinya sebagai "alas" penangkap mouse, biar gak perlu collider air
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayLength;

        // 3. Jika sinar mouse menabrak bidang datar ini...
        if (groundPlane.Raycast(ray, out rayLength))
        {
            
            Vector3 pointToLook = ray.GetPoint(rayLength);

           
            Vector3 targetPosition = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);

            
            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void DrawLaser()
    {
        if (aimLine != null && firePoint != null)
        {
            aimLine.positionCount = 2; // Pastikan ada 2 titik garis
            aimLine.SetPosition(0, firePoint.position); // Titik awal di ujung meriam

            // Titik akhir lurus ke depan sejauh jarak tembak
            aimLine.SetPosition(1, firePoint.position + transform.forward * attackRange);
        }
    }
}