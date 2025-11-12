using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeIndikator : MonoBehaviour
{
    [Header("Charge Bar Mode")]
    [Tooltip("TRUE = pakai sprite frames animation | FALSE = pakai fill bar")]
    [SerializeField] private bool useAnimatedSprite = true;
    
    [Header("Fill Bar Settings (jika useAnimatedSprite = false)")]
    [SerializeField] private Image chargeBarImage; 
    [SerializeField] private float chargeSpeed = 1.5f;
    
    [Header("Sprite Animation Settings (jika useAnimatedSprite = true)")]
    [SerializeField] private Image spriteRenderer; 
    [SerializeField] private Sprite[] chargeFrames;
    
    [Header("Follow Player Settings")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(-1f, -0.8f, 0f); 
    [SerializeField] private bool followPlayer = true; 

    private float currentFill;
    private bool isCharging = false;
    private RectTransform rectTransform;
    private Coroutine hideCoroutine;
    
    // === VARIABEL BARU UNTUK MENYIMPAN KAMERA ===
    private Camera mainCamera;
    private Canvas parentCanvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("ChargeIndikator: TIDAK BISA MENEMUKAN CANVAS INDUK!");
        }

        if (!useAnimatedSprite && chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
        
        if (!useAnimatedSprite && spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        if (useAnimatedSprite && chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
    }
    
    void Start()
    {
        // === PAKSA CARI PLAYER DAN KAMERA ===
        Debug.Log("ChargeIndikator: Start() - Memulai pencarian paksa...");
        
        // 1. Cari Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("ChargeIndikator: Start() - Player ditemukan via Tag 'Player'");
        }
        else
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                playerTransform = pc.transform;
                Debug.Log("ChargeIndikator: Start() - Player ditemukan via FindObjectOfType<PlayerController>");
            }
        }
        
        if (playerTransform == null)
        {
            Debug.LogError("ChargeIndikator: FATAL ERROR! TIDAK BISA MENEMUKAN PLAYER. Pastikan Player ada di Scene dan memiliki Tag 'Player'.");
        }

        // 2. Cari Kamera
        if (Camera.main != null)
        {
            mainCamera = Camera.main;
            Debug.Log($"ChargeIndikator: Start() - Kamera ditemukan via Camera.main (Tag: {mainCamera.tag})");
        }
        else
        {
            Debug.LogError("ChargeIndikator: FATAL ERROR! Camera.main adalah null. Pastikan kamera Anda memiliki Tag 'MainCamera'.");
            // Fallback, cari kamera apa saja
            mainCamera = FindObjectOfType<Camera>();
            if (mainCamera != null)
            {
                Debug.LogWarning($"ChargeIndikator: Menemukan kamera fallback (Nama: {mainCamera.name}), tapi Tag 'MainCamera' tidak ada.");
            }
            else
            {
                Debug.LogError("ChargeIndikator: FATAL ERROR! TIDAK ADA KAMERA SAMA SEKALI DI SCENE.");
            }
        }
        // === AKHIR PENCARIAN ===

        if (offsetFromPlayer.x > 0) offsetFromPlayer.x = -Mathf.Abs(offsetFromPlayer.x);
        
        if (useAnimatedSprite && spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        if (!useAnimatedSprite && chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (!gameObject.activeSelf) return;
        
        if (isCharging)
        {
            if (currentFill < 1f)
            {
                currentFill += chargeSpeed * Time.deltaTime;
                currentFill = Mathf.Clamp(currentFill, 0f, 1f);
                
                if (useAnimatedSprite) UpdateSpriteFrame();
                else if (chargeBarImage != null) chargeBarImage.fillAmount = currentFill;
            }
        }
        
        // Panggil UpdatePosition setiap frame
        if (followPlayer)
        {
            UpdatePosition();
        }
    }

    void UpdateSpriteFrame()
    {
        if (spriteRenderer == null || chargeFrames == null || chargeFrames.Length == 0) return;
        
        int frameIndex = Mathf.FloorToInt(currentFill * chargeFrames.Length);
        frameIndex = Mathf.Clamp(frameIndex, 0, chargeFrames.Length - 1);
        spriteRenderer.sprite = chargeFrames[frameIndex];
    }

    // === INI FUNGSI YANG DIPERBAIKI (LAGI) ===
    void UpdatePosition()
    {
        // Cek semua referensi penting. Jika salah satu null, jangan lakukan apa-apa.
        if (playerTransform == null || rectTransform == null || parentCanvas == null || mainCamera == null)
        {
            // Kita log sekali saja agar tidak spam
            if(playerTransform == null) Debug.LogWarning("UpdatePosition: playerTransform is NULL");
            if(mainCamera == null) Debug.LogWarning("UpdatePosition: mainCamera is NULL");
            return;
        }

        Vector3 worldPos = playerTransform.position + offsetFromPlayer;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        Vector2 canvasPos;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            screenPos, 
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, 
            out canvasPos
        );
        

        rectTransform.anchoredPosition = canvasPos;
    }

    public void StartCharge()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        
        isCharging = true;
        currentFill = 0.0f;
        
        if (useAnimatedSprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.gameObject.SetActive(true);
                if (chargeFrames != null && chargeFrames.Length > 0)
                {
                    spriteRenderer.sprite = chargeFrames[0];
                }
            }
            if (chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
        }
        else
        {
            if (chargeBarImage != null)
            {
                chargeBarImage.gameObject.SetActive(true);
                chargeBarImage.fillAmount = currentFill;
            }
            if (spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        }
        
        // Panggil sekali agar posisi langsung benar
        UpdatePosition();
    }

    public void StopCharge()
    {
        isCharging = false;
        
        if (!gameObject.activeSelf) return; 
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        
        hideCoroutine = StartCoroutine(HideAfterRenderFrame());
    }

    private IEnumerator HideAfterRenderFrame()
    {
        yield return null; 
        
        if (useAnimatedSprite && spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        if (!useAnimatedSprite && chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
        
        hideCoroutine = null;
    }

    public float GetCurrentChargeValue()
    {
        return currentFill;
    }
    
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    public void SetOffset(Vector3 offset)
    {
        offsetFromPlayer = offset;
        if (offsetFromPlayer.x > 0)
        {
            offsetFromPlayer.x = -Mathf.Abs(offsetFromPlayer.x);
        }
    }
}