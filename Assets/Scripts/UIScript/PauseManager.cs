using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings UI")]
    [SerializeField] private Button backFromSettingsButton;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pastikan semua panel mati saat awal game
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Setup Listeners
        if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseButtonClicked);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(CloseSettings);

        // Set sprite awal ke Pause
        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
    }

    void Update()
    {
        // Shortcut ESC untuk pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else if (settingsPanel != null && settingsPanel.activeSelf)
            {
                // Kalau settings panel terbuka, ESC = back to pause menu
                CloseSettings();
            }
            else
            {
                // Kalau pause panel terbuka, ESC = resume
                ResumeGame();
            }
        }
    }

    // Handle klik tombol pause (kanan atas)
    private void OnPauseButtonClicked()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            // Kalau udah pause, tombol ini jadi resume
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        if (playerController != null) playerController.enabled = false;

        // Ganti ke sprite Play (Resume)
        SetPauseButtonSprites(playNormalSprite, playHighlightedSprite, playPressedSprite);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Hapus seleksi tombol agar tidak ada tombol yang 'nyangkut' warnanya
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        if (playerController != null) playerController.enabled = true;

        // Kembalikan ke sprite Pause
        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
    }

    // Buka Settings Panel
    private void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // Tutup Settings Panel, kembali ke Pause Panel
    private void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

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
        // Cleanup listeners
        if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        if (resumeButton != null) resumeButton.onClick.RemoveListener(ResumeGame);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitGame);
        if (backFromSettingsButton != null) backFromSettingsButton.onClick.RemoveListener(CloseSettings);
    }
}