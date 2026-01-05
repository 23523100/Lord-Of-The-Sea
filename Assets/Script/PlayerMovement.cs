using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public Transform cam;

    [Header("Speed Settings")]
    public float normalSpeed = 15f;
    public float turboSpeed = 30f;  

    [Header("Rotation")]
    public float rotateSpeed = 2f;

    [Header("Visual Effects")] 
    public ParticleSystem speedEffect; 

    
    private ShipTurbo turboSystem; 
    private float activeSpeed;     
   

    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        turboSystem = GetComponent<ShipTurbo>(); 

        // Setting otomatis kamera
        if (cam == null && Camera.main != null)
            cam = Camera.main.transform;

        // Optimasi Rigidbody
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Set kecepatan awal
        activeSpeed = normalSpeed;
    }

    // Input System Unity baru (Untuk WASD/Analog)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        
        if (isShiftPressed && turboSystem != null && turboSystem.CanBoost())
        {
            
            activeSpeed = turboSpeed;
            turboSystem.DrainTurbo();

            if (speedEffect != null && !speedEffect.isPlaying)
            {
                speedEffect.Play();
            }
        }
        else
        {
            // MODE NORMAL
            activeSpeed = normalSpeed;
            if (turboSystem != null) turboSystem.RegenTurbo();

            
            if (speedEffect != null && speedEffect.isPlaying)
            {
                speedEffect.Stop();
            }
        }

        
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // A. Rotasi Mulus
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(nextRotation);

            // B. Maju (Pakai activeSpeed yang sudah dihitung di atas)
            Vector3 targetVelocity = moveDirection * activeSpeed;

            // Lerp velocity biar ada akselerasi sedikit (rasa berat kapal)
            Vector3 newVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);

            // Pertahankan Y (biar tetep ngapung)
            rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
        }
        else
        {
           
            Vector3 stopVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 3f);
            rb.linearVelocity = new Vector3(stopVelocity.x, rb.linearVelocity.y, stopVelocity.z);

           
            if (turboSystem != null) turboSystem.RegenTurbo();
        }

       
        if (transform.position.y > 0.5f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -5f, rb.linearVelocity.z);
        }
    }
}