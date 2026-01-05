using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events; // <--- WAJIB ADA

[RequireComponent(typeof(AudioSource))] // Otomatis nambah AudioSource di Inspector
public class StoryManager : MonoBehaviour
{
    [Header("UI COMPONENTS")]
    public Image backgroundImage;
    public TextMeshProUGUI storyTextTMP;
    public Button continueButton;

    [Header("SETTINGS")]
    public float typingSpeed = 0.04f;

    [Header("AUDIO ATMOSPHERE (BARU)")]
    [Tooltip("Musik background (akan di-loop otomatis)")]
    public AudioClip storyBGM;
    [Tooltip("Suara efek ketik (biarkan kosong jika tidak mau ada suara ketik)")]
    public AudioClip typingSFX;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("EVENT SYSTEM")]
    [Tooltip("Apa yang terjadi setelah cerita tamat? Masukkan Function Manager disini.")]
    public UnityEvent OnStoryFinished;

    [Header("DATA CERITA")]
    public string[] sentences;
    public Sprite[] backgroundSprites;

    private int index;
    private Coroutine typingCoroutine;
    private AudioSource audioSource; // Komponen Audio

    void Awake()
    {
        // Ambil referensi AudioSource dari object sendiri
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnClickContinue);
        }

        // Mulai Musik & Cerita
        PlayBGM();
        StartStory();
    }

    void PlayBGM()
    {
        if (storyBGM != null && audioSource != null)
        {
            audioSource.clip = storyBGM;
            audioSource.volume = musicVolume;
            audioSource.loop = true; // Musik berulang terus
            audioSource.Play();
        }
    }

    void StartStory()
    {
        index = 0;
        UpdateBackground();
        if (gameObject.activeInHierarchy)
            typingCoroutine = StartCoroutine(TypeSentence());
    }

    void UpdateBackground()
    {
        if (backgroundSprites.Length > index && backgroundSprites[index] != null && backgroundImage != null)
        {
            backgroundImage.sprite = backgroundSprites[index];
        }
    }

    IEnumerator TypeSentence()
    {
        storyTextTMP.text = "";

        
        foreach (char letter in sentences[index].ToCharArray())
        {
            storyTextTMP.text += letter;

           
            if (typingSFX != null && audioSource != null && storyTextTMP.text.Length % 2 == 0)
            {
                
                audioSource.PlayOneShot(typingSFX, 0.6f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    public void OnClickContinue()
    {
       
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            storyTextTMP.text = sentences[index];
            typingCoroutine = null;
        }
        
        else
        {
            if (index < sentences.Length - 1)
            {
                index++;
                UpdateBackground();
                typingCoroutine = StartCoroutine(TypeSentence());
            }
            else
            {
                FinishStory();
            }
        }
    }

    void FinishStory()
    {
        Debug.Log("Cerita Selesai. Memanggil Event Selanjutnya...");

      
        if (audioSource != null) audioSource.Stop();

        if (OnStoryFinished != null) OnStoryFinished.Invoke();
        gameObject.SetActive(false);
    }

    
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Pindah ke Scene: " + sceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}