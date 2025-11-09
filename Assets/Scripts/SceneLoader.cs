using UnityEngine;
using UnityEngine.SceneManagement;

// Tanggung Jawab: Hanya memuat scene baru.
public class SceneLoader : MonoBehaviour
{
    // Fungsi 'public' agar bisa dipanggil oleh 'HomeButton'
    public void LoadMainMenu()
    {
        // PENTING! Selalu kembalikan waktu ke 1
        // sebelum pindah scene dari menu pause.
        Time.timeScale = 1f;

        // Ganti "MainMenuScene" dengan nama scene
        // yang sedang dibuat teman Anda.
        SceneManager.LoadScene("MainMenuScene");
    }
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;

        // 2. Dapatkan scene yang sedang aktif SAAT INI
        //    (Ini akan menjadi "Level_1" atau "Level_2", BUKAN "PauseMenuScene")
        Scene currentActiveScene = SceneManager.GetActiveScene();

        // 3. Muat ulang scene tersebut menggunakan namanya
        SceneManager.LoadScene(currentActiveScene.name);
    }
}