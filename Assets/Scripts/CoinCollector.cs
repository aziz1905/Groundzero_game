using UnityEngine;
using UnityEngine.Audio;

public class CoinCollector : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip coinSFX;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    
    [Header("Optional - Particle Effect")]
    [SerializeField] private GameObject collectEffect;

    private ResettableCoin resettableCoin; // ✅ TAMBAHAN

    private void Awake()
    {
        // ✅ Ambil reference ke ResettableCoin
        resettableCoin = GetComponent<ResettableCoin>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        // 1. Play SFX
        if (coinSFX != null)
        {
            GameObject tempGO = new GameObject("TempAudio");
            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            
            if (sfxMixerGroup != null)
            {
                tempSource.outputAudioMixerGroup = sfxMixerGroup;
            }
            
            tempSource.clip = coinSFX;
            tempSource.Play();
            Destroy(tempGO, coinSFX.length);
        }

        // 2. Spawn particle effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // 3. ✅ GANTI: Jangan destroy, tapi hide via ResettableCoin
        if (resettableCoin != null)
        {
            resettableCoin.CollectCoin();
        }
        else
        {
            // Fallback kalau tidak ada ResettableCoin
            Destroy(gameObject);
        }
    }
}