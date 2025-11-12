using System.Collections;
using UnityEngine;

public class FallingRockTrap : MonoBehaviour
{
    [SerializeField] private GameObject rockObject; // Objek Duri/Batu
    [SerializeField] private float activeTime = 5f; // Waktu sebelum duri/batu otomatis reset
    
    private bool hasBeenTriggered = false;
    private Collider2D trapCollider;

    // --- Variabel baru untuk Reset ---
    private Rigidbody2D rockRb;
    private Vector3 originalRockPosition;
    private Coroutine activeCoroutine; // Untuk melacak coroutine
    // ---------------------------------

    // === TAMBAHAN: DENGARKAN GAME MANAGER ===
    private void OnEnable()
    {
        // Daftarkan 'ResetTrap' ke sinyal GameManager
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    private void OnDisable()
    {
        // Berhenti langganan sinyal (penting)
        GameManager.OnPlayerRespawn -= ResetTrap;
    }
    // =======================================

    private void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        
        // Simpan info awal dari duri/batu
        if (rockObject != null)
        {
            rockRb = rockObject.GetComponent<Rigidbody2D>();
            originalRockPosition = rockObject.transform.position; 
        }
        
        // Atur ke kondisi awal saat game dimulai
        ResetTrap(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek player & pastikan belum aktif
        if (collision.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            trapCollider.enabled = false; // Matikan trigger

            // Aktifkan batu/duri
            if (rockObject != null)
            {
                rockObject.SetActive(true);
                if (rockRb != null)
                {
                    rockRb.isKinematic = false; // Jatuhkan
                }
            }
            
            // Mulai coroutine (bukan untuk hancur, tapi untuk reset otomatis)
            activeCoroutine = StartCoroutine(ResetObjectAfterDelay());
        }
    }

    // --- FUNGSI DIUBAH TOTAL ---
    // Coroutine ini sekarang menyembunyikan, bukan menghancurkan
    private IEnumerator ResetObjectAfterDelay()
    {
        yield return new WaitForSeconds(activeTime);

        // Sembunyikan batu/duri
        if (rockObject != null)
        {
            rockObject.SetActive(false);
            if (rockRb != null)
            {
                rockRb.isKinematic = true; // Hentikan fisika
                rockRb.velocity = Vector2.zero; // Reset kecepatan
            }
            rockObject.transform.position = originalRockPosition; // Kembalikan ke posisi awal
        }
    }
    // ----------------------------

    // === FUNGSI RESET UTAMA ===
    // Fungsi ini dipanggil oleh GameManager SAAT PLAYER MATI
    private void ResetTrap()
    {
        // 1. Hentikan coroutine 'ResetObjectAfterDelay' (jika sedang berjalan)
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        // 2. Kembalikan semua variabel ke kondisi awal
        hasBeenTriggered = false;
        
        if (trapCollider != null)
        {
            trapCollider.enabled = true; // Nyalakan lagi collider trigger
        }

        // 3. Kembalikan batu/duri ke kondisi awal
        if (rockObject != null)
        {
            rockObject.SetActive(false); // Sembunyikan lagi
            if (rockRb != null)
            {
                rockRb.isKinematic = true; // Matikan gravitasi
                rockRb.velocity = Vector2.zero;
            }
            // Kembalikan ke posisi awal
            rockObject.transform.position = originalRockPosition; 
        }
    }
    // ============================
}