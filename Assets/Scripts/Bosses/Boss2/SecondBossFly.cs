using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossFly class is a StateMachineBehaviour that controls the movement of the boss as it flies upwards.
/// </summary>
public class SecondBossFly : StateMachineBehaviour
{
    #region Inspector fields
    [Tooltip("How fast will the boss be when flying up.")]
    [SerializeField] private float _verticalSpeed = 3;
    #endregion

    /// <summary>
    /// Moves the upwards until it reaches the upper bound.
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

        if(animator.transform.position.y >= SecondBossScript.UPPER_BOUND)
        {
            animator.transform.position = new Vector3(animator.transform.position.x, SecondBossScript.UPPER_BOUND, animator.transform.position.z);
            animator.SetBool("Jump", true);
        }
    }
}
