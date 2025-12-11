using UnityEngine;
using UnityEngine.Audio;

public class AudioInitializer : MonoBehaviour
{
    [Header("Referensi Mixer")]
    public AudioMixer mainMixer; // Drag Audio Mixer di sini

    // Nama Parameter HARUS SAMA PERSIS dengan di SettingsMenu
    private const string MASTER_PARAM = "MasterVolume";
    private const string BGM_PARAM = "BGMVol";
    private const string SFX_PARAM = "SFXVol";

    void Start()
    {
        // Panggil fungsi untuk apply volume saat scene baru mulai
        ApplyVolume();
    }

    void ApplyVolume()
    {
        // 1. Ambil data tersimpan (PlayerPrefs), default 1 (Full)
        float masterVal = PlayerPrefs.GetFloat(MASTER_PARAM, 1f);
        float bgmVal = PlayerPrefs.GetFloat(BGM_PARAM, 1f);
        float sfxVal = PlayerPrefs.GetFloat(SFX_PARAM, 1f);

        // 2. Set ke Mixer (Pakai rumus Logaritma yang sama biar konsisten)
        mainMixer.SetFloat(MASTER_PARAM, Mathf.Log10(Mathf.Max(masterVal, 0.0001f)) * 20);
        mainMixer.SetFloat(BGM_PARAM, Mathf.Log10(Mathf.Max(bgmVal, 0.0001f)) * 20);
        mainMixer.SetFloat(SFX_PARAM, Mathf.Log10(Mathf.Max(sfxVal, 0.0001f)) * 20);
    }
}