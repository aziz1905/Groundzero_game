using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events; // <-- Penting untuk Events

// --- PRINSIP S.O.L.I.D. ---
// Single Responsibility: HANYA mengelola state game (Nyawa, Game Over).
// Open/Closed: Kelas lain bisa "mendengarkan" event-nya tanpa mengubah kode ini.
public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Ini adalah "Instance" global agar script lain (Player, UI)
    // bisa memanggilnya dengan mudah: GameManager.Instance
    public static GameManager Instance { get; private set; }

    [Header("Pengaturan Nyawa")]
    [SerializeField] private int startLives = 3;
    private int currentLives;

    [Header("Pengaturan Scene")]
    [SerializeField] private string sceneGameOver = "GameOverScene"; // Nama Scene UI/Game Over Anda
    [SerializeField] private string sceneMainMenu = "MainMenuScene"; // Nama Scene Main Menu

    // --- Events (S.O.L.I.D. - Dependency Inversion) ---
    // GameManager "memberi sinyal" ke UIManager tanpa perlu tahu UIManager itu ada.
    // Sinyal ini membawa data (int) jumlah nyawa baru.
    public UnityEvent<int> OnLivesChanged;

    private void Awake()
    {
        // --- Setup Singleton ---
        if (Instance != null && Instance != this)
        {
            // Jika sudah ada GameManager lain, hancurkan yang ini.
            Destroy(gameObject);
        }
        else
        {
            // Jika ini satu-satunya, jadikan ini Instance utama.
            Instance = this;
            // Jangan hancur saat pindah scene (opsional, tapi bagus)
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        currentLives = startLives;

        // Kirim sinyal awal ke UI saat game dimulai
        // '?' berarti "jika ada yang mendengarkan"
        OnLivesChanged?.Invoke(currentLives);
    }

    // --- FUNGSI UTAMA ---
    // Ini dipanggil oleh PlayerController saat dia mati
    public void PlayerHasDied()
    {
        currentLives--;

        // 1. Kirim sinyal ke UI bahwa nyawa berkurang
        OnLivesChanged?.Invoke(currentLives);

        if (currentLives > 0)
        {
            // Masih ada nyawa. Biarkan PlayerController yang mengurus respawn
            // di checkpoint (script Anda sudah melakukan ini).
            // Kita tidak perlu me-restart level.
            Debug.Log("Nyawa berkurang, respawn di checkpoint.");
        }
        else
        {
            // Nyawa habis!
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        // Ini adalah permintaan Anda: "memanggil scene ui ketika mati"
        // dan "mengulang kembali level".
        Debug.Log("GAME OVER! Nyawa habis.");

        // Hentikan game
        Time.timeScale = 0f;

        // Panggil Scene UI Game Over secara Aditif (menumpuk)
        // Pastikan "GameOverScene" ada di Build Settings
        SceneManager.LoadScene(sceneGameOver, LoadSceneMode.Additive);

        // Anda bisa memilih:
        // 1. Panggil Scene UI (di atas).
        // 2. ATAU, langsung restart level (di bawah).
        // RestartCurrentLevel();
    }

    // Fungsi ini bisa dipanggil oleh tombol di GameOverScene
    public void RestartCurrentLevel()
    {
        // Kembalikan waktu ke normal dulu!
        Time.timeScale = 1f;
        // Set nyawa kembali ke awal
        currentLives = startLives;
        OnLivesChanged?.Invoke(currentLives);

        // Muat ulang scene yang sedang aktif
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Fungsi ini bisa dipanggil oleh tombol di GameOverScene
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        // Hancurkan GameManager ini agar yang di MainMenu bisa dipakai
        Destroy(gameObject);
        SceneManager.LoadScene(sceneMainMenu);
    }
}