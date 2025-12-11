using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;    

public class SettingsMenu : MonoBehaviour
{
    [Header("--- Audio Mixer Reference ---")]
    public AudioMixer mainMixer; 

    [Header("--- UI Sliders ---")]
    public Slider masterSlider; 
    public Slider bgmSlider;   
    public Slider sfxSlider;    

    private const string MASTER_PARAM = "MasterVolume";
    private const string BGM_PARAM = "BGMVol";
    private const string SFX_PARAM = "SFXVol";

    void Start()
    {
        float masterVal = PlayerPrefs.GetFloat(MASTER_PARAM, 1f);
        float bgmVal = PlayerPrefs.GetFloat(BGM_PARAM, 1f);
        float sfxVal = PlayerPrefs.GetFloat(SFX_PARAM, 1f);

        if (masterSlider != null) masterSlider.value = masterVal;
        if (bgmSlider != null) bgmSlider.value = bgmVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        SetMasterVolume(masterVal);
        SetBGMVolume(bgmVal);
        SetSFXVolume(sfxVal);

        if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    public void SetMasterVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;

        mainMixer.SetFloat(MASTER_PARAM, dbValue);
        PlayerPrefs.SetFloat(MASTER_PARAM, sliderValue); 
    }

    public void SetBGMVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;

        mainMixer.SetFloat(BGM_PARAM, dbValue);
        PlayerPrefs.SetFloat(BGM_PARAM, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;

        mainMixer.SetFloat(SFX_PARAM, dbValue);
        PlayerPrefs.SetFloat(SFX_PARAM, sliderValue); 
    }
}