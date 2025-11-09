using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    public delegate void CharacterModelEvent();
    [SerializeField] float jumpForce;
    [SerializeField] Rigidbody2D rb;
    public CharacterModelEvent onJumpCalled;
    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);   
        onJumpCalled?.Invoke();
    }
}
