using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Pause Button Sprites")]
    [SerializeField] private Image pauseButtonImage;
    [SerializeField] private Button pauseButtonComponent;
    
    [Header("Pause Button States")]
    [SerializeField] private Sprite pauseNormalSprite;
    [SerializeField] private Sprite pauseHighlightedSprite;
    [SerializeField] private Sprite pausePressedSprite;
    
    [Header("Resume Button States (saat paused)")]
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
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
    }

    // Fungsi Update KOSONG: Memastikan tidak ada deteksi keyboard di sini.
    void Update()
    {
        // Semua fungsi jeda/lanjut HANYA via klik tombol UI.
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; 
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        // Mematikan script PlayerController.
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        SetPauseButtonSprites(playNormalSprite, playHighlightedSprite, playPressedSprite);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // TAMBAHAN: Hapus objek yang saat ini terpilih oleh EventSystem
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }

        // Menghidupkan kembali script PlayerController.
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        SetPauseButtonSprites(pauseNormalSprite, pauseHighlightedSprite, pausePressedSprite);
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

    private void OpenSettings()
    {
        Debug.Log("Settings button clicked - belum diimplementasi");
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
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(TogglePause);
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OpenSettings);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }
}