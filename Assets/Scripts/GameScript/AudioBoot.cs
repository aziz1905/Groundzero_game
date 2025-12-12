using UnityEngine;
using UnityEngine.Audio;

public class AudioBoot : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;

    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_BGM = "BGMVol";
    private const string MIXER_SFX = "SFXVol";

    void Start()
    {
        LoadVolume();
    }

    public void LoadVolume()
    {
        // Ambil data save, default 1 (Full)
        float masterVal = PlayerPrefs.GetFloat(MIXER_MASTER, 1f);
        float bgmVal = PlayerPrefs.GetFloat(MIXER_BGM, 1f);
        float sfxVal = PlayerPrefs.GetFloat(MIXER_SFX, 1f);

        // Set ke Mixer (Tanpa pengurangan -5f atau -15f lagi)
        mainMixer.SetFloat(MIXER_MASTER, Mathf.Log10(Mathf.Max(masterVal, 0.0001f)) * 20);
        mainMixer.SetFloat(MIXER_BGM, Mathf.Log10(Mathf.Max(bgmVal, 0.0001f)) * 20);
        mainMixer.SetFloat(MIXER_SFX, Mathf.Log10(Mathf.Max(sfxVal, 0.0001f)) * 20);
    }
}