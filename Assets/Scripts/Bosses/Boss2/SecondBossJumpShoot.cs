using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossJumpShoot : StateMachineBehaviour
{

    #region Inspector fields
    [SerializeField] private float horizontalSpeed = 3;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(bossScript.onRight)
        {
            animator.transform.Translate(new Vector3(-horizontalSpeed, bossScript.verticalSpeed, 0)  * Time.deltaTime);
        }
        else
        {
            animator.transform.Translate(new Vector3(horizontalSpeed, bossScript.verticalSpeed, 0)  * Time.deltaTime);
        }
        bossScript.verticalSpeed -= 9.8f * Time.deltaTime;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.shootingLandCount++;
        animator.ResetTrigger("Land");
    }
}
