// Pastikan file ini bernama ChargeIndikator.cs
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
    [SerializeField] private Sprite[] chargeFrames; // <-- Array untuk animasi kodok
    
    [Header("Follow Player Settings")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(-1f, -0.8f, 0f); 
    [SerializeField] private bool followPlayer = true; 

    private float currentFill;
    private bool isCharging = false;
    private RectTransform rectTransform;
    private Coroutine hideCoroutine;
    private Camera mainCamera;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Setup mode
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
        // Cari Player dan Kamera secara otomatis
        InitializePlayerReference(); 
        if (mainCamera == null) mainCamera = Camera.main;

        if (playerTransform == null) Debug.LogError("ChargeIndikator: TIDAK BISA MENEMUKAN PLAYER!");
        if (mainCamera == null) Debug.LogError("ChargeIndikator: TIDAK BISA MENEMUKAN KAMERA! (Pastikan Tag 'MainCamera')");
        
        // Mulai dalam keadaan mati
        if (useAnimatedSprite && spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        if (!useAnimatedSprite && chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Jangan lakukan apa-apa jika nonaktif
        if ((useAnimatedSprite && !spriteRenderer.gameObject.activeSelf) || 
            (!useAnimatedSprite && !chargeBarImage.gameObject.activeSelf))
        {
            return;
        }

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

    // Ganti frame sprite berdasarkan 'currentFill'
    void UpdateSpriteFrame()
    {
        if (spriteRenderer == null || chargeFrames == null || chargeFrames.Length == 0) return;
        
        int frameIndex = Mathf.FloorToInt(currentFill * chargeFrames.Length);
        frameIndex = Mathf.Clamp(frameIndex, 0, chargeFrames.Length - 1);
        spriteRenderer.sprite = chargeFrames[frameIndex];
    }

    // === INI FUNGSI YANG DIPERBAIKI (METODE SIMPEL) ===
    void UpdatePosition()
    {
        if (playerTransform == null || mainCamera == null || rectTransform == null) return;

        Vector3 worldPos = playerTransform.position + offsetFromPlayer;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        
        // Langsung 'paksa' posisi UI ke pixel layar
        rectTransform.position = screenPos;
    }

    void InitializePlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else
            {
                PlayerController pc = FindObjectOfType<PlayerController>();
                if (pc != null) playerTransform = pc.transform;
            }
        }
    }

    // Player memanggil ini
    public void StartCharge()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        
        isCharging = true;
        currentFill = 0.0f;
        
        if (useAnimatedSprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.gameObject.SetActive(true); // Skrip menyalakan dirinya sendiri
                if (chargeFrames != null && chargeFrames.Length > 0)
                {
                    spriteRenderer.sprite = chargeFrames[0];
                }
            }
        }
        else
        {
            if (chargeBarImage != null)
            {
                chargeBarImage.gameObject.SetActive(true); // Skrip menyalakan dirinya sendiri
                chargeBarImage.fillAmount = currentFill;
            }
        }
        
        UpdatePosition(); // Langsung pindah ke posisi yang benar
    }

    // Player memanggil ini
    public void StopCharge()
    {
        isCharging = false;
        
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterRenderFrame());
    }

    // Sembunyi setelah 1 frame
    private IEnumerator HideAfterRenderFrame()
    {
        yield return null; 
        
        if (useAnimatedSprite && spriteRenderer != null) spriteRenderer.gameObject.SetActive(false);
        if (!useAnimatedSprite && chargeBarImage != null) chargeBarImage.gameObject.SetActive(false);
        
        hideCoroutine = null;
    }

    // Player memanggil ini
    public float GetCurrentChargeValue()
    {
        return currentFill;
    }
}