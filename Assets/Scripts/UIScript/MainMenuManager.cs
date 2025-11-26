using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject infoPanel;          // Panel universal untuk semua konten
    public TextMeshProUGUI titleText;     // Title panel
    public TextMeshProUGUI contentText;   // ✅ TAMBAHAN: Text untuk isi konten
    
    [Header("Panel Content Containers")]
    public GameObject howToPlayContent;   // ✅ Container untuk How To Play
    public GameObject creditsContent;     // ✅ Container untuk Credits
    public GameObject settingsContent;    // ✅ Container untuk Settings
    
    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    
    [Header("General UI")]
    public GameObject dimBackground;
    
    [SerializeField] private Button quitButton;

    void Start()
    {
        #if UNITY_WEBGL
            if (quitButton != null) 
                quitButton.gameObject.SetActive(false);
        #endif
        
        // ✅ Matikan semua panel konten di awal
        CloseAllPanelContents();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Design-Level");
        
        float initialVolume = 1.0f;
        if (musicSlider != null)
        {
            musicSlider.value = initialVolume;
        }
        SetMusicVolume(initialVolume);
    }

    // ✅ FUNGSI BARU: Buka How To Play
    public void OpenHowToPlay()
    {
        CloseAllPanelContents();
        
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InfoPanel belum di-assign di Inspector!");
            return;
        }
        
        if (titleText != null)
        {
            titleText.text = "How To Play";
        }
        else
        {
            Debug.LogWarning("TitleText belum di-assign di Inspector!");
        }
        
        if (howToPlayContent != null)
        {
            howToPlayContent.SetActive(true);
        }
        else
        {
            Debug.LogWarning("HowToPlayContent belum di-assign di Inspector!");
        }
    }

    // ✅ FUNGSI BARU: Buka Credits
    public void OpenCredits()
    {
        CloseAllPanelContents();
        
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InfoPanel belum di-assign di Inspector!");
            return;
        }
        
        if (titleText != null)
        {
            titleText.text = "Credits";
        }
        
        if (creditsContent != null)
        {
            creditsContent.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CreditsContent belum di-assign di Inspector!");
        }
    }

    // ✅ FUNGSI BARU: Buka Settings
    public void OpenSettings()
    {
        CloseAllPanelContents();
        
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InfoPanel belum di-assign di Inspector!");
            return;
        }
        
        if (titleText != null)
        {
            titleText.text = "Settings";
        }
        
        if (settingsContent != null)
        {
            settingsContent.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SettingsContent belum di-assign di Inspector!");
        }
    }

    // ✅ FUNGSI HELPER: Matikan semua konten panel
    private void CloseAllPanelContents()
    {
        if (howToPlayContent != null)
            howToPlayContent.SetActive(false);
            
        if (creditsContent != null)
            creditsContent.SetActive(false);
            
        if (settingsContent != null)
            settingsContent.SetActive(false);
    }

    // ✅ Tutup Panel
    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
        CloseAllPanelContents();
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    [Header("WebGL Settings")]
    public GameObject thankYouPanel; // ✅ Drag panel "Thanks for Playing" ke sini
    
    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        
        #if UNITY_WEBGL && !UNITY_EDITOR
            // Untuk WebGL, tampilkan panel Thank You
            if (thankYouPanel != null)
            {
                thankYouPanel.SetActive(true);
            }
            else
            {
                // Fallback: Redirect ke game page
                Application.OpenURL("https://yourusername.itch.io/yourgame");
            }
        #else
            // Untuk Windows/Mac/Linux build
            Application.Quit();
            
            #if UNITY_EDITOR
                // Untuk testing di Unity Editor
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        #endif
    }
}