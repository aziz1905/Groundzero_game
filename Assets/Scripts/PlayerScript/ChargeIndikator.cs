using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeIndikator : MonoBehaviour
{
    [SerializeField] private Image chargeBarImage; // Slot untuk gambar HIJAU (Fill)
    [SerializeField] private float chargeSpeed = 1.5f;

    private float currentFill;
    private bool isCharging = false;

    void Start()
    {
        // Pastikan gambar HIJAU diatur dengan benar
        if (chargeBarImage != null)
        {
            chargeBarImage.type = Image.Type.Filled;
            chargeBarImage.fillMethod = Image.FillMethod.Vertical;
            chargeBarImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
        
        // --- PERUBAHAN UTAMA ---
        // Sembunyikan SELURUH OBJEK UI ini saat game dimulai
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isCharging) return;

        if (currentFill < 1f)
        {
            currentFill += chargeSpeed * Time.deltaTime;
            currentFill = Mathf.Clamp(currentFill, 0f, 1f); 
            
            if (chargeBarImage != null) // Pastikan slot terisi
                chargeBarImage.fillAmount = currentFill;
        }
    }

    // Dipanggil saat Player menekan Spasi
    public void StartCharge()
    {
        isCharging = true;
        currentFill = 0f; 
        
        // --- PERUBAHAN UTAMA ---
        // Munculkan SELURUH OBJEK UI ini
        gameObject.SetActive(true); 
        
        if (chargeBarImage != null)
            chargeBarImage.fillAmount = 0f; // Reset bar hijaunya
    }

    // Dipanggil saat Player melepas Spasi
    public void StopCharge()
    {
        isCharging = false;
        
        // --- PERUBAHAN UTAMA ---
        // Sembunyikan SELURUH OBJEK UI ini
        gameObject.SetActive(false); 
    }

    // Dipanggil Player saat melompat
    public float GetCurrentChargeValue()
    {
        return currentFill;
    }
}