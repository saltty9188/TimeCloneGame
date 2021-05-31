using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The SecondBossIdle class is a StateMachineBehaviour that resets the values of the SecondBossScript.
/// </summary>
public class SecondBossIdle : StateMachineBehaviour
{

    /// <summary>
    /// Resets the SecondBossScript values.
    /// </summary>
    /// <remarks>
    /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// </remarks>
    /// <param name="animator">The animator attached to the Boss.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The current animation layer index.</param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.GetComponent<SecondBossScript>().ResetBoss();
    }
}
