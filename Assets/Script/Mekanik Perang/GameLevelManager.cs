using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;

    [Header("CONFIG LEVEL")]
    public int maxLevel = 6;

    [Header("STORY & UI PANELS")]
    [Tooltip("Panel Cerita sebelum lawan Boss (Muncul setelah Wave 5)")]
    public GameObject preBossStoryPanel;

    [Tooltip("Panel Cerita setelah Boss Kalah")]
    public GameObject outroStoryPanel;

    [Tooltip("Panel Kemenangan (Win) - Muncul setelah Outro")]
    public GameObject winPanel;

    [Header("SPAWN SETTINGS")]
    public Transform[] spawnPoints;

    [Header("ENEMY PREFABS")]
    public GameObject normalEnemyPrefab;
    public GameObject bossEnemyPrefab;

    [Header("UI REFERENCES (HUD)")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI enemyCountText;
    public Image levelProgressBar;
    public GameObject hudContainer;

    [Header("ANNOUNCEMENT (POPUP)")]
    public GameObject announcementPanel;
    public TextMeshProUGUI announcementText;
    public AudioClip announcementSFX;
    public AudioSource audioSource;

    // Data Runtime
    [HideInInspector] public int currentLevel;
    [HideInInspector] public int enemiesAlive;
    private int totalEnemiesThisLevel;
    private bool isLevelFinishing = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Matikan semua panel saat mulai
        if (preBossStoryPanel != null) preBossStoryPanel.SetActive(false);
        if (outroStoryPanel != null) outroStoryPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (announcementPanel != null) announcementPanel.SetActive(false);

        isLevelFinishing = false;

        currentLevel = PlayerPrefs.GetInt("CurrentGameLevel", 1);

        StartLevel();
        UpdateUI();
    }

    void StartLevel()
    {
        isLevelFinishing = false;

        // Pastikan HUD Nyala setiap level mulai
        if (hudContainer != null) hudContainer.SetActive(true);

        // --- LOGIKA WAVE NORMAL (1-5) ---
        if (currentLevel < 6)
        {
            int enemyCountNeeded = 1 + currentLevel;
            totalEnemiesThisLevel = enemyCountNeeded;

            StartCoroutine(ShowAnnouncementRoutine("WAVE " + currentLevel, false));
            SpawnEnemies(normalEnemyPrefab, enemyCountNeeded);
        }
        // --- LOGIKA BOSS (LEVEL 6) ---
        else
        {
            totalEnemiesThisLevel = 1;
            // Teks Merah "FINAL BOSS"
            StartCoroutine(ShowAnnouncementRoutine("FINAL BOSS", true));
            SpawnEnemies(bossEnemyPrefab, 1);
        }
    }

    // --- FUNGSI BOSS ---
    public void StartBossBattle()
    {
        Debug.Log("Cerita Selesai, Memulai Boss Battle!");

        if (preBossStoryPanel != null) preBossStoryPanel.SetActive(false);

        currentLevel = 6;
        PlayerPrefs.SetInt("CurrentGameLevel", currentLevel);

        StartLevel();
        UpdateUI();
    }

    // --- FUNGSI MENANG ---
    public void ShowWinScreen()
    {
        Debug.Log("Menang! Pindah ke Win Scene...");
        PlayerPrefs.DeleteAll();
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene");
    }

    IEnumerator ShowAnnouncementRoutine(string text, bool isBoss)
    {
        if (announcementPanel != null && announcementText != null)
        {
            announcementText.text = text;
            announcementText.color = isBoss ? Color.red : new Color(1f, 0.8f, 0.4f);
            announcementPanel.SetActive(true);
            announcementPanel.transform.localScale = Vector3.zero;

            // Animasi Popup
            announcementPanel.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);
            if (isBoss) announcementPanel.transform.DOShakeRotation(0.5f, 15f, 20, 90, true);

            if (audioSource != null && announcementSFX != null) audioSource.PlayOneShot(announcementSFX);

            yield return new WaitForSeconds(3f);

            announcementPanel.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.4f);
            announcementPanel.SetActive(false);
        }
    }

    void SpawnEnemies(GameObject prefab, int countNeeded)
    {
        int finalCount = Mathf.Min(countNeeded, spawnPoints.Length);
        enemiesAlive = finalCount;

        if (countNeeded > spawnPoints.Length) Debug.LogWarning("Spawn Point kurang, tapi game tetap jalan.");

        List<Transform> availablePoints = new List<Transform>(spawnPoints);
        ShuffleList(availablePoints);

        for (int i = 0; i < finalCount; i++)
        {
            GameObject newEnemy = Instantiate(prefab, availablePoints[i].position, Quaternion.identity);
            // Putar musuh acak biar variatif
            newEnemy.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            SetupEnemyStats(newEnemy);
        }
        UpdateUI();
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1) { n--; int k = Random.Range(0, n + 1); T value = list[k]; list[k] = list[n]; list[n] = value; }
    }

    void SetupEnemyStats(GameObject enemyObj)
    {
        var enemyHealth = enemyObj.GetComponent<ShipHealth>();
        var enemyAI = enemyObj.GetComponent<EnemyShipAI>();

        if (enemyHealth != null)
        {
            if (currentLevel == 6)
            {
                // KHUSUS BOSS HP 1000
                enemyHealth.maxHealth = 1000f;
            }
            else
            {
                enemyHealth.maxHealth += (currentLevel * 50f);
            }
            enemyHealth.currentHealth = enemyHealth.maxHealth;
        }

        if (enemyAI != null)
        {
            enemyAI.damage += (currentLevel * 2f);

            // KHUSUS BOSS LEBIH SAKIT & DETEKSI JAUH
            if (currentLevel == 6)
            {
                enemyAI.damage *= 2f;
                enemyAI.detectionRange += 40f;
                enemyAI.attackRange += 25f;
            }
        }
    }

    public void EnemyDefeated()
    {
        enemiesAlive--;
        UpdateUI();
        if (enemiesAlive <= 0) StartCoroutine(LevelCompleteRoutine());
    }

    // 🔥 [UPDATE PENTING] FITUR ANTI-LAG / MEMORY CLEANER 🔥
    IEnumerator LevelCompleteRoutine()
    {
        if (isLevelFinishing) yield break;
        isLevelFinishing = true;

        Debug.Log("Wave/Boss Selesai! Melakukan Pembersihan Memori...");

        // --- MULAI PROSES PEMBERSIHAN ---
        // 1. Pause Fisika sejenak biar CPU fokus bersih-bersih
        Time.timeScale = 0f;

        // 2. Buang aset (texture/audio) tak terpakai dari RAM
        yield return Resources.UnloadUnusedAssets();

        // 3. Panggil Garbage Collector (Tukang Sampah Sistem) Paksa
        System.GC.Collect();

        // 4. Lanjut lagi
        Time.timeScale = 1f;
        // --------------------------------

        yield return new WaitForSeconds(2f);

        // KASUS 1: WAVE 5 SELESAI -> MUNCUL PANEL PRE-BOSS
        if (currentLevel == 5)
        {
            Debug.Log("Wave 5 Clear. Munculkan Story Pre-Boss...");
            if (preBossStoryPanel != null)
            {
                if (hudContainer != null) hudContainer.SetActive(false); // Sembunyikan HUD
                preBossStoryPanel.SetActive(true);
            }
            else
            {
                StartBossBattle(); // Fallback langsung Boss
            }
        }
        // KASUS 2: BOSS MATI (LEVEL 6) -> MUNCUL PANEL OUTRO
        else if (currentLevel == 6)
        {
            Debug.Log("BOSS MATI! Munculkan Story Outro...");
            if (outroStoryPanel != null)
            {
                if (hudContainer != null) hudContainer.SetActive(false);
                outroStoryPanel.SetActive(true);
            }
            else
            {
                ShowWinScreen();
            }
        }
        // KASUS 3: WAVE BIASA -> LANJUT LEVEL
        else
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentGameLevel", currentLevel);
            PlayerPrefs.Save();
            StartLevel();
        }
    }

    public void PlayerDied()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateUI()
    {
        if (levelText != null) levelText.text = (currentLevel == 6) ? "BOSS" : "WAVE " + currentLevel;
        if (enemyCountText != null) enemyCountText.text = "Musuh: " + enemiesAlive;
        if (levelProgressBar != null && totalEnemiesThisLevel > 0)
            levelProgressBar.fillAmount = 1f - ((float)enemiesAlive / (float)totalEnemiesThisLevel);
    }
}