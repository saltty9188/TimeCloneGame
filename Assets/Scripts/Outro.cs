using System.Collections;
using System;
using UnityEngine;

public class Outro : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private Animator elevator;
    [SerializeField] private CanvasGroup fadeToBlack;
    [SerializeField] private Animator player;
    [SerializeField] private Credits credits;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Camera.main.transform.position = new Vector3(-1.5f, -7f, -10);
        player.transform.position = new Vector3(-1.5f, -0.7f);
        StartCoroutine(OutroScene());
        AudioManager.instance.PlayMusic("Credits");
    }

    IEnumerator OutroScene()
    {
        // Elevator transitions upwards
        while(Camera.main.transform.position.y < 2)
        {
            Camera.main.transform.Translate(new Vector3(0, 2 * Time.deltaTime, 0), Space.World);
            float distToFade = 2.0f - Camera.main.transform.position.y;
            float fadeAlpha = distToFade / 9;
            fadeToBlack.alpha = fadeAlpha;
            yield return null;
        }
        
        // Elevator door opens
        elevator.SetTrigger("EnterLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = elevator.runtimeAnimatorController.animationClips;
        float animationTime = Array.Find<AnimationClip>(ac, clip => clip.name == "ElevatorEnterLevel").length;
        yield return new WaitForSeconds(animationTime);

        //Player runs outside
        player.SetFloat("Speed", 5.0f);
        while(player.transform.position.x < 10.7f)
        {
            player.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            Camera.main.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            yield return null;
        }

        // Player hugs tree
        player.SetFloat("Speed", 0);
        player.SetBool("Hugging", true);
        // wait one frame for the animation to start
        yield return null;

        ac = player.runtimeAnimatorController.animationClips;
        animationTime = Array.Find<AnimationClip>(ac, clip => clip.name == "Hug").length;
        yield return new WaitForSeconds(animationTime);

        //Player runs offscreen
        player.SetBool("Hugging", false);
        player.SetFloat("Speed", 5.0f);
        while(player.transform.position.x < 22f)
        {
            player.transform.Translate(new Vector3(5 * Time.deltaTime, 0, 0));
            float alpha = (player.transform.position.x - 10.7f) / 11.3f;
            fadeToBlack.alpha = alpha;
            yield return null;
        }

        credits.gameObject.SetActive(true);
        credits.StartCredits();
    }

}
