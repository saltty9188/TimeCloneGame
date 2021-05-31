using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossJump class is a StateMachineBehaviour that controls the movement of the boss as it jumps to and from the floating platforms.
/// </summary>
public class SecondBossJump : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("How fast will the boss move horizontally while jumping.")]
    [SerializeField] private float _horizontalSpeed = 3.5f;
    [Tooltip("How fast will the inital jump launch be.")]
    [SerializeField] private float _jumpForce = 10;
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
        if(_bossScript.SecondJump) _bossScript.VerticalSpeed /= 1.5f;
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

        if(animator.transform.position.x >= SecondBossScript.RIGHT_BOUND)
        {
            animator.transform.position = new Vector3(SecondBossScript.RIGHT_BOUND, animator.transform.position.y, animator.transform.position.z);
        }

        if(_bossScript.VerticalSpeed < 0 && _bossScript.transform.position.y <= SecondBossScript.PLATFORM_HEIGHT && !_bossScript.SecondJump)
        {
            animator.SetTrigger("Land");
            animator.transform.position = new Vector3(animator.transform.position.x, SecondBossScript.PLATFORM_HEIGHT, animator.transform.position.z);
            _bossScript.VerticalSpeed = 0;
        } 
        else if(_bossScript.VerticalSpeed < 0 && _bossScript.transform.position.y <= SecondBossScript.LOWER_BOUND && _bossScript.SecondJump)
        {
            animator.SetTrigger("Land");
            animator.transform.position = new Vector3(animator.transform.position.x, SecondBossScript.LOWER_BOUND, animator.transform.position.z);
            _bossScript.VerticalSpeed = 0;
            animator.SetBool("Jump", false);
        }
    }

    /// <summary>
    /// Resets the "Land Count" animator parameter and selects the next ground action if it's the jump to the floor.
    /// </summary>
    /// <remarks>
    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_bossScript.SecondJump)
        {
            _bossScript.Flip();
            animator.SetBool("On Right", true);

            if(Random.Range(0, 4) == 1)
            {
                animator.SetTrigger("Start Over-Run");
            }
            else
            {
                animator.SetTrigger("Start Over-Shoot");
            }
        }
        animator.ResetTrigger("Land");
        _bossScript.SecondJump = false;
        animator.SetInteger("Land Count", 0);
    }
}
