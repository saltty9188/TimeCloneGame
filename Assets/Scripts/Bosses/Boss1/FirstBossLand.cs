using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossLand : StateMachineBehaviour
{
    #region Private fields
    private Rigidbody2D rigidbody2D;
    private float distUp = 8;
    private float distanceFallen;
    private float fallSpeed;
    #endregion

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        distanceFallen = 0;
        rigidbody2D = animator.GetComponent<Rigidbody2D>();
        fallSpeed = distUp / (stateInfo.length - 0.1f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(distanceFallen < distUp)
        {
            //animator.transform.Translate(new Vector3(0, -fallSpeed * Time.deltaTime, 0));
            rigidbody2D.velocity = new Vector2(0, -fallSpeed);
            distanceFallen += fallSpeed * Time.deltaTime;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rigidbody2D.velocity = Vector2.zero;
    }   
}
