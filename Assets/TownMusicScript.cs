using UnityEngine;
using UnityEngine.Audio;

public class TownMusicFader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    
    [Header("Distance Settings")]
    [SerializeField] private float maxDistance = 100f; // Distance where music is silent
    [SerializeField] private float minDistance = 20f;  // Distance where music is full volume
    
    [Header("Volume Settings")]
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float fadeSpeed = 2f; // How quickly volume changes
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float minimumVolumeThreshold = 0.01f; // Stop audio below this volume
    
    [Header("Audio Source Settings - VR Optimized")]
    [SerializeField] private float spatialBlend = 0.6f; // 0 = 2D, 1 = fully 3D (0.5-0.7 recommended for VR music)
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
    [SerializeField] private float dopplerLevel = 0f; // Set to 0 for music (prevents pitch changes)
    
    private float targetVolume;
    private bool isPlaying = false;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("TownMusicFader: No player found. Make sure your player has the 'Player' tag.");
            }
        }
        
        // Setup audio source if not assigned
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                Debug.LogError("TownMusicFader: No AudioSource found. Please add an AudioSource component or assign one.");
                enabled = false;
                return;
            }
        }
        
        // Configure audio source
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = 0f;
        
        // Configure spatial audio for VR
        musicSource.spatialBlend = spatialBlend; // Hybrid 2D/3D for best VR experience
        musicSource.rolloffMode = rolloffMode;
        musicSource.minDistance = minDistance;
        musicSource.maxDistance = maxDistance;
        musicSource.dopplerLevel = dopplerLevel; // Disable doppler effect for music
        
        // Assign to audio mixer group if provided
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        
        // Start with music stopped
        isPlaying = false;
    }
    
    void Update()
    {
        if (player == null || musicSource == null) return;
        
        // Calculate distance to town center
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Calculate target volume based on distance with curve
        if (distance <= minDistance)
        {
            targetVolume = maxVolume;
        }
        else if (distance >= maxDistance)
        {
            targetVolume = 0f;
        }
        else
        {
            // Normalize distance between 0 (at minDistance) and 1 (at maxDistance)
            float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
            
            // Evaluate curve (inverted so 0 distance = full volume)
            float curveValue = fadeCurve.Evaluate(1f - normalizedDistance);
            targetVolume = maxVolume * curveValue;
        }
        
        // Smoothly interpolate to target volume
        musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        
        // Start/stop audio based on volume threshold to save CPU
        if (targetVolume > minimumVolumeThreshold && !isPlaying)
        {
            musicSource.Play();
            isPlaying = true;
        }
        else if (targetVolume <= minimumVolumeThreshold && isPlaying && musicSource.volume < minimumVolumeThreshold)
        {
            musicSource.Stop();
            isPlaying = false;
        }
    }
    
    // Optional: Visualize the fade zones in the editor
    void OnDrawGizmosSelected()
    {
        // Draw max distance sphere (where music is silent)
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
        
        // Draw min distance sphere (where music is full volume)
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minDistance);
        
        // Draw fade zone
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, (maxDistance + minDistance) / 2f);
    }
}