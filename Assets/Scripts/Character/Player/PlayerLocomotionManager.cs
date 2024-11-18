using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;
    private Vector3 targetrotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 15;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    
    public void HandleAllMovement()
    {
        // Grounded movement
        HandleGroundedMovement();
        HandleRotation();
    }

    private void GetVerticalAndHorizontalMovement()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        // Clamp movements
    }

    private void HandleGroundedMovement()
    {
        GetVerticalAndHorizontalMovement();
        // Movement direction based on camera FOV perspective & movement inputs 
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            // Move at running speed
            // (Micro-Optimization) It is faster to calculate speed * deltaTime * moveDirection (float*float, float*Vector3)
            // instead of moveDirection * speed * deltaTime (Vector3*float, Vector3*float)
            player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f && PlayerInputManager.instance.moveAmount != 0)
        {
            // Move at walking speed
            player.characterController.Move(walkingSpeed * Time.deltaTime * moveDirection);
        }
    }

    private void HandleRotation()
    {
        targetrotationDirection = Vector3.zero;
        targetrotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetrotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetrotationDirection.y = 0;
        targetrotationDirection.Normalize();

        if (targetrotationDirection == Vector3.zero)
        {
            targetrotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetrotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }
}
