using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector]public PlayerAnimatorManager PlayerAnimatorManager;
    [HideInInspector]public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector]public PlayerNetworkManager playerNetworkManager;
    [HideInInspector]public PlayerStatsManager playerStatsManager;
    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        PlayerAnimatorManager =  GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        // if we dont own the gameobject, we dont control or edit it
        if (!IsOwner)
            return;

        // Handle player movement
        playerLocomotionManager.HandleAllMovement();

        // Regen Stamina
        playerStatsManager.RegenerateStamina();
    }

     protected virtual void LateUpdate(){
        if(!IsOwner)
            return;
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }
    
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();

        if(IsOwner){
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }
    } 
}
