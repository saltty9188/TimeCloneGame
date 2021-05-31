using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossAirDash class is a StateMachineBehaviour that controls the movement of the boss as it dashes in the air.
/// </summary>
public class SecondBossAirDash : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("The speed the boss will move while dashing.")]
    [SerializeField] private float _runSpeed = 20;
    #endregion

    #region Private fields
    private Rigidbody2D _rigidbody2D;
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
       _rigidbody2D = animator.GetComponent<Rigidbody2D>();
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
        if(animator.transform.position.x >= SecondBossScript.RIGHT_BOUND)
        {
            animator.SetTrigger("Drop");

            animator.transform.position = new Vector3(SecondBossScript.RIGHT_BOUND, animator.transform.position.y, animator.transform.position.z);
        }
        else
        {
            animator.transform.Translate(new Vector3(_runSpeed * Time.deltaTime, 0, 0));    
        }
    }
}
