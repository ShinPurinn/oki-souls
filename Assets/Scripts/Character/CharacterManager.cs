using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }

    protected virtual void Update()
    {
        animator.SetBool("IsGrounded", isGrounded);

        // If the character is being controlled by our side, then assign its network position to the position of our transform
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        // If the character is being controlled from somewhere else, then assign its position here locally by the position of its network transform
        else
        {
            // Position
            transform.position = Vector3.SmoothDamp(
                transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime
            );
            
            // Rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime
            );
        }
    }

   protected virtual void LateUpdate(){
    
    }
}
