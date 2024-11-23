using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector]public PlayerAnimatorManager PlayerAnimatorManager;
    [HideInInspector]public PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        PlayerAnimatorManager =  GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        // if we dont own the gameobject, we dont control or edit it
        if (!IsOwner)
            return;

        // Handle player movement
        playerLocomotionManager.HandleAllMovement();
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
        }
    } 
}
