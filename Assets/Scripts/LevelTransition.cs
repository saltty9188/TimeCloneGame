using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private bool isEntrance;
    [SerializeField] private float elevatorSpeed = 2;
    [SerializeField] private float elevatorTravelHeight = 10;
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private GameObject player;
    #endregion

    #region Private fields
    private Animator animator;
    private float fadeoutHeight;
    private float fadeinHeight;
    private CameraTracker cameraTracker;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraTracker = Camera.main.GetComponent<CameraTracker>();
        if(isEntrance)
        {
            player.transform.position = new Vector3(transform.position.x, player.transform.position.y, 0);
            fadeinHeight = (player.transform.position.y + cameraTracker.verticalOffset) - elevatorTravelHeight;
            Camera.main.transform.position = new Vector3(player.transform.position.x, fadeinHeight, Camera.main.transform.position.z);
            cameraTracker.enabled = false;
            player.GetComponent<PlayerMovement>().enabled = false;
            player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            player.transform.GetChild(0).GetComponent<Aim>().enabled = false;
            StartCoroutine(EnterLevelTransition());
            AudioManager.instance.PlayLevelTheme(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator EnterLevelTransition()
    {
        while(Camera.main.transform.position.y < fadeinHeight + elevatorTravelHeight)
        {
            Camera.main.transform.Translate(new Vector3(0, elevatorSpeed*Time.deltaTime, 0), Space.World);
            float distToFade = (fadeinHeight + elevatorTravelHeight) - Camera.main.transform.position.y;
            float fadeAlpha = distToFade / elevatorTravelHeight;
            fadeGroup.alpha = fadeAlpha;
            yield return null;
        }

        animator.SetTrigger("EnterLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "ElevatorEnterLevel")
            {
                animationTime = clip.length;
            }
        }
        yield return new WaitForSeconds(animationTime);
        
        player.GetComponent<PlayerMovement>().enabled = true;
        player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        cameraTracker.enabled = true;

        animator.SetTrigger("Close");
    }

    IEnumerator ExitLevelTransition()
    {
        animator.SetTrigger("ExitLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "ElevatorExitLevel")
            {
                animationTime = clip.length;
            }
        }
        yield return new WaitForSeconds(animationTime);

        while(Camera.main.transform.position.y < fadeoutHeight)
        {
            Camera.main.transform.Translate(new Vector3(0, elevatorSpeed*Time.deltaTime, 0), Space.World);
            float distToFade = fadeoutHeight - Camera.main.transform.position.y;
            float fadeAlpha = 1.0f - (distToFade / elevatorTravelHeight);
            fadeGroup.alpha = fadeAlpha;
            yield return null;
        }
        LoadNextLevel();
    }

    void StopPlayerMoving(GameObject playerObject)
    {
        playerObject.GetComponent<PlayerMovement>().enabled = false;
        playerObject.GetComponent<Animator>().SetFloat("Speed", 0);
        playerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Aim aim = playerObject.transform.GetChild(0).GetComponent<Aim>();
        aim.enabled = false;
        if(aim.CurrentWeapon)
        {
            aim.CurrentWeapon.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            fadeoutHeight = Camera.main.transform.position.y + elevatorTravelHeight;
            cameraTracker.enabled = false;
            other.transform.position = new Vector3(transform.position.x, other.transform.position.y, 0);
            StopPlayerMoving(other.gameObject);
            StartCoroutine(ExitLevelTransition());
        }
    }

    void LoadNextLevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        if(SaveData.currentSaveFile != null && SaveData.currentSaveFile.LevelIndex <= levelIndex)
        {
            SaveData.currentSaveFile.LevelIndex = levelIndex + 1;
            SaveSystem.SaveGame(SaveData.currentSaveFile);
        }
        SceneManager.LoadScene(levelIndex + 1);
    }
}
