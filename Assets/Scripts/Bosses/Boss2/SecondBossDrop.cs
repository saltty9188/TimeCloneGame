using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossDrop : StateMachineBehaviour
{

    #region Private fields
    private SecondBossScript bossScript;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.Flip();
        bossScript.onRight = !bossScript.onRight;
        animator.SetBool("On Right", bossScript.onRight);
        animator.ResetTrigger("Drop");

        if(Random.Range(0, 4) == 1)
        {
            animator.SetTrigger("Start Over-Run");
        }
        else
        {
            animator.SetTrigger("Start Over-Shoot");
        }
    }

}
