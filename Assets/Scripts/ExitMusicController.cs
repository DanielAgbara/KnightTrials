using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExitMusicController : MonoBehaviour
{
    [Header("Music Settings")]
    public float maxVolume = 0.8f;
    
    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Knight").transform;
        
        // Start with volume at 0 (we'll control it manually)
        audioSource.volume = 0;
        audioSource.Play();
    }

    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        UpdateVolume(distance);
    }

    void UpdateVolume(float distance)
    {
        // Calculate volume based on distance
        float volume = Mathf.InverseLerp(
            audioSource.maxDistance, 
            audioSource.minDistance, 
            distance
        );
        
        // Apply volume with maxVolume limit
        audioSource.volume = Mathf.Clamp(volume, 0, maxVolume);
    }
}