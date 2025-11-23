using UnityEngine;
using UnityEngine.UI;
using TMPro; // Wajib untuk teks

public class TutorialKeyUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image buttonImage;       // Gambar Background Tombol
    [SerializeField] private TextMeshProUGUI keyText; // Teks di atas tombol (A/D/Space)

    [Header("Sprite Settings")]
    [SerializeField] private Sprite normalSprite;     // Gambar tombol diam
    [SerializeField] private Sprite pressedSprite;    // Gambar tombol ditekan (gepeng)

    // Fungsi untuk mengubah huruf secara manual (bisa dipanggil dari Inspector atau code)
    public void SetKeyName(string letter)
    {
        if (keyText != null) keyText.text = letter;
    }

    // Fungsi visual: Dipanggil terus menerus oleh Manager
    public void SetPressedState(bool isPressed)
    {
        if (buttonImage == null) return;

        if (isPressed)
        {
            buttonImage.sprite = pressedSprite;
            // Opsional: Turunkan teks sedikit biar realistis
            keyText.rectTransform.anchoredPosition = new Vector2(0, -5); 
        }
        else
        {
            buttonImage.sprite = normalSprite;
            // Kembalikan posisi teks
            keyText.rectTransform.anchoredPosition = new Vector2(0, 0);
        }
    }
}