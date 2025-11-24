using System.Collections;
using UnityEngine;

public class TriggerSpikeTrap : MonoBehaviour
{
    [SerializeField] private GameObject spikes; 
    [SerializeField] private float activationDelay = 0.5f; 

    private bool hasBeenTriggered = false;
    
    // === TAMBAHAN UNTUK RESET ===
    private Collider2D trapCollider; // Variabel untuk collider trigger
    private Coroutine activeCoroutine; // Variabel untuk melacak coroutine
    // =============================

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
        // Ambil komponen collider
        trapCollider = GetComponent<Collider2D>();
        
        // Atur ke kondisi awal saat game dimulai
        ResetTrap(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek player & pastikan belum aktif
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            trapCollider.enabled = false;

            // Mulai animasi trap
            activeCoroutine = StartCoroutine(ShowSpikesAfterDelay());

            // === PANGGIL FUNGSI MATI PLAYER ===
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.DieAndRespawn();
            }
        }
    }

    private IEnumerator ShowSpikesAfterDelay()
    {
        // Tunggu sesuai waktu delay
        yield return new WaitForSeconds(activationDelay);

        // Aktifkan objek duri
        if (spikes != null)
        {
            spikes.SetActive(true);
        }

        // --- HAPUS 'Destroy(gameObject);' ---
        // Destroy(gameObject); // <-- JANGAN LAKUKAN INI
        // ------------------------------------
    }

    // === TAMBAHAN: FUNGSI RESET UTAMA ===
    // Fungsi ini dipanggil oleh GameManager SAAT PLAYER MATI
    private void ResetTrap()
    {
        // 1. Hentikan coroutine 'ShowSpikes' (jika sedang berjalan)
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

        // 3. Kembalikan duri ke kondisi awal
        if (spikes != null)
        {
            spikes.SetActive(false); // Sembunyikan lagi duri
        }
    }
    // ====================================
}