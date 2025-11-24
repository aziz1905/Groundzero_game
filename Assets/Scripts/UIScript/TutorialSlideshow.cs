using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideshow : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image displayImage;
    [SerializeField] private Image displayImage1;
    [SerializeField] private Image displayImage2;
    [SerializeField] private Image displayImage3; // Tempat gambar muncul
    [SerializeField] private Button nextButton;  // Tombol Maju
    [SerializeField] private Button prevButton;  // Tombol Mundur

    [Header("Content Data")]
    // Tarik semua gambar tutorial kamu ke sini nanti
    [SerializeField] private Sprite[] tutorialSlides; 

    private int currentIndex = 0;

    private void Start()
    {
        // Pasang fungsi ke tombol secara otomatis via kode
        // (Jadi kamu gak perlu setting OnClick manual di Inspector)
        nextButton.onClick.AddListener(OnNextClick);
        prevButton.onClick.AddListener(OnPrevClick);

        // Update tampilan awal
        UpdateSlideUI();
    }

    // Fungsi ini jalan setiap kali objek ini Nyala (misal baru dibuka dari menu)
    private void OnEnable()
    {
        currentIndex = 0; // Reset ke halaman 1
        UpdateSlideUI();
    }

    public void OnNextClick()
    {
        // Cek apakah masih ada halaman selanjutnya?
        if (currentIndex < tutorialSlides.Length - 1)
        {
            currentIndex++;
            UpdateSlideUI();
        }
    }

    public void OnPrevClick()
    {
        // Cek apakah masih ada halaman sebelumnya?
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateSlideUI();
        }
    }

    private void UpdateSlideUI()
    {
        // 1. Cek apakah array gambar kosong?
        if (tutorialSlides.Length == 0) return;

        // 2. Ganti Gambar sesuai halaman sekarang
        displayImage.sprite = tutorialSlides[currentIndex];
        
        // Opsional: Agar gambar pixel art tidak gepeng/penyok
        // displayImage.SetNativeSize(); 
        // ATAU nyalakan 'Preserve Aspect' di komponen Image di Inspector

        // 3. Atur Logika Tombol (Mati/Nyala)
        // Kalau di halaman 0 (awal), tombol Back mati (interactable = false)
        prevButton.interactable = (currentIndex > 0);

        // Kalau di halaman terakhir, tombol Next mati
        nextButton.interactable = (currentIndex < tutorialSlides.Length - 1);
    }
}