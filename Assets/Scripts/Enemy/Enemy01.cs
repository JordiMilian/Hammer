using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using static Generic_DamageDetector;

public class Enemy01 : MonoBehaviour
{

    public Collider2D WeaponCollider;
    [SerializeField] Animator EnemyAnimator;
    [SerializeField] Enemy_Movement enemyMovement;
    [SerializeField] HitStop hitStop;
    [SerializeField] Generic_Flash flasher;
    [SerializeField] TrailRenderer WeaponTrail;
    [SerializeField] Generic_DamageDetector damageDetector;

    private void Awake()
    {
        hitStop = FindObjectOfType<HitStop>();
    }
    private void OnEnable()
    {
        damageDetector.OnReceiveDamage += ReceiveDamage;
    }
    private void OnDisable()
    {
        damageDetector.OnReceiveDamage -= ReceiveDamage;
    }
    public void ReceiveDamage(object sender, EventArgs_ReceivedAttackInfo receivedAttackinfo)
    {
        
        flasher.CallFlasher();
        enemyMovement.EV_SlowRotationSpeed();
        enemyMovement.EV_SlowMovingSpeed();
        enemyMovement.IsAgroo = true;
        hitStop.Stop(0.05f);
        EnemyAnimator.SetTrigger("PushBack");
        StartCoroutine(WaitReceiveDamage());
       
    }  
    IEnumerator WaitReceiveDamage()
    {
        yield return new WaitForSeconds(0.3f);
        enemyMovement.EV_ReturnAllSpeed();
    }
    public void HitShield()
    {
        EnemyAnimator.SetBool("HitShield", true);
        WeaponCollider.enabled = false;
    }
    public void EndHitShield()
    {
        EnemyAnimator.SetBool("HitShield", false);
    }

    public void Enemy_ShowAttackCollider()
    {
        WeaponCollider.enabled = true;
    }
    public void Enemy_HideAttackCollider()
    {
        
        enemyMovement.EV_ReturnRotationSpeed();
    }
    
    public void ShowTrail() { WeaponTrail.enabled = true; }
    public void HideTrail() { WeaponTrail.enabled = false; }
}
