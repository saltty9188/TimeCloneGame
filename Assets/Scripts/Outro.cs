using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// The Outro class is responsible for playing the outro cutscene.
/// </summary>
public class Outro : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Animator for the elevator.")]
    [SerializeField] private Animator _elevator;
    [Tooltip("Canvas group used to fade to black.")]
    [SerializeField] private CanvasGroup _fadeToBlack;
    [Tooltip("Player animator.")]
    [SerializeField] private Animator _player;
    [Tooltip("Credits sequence.")]
    [SerializeField] private Credits _credits;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.position = new Vector3(-1.5f, -7f, -10);
        _player.transform.position = new Vector3(-1.5f, -0.7f);
        StartCoroutine(OutroScene());
        AudioManager.Instance.PlayMusic("Credits");
    }

    IEnumerator OutroScene()
    {
        // Elevator transitions upwards
        while(Camera.main.transform.position.y < 2)
        {
            Camera.main.transform.Translate(new Vector3(0, 2 * Time.deltaTime, 0), Space.World);
            float distToFade = 2.0f - Camera.main.transform.position.y;
            float fadeAlpha = distToFade / 9;
            _fadeToBlack.alpha = fadeAlpha;
            yield return null;
        }
        
        // Elevator door opens
        _elevator.SetTrigger("EnterLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = _elevator.runtimeAnimatorController.animationClips;
        float animationTime = Array.Find<AnimationClip>(ac, clip => clip.name == "ElevatorEnterLevel").length;
        yield return new WaitForSeconds(animationTime);

        //Player runs outside
        _player.SetFloat("Speed", 5.0f);
        while(_player.transform.position.x < 10.7f)
        {
            _player.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            Camera.main.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            yield return null;
        }

        // Player hugs tree
        _player.SetFloat("Speed", 0);
        _player.SetBool("Hugging", true);
        // wait one frame for the animation to start
        yield return null;

        ac = _player.runtimeAnimatorController.animationClips;
        animationTime = Array.Find<AnimationClip>(ac, clip => clip.name == "Hug").length;
        yield return new WaitForSeconds(animationTime);

        //Player runs offscreen
        _player.SetBool("Hugging", false);
        _player.SetFloat("Speed", 5.0f);
        while(_player.transform.position.x < 22f)
        {
            _player.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            float alpha = (_player.transform.position.x - 10.7f) / 11.3f;
            _fadeToBlack.alpha = alpha;
            yield return null;
        }

        // start the credits
        _credits.gameObject.SetActive(true);
        _credits.StartCredits();
    }

}
