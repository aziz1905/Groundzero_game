using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeIndikator : MonoBehaviour
{
    [SerializeField] private Image chargeBarImage; 
    [SerializeField] private float chargeSpeed = 1.5f;
    
    [Header("Follow Player Settings")]
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private Vector3 offsetFromPlayer = new Vector3(-1f, -0.8f, 0f); 
    [SerializeField] private bool followPlayer = true; 

    private float currentFill;
    private bool isCharging = false;
    private RectTransform rectTransform;
    private Camera mainCamera;
    private Coroutine hideCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null) mainCamera = Camera.main;
        
        if (chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
    }
    
    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        InitializePlayerReference();
        if (offsetFromPlayer.x > 0) offsetFromPlayer.x = -Mathf.Abs(offsetFromPlayer.x);
        
        gameObject.SetActive(false); // Mulai dalam keadaan mati
    }
    
    void InitializePlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else
            {
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null) playerTransform = playerController.transform;
            }
        }
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
                if (chargeBarImage != null)
                    chargeBarImage.fillAmount = currentFill;
            }
        }
        
        if (followPlayer && playerTransform != null)
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (playerTransform == null)
        {
            InitializePlayerReference();
            if (playerTransform == null) return;
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }
        
        Vector3 offset = offsetFromPlayer;
        if (offset.x > 0) offset.x = -Mathf.Abs(offset.x);
        
        Vector3 worldPos = playerTransform.position + offset;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        rectTransform.position = screenPos;
    }

    public void StartCharge()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        InitializePlayerReference();
        if (mainCamera == null) mainCamera = Camera.main;
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        
        isCharging = true;
        
        // ==========================================================
        // === PERUBAHAN DI SINI ===
        // ==========================================================
        // Jangan atur ke 0, tapi ke nilai kecil agar langsung terlihat
        currentFill = 0.01f; 
        
        if (chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
            chargeBarImage.fillAmount = currentFill; // Atur ke nilai kecil
        }
        // ==========================================================
        
        gameObject.SetActive(true); 
        
        if (followPlayer && playerTransform != null)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (mainCamera != null && rectTransform != null)
            {
                UpdatePosition();
            }
        }
    }

    public void StopCharge()
    {
        isCharging = false;
        
        if (!gameObject.activeSelf) // Fix error coroutine
        {
            return; 
        }
        
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterRenderFrame());
    }

    private IEnumerator HideAfterRenderFrame()
    {
        yield return null; // Tunggu 1 frame
        gameObject.SetActive(false); 
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