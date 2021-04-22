using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossMove : StateMachineBehaviour
{
    #region Inspector fields
    [SerializeField] private float moveSpeed;
    #endregion

    #region Private fields
    private bool onRight = true;
    private bool goUp;
    private float hoverHeight;
    private float distanceTraveled;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       goUp = true;
       hoverHeight = animator.transform.position.y + 8;
       distanceTraveled = 0;
       onRight = animator.GetBool("OnRight");
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(goUp)
        {
            animator.transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
            if(animator.transform.position.y >= hoverHeight) goUp = false;
        }
        else
        {
            if(onRight)
            {
                animator.transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
            }
            else
            {
                animator.transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
            }
            distanceTraveled += moveSpeed * Time.deltaTime;
            if(distanceTraveled >= 20)
            {    
                onRight = !onRight;
                animator.SetBool("OnRight", onRight);
                animator.SetBool("Moving", false);
                animator.GetComponents<FirstBossWeakpoint>()[0].HideWeakPoint();
            }
        }
    }
}
