using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossIdle : StateMachineBehaviour
{
    #region Private fields
    private SecondBossScript bossScript;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bossScript = animator.GetComponent<SecondBossScript>();
       bossScript.ResetBoss();
    }
}
