using System.Collections;
using UnityEngine;
using TMPro;

// Pastikan Canvas Group ada di objek ini
[RequireComponent(typeof(CanvasGroup))]
public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathText;

    private CanvasGroup canvasGroup;
    public bool isFading { get; private set; } // Agar GameManager tahu

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Mulai dalam keadaan mati
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void ShowScreen(int deathCount, float fadeTime)
    {
        isFading = true;
        gameObject.SetActive(true); // Nyalakan Canvas
        deathText.text = "DEATHS: " + deathCount;
        StartCoroutine(Fade(1f, fadeTime)); // Fade In
    }

    public void HideScreen(float fadeTime)
    {
        StartCoroutine(Fade(0f, fadeTime)); // Fade Out
    }

    private IEnumerator Fade(float targetAlpha, float time)
    {
        canvasGroup.blocksRaycasts = (targetAlpha == 1);
        canvasGroup.interactable = (targetAlpha == 1);

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / time);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Jika fade out, matikan canvas di akhir
        if (targetAlpha == 0)
        {
            gameObject.SetActive(false);
        }
        isFading = false;
    }
}