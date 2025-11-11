using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // <-- WAJIB untuk pindah scene


public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Settings")]
    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public GameObject creditsPanel;
    public GameObject optionsPanel;
    public GameObject dimBackground;
    // 1. Fungsi untuk Tombol START
    //    Pastikan "NamaSceneGameAnda" sesuai nama file scene game Anda.
    public void StartGame()
    {
        // Ganti "Level_1" dengan nama scene game Anda
        SceneManager.LoadScene("Level_1");
    }
    // 3. Fungsi untuk Tombol CREDITS
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        dimBackground.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        dimBackground.SetActive(false);
    }
    // 4. Fungsi untuk Tombol OPTIONS
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        dimBackground.SetActive(true); // Kita pakai dimmer yang sama
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        dimBackground.SetActive(false);
    }

    // 5. Fungsi untuk SLIDER MUSIK
    // Fungsi ini akan dipanggil SETIAP KALI slider digerakkan
    public void SetMusicVolume(float volume)
    {
        // 'volume' adalah nilai dari slider (0.0001 sampai 1)
        // 'MusicVolume' adalah nama parameter yang kita buat di Mixer
        // Rumus Log10*20 ini mengubah nilai linear (1,2,3)
        // menjadi desibel (dB), cara telinga mendengar.
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    // 2. Fungsi untuk Tombol QUIT
    public void QuitGame()
    {
        Debug.Log("QUIT GAME!"); // Pesan ini untuk testing di Editor
        Application.Quit(); // Ini hanya berfungsi di game yang sudah di-build
    }
}