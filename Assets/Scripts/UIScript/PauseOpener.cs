using UnityEngine;
using UnityEngine.SceneManagement;

// TANGGUNG JAWAB:
// 1. Hidup di GameScene
// 2. Mendeteksi input 'Escape'
// 3. Membuka PauseMenuScene (LoadSceneAdditive)
// 4. Menutup PauseMenuScene (memanggil ResumeGame() milik PauseManager)
public class PauseOpener : MonoBehaviour
{
    [SerializeField] private string pauseMenuSceneName = "PauseMenuScene";

    // Variabel untuk melacak status
    // (Kita asumsikan game tidak dijeda di awal)
    private bool isPaused = false;

    void Update()
    {
        // Mendeteksi tombol Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // Jika sudah dijeda, panggil fungsi Resume
                ResumeGame();
            }
            else
            {
                // Jika belum dijeda, panggil fungsi Open
                OpenPauseMenu();
            }
        }
    }

    // Ini adalah fungsi BUKA dari script Anda
    public void OpenPauseMenu()
    {
        // Hanya buka jika game sedang berjalan
        if (!isPaused && Time.timeScale > 0f)
        {
            isPaused = true;
            // Memuat scene menu di atas scene game
            SceneManager.LoadScene(pauseMenuSceneName, LoadSceneMode.Additive);

            // PauseManager.cs di scene itu akan otomatis
            // mengatur Time.timeScale = 0f via Start()
        }
    }

    // Ini adalah fungsi TUTUP
    public void ResumeGame()
    {
        isPaused = false;

        // Kita harus menemukan PauseManager yang ada di PauseMenuScene
        // dan menyuruhnya menjalankan fungsi ResumeGame()
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            // Panggil fungsi ResumeGame() di script itu
            pauseManager.ResumeGame();
        }
        else
        {
            // Jika gagal ketemu (jarang terjadi), lakukan manual
            Debug.LogWarning("Tidak menemukan PauseManager. Resume manual.");
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(pauseMenuSceneName);
        }
    }
}