using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_AttacksProviderV2 : MonoBehaviour
{
    [SerializeField] Enemy_References enemyRefs;

    public bool isAttacking;
    public bool isProviding;
    public bool PlayerIsInAnyRange;
    [SerializeField] bool ShowDebug;
    

    public EnemyAttack[] Enemy_Attacks = new EnemyAttack[4];
    
    Coroutine CurrentWaiting;

    [Serializable]
    public class EnemyAttack
    {
        public string ShortDescription;
        public Enemy_AttackRangeDetector rangeDetector;
        [HideInInspector] public BoxCollider2D boxCollider;
        [HideInInspector] public bool isActive;
        
        [Header("Stats:")]
        public float Damage;
        public int Probability;
        public float KnockBack;
        public float Hitstop;
        public string TriggerName;
        public AnimationClip animationClip;
        [Header("Cooldown")]
        [HideInInspector] public bool isInCooldown;
        public bool HasCooldown;
        public float CooldownTime;
  
        public  void isInRange(object sender, EventArgs args) { isActive = true; }
        public void isNotInRange(object sender, EventArgs args) { isActive = false; }
         
        public IEnumerator Cooldown()
        {
            isInCooldown = true;
            yield return new WaitForSeconds(animationClip.length + CooldownTime);
            isInCooldown = false;
        }
    }
    private void OnEnable()
    {
        enemyRefs.enemyEvents.OnGettingParried += OnCancelAttack;
        enemyRefs.enemyEvents.OnStanceBroken += OnCancelAttack;
        foreach (EnemyAttack attack in Enemy_Attacks)
        {
            attack.rangeDetector.OnPlayerEntered += attack.isInRange;
            attack.rangeDetector.OnPlayerExited += attack.isNotInRange;
            attack.rangeDetector.OnPlayerEntered += RunChecker;
            attack.rangeDetector.OnPlayerExited += RunChecker;
        }
    }
    private void OnDisable()
    {
        enemyRefs.enemyEvents.OnGettingParried -= OnCancelAttack;
        enemyRefs.enemyEvents.OnStanceBroken -= OnCancelAttack;
        foreach (EnemyAttack attack in Enemy_Attacks)
        {
            attack.rangeDetector.OnPlayerEntered -= attack.isInRange;
            attack.rangeDetector.OnPlayerExited -= attack.isNotInRange;
            attack.rangeDetector.OnPlayerEntered -= RunChecker;
            attack.rangeDetector.OnPlayerExited -= RunChecker;
        }
    }
    public void RunChecker(object sender, EventArgs args)
    {
        PlayerIsInAnyRange = CheckIfPlayerIsInAnyRange();
    }
    public bool CheckIfPlayerIsInAnyRange()
    {
        foreach (EnemyAttack attack in Enemy_Attacks)
        {
            if (attack.isActive == true) return (true);
        }
        return (false);
    }
    private void OnDrawGizmosSelected()
    {
        foreach (EnemyAttack attack in Enemy_Attacks)
        {
            if (attack.isActive) Gizmos.color = Color.blue;
            else Gizmos.color = Color.red;

            BoxCollider2D boxCollider = attack.rangeDetector.ownCollider;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCollider.transform.position, boxCollider.transform.rotation, boxCollider.transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
    }
    void Update()
    {
        if (PlayerIsInAnyRange)
        {
            if (!isAttacking && isProviding) 
            {
                ResetAllTriggers();
                PickAvailableAttacks();
            }
        }
    }

    public void PerformAttack(EnemyAttack selectedAttack)
    {
        isAttacking = true;

        enemyRefs.damageDealer.Damage = selectedAttack.Damage * enemyRefs.stats.DamageMultiplier;
        enemyRefs.damageDealer.Knockback = selectedAttack.KnockBack;
        enemyRefs.damageDealer.HitStop = selectedAttack.Hitstop;
        enemyRefs.animator.SetTrigger(selectedAttack.TriggerName);
        enemyRefs.animator.SetBool("isAttacking", true);

        if (CurrentWaiting != null) { StopCoroutine(CurrentWaiting); }
        CurrentWaiting = StartCoroutine(WaitAnimationTime(selectedAttack));

        if(selectedAttack.HasCooldown)
        {
            StartCoroutine(selectedAttack.Cooldown());
        }
    }
    IEnumerator WaitAnimationTime(EnemyAttack selectedAttack)
    {
        yield return new WaitForSeconds(selectedAttack.animationClip.length);
        isAttacking = false;
        enemyRefs.animator.SetBool("isAttacking", false);
        if(enemyRefs.enemyEvents.OnAttackFinished != null) { enemyRefs.enemyEvents.OnAttackFinished(); }
    }
    void PickAvailableAttacks()
    {
        //Make a list of all active attacks and not in Cooldown
        List<EnemyAttack> ActiveAttacks = new List<EnemyAttack>();
        foreach (EnemyAttack attack in Enemy_Attacks)
        {
            if (attack.isActive && !attack.isInCooldown)
            {
                ActiveAttacks.Add(attack);
            }
        }
        //Add up all the probabilities and take a random number
        float ActiveAttacksProbability = AddAttacksProbability(ActiveAttacks);
        Debuguer("All Active attacks combined make: " + ActiveAttacksProbability);

        float randomFloat = UnityEngine.Random.Range(0, ActiveAttacksProbability);
        Debuguer("Random float is: " + randomFloat);

        //Check if the random number matches with the probabilities of the active attacks
        for (int i = 0; i < ActiveAttacks.Count; i++)
        {
            float probabilitatMinima = 0;
            for (int u = 0; u < i; u++)
            {
                probabilitatMinima += ActiveAttacks[u].Probability;
            }
            if ((randomFloat > probabilitatMinima) && ( randomFloat < probabilitatMinima + ActiveAttacks[i].Probability))
            {
                PerformAttack(ActiveAttacks[i]);
                Debuguer(i + " succede chance");
            }
            else
            {
                Debuguer(i + " failed chance");
            }
        }
    }
    float AddAttacksProbability(List<EnemyAttack> attacks)
    {
        float Probabilities = 0;
        foreach (EnemyAttack attack in attacks) { Probabilities += attack.Probability; }
        return Probabilities;
    }
    public void ResetAllTriggers()
    {
        foreach (var param in enemyRefs.animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                enemyRefs.animator.ResetTrigger(param.name);
            }
        }
    }
    void OnCancelAttack()
    {
        if (CurrentWaiting != null) { StopCoroutine(CurrentWaiting); } 
        StartCoroutine(WaitForCurrentAnimation());
    }
    IEnumerator WaitForCurrentAnimation()
    {
        while(enemyRefs.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        isAttacking = false;
    }
    void Debuguer(string text)
    {
        if (ShowDebug) { Debug.Log(text); }
    }
}
