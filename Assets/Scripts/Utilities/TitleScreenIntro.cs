using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The TitleScreenIntro class is responsible for playing the title screen intro sequence to the player.
/// </summary>
public class TitleScreenIntro : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The canvas group used for fading in.")]
    [SerializeField] private CanvasGroup _fadeCanvas;
    [Tooltip("The canvas group used fade the title in.")]
    [SerializeField] private CanvasGroup _titleCanvas;
    [Tooltip("The canvas group used to fade the buttons in.")]
    [SerializeField] private CanvasGroup _buttonCanvas;
    [Tooltip("The desired final Y position of the camera.")]
    [SerializeField] private float _cameraY;
    [Tooltip("The speed the camera pans.")]
    [SerializeField] private float _panSpeed;
    [Tooltip("The current event system.")]
    [SerializeField] EventSystem _eventSystem;
    #endregion

    #region Public fields
    /// <value>True if the intro is playing, false otherwise.</value>
    public bool InIntro
    {
        get {return _inIntro;}
    }
    #endregion

    #region Private fields
    private bool _inIntro;
    private Coroutine _coroutine;
    private Vector3 _finalPosition;
    private float _panDist;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _inIntro = true;
        Camera.main.transform.position = new Vector3(-0.5f, 11f, -10f);
        _buttonCanvas.alpha = 0;
        _finalPosition = new Vector3(Camera.main.transform.position.x, _cameraY, Camera.main.transform.position.z);
        _panDist = Camera.main.transform.position.y - _cameraY;
        // deactivate the event system so button inputs don't stack
        _eventSystem.gameObject.SetActive(false);
        _coroutine = StartCoroutine(Intro());
        AudioManager.Instance.PlayMusic("TitleTheme");
    }

    // pan the camera and adjust the canvas group fades
    IEnumerator Intro()
    {
        // pan the camera down and crossfade the black screen and title
        while(Camera.main.transform.position.y != _cameraY)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, _finalPosition, _panSpeed * Time.deltaTime);
            float distToFade = Camera.main.transform.position.y - _cameraY;
            float fadeAlpha = distToFade / _panDist;
            _fadeCanvas.alpha = fadeAlpha;
            _titleCanvas.alpha = 1.0f - fadeAlpha;
            yield return null;
        }

        // wait a bit before quickly fading the buttons in
        yield return new WaitForSeconds(1.5f);

        while(_buttonCanvas.alpha < 1.0f)
        {
            float alpha = _buttonCanvas.alpha;
            alpha += 16 * Time.deltaTime;
            _buttonCanvas.alpha = alpha;
            yield return null;
        }

        _coroutine = null;
        _inIntro = false;
        // reactivate the event system
        _eventSystem.gameObject.SetActive(true);
    }

    /// <summary>
    /// Skips the intro animation to the end.
    /// </summary>
    public void SkipIntro()
    {
        StopCoroutine(_coroutine);
        _fadeCanvas.alpha = 0;
        _titleCanvas.alpha = 1;
        _buttonCanvas.alpha = 1;
        Camera.main.transform.position = _finalPosition;
        _inIntro = false;
        _eventSystem.gameObject.SetActive(true);
    }
}
