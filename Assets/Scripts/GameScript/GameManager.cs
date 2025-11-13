using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events; // <-- PENTING: Tambahkan ini

public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static GameManager Instance { get; private set; }
    // ------------------------

    // --- Event untuk Reset Trap ---
    // (Biarkan ini sebagai 'Action' static, ini sudah benar untuk trap)
    public static event Action OnPlayerRespawn;

    // === INI PERUBAHANNYA ===
    // 'UIManager' Anda mencari UnityEvent, bukan C# Action.

    // 1. Hapus baris lama:
    // public static event Action<int> OnLivesChanged; 

    // 2. Tambahkan ini:
    // Kita buat 'class' baru agar UnityEvent bisa kirim 'int'
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Header("UI Events")]
    // 3. Ini adalah event yang dicari UIManager (non-static, tapi ada di Instance)
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
        // Kita pindahkan panggilannya ke sini, 
        // agar UI Manager yang 'Start'-nya belakangan tetap dapat
        StartCoroutine(SendInitialLivesUpdate());
    }

    private IEnumerator SendInitialLivesUpdate()
    {
        // Tunggu 1 frame agar semua UIManager 'Start' dan 'AddListener' dulu
        yield return null;

        // Beri tahu UI di awal game berapa nyawanya
        OnLivesChanged.Invoke(currentLives);
    }

    // Fungsi ini dipanggil oleh PlayerController.DieAndRespawn()
    public void StartDeathSequence()
    {
        if (deathScreenUI.isFading) return;

        currentLives--;
        deathCount++;

        // Panggil event (sekarang pakai 'OnLivesChanged.Invoke')
        OnLivesChanged.Invoke(currentLives);

        if (currentLives <= 0)
        {
            Debug.Log("GAME OVER");
            StartCoroutine(DeathSequenceCoroutine()); // Respawn saja untuk tes
        }
        else
        {
            StartCoroutine(DeathSequenceCoroutine());
        }
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        deathScreenUI.ShowScreen(deathCount, timeToBlack);
        yield return new WaitForSeconds(timeToBlack);

        player.RespawnAtCheckpoint();
        OnPlayerRespawn?.Invoke();

        yield return new WaitForSeconds(timeBlack);

        deathScreenUI.HideScreen(timeToClear);
    }

    // --- TAMBAHAN BARU ---
    // (Agar UIManager Anda bisa mengambil nyawa awal jika perlu)
    public int GetCurrentLives()
    {
        return currentLives;
    }
}