using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FirstBossLand class is the StateMachineBehaviour controls the FirstBoss movement while it lands.
/// </summary>
public class FirstBossLand : StateMachineBehaviour
{
    #region Private fields
    private Rigidbody2D _rigidbody2D;
    private float _distUp = 8;
    private float _distanceFallen;
    private float _fallSpeed;
    #endregion

    /// <summary>
    /// Sets the inital values whenever this animation state is entered.
    /// </summary>
    /// <remarks>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state.
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distanceFallen = 0;
        _rigidbody2D = animator.GetComponent<Rigidbody2D>();
        _fallSpeed = _distUp / (stateInfo.length - 0.1f);
    }

    /// <summary>
    /// Moves the boss from it's position in the air to the ground.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks.
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_distanceFallen < _distUp)
        {
            _rigidbody2D.velocity = new Vector2(0, -_fallSpeed);
            _distanceFallen += _fallSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Resets the boss's velocity.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody2D.velocity = Vector2.zero;
    }   
}
