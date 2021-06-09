using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The LevelTransition class is responsible for transitioning the player between levels.
/// </summary>
public class LevelTransition : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Is theis transition the entrace?")]
    [SerializeField] private bool _isEntrance;
    [Tooltip("How fast the elevator moves.")]
    [SerializeField] private float _elevatorSpeed = 2;
    [Tooltip("How far the elevator travels.")]
    [SerializeField] private float _elevatorTravelHeight = 10;
    [Tooltip("The canvas group that fades to black.")]
    [SerializeField] private CanvasGroup _fadeGroup;
    [Tooltip("The player.")]
    [SerializeField] private GameObject _player;
    #endregion

    #region Private fields
    private Animator _animator;
    private float _fadeoutHeight;
    private float _fadeinHeight;
    private CameraTracker _cameraTracker;
    #endregion

    void Start()
    {
        _animator = GetComponent<Animator>();
        _cameraTracker = Camera.main.GetComponent<CameraTracker>();
        if(_isEntrance)
        {
            _player.transform.position = new Vector3(transform.position.x, _player.transform.position.y, 0);
            _fadeinHeight = (_player.transform.position.y + _cameraTracker.VerticalOffset) - _elevatorTravelHeight;
            Camera.main.transform.position = new Vector3(_player.transform.position.x, _fadeinHeight, Camera.main.transform.position.z);
            _cameraTracker.enabled = false;
            _player.GetComponent<PlayerMovement>().enabled = false;
            _player.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            _player.transform.GetChild(0).GetComponent<Aim>().enabled = false;
            StartCoroutine(EnterLevelTransition());
            AudioManager.Instance.PlayLevelTheme(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator EnterLevelTransition()
    {
        // pan up and fade in
        while(Camera.main.transform.position.y < _fadeinHeight + _elevatorTravelHeight)
        {
            Camera.main.transform.Translate(new Vector3(0, _elevatorSpeed*Time.deltaTime, 0), Space.World);
            float distToFade = (_fadeinHeight + _elevatorTravelHeight) - Camera.main.transform.position.y;
            float fadeAlpha = distToFade / _elevatorTravelHeight;
            _fadeGroup.alpha = fadeAlpha;
            yield return null;
        }

        _animator.SetTrigger("EnterLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = _animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "ElevatorEnterLevel")
            {
                animationTime = clip.length;
            }
        }
        yield return new WaitForSeconds(animationTime);
        
        _player.GetComponent<PlayerMovement>().enabled = true;
        _cameraTracker.enabled = true;

        _animator.SetTrigger("Close");
    }

    IEnumerator ExitLevelTransition()
    {
        _animator.SetTrigger("ExitLevel");
        // Wait one frame for the animation to start
        yield return null;

        AnimationClip[] ac = _animator.runtimeAnimatorController.animationClips;
        float animationTime = 0;
        foreach(AnimationClip clip in ac)
        {
            if(clip.name == "ElevatorExitLevel")
            {
                animationTime = clip.length;
            }
        }
        yield return new WaitForSeconds(animationTime);

        // pan up and fade to black
        while(Camera.main.transform.position.y < _fadeoutHeight)
        {
            Camera.main.transform.Translate(new Vector3(0, _elevatorSpeed*Time.deltaTime, 0), Space.World);
            float distToFade = _fadeoutHeight - Camera.main.transform.position.y;
            float fadeAlpha = 1.0f - (distToFade / _elevatorTravelHeight);
            _fadeGroup.alpha = fadeAlpha;
            yield return null;
        }
        LoadNextLevel();
    }

    // stop the player from being able to move
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
    
    // start the level exit
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            _fadeoutHeight = Camera.main.transform.position.y + _elevatorTravelHeight;
            _cameraTracker.enabled = false;
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
