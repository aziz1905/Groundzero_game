using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject infoPanel;          // Panel universal pembungkus
    
    [Header("Panel Content Containers")]
    public GameObject howToPlayContent;   // Container untuk How To Play
    public GameObject creditsContent;     // Container untuk Credits
    public GameObject settingsContent;    // Container untuk Settings
    
    [Header("Audio Settings")]
    public AudioMixer mainMixer;          // WAJIB DRAG AUDIOMIXER
    public Slider musicSlider;            // Slider di Panel Setting
    
    [Header("General UI")]
    public GameObject dimBackground;      // Background gelap (opsional)
    [SerializeField] private Button quitButton;

    void Start()
    {
        // 1. Matikan tombol Quit di WebGL (karena gak guna)
        #if UNITY_WEBGL
            if (quitButton != null) 
                quitButton.gameObject.SetActive(false);
        #endif
        
        // 2. Matikan semua panel konten di awal
        CloseInfoPanel(); // Sekaligus matiin infoPanel dan isinya

        // 3. --- FIX VOLUME STARTUP ---
        // Load volume terakhir player, atau default ke Full
        if (PlayerPrefs.HasKey("MasterVolume")) 
        {
            float savedVol = PlayerPrefs.GetFloat("MasterVolume");
            
            // Set Slider UI biar sinkron sama data tersimpan
            if (musicSlider != null)
                musicSlider.value = savedVol;

            // Set Volume Mixer (Convert 0-1 ke Decibel)
            SetMusicVolume(savedVol);
        }
        else
        {
            // Belum ada save-an (User Baru), set ke 100%
            if (musicSlider != null) musicSlider.value = 1f;
            SetMusicVolume(1f);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Design-Level");
    }

    // --- FUNGSI PANEL ---

    public void OpenHowToPlay()
    {
        OpenPanel(howToPlayContent);
    }

    public void OpenCredits()
    {
        OpenPanel(creditsContent);
    }

    public void OpenSettings()
    {
        OpenPanel(settingsContent);
    }

    // Helper biar kodenya gak duplikat terus
    private void OpenPanel(GameObject contentToOpen)
    {
        CloseAllPanelContents(); // Matiin yg lain dulu

        if (infoPanel != null)
            infoPanel.SetActive(true); // Nyalain bungkusnya
        
        if (contentToOpen != null)
            contentToOpen.SetActive(true); // Nyalain isinya
        else
            Debug.LogWarning("Content Panel belum di-assign di Inspector!");
    }

    public void CloseInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
            
        CloseAllPanelContents();
    }

    private void CloseAllPanelContents()
    {
        if (howToPlayContent != null) howToPlayContent.SetActive(false);
        if (creditsContent != null) creditsContent.SetActive(false);
        if (settingsContent != null) settingsContent.SetActive(false);
    }

    // --- FUNGSI AUDIO & SETTING ---

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    // Dipanggil oleh Slider di OnValueChanged
    public void SetMusicVolume(float volume)
    {
        // Rumus Logaritma: Slider 0.0001 - 1 -> -80dB - 0dB
        // Mencegah error log(0) = minus infinity
        float volumeToDb = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        
        mainMixer.SetFloat("MasterVolume", volumeToDb); // Pastikan nama param di Mixer "MasterVolume"

        // Simpan biar diingat pas restart
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    // --- QUIT GAME ---

    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        
        #if UNITY_WEBGL
        #else
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        #endif
    }
}