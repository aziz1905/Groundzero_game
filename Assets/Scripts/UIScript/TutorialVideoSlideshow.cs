using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; // Wajib untuk Teks
using System.Collections.Generic;

public class TutorialVideoSlideshow : MonoBehaviour
{
    // KELAS DATA BARU (Paket Video + Teks)
    [System.Serializable]
    public class TutorialStep
    {
        public string title;        // Judul (Merah)
        public VideoClip videoClip; // Video (Kuning)
        [TextArea(3, 5)] 
        public string description;  // Penjelasan (Hijau)
    }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;       // UI Judul
    [SerializeField] private VideoPlayer videoPlayer;         // Mesin Video
    [SerializeField] private TextMeshProUGUI descriptionText; // UI Deskripsi
    
    [Header("Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Dots")]
    [SerializeField] private Transform dotsContainer;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Data Content")]
    // Ganti array lama jadi array class baru
    [SerializeField] private TutorialStep[] tutorialSteps; 

    private int currentIndex = 0;
    private List<Image> dotImages = new List<Image>();

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextClick);
        prevButton.onClick.AddListener(OnPrevClick);
        InitializeDots();
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

    // FUNGSI UTAMA GANTI KONTEN
    private void ShowStep(int index)
    {
        if (tutorialSteps.Length == 0) return;

        // 1. Ambil Data
        TutorialStep currentStep = tutorialSteps[index];

        // 2. Update UI Teks
        titleText.text = currentStep.title;
        descriptionText.text = currentStep.description;

        // 3. Update Video
        videoPlayer.clip = currentStep.videoClip;
        videoPlayer.Stop();
        videoPlayer.Play();

        // 4. Update Tombol
        prevButton.interactable = (index > 0);
        nextButton.interactable = (index < tutorialSteps.Length - 1);

        // 5. Update Dots
        for (int i = 0; i < dotImages.Count; i++)
        {
            dotImages[i].color = (i == index) ? activeColor : inactiveColor;
        }
    }
}