using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float triggerDelay = 1f;
    [SerializeField] private float resetDelay = 2f; // Waktu tunggu sebelum reset otomatis 

    [Header("Visuals")]
    [SerializeField] private Sprite triggeredSprite; 
    [SerializeField] [Range(0f, 1f)] private float triggeredOpacity = 0.7f; 
    private Sprite originalSprite; 
    private Color originalColor; 

    // --- Komponen ---
    private SpriteRenderer spriteRenderer;
    private Collider2D c2d;

    // --- Kontrol State ---
    private bool hasBeenTriggered = false;
    private Coroutine activeCoroutine; 

    // === MENDENGARKAN SINYAL RESET DARI GAMEMANAGER ===
    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= ResetTrap;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        c2d = GetComponent<Collider2D>();
        
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
            originalColor = spriteRenderer.color;
        }

        ResetTrap();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            // Cek jika player mendarat di atas
            if (collision.contacts[0].normal.y < -0.5) 
            {
                // Mulai coroutine dan simpan
                activeCoroutine = StartCoroutine(BecomeTrigger());
            }
        }
    }

    private IEnumerator BecomeTrigger()
    {
        hasBeenTriggered = true;

        // 1. TUNGGU JEDA DULU
        yield return new WaitForSeconds(triggerDelay);

        // 2. UBAH SPRITE DAN KURANGI OPACITY
        if (spriteRenderer != null && triggeredSprite != null)
        {
            spriteRenderer.sprite = triggeredSprite;
            Color newColor = spriteRenderer.color;
            newColor.a = triggeredOpacity;
            spriteRenderer.color = newColor;
        }

        // 3. JADI TRIGGER 
        if (c2d != null)
        {
            c2d.isTrigger = true;
        }

        // 4. TUNGGU SEBELUM RESET OTOMATIS
        yield return new WaitForSeconds(resetDelay);

        // 5. RESET KEMBALI KE KONDISI AWAL
        ResetTrap();
    }

    // === FUNGSI RESET UNTUK GAMEMANAGER ===
    private void ResetTrap()
    {
        // Hentikan coroutine jika sedang berjalan (misal player mati saat daun goyang atau reset otomatis)
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = null;

        // Kembalikan semua ke kondisi awal
        hasBeenTriggered = false;
        
        // Kembalikan sprite asli dan warna/opacity asli
        if (spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.color = originalColor;
        }

        // Kembalikan collider jadi solid
        if (c2d != null)
        {
            c2d.isTrigger = false;
        }
    }
}