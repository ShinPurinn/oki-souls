using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
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
}
