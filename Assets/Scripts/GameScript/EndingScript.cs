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
        }
        
        if (legendaryFly != null && flyAnimator == null)
            flyAnimator = legendaryFly.GetComponent<Animator>();
        
        originalCameraSize = mainCamera.orthographicSize;
        
        // Hide UI di awal
        if (promptUI != null)
            promptUI.SetActive(false);
        
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

        yield return null; // beri 1 frame supaya animator masuk ke state Eat

        // 4. Ambil durasi animasi Eat dari clip
        float eatDuration = 2f; // fallback
        if (playerAnimator != null)
        {
            AnimatorClipInfo[] clips = playerAnimator.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                eatDuration = clips[0].clip.length;
                Debug.Log($"Eat clip duration: {eatDuration:F2}s");
            }
        }

        // 5. Tunggu sampai mendekati akhir animasi (0.9x durasi)
        yield return new WaitForSeconds(eatDuration * 0.9f);

        // 6. Destroy lalat sedikit sebelum animasi berakhir
        if (legendaryFly != null)
        {
            Destroy(legendaryFly);
            Debug.Log("Fly destroyed right before Eat animation ended");
        }

        // 7. Tunggu sisa animasi biar halus
        yield return new WaitForSeconds(eatDuration * 0.53f);

        // 8. Freeze player di frame terakhir (biar gak balik idle)
        if (playerAnimator != null)
        {
            playerAnimator.Update(0);
            playerAnimator.enabled = false;
            Debug.Log("Player animator frozen at last frame");
        }

        // 9. Delay kecil sebelum fade
        yield return new WaitForSeconds(delayBeforeDestroy);

        // 10. Fade to black
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
            yield break;

        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            c.a = Mathf.Lerp(0f, 1f, t);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;
        Debug.Log("Fade to black complete");
    }
}
