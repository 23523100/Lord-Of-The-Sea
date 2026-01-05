using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject menuPanel;
    public TextMeshProUGUI menuCoinText;

    [Header("Health UI")]
    public TextMeshProUGUI hpCostText;
    public TextMeshProUGUI hpLevelText;

    [Header("Speed UI")]
    public TextMeshProUGUI speedCostText;
    public TextMeshProUGUI speedLevelText;

    [Header("Damage UI")]
    public TextMeshProUGUI damageCostText;
    public TextMeshProUGUI damageLevelText;

    [Header("Stamina UI")]
    public TextMeshProUGUI staminaCostText;
    public TextMeshProUGUI staminaLevelText;

    [Header("Player References")]
    public ShipHealth playerHP;
    public PlayerMovement playerMove;
    public ShipTurbo playerTurbo;

    [Header("Config")]
    public int maxLevel = 5;
    public int baseCost = 100;

    // Data Level (Default 1)
    private int hpLevel = 1;
    private int speedLevel = 1;
    private int damageLevel = 1;
    private int staminaLevel = 1;

    private bool isPaused = false;

    void Start()
    {
        
        LoadUpgradeData();

       
        menuPanel.SetActive(false);
        Time.timeScale = 1f;

        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    
    public void ToggleMenu()
    {
        isPaused = !isPaused;
        menuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f; 
        if (isPaused)
        {
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            UpdateUI();
        }
        else
        {
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    
    void LoadUpgradeData()
    {
      
        hpLevel = PlayerPrefs.GetInt("HpLvl", 1);
        speedLevel = PlayerPrefs.GetInt("SpdLvl", 1);
        damageLevel = PlayerPrefs.GetInt("DmgLvl", 1);
        staminaLevel = PlayerPrefs.GetInt("StmLvl", 1);
    }

    
    public void BuyHP()
    {
        if (TryBuyUpgrade(ref hpLevel))
        {
            PlayerPrefs.SetInt("HpLvl", hpLevel);
            if (playerHP != null) playerHP.maxHealth += 50;
        }
    }
    public void BuySpeed()
    {
        if (TryBuyUpgrade(ref speedLevel))
        {
            PlayerPrefs.SetInt("SpdLvl", speedLevel);
            if (playerMove != null)
            {
                playerMove.normalSpeed += 2f;
                playerMove.turboSpeed += 5f;
            }
            if (playerTurbo != null) playerTurbo.maxEnergy += 20f;
        }
    }
    public void BuyDamage()
    {
        if (TryBuyUpgrade(ref damageLevel))
        {
            PlayerPrefs.SetInt("DmgLvl", damageLevel);
            PlayerPrefs.SetInt("DamageLevel", damageLevel);
        }
    }
    public void BuyStamina()
    {
        if (TryBuyUpgrade(ref staminaLevel))
        {
            PlayerPrefs.SetInt("StmLvl", staminaLevel);
            PlayerPrefs.SetInt("StaminaLevel", staminaLevel);
        }
    }

    bool TryBuyUpgrade(ref int currentLevel)
    {
        if (currentLevel >= maxLevel) return false;
        int cost = baseCost * currentLevel;
        if (GameManager.Instance.currentCoins >= cost)
        {
            GameManager.Instance.AddCoin(-cost);
            currentLevel++;
            UpdateUI();
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // --- UPDATE TAMPILAN ---
    void UpdateUI()
    {
        if (GameManager.Instance != null)
            menuCoinText.text = "G : " + GameManager.Instance.currentCoins;

        UpdateSingleUI(hpLevel, hpCostText, hpLevelText);
        UpdateSingleUI(speedLevel, speedCostText, speedLevelText);
        UpdateSingleUI(damageLevel, damageCostText, damageLevelText);
        UpdateSingleUI(staminaLevel, staminaCostText, staminaLevelText);
    }

    void UpdateSingleUI(int level, TextMeshProUGUI costTxt, TextMeshProUGUI lvlTxt)
    {
        if (costTxt == null || lvlTxt == null) return;

        if (level < maxLevel)
        {
            costTxt.text = (baseCost * level) + " G";
            lvlTxt.text = "Level : " + level + " / " + maxLevel;
        }
        else
        {
            costTxt.text = "MAX";
            lvlTxt.text = "Level : MAX";
        }
    }

    // --- FUNGSI TOMBOL ---

    public void ResumeGame()
    {
        if (isPaused) ToggleMenu();
    }

    public void ExitToMainMenu()
    {
       
        ResetProgress();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationQuit()
    {
        ResetProgress();
    }

   
    void ResetProgress()
    {
        
        PlayerPrefs.DeleteKey("HpLvl");
        PlayerPrefs.DeleteKey("SpdLvl");
        PlayerPrefs.DeleteKey("DmgLvl");
        PlayerPrefs.DeleteKey("StmLvl");

        
        PlayerPrefs.DeleteKey("DamageLevel");
        PlayerPrefs.DeleteKey("StaminaLevel");

        
        PlayerPrefs.DeleteKey("CurrentGameLevel");

        
        PlayerPrefs.DeleteKey("Coins");

        PlayerPrefs.Save();
        Debug.Log(" DATA DIRESET KARENA KELUAR GAME ");
    }
}