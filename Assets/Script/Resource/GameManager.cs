using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Economy")]
    public int currentCoins = 0;

    [Header("UI Reference")]
    
    public TextMeshProUGUI hudCoinText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
       
        currentCoins = PlayerPrefs.GetInt("Coins", 0);

        UpdateHUD();
    }

    public void AddCoin(int amount)
    {
        currentCoins += amount;

        
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.Save();

        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (hudCoinText != null)
            hudCoinText.text = currentCoins.ToString();
    }

    
    void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.C))
        {
            AddCoin(1000);
        }
    }
}