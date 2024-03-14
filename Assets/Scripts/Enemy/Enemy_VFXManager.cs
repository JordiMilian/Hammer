using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy_VFXManager : MonoBehaviour
{
    [SerializeField] GameObject StanceBrokenVFX;
    [SerializeField] GameObject SucesfullParryVFX;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform GroundBlood_Offset;
    [SerializeField] VisualEffect GroundBloodVFX;
    [SerializeField] Enemy_EventSystem eventSystem;
    Transform playerTf;
   
    
    private void OnEnable()
    {
        playerTf = GameObject.Find(TagsCollection.Instance.MainCharacter).transform;
        eventSystem.OnStanceBroken += InstantiateStanceBrokenVFX;
        eventSystem.OnSuccessfulParry += InstantiateSuccesfulParryVFX;
        eventSystem.OnReceiveDamage += PlayGroundBlood;
    }
    private void OnDisable()
    {
        eventSystem.OnStanceBroken -= InstantiateStanceBrokenVFX;
        eventSystem.OnSuccessfulParry -= InstantiateSuccesfulParryVFX;
    }
    void InstantiateStanceBrokenVFX()
    {
        Instantiate(StanceBrokenVFX,transform.position,Quaternion.identity);
    }
    void InstantiateSuccesfulParryVFX(object sender, Generic_EventSystem.EventArgs_SuccesfulParryInfo args)
    {
        Instantiate(SucesfullParryVFX, args.vector3data, Quaternion.identity);
    }
    public void EV_ShowTrail()
    {
        trailRenderer.enabled = true;
    }
    public void EV_HideTrail()
    {
        trailRenderer.enabled= false;
    }
    void PlayGroundBlood(object sender, Generic_EventSystem.EventArgs_ReceivedAttackInfo args)
    {
        Vector2 thisPosition = transform.position;
        Vector2 otherPosition = args.Attacker.transform.root.position;
        Vector2 opositeDirection = (thisPosition - otherPosition).normalized;

        GroundBlood_Offset.up = opositeDirection;
        GroundBloodVFX.Play();
    }
  
}
