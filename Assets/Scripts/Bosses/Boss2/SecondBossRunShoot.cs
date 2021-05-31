using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossRunShoot class is a StateMachineBehaviour that controls the movement of the boss as it runs on the while shooting.
/// </summary>
public class SecondBossRunShoot : StateMachineBehaviour
{

    #region Inspector fields
    [Tooltip("How fast the boss will move while running.")]
    [SerializeField] private float _runSpeed = 15;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
    private bool nextStateSet;
    #endregion

    /// <summary>
    /// Sets the inital values whenever this animation state is entered.
    /// </summary>
    /// <remarks>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       bossScript = animator.GetComponent<SecondBossScript>();
       bossScript.transform.position = new Vector3(SecondBossScript.RIGHT_BOUND, SecondBossScript.LOWER_BOUND, 0);
       animator.GetComponent<Rigidbody2D>().isKinematic = true;
       nextStateSet = false;
    }

    /// <summary>
    /// Moves the boss across the arena and sets the next state trigger once it reaches the other end of the arena.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Translate(new Vector3(-_runSpeed * Time.deltaTime, 0, 0));
    
       if(animator.transform.position.x <= SecondBossScript.LEFT_BOUND)
       {
            animator.transform.position = new Vector3(SecondBossScript.LEFT_BOUND, animator.transform.position.y, animator.transform.position.z);

            if(!nextStateSet)
            {
                if(Random.Range(0, 4) == 1)
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

    /// <summary>
    /// Sets the boss to being on the left and flips the sprite.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Start Over-Shoot");
        animator.SetBool("On Right", false);
        bossScript.Flip();
    }
}
