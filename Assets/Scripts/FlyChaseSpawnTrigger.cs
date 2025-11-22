using UnityEngine;

public class FlyChaseSpawnTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flyPrefab; // Prefab lalat
    [SerializeField] private Transform nextSpawnPoint; // Posisi spawn lalat berikutnya
    [SerializeField] private GameObject currentFly; // Lalat yang ada saat ini (untuk trigger pertama)
    [SerializeField] private FlyChaseSpawnTrigger nextTrigger; // Reference ke trigger berikutnya (untuk auto-assign)
    
    [Header("Settings")]
    [SerializeField] private int triggerIndex = 0; // Index trigger (0-3) untuk debug
    [SerializeField] private bool isLastTrigger = false; // Apakah ini trigger terakhir (no spawn after)
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip triggerSound;
    
    private bool hasTriggered = false;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Validasi
        if (flyPrefab == null)
            Debug.LogWarning($"FlyChaseSpawnTrigger {triggerIndex}: Fly Prefab not assigned!");
        
        if (!isLastTrigger && nextSpawnPoint == null)
            Debug.LogWarning($"FlyChaseSpawnTrigger {triggerIndex}: Next Spawn Point not assigned!");
        
        if (!isLastTrigger && nextTrigger == null)
            Debug.LogWarning($"FlyChaseSpawnTrigger {triggerIndex}: Next Trigger not assigned! Current fly won't be auto-assigned to next trigger.");
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Trigger hanya sekali dan hanya oleh player
        if (hasTriggered || !collision.CompareTag("Player"))
            return;
        
        hasTriggered = true;
        
        Debug.Log($"FlyTrigger {triggerIndex} activated by player");
        
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
                Debug.Log($"Current fly (Trigger {triggerIndex}) is escaping");
            }
            else
            {
                Debug.LogWarning($"FlyEscapeController not found on current fly (Trigger {triggerIndex})!");
            }
        }
        else
        {
            Debug.LogWarning($"Current fly not assigned for Trigger {triggerIndex}!");
        }
        
        // 3. Spawn lalat baru di posisi berikutnya (kecuali trigger terakhir)
        if (!isLastTrigger && nextSpawnPoint != null && flyPrefab != null)
        {
            GameObject newFly = Instantiate(flyPrefab, nextSpawnPoint.position, Quaternion.identity);
            newFly.name = $"LegendaryFly_{triggerIndex + 1}";
            
            Debug.Log($"Spawned new fly at {nextSpawnPoint.name} (Trigger {triggerIndex})");
            
            // AUTO-ASSIGN ke trigger berikutnya
            if (nextTrigger != null)
            {
                nextTrigger.SetCurrentFly(newFly);
                Debug.Log($"Auto-assigned new fly to Trigger {triggerIndex + 1}");
            }
            else
            {
                Debug.LogWarning($"Next trigger not assigned! Please manually assign or set nextTrigger reference.");
            }
        }
        else if (isLastTrigger)
        {
            Debug.Log($"Last trigger ({triggerIndex}) - no more flies to spawn");
        }
    }
    
    /// <summary>
    /// Set lalat saat ini dari script lain (untuk trigger yang dinamis)
    /// </summary>
    public void SetCurrentFly(GameObject fly)
    {
        currentFly = fly;
        Debug.Log($"Trigger {triggerIndex}: Current fly set to {fly.name}");
    }
}