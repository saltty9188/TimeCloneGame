using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossAirDash : StateMachineBehaviour
{
     #region Inspector fields
    [SerializeField] private float runSpeed = 20;
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
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if((!bossScript.onRight && animator.transform.position.x >= bossScript.upperRight.x) ||
            (bossScript.onRight && animator.transform.position.x <= bossScript.lowerLeft.x))
        {
            rigidbody2D.isKinematic = false;
            animator.SetTrigger("Drop");

            if(bossScript.onRight)
            {
                animator.transform.position = new Vector3(bossScript.lowerLeft.x, animator.transform.position.y, animator.transform.position.z);
            }
            else
            {
                animator.transform.position = new Vector3(bossScript.upperRight.x, animator.transform.position.y, animator.transform.position.z);
            }
        }
       else
        {
            if(bossScript.onRight)
            {
                animator.transform.Translate(new Vector3(-runSpeed * Time.deltaTime, 0, 0));
            }
            else
            {
                animator.transform.Translate(new Vector3(runSpeed * Time.deltaTime, 0, 0));
            }
        }
    }
}
