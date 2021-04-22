using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossJumpShootLaunch : StateMachineBehaviour
{
    #region Inspector fields
    [SerializeField] private float horizontalSpeed = 3;
    [SerializeField] private float jumpForce = 10;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
        bossScript.verticalSpeed = jumpForce;
        animator.SetBool("Jump", true);
        animator.GetComponent<Rigidbody2D>().isKinematic = true;
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
}
