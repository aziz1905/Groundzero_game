using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // <-- WAJIB untuk pindah scene



public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Settings")]
    [Header("Audio Settings")]
    [Header("General UI")]
    public AudioMixer mainMixer;
    public GameObject creditsPanel;
    public GameObject optionsPanel;
    public GameObject dimBackground;
    public Slider musicSlider;

    public GameObject infoPanel;          // Referensi ke Panel Hitam (Wadah)
    public TextMeshProUGUI titleText;

    // 1. Fungsi untuk Tombol START
    //    Pastikan "NamaSceneGameAnda" sesuai nama file scene game Anda.
    public void StartGame()
    {
        // Ganti "Level_1" dengan nama scene game Anda
        SceneManager.LoadScene("Design-Level");

        float initialVolume = 1.0f;

        // 1. Atur UI Slider-nya secara manual ke nilai itu
        if (musicSlider != null)
        {
            musicSlider.value = initialVolume;
        }

        // 2. Panggil fungsi SetMusicVolume untuk
        //    mengatur Audio Mixer-nya ke nilai itu juga
        SetMusicVolume(initialVolume);
    }
    // FUNGSI Buka Panel & Set Judul
    // Fungsi ini menerima 'teks' yang dikirim dari tombol
    public void OpenInfoPanel(string judul)
    {
        infoPanel.SetActive(true);      // 1. Nyalakan Panel
        titleText.text = judul;         // 2. Ubah Teks Judul sesuai parameter
    }

    // FUNGSI 2: Tutup Panel
    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);     // Matikan Panel
    }
    // 3. Fungsi untuk Tombol CREDITS

    public void ToggleFullscreen()
    {
        // Baris ini adalah kuncinya.
        // Screen.fullScreen = !Screen.fullScreen;
        // Ini membaca status fullscreen saat ini (true/false)
        // dan mengaturnya ke nilai KEBALIKANNYA.

        // Versi yang lebih mudah dibaca:
        if (Screen.fullScreen == true)
        {
            // Jika sedang fullscreen, buat jadi windowed
            Screen.fullScreen = false;
        }
        else
        {
            // Jika sedang windowed, buat jadi fullscreen
            Screen.fullScreen = true;
        }
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

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!"); // Pesan ini untuk testing di Editor
        Application.Quit(); // Ini hanya berfungsi di game yang sudah di-build
    }
}