using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] Animator player_Animator;
    //Player_ComboSystem comboSystem;
    [SerializeField] Player_ComboSystem_chargeless comboSystem;

    [Header("BASE MOVEMENT")]
    public float CurrentSpeed;
    public float BaseSpeed;

    [Header("RUN")]
    [SerializeField] float RunningSpeed = 40;
    [SerializeField] float RunInputDelayTime = 0.5f;
    float TimerDelay;
    bool IsWaitingInputDelay;
    bool isRunning = false;


    [Header("ROLL")]
    public bool canDash = true;
    bool isWaitingDash;
    public bool isDashing;
    [SerializeField] float RollTime;
    [SerializeField] float RollMaxForce;
    [SerializeField] float RollCooldown;
    public AnimationCurve RollCurve;
    [SerializeField] Collider2D damageCollider;

    [SerializeField] Player_EventSystem eventSystem;
    [SerializeField] FloatVariable playerStamina;
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        player_Animator = GetComponent<Animator>();
        
        CurrentSpeed = BaseSpeed;
    }

    
    void Update()
    {
        Vector2 input = new Vector2(x: Input.GetAxisRaw("Horizontal"), y: Input.GetAxisRaw("Vertical"));
        Move(input);

        if (isRunning)
        {
            CurrentSpeed = RunningSpeed;
            player_Animator.SetBool("Running", true);
        }
       
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))
        {
            IsWaitingInputDelay = true;
            TimerDelay = 0;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space))
        {
            DelayInput();
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.Space))
        {
            if ((Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) == (0, 0)) { return; }
            if(playerStamina.Value == 0) { return; }

            isRunning = false;
            CurrentSpeed = BaseSpeed;
            player_Animator.SetBool("Running", false);
            if (IsWaitingInputDelay)
            {
                IsWaitingInputDelay = false;
                switch (canDash)
                {
                    case true:
                        StartCoroutine(Dash());
                        break;
                    case false:
                        if (!isWaitingDash) { StartCoroutine(WaitForCanDash()); }

                        break;
                }
            } 
        }
    }
    void Move(Vector2 vector2)
    {
        rigidbody.AddForce(vector2.normalized * CurrentSpeed * Time.deltaTime * 100);
        WalkingAnimation();
    }
  
    void DelayInput()
    { 
        TimerDelay += Time.deltaTime;
       
        if(TimerDelay > RunInputDelayTime)
        {
            IsWaitingInputDelay = false;
            isRunning = true;
        }

    }
    void WalkingAnimation()
    {
        if (!isDashing)
        {
            if ((Input.GetAxisRaw("Horizontal") != 0) || (Input.GetAxisRaw("Vertical") != 0))
            {
                player_Animator.SetBool("Walking", true);
            }
            else
            {
                player_Animator.SetBool("Walking", false);
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        comboSystem.canAttack = false;
        isDashing = true;
       //comboSystem.isCurrentAttackCanceled = true;
        if(eventSystem.OnPerformRoll != null) eventSystem.OnPerformRoll(this, EventArgs.Empty);
        player_Animator.SetTrigger("Roll");
        eventSystem.OnStaminaAction?.Invoke(this, new Generic_EventSystem.EventArgs_StaminaConsumption(1f));

        Vector2 Axis = new Vector2(x: Input.GetAxisRaw("Horizontal"), y: Input.GetAxisRaw("Vertical")).normalized;

        float time = 0;
        float weight = 0;
        while (time < RollTime)
        {
            time = time + Time.deltaTime;
            weight = RollCurve.Evaluate(time/RollTime);
            rigidbody.AddForce(Axis * RollMaxForce * weight* Time.deltaTime);
            yield return null;
        }
        isDashing = false;
        yield return new WaitForSeconds(RollCooldown);
        canDash = true;

    }
    IEnumerator WaitForCanDash()
    {
        isWaitingDash = true;
        while (!canDash) 
        {
            yield return null;
        }
        isWaitingDash = true;
        StartCoroutine(Dash());
    }
    public void EV_SlowDownSpeed() { CurrentSpeed /= 2; }
    public void EV_ReturnSpeed() { CurrentSpeed = BaseSpeed; }
    public void EV_CantDash() { canDash = false; }
    public void EV_CanDash() { canDash = true; }
    public void EV_HidePlayerCollider() { damageCollider.enabled = false; }
    public void EV_ShowPlayerCollider() { damageCollider.enabled = true; }
}
