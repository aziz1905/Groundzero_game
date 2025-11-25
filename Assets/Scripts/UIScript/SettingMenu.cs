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
        
        float masterVal = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
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

    public void SetMasterVolume(float sliderValue)
    {
        // Ubah volume mixer (Logaritma)
        mainMixer.SetFloat(MIXER_MASTER, Mathf.Log10(sliderValue) * 20);
        
        // Simpan data
        PlayerPrefs.SetFloat(MIXER_MASTER, sliderValue);
    }

    public void SetBGMVolume(float sliderValue)
    {
        mainMixer.SetFloat(MIXER_BGM, Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(MIXER_BGM, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        mainMixer.SetFloat(MIXER_SFX, Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat(MIXER_SFX, sliderValue);
    }

    
}