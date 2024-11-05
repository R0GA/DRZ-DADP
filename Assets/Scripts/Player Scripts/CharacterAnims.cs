using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterAnims : MonoBehaviour
{
    [SerializeField]
    private FirstPersonControls fpc;
    [SerializeField]
    private Animator animator;
    private int animState;

    private void Start()
    {
        //animState = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (!fpc.holdingFlashlight && fpc.characterController.isGrounded && !fpc.isMoving)
        {
            animState = 0;
        }
        if (!fpc.holdingFlashlight && fpc.characterController.isGrounded && fpc.isMoving)
        {
            animState = 1;
        }
        if (!fpc.holdingFlashlight && !fpc.characterController.isGrounded)
        {
            animState = 2;
        }
        if (fpc.holdingFlashlight && fpc.characterController.isGrounded && !fpc.isMoving)
        {
            animState = 3;
        }
        if (fpc.holdingFlashlight && fpc.characterController.isGrounded && fpc.isMoving)
        {
            animState = 4;
        }
        if (fpc.holdingFlashlight && !fpc.characterController.isGrounded)
        {
            animState = 5;
        }

        animator.SetInteger("AnimState", animState);
        Debug.Log(fpc.characterController.velocity.magnitude)   ;
        

    }
}
