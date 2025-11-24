using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))] // Memastikan ada trigger
public class BiomeSoundFader : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Berapa lama waktu (detik) suara muncul/hilang")]
    public float fadeDuration = 2.0f; 
    
    [Tooltip("Volume maksimal suara hujan (0.0 sampai 1.0)")]
    [Range(0f, 1f)]
    public float maxVolume = 1.0f;

    [Tooltip("Tag Player kamu (sesuaikan dengan setting Unity)")]
    public string playerTag = "Player";

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Pastikan volume awal 0 agar tidak kaget
        audioSource.volume = 0f;
        // Pastikan loop aktif
        audioSource.loop = true;
        // Mulai memutar suara (tapi hening karena volume 0)
        if (!audioSource.isPlaying) audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang masuk adalah Player
        if (other.CompareTag(playerTag))
        {
            // Stop proses fade out (jika ada), lalu mulai fade in
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeAudio(maxVolume));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cek apakah yang keluar adalah Player
        if (other.CompareTag(playerTag))
        {
            // Stop proses fade in (jika ada), lalu mulai fade out ke 0
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeAudio(0f));
        }
    }

    // Fungsi logika Fade In / Fade Out
    private IEnumerator FadeAudio(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Rumus Lerp (Linear Interpolation) untuk perubahan halus
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null; // Tunggu frame berikutnya
        }

        // Pastikan volume tepat di angka target saat selesai
        audioSource.volume = targetVolume;
    }
}