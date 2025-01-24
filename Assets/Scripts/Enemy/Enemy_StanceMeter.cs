using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Generic_EventSystem;

public class Enemy_StanceMeter : MonoBehaviour
{
    [SerializeField] Enemy_References enemyRefs;
    
    [SerializeField] float CurrentStance;
    [SerializeField] float CooldownAfterDamage;
    [SerializeField] float RecoveryPerSecond;
    [SerializeField] Enemy_EventSystem eventSystem;
    [SerializeField] Animator animator;
    bool isRecovering;

    //Each enemy has some Stance, whenever its damaged, the amount of damage is removed from the Stance meter.
    //There is a cooldown after damaged to slowly recover stance if not damaged for a while
    //If the stance reaches 0 the stance event is called and the animation, which cancels the current attack or action.
    //After breaking the stance the enemy will recover all their stance instantly 

    private void OnEnable()
    {
        eventSystem.OnReceiveDamage += RemoveStance;
    }
    private void OnDisable()
    {
        eventSystem.OnReceiveDamage -= RemoveStance;
    }
    private void Start()
    {
        CurrentStance = enemyRefs.baseEnemyStats.MaxStance;
    }
    void RemoveStance(object sender, ReceivedAttackInfo receivedAttackInfo)
    {

        CurrentStance -= receivedAttackInfo.Damage;
        if (CurrentStance <= 0)
        {
            CurrentStance = enemyRefs.baseEnemyStats.MaxStance;
            eventSystem.OnStanceBroken?.Invoke();
        }
        else
        {
            StartCoroutine(Cooldown(CooldownAfterDamage));
        }
        
    }
    IEnumerator Cooldown(float cooldownSeconds)
    {
        isRecovering = false;
        yield return new WaitForSeconds(cooldownSeconds);
        isRecovering = true;
    }
    private void Update()
    {
        if(isRecovering)
        {
            CurrentStance += Time.deltaTime * RecoveryPerSecond;
            if (CurrentStance > enemyRefs.baseEnemyStats.MaxStance) 
            {
                CurrentStance = enemyRefs.baseEnemyStats.MaxStance; 
                isRecovering = false;
            }
        }
    }
    float TemporalBaseMaxStance;
    public void MakeStanceUnbreakeable()
    {
        TemporalBaseMaxStance = enemyRefs.baseEnemyStats.MaxStance;
        enemyRefs.baseEnemyStats.MaxStance = 9999;
        CurrentStance = 9999;
    }
    public void ReturnToRegularStance()
    {
        enemyRefs.baseEnemyStats.MaxStance = TemporalBaseMaxStance;
        CurrentStance = enemyRefs.baseEnemyStats.MaxStance;
    }
}
