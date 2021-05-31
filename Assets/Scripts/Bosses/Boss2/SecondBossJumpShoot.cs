using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossJumpShoot class is a StateMachineBehaviour that controls the movement of the boss as it jumps across the platforms while shooting.
/// </summary>
public class SecondBossJumpShoot : StateMachineBehaviour
{

    #region Inspector fields
    [SerializeField] private float _horizontalSpeed = 5;
    #endregion

    #region Private fields
    private SecondBossScript bossScript;
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
        animator.transform.Translate(new Vector3(_horizontalSpeed, bossScript.VerticalSpeed, 0)  * Time.deltaTime);
        
        bossScript.VerticalSpeed -= 9.8f * Time.deltaTime;

        if(bossScript.VerticalSpeed < 0 && bossScript.transform.position.y <= SecondBossScript.PLATFORM_HEIGHT)
        {
            animator.SetTrigger("Land");
            animator.transform.position = new Vector3(animator.transform.position.x, SecondBossScript.PLATFORM_HEIGHT, animator.transform.position.z);
            bossScript.VerticalSpeed = 0;
        }
    }

    /// <summary>
    /// Increments the "Land Count" animator parameter.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossScript.SecondJump = true;
        animator.SetInteger("Land Count", animator.GetInteger("Land Count") + 1);
        animator.ResetTrigger("Land");
    }
}
