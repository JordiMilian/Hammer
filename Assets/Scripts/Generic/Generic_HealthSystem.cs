using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Generic_EventSystem;

public class Generic_HealthSystem : MonoBehaviour
{
    [SerializeField] Generic_References Refs;
    public FloatReference MaxHP;
    public FloatReference CurrentHP;
    [SerializeField] bool FillHealthOnStart = true;
    [SerializeField] bool isPlayers;

    void Awake()
    {
        if (!isPlayers) { MaxHP.ChangeValue(Refs.stats.MaxHealth); }
        if (FillHealthOnStart) { RestoreAllHealth(); }
    }
    private void OnEnable()
    {

        Refs.genericEvents.OnReceiveDamage += (object sender, ReceivedAttackInfo info) => RemoveLife(info.Damage, info.Attacker);
    }
    private void OnDisable()
    {
        Refs.genericEvents.OnReceiveDamage -= (object sender, ReceivedAttackInfo info) => RemoveLife(info.Damage, info.Attacker);
    }
    public void RemoveLife(float damage, GameObject damager)
    {
        CurrentHP.ChangeValue(CurrentHP.GetValue() - damage);

        if (CurrentHP.GetValue() <= 0f)
        {
            Death(damager);
            CurrentHP.ChangeValue(0);
        }
        if (CurrentHP.GetValue() > MaxHP.GetValue())
        {
            CurrentHP.ChangeValue(MaxHP.Variable.Value);
        }
    }
    public void RestoreAllHealth()
    {
        CurrentHP.ChangeValue(MaxHP.GetValue());
    }
    public virtual void Death(GameObject killer)
    {
        if (Refs.genericEvents.OnDeath != null) Refs.genericEvents.OnDeath(this, new Generic_EventSystem.DeadCharacterInfo(gameObject,killer));
        Destroy(gameObject);
    }
}
