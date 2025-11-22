using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance { get; private set; }
    // ------------------------

    // --- Event untuk Reset Trap ---
    public static event Action OnPlayerRespawn;

    // === UnityEvent untuk Lives ===
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Header("UI Events")]
    public IntEvent OnLivesChanged;
    // ------------------------------------

    [Header("Referensi (Drag dari Hierarchy)")]
    [SerializeField] private PlayerController player;
    [SerializeField] private DeathScreenUI deathScreenUI;

    [Header("Pengaturan Game")]
    [SerializeField] private int totalLives = 3;

    [Header("Pengaturan Respawn")]
    [SerializeField] private float timeToBlack = 0.5f;
    [SerializeField] private float timeBlack = 1.0f;
    [SerializeField] private float timeToClear = 0.5f;

    private int deathCount = 0;
    private int currentLives;

    private void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Setup nyawa di awal
        currentLives = totalLives;

        // Inisialisasi event jika belum (penting)
        if (OnLivesChanged == null)
            OnLivesChanged = new IntEvent();
    }

    private void Start()
    {
        StartCoroutine(SendInitialLivesUpdate());
    }

    private IEnumerator SendInitialLivesUpdate()
    {
        // Tunggu 1 frame agar semua UIManager 'Start' dan 'AddListener' dulu
        yield return null;

        // Beri tahu UI di awal game berapa nyawanya
        OnLivesChanged.Invoke(currentLives);
    }

    // === UPDATED FUNCTION ===
    // Fungsi ini dipanggil oleh PlayerController.DeathAnimationSequence()
    // SETELAH animasi mati selesai (player sudah jatuh keluar screen)
    public void StartDeathSequence()
    {
        if (deathScreenUI.isFading) return;

        currentLives--;
        deathCount++;

        // Panggil event untuk update UI lives
        OnLivesChanged.Invoke(currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("GAME OVER");
            // TODO: Tambahkan Game Over screen nanti
            StartCoroutine(DeathSequenceCoroutine()); // Respawn dulu untuk testing
        }
        else
        {
            StartCoroutine(DeathSequenceCoroutine());
        }
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        // 1. Show death screen (fade to black)
        deathScreenUI.ShowScreen(deathCount, timeToBlack);
        yield return new WaitForSeconds(timeToBlack);

        // 2. Respawn player (di tengah layar hitam)
        player.RespawnAtCheckpoint();
        OnPlayerRespawn?.Invoke(); // Reset traps
        
        Debug.Log("Player respawned, waiting before clearing screen...");

        // 3. Wait saat layar hitam (biar smooth)
        yield return new WaitForSeconds(timeBlack);

        // 4. Fade out death screen (clear to game)
        deathScreenUI.HideScreen(timeToClear);
        
        Debug.Log("Death sequence complete!");
    }

    // --- TAMBAHAN ---
    public int GetCurrentLives()
    {
        return currentLives;
    }
    
    public int GetDeathCount()
    {
        return deathCount;
    }
}