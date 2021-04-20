using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossJumpShootLaunch : StateMachineBehaviour
{
    #region Inspector fields
    [SerializeField] private float horizontalSpeed = 3;
    [SerializeField] private float jumpForce = 600;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    private Rigidbody2D rigidbody2D;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
        rigidbody2D.isKinematic = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(!animator.GetBool("Jump"))
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("Jump", true);
        }

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
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

}
