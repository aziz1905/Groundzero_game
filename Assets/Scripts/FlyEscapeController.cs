using System.Collections;
using UnityEngine;

public class FlyEscapeController : MonoBehaviour
{
    [Header("Fly Settings")]
    [SerializeField] private float flyUpSpeed = 5f; // Kecepatan terbang ke atas
    [SerializeField] private float flyUpDuration = 2f; // Durasi terbang sebelum destroy
    [SerializeField] private float flyUpHeight = 10f; // Tinggi yang dituju (relatif)
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flyingAnimationTrigger = "Fly"; // Trigger animasi terbang (optional)
    
    [Header("Audio")]
    [SerializeField] private AudioClip escapeSound;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Visual Effects")]
    [SerializeField] private bool fadeOut = true; // Fade out saat terbang
    
    private bool isEscaping = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // Auto-assign components
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// Dipanggil oleh FlySpawnTrigger untuk membuat lalat kabur
    /// </summary>
    public void Escape()
    {
        if (isEscaping) return; // Prevent multiple calls
        
        isEscaping = true;
        StartCoroutine(FlyUpAndDestroy());
    }
    
    IEnumerator FlyUpAndDestroy()
    {
        Debug.Log($"{gameObject.name} escaping!");
        
        // 1. Play animasi terbang (jika ada)
        if (animator != null && !string.IsNullOrEmpty(flyingAnimationTrigger))
        {
            animator.SetTrigger(flyingAnimationTrigger);
        }
        
        // 2. Play SFX
        if (audioSource != null && escapeSound != null)
        {
            audioSource.PlayOneShot(escapeSound);
        }
        
        // 3. Simpan posisi awal
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * flyUpHeight;
        
        float elapsed = 0f;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        
        // 4. Terbang ke atas dengan fade out
        while (elapsed < flyUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flyUpDuration;
            
            // Gerak ke atas (ease out)
            float smoothT = 1f - (1f - t) * (1f - t); // Ease out quad
            transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            
            // Fade out (optional)
            if (fadeOut && spriteRenderer != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = c;
            }
            
            yield return null;
        }
        
        // 5. Destroy lalat
        Debug.Log($"{gameObject.name} destroyed");
        Destroy(gameObject);
    }
}