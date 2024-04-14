using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_ParryAction : SMB_BaseAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isInputing", false);
        animator.SetBool("canTransition", false);

        animator.gameObject.GetComponent<Player_EventSystem>().OnPerformParry?.Invoke();
    }
}

