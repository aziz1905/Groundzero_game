using UnityEngine;
using System.Collections; // <-- PENTING: Tambahkan ini untuk Coroutine

public class FakeCoinPlatformDestroy : MonoBehaviour
{
    [SerializeField] private GameObject Coin;
    [SerializeField] private GameObject Platform;
    
    // --- TAMBAHAN BARU ---
    [Tooltip("Jeda (detik) antara koin hilang & platform jatuh")]
    [SerializeField] private float platformFallDelay = 0.5f; 
    // ---------------------

    private bool hasBeenTriggered = false;
    private Collider2D trapCollider;
    
    // --- TAMBAHAN BARU ---
    private Coroutine activeCoroutine; // Untuk melacak coroutine
    // ---------------------

    // === Sistem Reset (Tetap Sama) ===
    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= ResetTrap;
    }
    // =================================

    private void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        ResetTrap(); 
    }

    // === OnTriggerEnter2D (Sekarang Memanggil Coroutine) ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek player & pastikan belum aktif
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; 
            
            if (trapCollider != null)
                trapCollider.enabled = false; 

            // Jangan langsung jatuhkan platform, panggil Coroutine
            activeCoroutine = StartCoroutine(ActivateTrapSequence());
        }
    }

    // === COROUTINE BARU (Jantung Logika) ===
    private IEnumerator ActivateTrapSequence()
    {
        // 1. Koin HILANG DULUAN (seolah diambil)
        if (Coin != null)
            Coin.SetActive(false); 
        
        // (Anda bisa tambahkan suara koin di sini)
        
        // 2. TUNGGU SEBENTAR
        yield return new WaitForSeconds(platformFallDelay);

        // 3. Platform HILANG KEMUDIAN
        Debug.Log("✅ Platform hilang!");
        if (Platform != null)
            Platform.SetActive(false);
            
        activeCoroutine = null;
    }
    // ======================================

    // === ResetTrap (Dimodifikasi untuk Coroutine) ===
    private void ResetTrap()
    {
        // 1. HENTIKAN COROUTINE (jika player mati saat sedang delay)
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        
        // 2. Kembalikan status
        hasBeenTriggered = false;

        // 3. Nyalakan lagi collider trigger
        if (trapCollider != null)
            trapCollider.enabled = true; 

        // 4. Munculkan lagi platform
        if (Platform != null)
            Platform.SetActive(true);

        // 5. Munculkan lagi koin
        if (Coin != null)
            Coin.SetActive(true);
    }
    // ===============================================
}