using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    
    [HideInInspector]public float verticalMovement;
    [HideInInspector]public float horizontalMovement;
    [HideInInspector]public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetrotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 6.5f;
    [SerializeField] float rotationSpeed = 15;
    [SerializeField] int sprintingStaminaCost = 2;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 15;
    [SerializeField] float jumpStaminaCost = 15;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    protected override void Update(){
        base.Update();

        if(player.IsOwner){
             player.characterNetworkManager.verticalMovement.Value = verticalMovement;
             player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
             player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else{
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;
            //if not locked on, pass the amount
            player.PlayerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            //if locked on , pass the hor and ver
        }
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
        moveAmount = PlayerInputManager.instance.moveAmount;
        // Clamp movements
    }

    private void HandleGroundedMovement()
    {
         if (!player.canMove)
            return;
        GetVerticalAndHorizontalMovement();
       
        // Movement direction based on camera FOV perspective & movement inputs 
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (player.playerNetworkManager.isSprinting.Value){
            player.characterController.Move(sprintingSpeed * Time.deltaTime * moveDirection);
        }
        else {
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

    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;
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

    public void HandleSprinting(){
        if (player.isPerformingAction){
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0) {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        //IF WE ARE MOVING SPRINTING IS TRUE
        if (moveAmount >= 0.5){
            player.playerNetworkManager.isSprinting.Value = true;
        }
        //IF WEW ARE STATIONARY OR MOVING SLOWLY SPRINTING IS FALSE
        else {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value) {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }


    public void AttemptToPerformDodge(){

        if(player.isPerformingAction)
        {
            return;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0) {
            return;
        }

        if (PlayerInputManager.instance.moveAmount > 0)
        {
        rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
        rollDirection.y = 0;
        rollDirection.Normalize();

        Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
        player.transform.rotation = playerRotation;

        player.PlayerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
        }

        else{
            player.PlayerAnimatorManager.PlayTargetActionAnimation("Roll_Backward_01", true, true);
        }

        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void AttemptToPerformJump()
    {
        // IF WE ARE PERFORMING A GENERAL ACTION, WE DO NOT WANT TO ALLOW A JUMP (WILL CHANGE WHEN COMBAT IS ADDED)
        if (player.isPerformingAction)
        {
            return;
        }

        // IF WE ARE OUT OF STAMINA, WE DO NOT WISH TO ALLOW A JUMP
        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        // IF WE ARE ALREADY IN A JUMP, WE DO NOT WANT TO ALLOW A JUMP AGAIN UNTIL THE CURRENT JUMP HAS FINISHED
        if (player.isJumping)
        {
            return;
        }

        // IF WE ARE NOT GROUNDED, WE DO NOT WANT TO ALLOW A JUMP
        if (!player.isGrounded)
        {
            return;
        }

        player.PlayerAnimatorManager.PlayTargetActionAnimation("Main_Jump_Start_01", false);

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;
    }

    public void ApplyJumpingVelocity()
    {
        // APPLY AN UPWARD VELOCITY
    }
}

