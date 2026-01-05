using UnityEngine;
using System.Collections;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Settings")]
    public AudioSource source;
    public AudioClip[] soundClips; 
    [Header("Waktu Jeda (Detik)")]
    public float minWaitTime = 5f;  
    public float maxWaitTime = 15f; 

    void Start()
    {
        if (source == null) source = GetComponent<AudioSource>();

        
        StartCoroutine(PlaySoundRandomly());
    }

    IEnumerator PlaySoundRandomly()
    {
        while (true) 
        {
           
            float waitTime = Random.Range(minWaitTime, maxWaitTime);

            
            yield return new WaitForSeconds(waitTime);

            
            if (soundClips.Length > 0 && source != null)
            {
               
                int randomIndex = Random.Range(0, soundClips.Length);
                source.PlayOneShot(soundClips[randomIndex]);
            }
        }
    }
}