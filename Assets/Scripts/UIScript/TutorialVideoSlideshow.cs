using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;
using System.IO; // Wajib untuk Path

public class TutorialVideoSlideshow : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string title;        
        
        [Header("Video Settings")]
        public VideoClip videoClip; // Dipakai saat Play di Unity Editor
        [Tooltip("Wajib isi nama file lengkap untuk WebGL! Contoh: GIF1.mp4")]
        public string videoFileName; // Dipakai saat Build WebGL (Contoh: "GIF1.mp4")

        [TextArea(3, 5)] 
        public string description;  
    }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;       
    [SerializeField] private VideoPlayer videoPlayer;         
    [SerializeField] private TextMeshProUGUI descriptionText; 
    
    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Dots")]
    [SerializeField] private Transform dotsContainer;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Data Content")]
    [SerializeField] private TutorialStep[] tutorialSteps; 

    private int currentIndex = 0;
    private List<Image> dotImages = new List<Image>();

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextClick);
        prevButton.onClick.AddListener(OnPrevClick);
        InitializeDots();
        
        // PENTING: Paksa render mode ke API Video yang benar
        videoPlayer.renderMode = VideoRenderMode.RenderTexture; 
        
        ShowStep(0);
    }

    private void OnEnable()
    {
        currentIndex = 0;
        if(tutorialSteps.Length > 0) ShowStep(0);
    }

    private void InitializeDots()
    {
        foreach (Transform child in dotsContainer) Destroy(child.gameObject);
        dotImages.Clear();

        for (int i = 0; i < tutorialSteps.Length; i++)
        {
            GameObject newDot = Instantiate(dotPrefab, dotsContainer);
            dotImages.Add(newDot.GetComponent<Image>());
        }
    }

    public void OnNextClick()
    {
        if (currentIndex < tutorialSteps.Length - 1)
        {
            currentIndex++;
            ShowStep(currentIndex);
        }
    }

    public void OnPrevClick()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowStep(currentIndex);
        }
    }

    // --- BAGIAN INI YANG DIMODIFIKASI BIAR WEBGL JALAN ---
    private void ShowStep(int index)
    {
        if (tutorialSteps.Length == 0) return;

        TutorialStep currentStep = tutorialSteps[index];

        // 1. Update Teks
        titleText.text = currentStep.title;
        descriptionText.text = currentStep.description;

        // 2. LOGIKA VIDEO HIBRIDA (EDITOR vs WEBGL)
        #if UNITY_WEBGL && !UNITY_EDITOR
            // --- LOGIKA WEBGL ---
            // Otomatis mencari file di folder StreamingAssets/Video
            // Pastikan kamu sudah isi 'videoFileName' di Inspector!
            string videoPath = Path.Combine(Application.streamingAssetsPath, "Video", currentStep.videoFileName);
            
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoPath;
        #else
            // --- LOGIKA EDITOR / WINDOWS ---
            // Pakai cara lama biar gampang preview
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = currentStep.videoClip;
        #endif

        videoPlayer.Stop();
        videoPlayer.Play();

        // 3. Update Tombol & Dots
        prevButton.interactable = (index > 0);
        nextButton.interactable = (index < tutorialSteps.Length - 1);

        for (int i = 0; i < dotImages.Count; i++)
        {
            dotImages[i].color = (i == index) ? activeColor : inactiveColor;
        }
    }
}