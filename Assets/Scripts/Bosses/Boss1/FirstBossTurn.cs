using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossTurn : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        animator.transform.position = new Vector3(animator.transform.position.x, animator.GetComponent<FirstBossScript>().initialPosition.y, animator.transform.position.z);
    }
}
