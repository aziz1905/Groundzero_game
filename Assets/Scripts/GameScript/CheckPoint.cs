using UnityEngine;

// Pastikan objek Anda punya Animator!
[RequireComponent(typeof(Animator))] 
public class Checkpoint : MonoBehaviour
{
    public static Checkpoint activeCheckpoint;
    private Animator anim;  // Komponen Animator

    [Header("Audio")]
    [SerializeField] private AudioClip activationSound;
    private AudioSource audioSource;

    private bool isActivated = false;

    private void Awake()
    {
        anim = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            PlayerController player = other.GetComponent<PlayerController>(); 
            if (player != null)
            {
                player.SetNewRespawnPoint(transform.position);
                if (activeCheckpoint != null) activeCheckpoint.Deactivate();
                Activate();
            }
        }
    }

    private void Activate()
    {
        isActivated = true;
        // Beri tahu Animator untuk MENYALA
        anim.SetBool("isActivated", true); 
        activeCheckpoint = this;
        if (activationSound != null) audioSource.PlayOneShot(activationSound);
    }

    public void Deactivate()
    {
        isActivated = false;
        // Beri tahu Animator untuk MATI
        anim.SetBool("isActivated", false);
    }
}