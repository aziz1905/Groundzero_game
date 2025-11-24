using System.Collections;
using UnityEngine;

public class TriggerSpikeTrap : MonoBehaviour
{
    [SerializeField] private GameObject spikes; 
    [SerializeField] private float activationDelay = 0.5f; 

    private bool hasBeenTriggered = false;
    private Collider2D trapCollider;
    private Coroutine activeCoroutine;

    private void OnEnable()
    {
        GameManager.OnPlayerRespawn += ResetTrap;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRespawn -= ResetTrap;
    }

    private void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        ResetTrap(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            trapCollider.enabled = false;

            // aktifkan duri
            activeCoroutine = StartCoroutine(ShowSpikesAfterDelay());
        }
    }

    private IEnumerator ShowSpikesAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        if (spikes != null)
        {
            spikes.SetActive(true);
        }
    }

    private void ResetTrap()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        hasBeenTriggered = false;
        
        if (trapCollider != null)
        {
            trapCollider.enabled = true;
        }

        if (spikes != null)
        {
            spikes.SetActive(false);
        }
    }
}