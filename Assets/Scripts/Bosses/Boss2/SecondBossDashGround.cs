using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossDashGround : StateMachineBehaviour
{
     #region Inspector fields
    [SerializeField] private float runSpeed = 20;

    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    private bool nextStateSet;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bossScript = animator.GetComponent<SecondBossScript>();
       nextStateSet = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(bossScript.onRight)
       {
           animator.transform.Translate(new Vector3(-runSpeed * Time.deltaTime, 0, 0));
       }
       else
       {
           animator.transform.Translate(new Vector3(runSpeed * Time.deltaTime, 0, 0));
       }

       if((!bossScript.onRight && animator.transform.position.x >= bossScript.upperRight.x) ||
            (bossScript.onRight && animator.transform.position.x <= bossScript.lowerLeft.x))
       {
           if(bossScript.onRight)
           {
               animator.transform.position = new Vector3(bossScript.lowerLeft.x, animator.transform.position.y, animator.transform.position.z);
           }
           else
           {
               animator.transform.position = new Vector3(bossScript.upperRight.x, animator.transform.position.y, animator.transform.position.z);
           }

            if(!nextStateSet)
            {
                if(Random.Range(0, 4) == 0)
                {
                    animator.SetTrigger("Fly");
                }
                else
                {
                    animator.SetBool("Jump", true);
                }
                nextStateSet = true;
            }
       }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.onRight = !bossScript.onRight;
        animator.SetBool("On Right", bossScript.onRight);
        bossScript.Flip();
    }
}
