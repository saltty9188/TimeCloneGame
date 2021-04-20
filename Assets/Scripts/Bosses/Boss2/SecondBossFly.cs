using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossFly : StateMachineBehaviour
{
    #region Inspector fields
    [SerializeField] private float verticalSpeed = 4;
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
        animator.transform.Translate(new Vector3(0, verticalSpeed * Time.deltaTime, 0));

        if(animator.transform.position.y >= bossScript.upperRight.y)
        {
            animator.transform.position = new Vector3(animator.transform.position.x, bossScript.upperRight.y, animator.transform.position.z);
            animator.SetBool("Jump", true);
        }
    }
}
