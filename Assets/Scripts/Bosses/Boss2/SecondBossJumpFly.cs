using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossJumpFly class is a StateMachineBehaviour that controls the movement of the boss as it jumps upwards to start flying.
/// </summary>
public class SecondBossJumpFly : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("How fast will the boss be when it jumps.")]
    [SerializeField] private float _verticalSpeed = 3;
    #endregion

    /// <summary>
    /// Moves the upwards as it jumps.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Translate(new Vector3(0, _verticalSpeed * Time.deltaTime, 0));
    }

     /// <summary>
    /// Resets the "Fly" trigger.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Fly");
    }
}
