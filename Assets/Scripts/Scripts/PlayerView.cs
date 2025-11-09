using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] Animator playerAnim;
    CharacterModel characterModel;
    private void Start()
    {
        characterModel = GetComponent<CharacterModel>();
        if (characterModel)
        {
            characterModel.onJumpCalled += PlayJump;
        }
    }
    public void PlayJump()
    {
        PlayAnimation("Jump");
    }
    public void PlayAnimation(string _animName)
    {
        playerAnim.Play(_animName);
    }
}
