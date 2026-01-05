using UnityEngine;
using UnityEngine.UI;

public class ShipTurbo : MonoBehaviour
{
    [Header("Turbo Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy;

    [Header("Pengaturan")]
    public float regenRate = 15f;
    public float drainRate = 40f;

    [Header("Anti-Spam (Cooldown)")]
    
    public float rechargeThreshold = 30f;
    public bool isOverheated = false; 

    [Header("UI Reference")]
    public Slider turboSlider;
    public Image fillImage; 
    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateUI();
    }

    void Update()
    {
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

       
        if (currentEnergy <= 0.1f)
        {
            isOverheated = true;
        }

       
        if (isOverheated)
        {
            if (currentEnergy >= rechargeThreshold)
            {
                isOverheated = false; 
            }
        }

        UpdateUI();
    }

    public void DrainTurbo()
    {
        
        if (currentEnergy > 0)
        {
            currentEnergy -= drainRate * Time.deltaTime;
        }
    }

    public void RegenTurbo()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += regenRate * Time.deltaTime;
        }
    }

    
    public bool CanBoost()
    {
        
        return currentEnergy > 0 && !isOverheated;
    }

    void UpdateUI()
    {
        if (turboSlider != null)
        {
            turboSlider.value = currentEnergy / maxEnergy;

          
            if (fillImage != null)
            {
                fillImage.color = isOverheated ? Color.red : Color.blue;
            }
        }
    }
}