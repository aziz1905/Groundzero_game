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

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
    }
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        InitializePlayerReference();
        
        if (offsetFromPlayer.x > 0)
        {
            offsetFromPlayer.x = -Mathf.Abs(offsetFromPlayer.x);
        }
        
        gameObject.SetActive(false);
    }
    
    void InitializePlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerTransform = playerController.transform;
                }
            }
        }
    }
    
    void Update()
    {
        if (!gameObject.activeSelf)
            return;
        
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
        // Pastikan playerTransform ada
        if (playerTransform == null)
        {
            InitializePlayerReference();
            if (playerTransform == null)
            {
                return;
            }
        }
        
        // Pastikan rectTransform ada
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogWarning("ChargeIndikator: RectTransform tidak ditemukan!");
                return;
            }
        }
        
        // Pastikan mainCamera ada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("ChargeIndikator: Main Camera tidak ditemukan!");
                return;
            }
        }
        
        Vector3 offset = offsetFromPlayer;
        if (offset.x > 0)
        {
            offset.x = -Mathf.Abs(offset.x);
        }
        
        Vector3 worldPos = playerTransform.position + offset;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        
        rectTransform.position = screenPos;
    }

    public void StartCharge()
    {
        // Pastikan semua komponen sudah diinisialisasi sebelum memulai charge
        InitializePlayerReference();
        
        // Pastikan camera sudah diinisialisasi
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Pastikan rectTransform sudah diinisialisasi
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        // Reset charge state
        isCharging = true;
        currentFill = 0f; 
        
        // Pastikan chargeBarImage sudah di-setup dengan benar
        if (chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
            chargeBarImage.fillAmount = 0f;
        }
        
        // Aktifkan GameObject
        gameObject.SetActive(true);
        
        // Update posisi segera setelah diaktifkan (jika semua komponen sudah siap)
        // Ini penting agar UI langsung terlihat di posisi yang benar
        if (followPlayer && playerTransform != null)
        {
            // Pastikan semua komponen sudah siap sebelum update posisi
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (mainCamera != null && rectTransform != null)
            {
                UpdatePosition();
            }
        }
    }

    public void StopCharge()
    {
        isCharging = false;
        
        gameObject.SetActive(false); 
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