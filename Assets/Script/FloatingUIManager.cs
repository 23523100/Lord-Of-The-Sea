using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class FloatingUIManager : MonoBehaviour
{
    [Header("UI References (Tarik dari Anak Canvas)")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public TextMeshProUGUI nameText;

    
    public TextMeshProUGUI healthNumberText;
   

    [Header("Data References (Otomatis cari di induk)")]
    public ShipHealth shipHealthData;
    public ShipStamina shipStaminaData;

    [Header("Settings")]
    public string playerName = "Bajak Laut";
    public Vector3 offset = new Vector3(0, 8f, 0); // Tinggi UI di atas kapal

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;


        if (nameText != null) nameText.text = playerName;

        
        if (transform.parent != null)
        {
            
        }
    }

    void LateUpdate()
    {
        
        if (mainCamera != null)
        {
            
            transform.rotation = mainCamera.transform.rotation;

            
            if (transform.parent != null)
            {
                transform.position = transform.parent.position + offset;
            }
        }

        
        if (shipHealthData != null)
        {
            
            if (healthSlider != null)
            {
                float healthPercent = shipHealthData.currentHealth / shipHealthData.maxHealth;
                healthSlider.value = healthPercent;
            }

           
            if (healthNumberText != null)
            {
                
                healthNumberText.text = shipHealthData.currentHealth.ToString("0");
            }
            
        }

        
        if (staminaSlider != null && shipStaminaData != null)
        {
            // Hitung persentase Stamina
            float staminaPercent = shipStaminaData.currentStamina / shipStaminaData.maxStamina;
            staminaSlider.value = staminaPercent;
        }
    }
}