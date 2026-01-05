using UnityEngine;

public class ShipStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f; 
    public float currentStamina;

    [Tooltip("Berapa stamina yang pulih per detik")]
    public float regenRate = 15f;

    [Tooltip("Berapa stamina yang dipakai sekali tembak")]
    public float costPerShot = 25f;

 

    void Start()
    {
        
       
        int currentLevel = PlayerPrefs.GetInt("StaminaLevel", 1);

        // 2. Hitung Max Stamina Baru
        // Rumus: Base 100 + ((Level - 1) * 50)
        // Level 1 = 100
        // Level 2 = 150 (Bisa nembak 6x)
        // Level 5 = 300 (Bisa nembak 12x!)
        float bonusStamina = (currentLevel - 1) * 50f;
        maxStamina = 100f + bonusStamina;

        // 3. Isi penuh stamina saat mulai game
        currentStamina = maxStamina;

        // if (staminaBar != null) staminaBar.maxValue = maxStamina;
    }

    void Update()
    {
        
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            
            currentStamina = Mathf.Min(currentStamina, maxStamina);

            
        }
    }

    public bool TryShoot()
    {
        if (currentStamina >= costPerShot)
        {
            currentStamina -= costPerShot;

            

            return true; 
        }
        else
        {
           
            return false; 
        }
    }
}