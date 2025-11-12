using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // <-- Gunakan TextMeshPro (standar baru)
// using UnityEngine.UI; // <-- (Jika masih pakai Text lama)

// --- PRINSIP S.O.L.I.D. ---
// Single Responsibility: HANYA mengurus update UI.
// Tidak tahu menahu soal Player atau cara mati.
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag objek TextMeshPro nyawa ke sini")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI livesText2;

    // (Opsional) Anda bisa taruh panel Game Over di sini
    // [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        // --- Mendengarkan Sinyal ---
        // Saat game dimulai, cari GameManager.Instance
        // dan "daftarkan" fungsi UpdateLivesText
        // untuk mendengarkan sinyal OnLivesChanged.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged.AddListener(UpdateLivesText);

            // Tampilkan nyawa awal saat game baru dimulai
            // (Kita butuh script GameManager versi baru untuk GetCurrentLives)
            // UpdateLivesText(GameManager.Instance.GetCurrentLives());
        }
        else
        {
            Debug.LogError("GameManager.Instance tidak ditemukan!");
        }

        // if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // Fungsi ini akan DIPANGGIL OTOMATIS oleh GameManager
    // setiap kali event OnLivesChanged di-Invoke.
    private void UpdateLivesText(int newLivesAmount)
    {
        if (livesText != null)
        {
            // Tampilkan nyawa di UI
            livesText.text = "x " + newLivesAmount;
        }
    }

    private void OnDestroy()
    {
        // Membersihkan listener saat objek UI hancur
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged.RemoveListener(UpdateLivesText);
        }
    }
}