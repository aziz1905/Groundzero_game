using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TANGGUNG JAWAB:
// 1. Hidup di PauseMenuScene
// 2. OTOMATIS menjeda game saat scene di-load (via Start)
// 3. Menyediakan fungsi ResumeGame() untuk dipanggil
public class PauseManager : MonoBehaviour
{
    // Start akan berjalan saat scene ini di-load (LoadSceneAdditive)
    void Start()
    {
        // Ini akan menjeda game (termasuk GameScene)
        Time.timeScale = 0f;
    }

    // Fungsi ini akan dipanggil oleh tombol Resume ATAU PauseOpener
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("PauseMenuScene");
    }
}