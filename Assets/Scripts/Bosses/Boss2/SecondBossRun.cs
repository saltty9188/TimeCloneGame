using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossRun : StateMachineBehaviour
{

    #region Inspector fields
    [SerializeField] private float runSpeed = 10;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bossScript = animator.GetComponent<SecondBossScript>();

       Rigidbody2D rigidbody2D = animator.GetComponent<Rigidbody2D>();
       rigidbody2D.velocity = new Vector2(0, 0);
       rigidbody2D.isKinematic = true;
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
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Start Over-Run");
    }

}
