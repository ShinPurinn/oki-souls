using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenerationAmount = 2;
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    public int CalculateStaminaBasedOnEnduranceLevel(int endurance) {
        float stamina = 0;

        stamina = endurance * 100;

        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        // ONLY OWNERS CAN EDIT THEIR NETWORK VARIABLES
        if (!character.IsOwner)
        {
            return;
        }

        // WE DO NOT WANT TO REGENERATE STAMINA IF WE ARE USING IT
        if (character.characterNetworkManager.isSprinting.Value)
        {
            return;
        }

        if (character.isPerformingAction)
        {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;
                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // WE ONLY WANT TO RESET THE REGENERATION IF THE ACTION USED STAMINA
        // WE DONT WANT TO RESET THE REGENERATION IF WE ARE ALREADY REGENERATING STAMINA
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }
}

