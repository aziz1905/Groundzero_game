using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Components")]
    [SerializeField] private AudioMixer mainMixer; // Masukkan Audio Mixer di sini

    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private GameObject SettingPanel;
    

    // Nama Parameter harus SAMA PERSIS dengan yang ada di Audio Mixer (Exposed Parameters)
    private const string MIXER_MASTER = "MasterVol";
    private const string MIXER_BGM = "BGMVol";
    private const string MIXER_SFX = "SFXVol";

    void Start()
    {
        // Saat game mulai, kita load volume terakhir yang disimpan
        // Jika belum ada save, default-nya 1 (Full Volume)
        
        float masterVal = PlayerPrefs.GetFloat(MIXER_MASTER, 10f);
        float bgmVal = PlayerPrefs.GetFloat(MIXER_BGM, 1f);
        float sfxVal = PlayerPrefs.GetFloat(MIXER_SFX, 1f);

        // 1. Update posisi Slider UI agar sesuai nilai save
        if (masterSlider != null) masterSlider.value = masterVal;
        if (bgmSlider != null) bgmSlider.value = bgmVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        

        // 2. Update suara di Mixer (karena slider tidak otomatis memicu fungsi saat start)
        SetMasterVolume(masterVal);
        SetBGMVolume(bgmVal);
        SetSFXVolume(sfxVal);
        
    }

    // --- FUNGSI YANG DIPANGGIL OLEH SLIDER ---

public void SetSFXVolume(float sliderValue)
    {
        // PENTING: Cegah nilai 0 murni agar tidak error Log10
        // Kita paksa minimal 0.0001
        float value = Mathf.Max(sliderValue, 0.0001f);

        // Rumus Logaritma
        float db = Mathf.Log10(value) * 20;

        mainMixer.SetFloat(MIXER_SFX, db);
        PlayerPrefs.SetFloat(MIXER_SFX, value);
    }
    
    // Lakukan hal yang sama untuk Master dan BGM
    public void SetMasterVolume(float sliderValue)
    {
        float value = Mathf.Max(sliderValue, 0.0001f);
        mainMixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MIXER_MASTER, value);
    }

    public void SetBGMVolume(float sliderValue)
    {
        float value = Mathf.Max(sliderValue, 0.0001f);
        mainMixer.SetFloat(MIXER_BGM, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(MIXER_BGM, value);
    }

}