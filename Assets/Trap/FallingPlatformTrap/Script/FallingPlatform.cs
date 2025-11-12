using System.Collections;
using UnityEngine;

// Pastikan nama file ini 'FallingLeaf.cs' atau sesuai
public class FallingLeaf : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float triggerDelay = 1f; // Ganti nama dari fallDelay

    [Header("Visuals")]
    [SerializeField] private Sprite triggeredSprite; // <-- Masukkan sprite ke-2 (layu) di sini
    private Sprite originalSprite; // Untuk menyimpan sprite asli

    // --- Komponen ---
    private SpriteRenderer spriteRenderer;
    private Collider2D c2d;
    // Kita tidak perlu Rigidbody2D lagi

    // --- Kontrol State ---
    private bool hasBeenTriggered = false;
    private Coroutine activeCoroutine; // Untuk melacak coroutine

    // === MENDENGARKAN SINYAL RESET DARI GAMEMANAGER ===
    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= ResetTrap;
    }
    // ===============================================

    private void Start()
    {
        // Ambil komponen
        spriteRenderer = GetComponent<SpriteRenderer>();
        c2d = GetComponent<Collider2D>();
        
        // Simpan sprite asli saat game dimulai
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }

        // Panggil ResetTrap() di awal untuk state yang benar
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

    // --- COROUTINE INI SUDAH DIUBAH ---
    private IEnumerator BecomeTrigger()
    {
        hasBeenTriggered = true;

        // (Opsional) Tambahkan efek goyang (shake) di sini

        // 1. UBAH SPRITE
        if (spriteRenderer != null && triggeredSprite != null)
        {
            spriteRenderer.sprite = triggeredSprite;
        }

        // 2. TUNGGU JEDA
        yield return new WaitForSeconds(triggerDelay);

        // 3. JADI TRIGGER (BUKAN JATUH)
        if (c2d != null)
        {
            c2d.isTrigger = true;
        }

        // 4. JANGAN HANCUR
        // (Baris Destroy(gameObject) dihapus)
    }

    // === FUNGSI RESET UNTUK GAMEMANAGER ===
    private void ResetTrap()
    {
        // Hentikan coroutine jika sedang berjalan (misal player mati saat daun goyang)
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        // Kembalikan semua ke kondisi awal
        hasBeenTriggered = false;
        
        // Kembalikan sprite asli
        if (spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        // Kembalikan collider jadi solid
        if (c2d != null)
        {
            c2d.isTrigger = false;
        }
    }
}