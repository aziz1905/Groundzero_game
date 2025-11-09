using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseOpener : MonoBehaviour
{
    // Kita kembalikan ini, ini cara yang benar
    [SerializeField] private string pauseMenuSceneName = "PauseMenuScene";

    public void OpenPauseMenu()
    {
        // Hapus semua Debug.Log, kita sudah tidak membutuhkannya
        if (Time.timeScale == 1f)
        {
            // Kita pakai lagi variabelnya
            SceneManager.LoadScene(pauseMenuSceneName, LoadSceneMode.Additive);
        }
    }
}