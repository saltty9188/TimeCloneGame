using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossJumpShoot : StateMachineBehaviour
{

    #region Inspector fields
    [SerializeField] private float horizontalSpeed = 3;
    [SerializeField] private float jumpForce = 600;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    private Rigidbody2D rigidbody2D;
    private int landCount;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(bossScript.onRight)
        {
            animator.transform.Translate(new Vector3(-horizontalSpeed * Time.deltaTime, 0, 0));
        }
        else
        {
            animator.transform.Translate(new Vector3(horizontalSpeed * Time.deltaTime, 0, 0));
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.shootingLandCount++;
        Debug.Log(bossScript.shootingLandCount);
        animator.ResetTrigger("Land");

        rigidbody2D.isKinematic = true;
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
    }
}
