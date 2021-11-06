using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthHandler))]
public class Healer : MonoBehaviour
{
    [SerializeField] ParticleSystem healthParticles;
    PlayerBehavior player;
    HealthHandler healthHandler;
    ParticleSystem.EmissionModule emissions;
    // Start is called before the first frame update
    void Start()
    {
        emissions = healthParticles.emission;
        player = GetComponentInChildren<PlayerBehavior>();
        healthHandler = GetComponent<HealthHandler>();
    }
    void OnHeal(OnHealParams healParams)
    {
        bool hasRoomToHeal = 1 - player.playerData.currentHunger > player.playerSettings.hungerRegen || healthHandler.currentHealth < healthHandler.maxHealth;
        if (player.playerData.bloodAmount >= healParams.woodConsumed && hasRoomToHeal)
        {
            healParams.campfire.sootLevel = Mathf.Min(healParams.campfire.sootAccumulationRate + healParams.campfire.sootLevel, 1f);
            player.playerData.bloodAmount -= healParams.woodConsumed;
            player.playerData.currentHunger = Mathf.Min(player.playerData.currentHunger + player.playerSettings.hungerRegen, 1f);
            int amountToHeal = Mathf.Min(healthHandler.maxHealth - healthHandler.currentHealth, healParams.healAmount);
            healthHandler.currentHealth += amountToHeal;
            GlobalEventSubject.RaisePlayerDataUpdatedEvent(player.playerData);
            emissions.enabled = true;
            healthParticles.Emit(30);
            emissions.enabled = false;
        }
        
        
    }
}

public class OnHealParams
{
    public int healAmount;
    public int woodConsumed;
    public CampfireHandler campfire;
}