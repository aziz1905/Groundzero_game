using UnityEngine;

public class FlyChaseSpawnTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flyPrefab; 
    [SerializeField] private Transform nextSpawnPoint; 
    [SerializeField] private GameObject currentFly; 
    [SerializeField] private FlyChaseSpawnTrigger nextTrigger; 
    
    [Header("Settings")]
    [SerializeField] private int triggerIndex = 0; 
    [SerializeField] private bool isLastTrigger = false; 
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip triggerSound;
    
    private bool hasTriggered = false;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Trigger hanya sekali dan hanya oleh player
        if (hasTriggered || !collision.CompareTag("Player"))
            return;
        
        hasTriggered = true;
        
        // 1. Play SFX (optional)
        if (audioSource != null && triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }
        
        // 2. Buat lalat saat ini kabur (terbang ke atas & destroy)
        if (currentFly != null)
        {
            FlyEscapeController escapeController = currentFly.GetComponent<FlyEscapeController>();
            if (escapeController != null)
            {
                escapeController.Escape();
            }
        }
        
        // 3. Spawn lalat baru di posisi berikutnya (kecuali trigger terakhir)
        if (!isLastTrigger && nextSpawnPoint != null && flyPrefab != null)
        {
            GameObject newFly = Instantiate(flyPrefab, nextSpawnPoint.position, Quaternion.identity);
            newFly.name = $"LegendaryFly_{triggerIndex + 1}";
        
            if (nextTrigger != null)
            {
                nextTrigger.SetCurrentFly(newFly);
            }
        }
        else if (isLastTrigger)
        {
            Debug.Log($"Last trigger ({triggerIndex}) - no more flies to spawn");
        }
    }
    
    public void SetCurrentFly(GameObject fly)
    {
        currentFly = fly;
    }
}