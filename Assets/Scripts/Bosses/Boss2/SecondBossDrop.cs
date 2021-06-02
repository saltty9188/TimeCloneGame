using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossDrop class is a StateMachineBehaviour that controls the movement of the boss as it drops to the ground.
/// </summary>
public class SecondBossDrop : StateMachineBehaviour
{

    #region Private fields
    private SecondBossScript _bossScript;
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
        _bossScript = animator.GetComponent<SecondBossScript>();
        _bossScript.VerticalSpeed = 0;
    }

    /// <summary>
    /// Moves the boss down while falling.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Translate(new Vector3(0, _bossScript.VerticalSpeed, 0)  * Time.deltaTime);
        
        _bossScript.VerticalSpeed -= 9.8f * Time.deltaTime;

        if(_bossScript.VerticalSpeed < 0 && _bossScript.transform.position.y <= SecondBossScript.LOWER_BOUND)
        {
            animator.SetTrigger("Land");
            animator.SetBool("Jump", false);
            animator.transform.position = new Vector3(animator.transform.position.x, SecondBossScript.LOWER_BOUND, animator.transform.position.z);
            _bossScript.VerticalSpeed = 0;
        }
    }

    /// <summary>
    /// Sets the boss to being on the left, flips the sprite, and chooses the next action.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossScript.Flip();
        animator.SetBool("On Right", true);
        animator.ResetTrigger("Drop");
        animator.ResetTrigger("Land");

        if(Random.Range(0, 4) == 1)
        {
            animator.SetTrigger("Start Over-Run");
        }
        else
        {
            animator.SetTrigger("Start Over-Shoot");
        }
    }

}
