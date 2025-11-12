using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro

public class BiomeTitleTrigger : MonoBehaviour
{
    // 1. Masukkan objek 'BiomeTitle_Text' dari Hierarchy ke sini
    [SerializeField] private TMP_Text biomeTextUI;

    // 2. Tulis nama biome yang akan muncul
    [SerializeField] private string biomeName;

    // 3. Atur durasi
    [SerializeField] private float fadeInTime = 1f;  // Waktu untuk muncul
    [SerializeField] private float stayTime = 3f;    // Waktu tampil di layar
    [SerializeField] private float fadeOutTime = 1f; // Waktu untuk menghilang

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Cek apakah itu Player
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            // --- TAMBAHKAN INI (Tes #2) ---
            Debug.LogWarning("PLAYER TERDETEKSI! Memulai Coroutine...");
            // ---------------------------------

            hasBeenTriggered = true;
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(ShowTitleFade());
        }
    }

    private IEnumerator ShowTitleFade()
    {
        // --- PERSIAPAN ---
        biomeTextUI.text = biomeName; // Set teksnya
        biomeTextUI.color = new Color(biomeTextUI.color.r, biomeTextUI.color.g, biomeTextUI.color.b, 0); // Mulai dari transparan (alpha = 0)
        biomeTextUI.gameObject.SetActive(true); // Nyalakan objek teksnya

        // --- FADE IN (MUNCUL) ---
        float timer = 0f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeInTime); // Hitung alpha dari 0 ke 1
            biomeTextUI.color = new Color(biomeTextUI.color.r, biomeTextUI.color.g, biomeTextUI.color.b, alpha);
            yield return null; // Tunggu frame berikutnya
        }

        // --- TAMPIL (STAY) ---
        yield return new WaitForSeconds(stayTime); // Tunggu bbrp detik

        // --- FADE OUT (MENGHILANG) ---
        timer = 0f; // Reset timer
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutTime); // Hitung alpha dari 1 ke 0
            biomeTextUI.color = new Color(biomeTextUI.color.r, biomeTextUI.color.g, biomeTextUI.color.b, alpha);
            yield return null; // Tunggu frame berikutnya
        }

        // --- SELESAI ---
        biomeTextUI.gameObject.SetActive(false); // Matikan lagi objek teksnya
        Destroy(gameObject); // Hancurkan trigger ini karena sudah tidak berguna
    }
}