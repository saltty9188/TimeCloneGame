using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossJumpShootLaunch class is a StateMachineBehaviour that controls the movement of the boss as it initially jumps from a platform before
/// it begins shooting.
/// </summary>
public class SecondBossJumpShootLaunch : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("How fast the boss will travel horizontally while shooting.")]
    [SerializeField] private float _horizontalSpeed = 3;
    [Tooltip("How fast will the inital jump launch be.")]
    [SerializeField] private float _jumpForce = 7.5f;
    #endregion

    #region Private fields
    private SecondBossScript _bossScript;
    #endregion

    /// <summary>
    /// Sets the inital values whenever this animation state is entered including setting the jump force.
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
        _bossScript.VerticalSpeed = _jumpForce;
        animator.SetBool("Jump", true);
        animator.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    /// <summary>
    /// Moves the boss across as it jumps and reduced the vertical speed of the boss as if to simulate gravity.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Translate(new Vector3(_horizontalSpeed, _bossScript.VerticalSpeed, 0)  * Time.deltaTime);   
        _bossScript.VerticalSpeed -= 9.8f * Time.deltaTime;
    }
}
