using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FirstBossMove class is a StateMachineBehaviour the moves the boss up and across the arena.
/// </summary>
public class FirstBossMove : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("The movement speed of the boss.")]
    [SerializeField] private float _moveSpeed;
    #endregion

    #region Private fields
    private bool _onRight = true;
    private bool _goUp;
    private float _hoverHeight;
    private float _distanceTraveled;
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
       _goUp = true;
       _hoverHeight = animator.transform.position.y + 8;
       _distanceTraveled = 0;
       _onRight = animator.GetBool("OnRight");
    }

    /// <summary>
    /// Moves the boss up and then across the arena.
    /// </summary>
    /// <remarks>
    /// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_goUp)
        {
            animator.transform.Translate(new Vector3(0, _moveSpeed * Time.deltaTime, 0));
            if(animator.transform.position.y >= _hoverHeight) _goUp = false;
        }
        else
        {
            if(_onRight)
            {
                animator.transform.Translate(new Vector3(_moveSpeed * Time.deltaTime, 0, 0));
            }
            else
            {
                animator.transform.Translate(new Vector3(-_moveSpeed * Time.deltaTime, 0, 0));
            }
            _distanceTraveled += _moveSpeed * Time.deltaTime;
            if(_distanceTraveled >= 20)
            {    
                _onRight = !_onRight;
                animator.SetBool("OnRight", _onRight);
                animator.SetBool("Moving", false);
                animator.GetComponents<FirstBossWeakpoint>()[0].HideWeakPoint();
            }
        }
    }
}
