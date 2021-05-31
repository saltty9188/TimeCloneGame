using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossDashGround class is a StateMachineBehaviour that controls the movement of the boss as it dashes on the ground.
/// </summary>
public class SecondBossDashGround : StateMachineBehaviour
{
     #region Inspector fields
     [Tooltip("How fast will the bos be when dashing.")]
    [SerializeField] private float _runSpeed = 20;
    #endregion

    #region Private fields
    private bool _nextStateSet;
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
       _nextStateSet = false;
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

            if(!_nextStateSet)
            {
                if(Random.Range(0, 4) == 0)
                {
                    animator.SetTrigger("Fly");
                }
                else
                {
                    animator.SetBool("Jump", true);
                }
                _nextStateSet = true;
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
        animator.SetBool("On Right", false);
        animator.GetComponent<SecondBossScript>().Flip();
    }
}
