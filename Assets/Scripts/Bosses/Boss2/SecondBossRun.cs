using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossRun class is a StateMachineBehaviour that controls the movement of the boss as it runs on the ground.
/// </summary>
public class SecondBossRun : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("How fast the boss will move while running.")]
    [SerializeField] private float _runSpeed = 10;
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
       animator.transform.position = new Vector3(SecondBossScript.RIGHT_BOUND, SecondBossScript.LOWER_BOUND, 0);
       animator.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    /// <summary>
    /// Moves the boss across the arena.
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
    }

    /// <summary>
    /// Resets the "Start Over-Run" trigger.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Start Over-Run");
    }

}
