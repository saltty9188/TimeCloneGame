using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossJump : StateMachineBehaviour
{
    #region Inspector fields
    [SerializeField] private float horizontalSpeed = 10;
    [SerializeField] private float jumpForce = 600;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    private Rigidbody2D rigidbody2D;
    private bool firstFrame;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript = animator.GetComponent<SecondBossScript>();
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
        rigidbody2D.isKinematic = false;
        firstFrame = true;
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

        if(firstFrame)
        {
            float modifiedJumpForce = jumpForce / (bossScript.shootingLandCount > 0 ? 1.5f : 1);

            rigidbody2D.AddForce(new Vector2(0, modifiedJumpForce));
            animator.SetBool("Jump", true);
            firstFrame = false;
        }

        if(bossScript.onRight && animator.transform.position.x <= bossScript.lowerLeft.x)
        {
            animator.transform.position = new Vector3(bossScript.lowerLeft.x, animator.transform.position.y, animator.transform.position.z);
        }
        else if(!bossScript.onRight && animator.transform.position.x >= bossScript.upperRight.x)
        {
            animator.transform.position = new Vector3(bossScript.upperRight.x, animator.transform.position.y, animator.transform.position.z);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(bossScript.shootingLandCount > 0)
        {
            bossScript.Flip();
            bossScript.onRight = !bossScript.onRight;
            animator.SetBool("On Right", bossScript.onRight);

            if(Random.Range(0, 4) == 1)
            {
                animator.SetTrigger("Start Over-Run");
            }
            else
            {
                animator.SetTrigger("Start Over-Shoot");
            }
            
        }
        bossScript.shootingLandCount = 0;
    }
}
