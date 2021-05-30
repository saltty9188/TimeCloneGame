using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FirstBossTurn class is the StateMachineBehaviour that controls the first boss's position and velocity while it turns.
/// </summary>
public class FirstBossTurn : StateMachineBehaviour
{
    /// <summary>
    /// Resets the velocity to zero and ensures the boss is touching the floor while it turns.
    /// </summary>
    /// <remarks>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state.
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        animator.transform.position = new Vector3(animator.transform.position.x, animator.GetComponent<FirstBossScript>().InitialPosition.y, animator.transform.position.z);
    }
}
