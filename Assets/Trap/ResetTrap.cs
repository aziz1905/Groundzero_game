using System.Collections;
using UnityEngine;

// 'abstract' berarti skrip ini tidak bisa dipakai langsung,
// tapi HARUS dijadikan 'induk' oleh skrip lain.
[RequireComponent(typeof(Collider2D))]
public abstract class ResetTraps : MonoBehaviour
{
    [Header("Pengaturan Dasar Trap")]
    [SerializeField] private float activationDelay = 0f;

    // --- Variabel 'shared' untuk semua trap ---
    private bool hasBeenTriggered = false;
    private Collider2D trapCollider;
    private Coroutine activeCoroutine;

    // --- LOGIKA RESET (Sudah beres, diurus Induk) ---
    protected virtual void OnEnable()
    {
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnPlayerRespawn -= ResetTrap;
    }
    // ------------------------------------------------

    // --- LOGIKA TRIGGER (Sudah beres, diurus Induk) ---
    protected virtual void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        // Panggil ResetTrapLogic() yang spesifik di awal
        ResetTrapLogic(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            if (trapCollider != null) trapCollider.enabled = false;
            
            // Panggil 'ActivateTrapLogic()' yang spesifik
            activeCoroutine = StartCoroutine(ActivateTrapCoroutine());
        }
    }
    // ------------------------------------------------

    // --- "Bagian Kosong" (Harus diisi oleh Anak) ---

    // 'abstract' memaksa skrip 'Anak' untuk
    // mengisi apa yang terjadi saat AKTIF
    protected abstract IEnumerator ActivateTrapLogic(); 

    // 'abstract' memaksa skrip 'Anak' untuk
    // mengisi apa yang terjadi saat RESET
    protected abstract void ResetTrapLogic();

    // ------------------------------------------------
    
    // --- Sistem Internal (Dilarang Diubah) ---
    private IEnumerator ActivateTrapCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        yield return StartCoroutine(ActivateTrapLogic());
        activeCoroutine = null;
    }

    // Fungsi reset publik yang dipanggil GameManager
    public void ResetTrap() 
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        hasBeenTriggered = false;
        if (trapCollider != null) trapCollider.enabled = true;
        
        // Panggil 'ResetTrapLogic()' yang spesifik
        ResetTrapLogic();
    }
}