using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingCutscene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject legendaryFly;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator flyAnimator;
    
    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private Text promptText;
    [SerializeField] private Image fadePanel;
    [SerializeField] private GameObject thankYouText; // GameObject dengan Text component

    [Header("Audio")]
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private float eatSoundDelay = 0.2f; // Delay SFX (bisa disesuaikan di Inspector)
    
    [Header("Camera Settings")]
    [SerializeField] private float targetZoom = 3f;
    [SerializeField] private float zoomDuration = 2f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0.5f, -10f);
    
    [Header("Timing")]
    [SerializeField] private float delayBeforePrompt = 0.5f;
    [SerializeField] private float delayBeforeDestroy = 0.3f;
    [SerializeField] private float fadeOutDuration = 2f;
    
    private bool cutsceneTriggered = false;
    private bool waitingForInput = false;
    private float originalCameraSize;
    private PlayerController playerController;
    private AudioSource playerAudioSource;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerAnimator = player.GetComponent<Animator>();
            playerAudioSource = player.GetComponent<AudioSource>();
        }
        
        if (legendaryFly != null && flyAnimator == null)
            flyAnimator = legendaryFly.GetComponent<Animator>();
        
        originalCameraSize = mainCamera.orthographicSize;
        
        // Hide UI di awal
        if (promptUI != null)
            promptUI.SetActive(false);
        
        // Hide Thank You text di awal
        if (thankYouText != null)
            thankYouText.SetActive(false);
        
        // Setup fade panel
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (waitingForInput && Input.GetKeyDown(KeyCode.E))
        {
            waitingForInput = false;
            StartCoroutine(PlayEatingSequence());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !cutsceneTriggered)
        {
            cutsceneTriggered = true;
            StartCoroutine(PlayEndingCutscene());
        }
    }

    IEnumerator PlayEndingCutscene()
    {
        Debug.Log("Ending Cutscene Started!");

        // 1. Disable control
        if (playerController != null)
            playerController.enabled = false;

        // 2. Stop player
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 3. Idle menghadap lalat
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", 0);
            playerAnimator.SetBool("isJumping", false);
            playerAnimator.SetBool("isCharging", false);

            if (legendaryFly != null)
            {
                Vector3 dirToFly = legendaryFly.transform.position - player.position;
                int direction = dirToFly.x > 0 ? 1 : -1;
                playerAnimator.SetInteger("Direction", direction);
            }
        }

        // 4. Zoom kamera
        Vector3 targetCameraPos = player.position + cameraOffset;
        yield return StartCoroutine(CameraZoomAndMove(targetCameraPos, targetZoom, zoomDuration));

        // 5. Delay kecil lalu munculkan prompt
        yield return new WaitForSeconds(delayBeforePrompt);

        if (promptUI != null)
            promptUI.SetActive(true);

        waitingForInput = true;
    }

    IEnumerator PlayEatingSequence()
    {
        Debug.Log("E pressed - starting eating sequence");

        // 1. Sembunyikan prompt
        if (promptUI != null)
            promptUI.SetActive(false);

        // 2. Freeze lalat
        if (flyAnimator != null)
        {
            flyAnimator.enabled = false;
            Debug.Log("Fly animator disabled");
        }

        // 3. Mainkan animasi makan
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Eat");
            playerAnimator.SetTrigger("Eat");
            Debug.Log("Playing Eat animation...");
        }

        // 4. Delay lalu mainkan SFX
        yield return new WaitForSeconds(eatSoundDelay);
        
        if (playerAudioSource != null && eatSound != null)
        {
            playerAudioSource.PlayOneShot(eatSound);
            Debug.Log("Playing eat sound");
        }

        yield return null; // beri 1 frame supaya animator masuk ke state Eat

        // 5. Ambil durasi animasi Eat dari clip
        float eatDuration = 1f; // fallback
        if (playerAnimator != null)
        {
            AnimatorClipInfo[] clips = playerAnimator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                eatDuration = clips[0].clip.length;
                Debug.Log($"Eat clip duration: {eatDuration:F2}s");
            }
        }

        // 6. Tunggu sampai mendekati akhir animasi (disesuaikan dengan SFX delay)
        float adjustedWait = (eatDuration * 0.9f) - eatSoundDelay;
        if (adjustedWait > 0)
        {
            yield return new WaitForSeconds(adjustedWait);
        }

        // 7. Destroy lalat sedikit sebelum animasi berakhir
        if (legendaryFly != null)
        {
            Destroy(legendaryFly);
            Debug.Log("Fly destroyed right before Eat animation ended");
        }

        // 8. Tunggu sisa animasi biar halus
        yield return new WaitForSeconds(eatDuration * 0.4f);

        // 9. Freeze player di frame terakhir (biar gak balik idle)
        if (playerAnimator != null)
        {
            playerAnimator.Update(0);
            playerAnimator.enabled = false;
            Debug.Log("Player animator frozen at last frame");
        }

        // 10. Delay kecil sebelum fade
        yield return new WaitForSeconds(delayBeforeDestroy);

        // 11. Fade to black
        yield return StartCoroutine(FadeToBlack());

        Debug.Log("Ending Complete! Fade done.");
    }

    IEnumerator CameraZoomAndMove(Vector3 targetPosition, float targetSize, float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // smoothstep

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = targetSize;
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
        {
            Debug.LogWarning("Fade panel not assigned!");
            yield break;
        }

        float elapsed = 0f;
        Color fadeColor = fadePanel.color;
        
        // Setup Thank You text
        Text thankYouTextComponent = null;
        Color textColor = Color.white;
        
        if (thankYouText != null)
        {
            thankYouText.SetActive(true);
            thankYouTextComponent = thankYouText.GetComponent<Text>();
            
            if (thankYouTextComponent != null)
            {
                textColor = thankYouTextComponent.color;
                textColor.a = 0f;
                thankYouTextComponent.color = textColor;
            }
        }

        // Fade to black + fade in text
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            
            // Fade panel ke hitam
            fadeColor.a = Mathf.Lerp(0f, 1f, t);
            fadePanel.color = fadeColor;
            
            // Fade in Thank You text (mulai muncul di 30% fade)
            if (thankYouTextComponent != null)
            {
                float textT = Mathf.Clamp01((t - 0.3f) / 0.7f);
                textColor.a = Mathf.Lerp(0f, 1f, textT);
                thankYouTextComponent.color = textColor;
            }
            
            yield return null;
        }

        // Pastikan full opacity
        fadeColor.a = 1f;
        fadePanel.color = fadeColor;
        
        if (thankYouTextComponent != null)
        {
            textColor.a = 1f;
            thankYouTextComponent.color = textColor;
        }

        Debug.Log("Fade to black complete");
    }
}