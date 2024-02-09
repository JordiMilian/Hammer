using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_HealthSystem : Generic_HealthSystem
{
    [SerializeField] GameObject deadBody;
    [SerializeField] GameObject deadHead;
    public override void Death()
    {
        if (eventSystem.OnDeath != null) eventSystem.OnDeath(this, EventArgs.Empty);
        if (deadBody != null) { var DeadBody = Instantiate(deadBody, transform.position, Quaternion.identity); }
        if (deadHead != null) { var DeadHead = Instantiate(deadHead, transform.position, Quaternion.identity); }
        TimeScaleEditor.Instance.SlowMotion(80, 2);
    }

}
