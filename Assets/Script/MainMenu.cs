using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("SCENE TUJUAN")]
    // Nama scene selanjutnya setelah klik 'Continue' di panel panduan
    public string nextSceneName = "Story_Intro";

    [Header("UI PANELS (DRAG DISINI)")]
    // Masukkan objek Panel yang berisi tombol Play & Quit
    public GameObject mainButtonsPanel;
    // Masukkan objek Panel Panduan Key (gambar dari Canva tadi)
    public GameObject keyGuidePanel;

    [Header("AUDIO SETTINGS")]
    public AudioSource sfxAudioSource; // Speaker buat efek suara
    public AudioClip clickSound;       // File suaranya

    void Start()
    {
        // RESET KONDISI AWAL
        // Pastikan waktu berjalan normal
        Time.timeScale = 1f;

        // Pastikan Menu Utama NYALA, dan Panel Panduan MATI saat game baru mulai
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (keyGuidePanel != null) keyGuidePanel.SetActive(false);
    }

    // --- FUNGSI SUARA ---
    public void PlayClickSound()
    {
        if (sfxAudioSource != null && clickSound != null)
        {
            sfxAudioSource.PlayOneShot(clickSound);
        }
    }

    // ==================================================================
    // FUNGSI BARU UNTUK TOMBOL-TOMBOL
    // ==================================================================

    // 1. Dipasang di Tombol "PLAY" (Menu Utama)
    public void OnClick_StartButton()
    {
        PlayClickSound();

        // Sembunyikan menu utama
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        // Munculkan panel panduan
        if (keyGuidePanel != null) keyGuidePanel.SetActive(true);
    }

    // 2. Dipasang di Tombol Transparan "CONTINUE" (Panel Panduan)
    public void OnClick_ContinueButton()
    {
        PlayClickSound();

        // Baru pindah scene ke Story Intro
        SceneManager.LoadScene(nextSceneName);
    }

    // 3. Dipasang di Tombol "QUIT"
    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }

    // (Opsional: Bisa dipakai di scene lain untuk kembali ke menu)
    public void BackToMainMenu()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}