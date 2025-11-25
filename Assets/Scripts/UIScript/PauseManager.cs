using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Tambahkan ini jika ingin menghapus highlight tombol saat resume

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel; // <--- BARU: Referensi ke Panel Settings

    [Header("Main Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings UI")]
    [SerializeField] private Button backFromSettingsButton; // <--- BARU: Tombol "Back" di dalam panel setting

    [Header("Pause Button Sprites")]
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Button pauseButtonComponent;
    
    [Header("Pause Button States")]
    [SerializeField] private Sprite pauseNormalSprite;
    [SerializeField] private Sprite pauseHighlightedSprite;
    [SerializeField] private Sprite pausePressedSprite;
    
    [Header("Resume Button States")]
    [SerializeField] private Sprite playNormalSprite;
    [SerializeField] private Sprite playHighlightedSprite;
    [SerializeField] private Sprite playPressedSprite;

    [Header("Game References")]
    [SerializeField] private PlayerController playerController;

    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Pastikan Panel Setting mati saat awal game
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false); // <--- BARU

        // Setup Listeners
        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        // 2. Tambahkan Listener untuk tombol Back
        if (backFromSettingsButton != null) 
            backFromSettingsButton.onClick.AddListener(CloseSettings); // <--- BARU

        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
    }

    void Update()
    {
        // Kosong
    }

    public void TogglePause()
    {
        if (!isPaused) PauseGame();
        else ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false); // <--- BARU: Pastikan setting tertutup
        
        if (playerController != null) playerController.enabled = false;

        SetPauseButtonSprites(playNormalSprite, playHighlightedSprite, playPressedSprite);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false); // <--- BARU: Tutup setting juga jika player menekan resume shortcut (jika ada)

        // Hapus seleksi tombol agar tidak ada tombol yang 'nyangkut' warnanya
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        if (playerController != null) playerController.enabled = true;

        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
    }

    // --- FUNGSI BARU UNTUK SETTINGS ---

    private void OpenSettings()
    {
        // Tutup Pause Panel, Buka Settings Panel
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        // Tutup Settings Panel, Balik ke Pause Panel
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // ----------------------------------

    private void SetPauseButtonSprites(Sprite normal, Sprite highlighted, Sprite pressed)
    {
        if (pauseButtonImage != null && normal != null)
        {
            pauseButtonImage.sprite = normal;
        }

        if (pauseButtonComponent != null && pauseButtonComponent.transition == Selectable.Transition.SpriteSwap)
        {
            SpriteState spriteState = new SpriteState
            {
                highlightedSprite = highlighted,
                pressedSprite = pressed,
                selectedSprite = normal,
                disabledSprite = normal
            };
            pauseButtonComponent.spriteState = spriteState;
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quit button clicked");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnDestroy()
    {
        if (pauseButton != null) pauseButton.onClick.RemoveListener(TogglePause);
        if (resumeButton != null) resumeButton.onClick.RemoveListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
        
        // Bersihkan listener tombol back
        if (backFromSettingsButton != null) 
            backFromSettingsButton.onClick.RemoveListener(CloseSettings); // <--- BARU
    }
}