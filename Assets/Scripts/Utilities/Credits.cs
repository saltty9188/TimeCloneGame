using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// The Credits class is responsible for displaying the credits to the screen.
/// </summary>
public class Credits : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Text file holding the credits text.")]
    [SerializeField] private TextAsset _creditsFile;
    [Tooltip("Text component that displays the credits.")]
    [SerializeField] private TextMeshProUGUI _creditsText;
    [Tooltip("Text component that displays the thank you message.")]
    [SerializeField] private GameObject _thankYouText;
    [Tooltip("The button that initiates the credits.")]
    [SerializeField] private GameObject _creditsButton;
    [Tooltip("The background game object.")]
    [SerializeField] private GameObject _backGround;
    [Tooltip("The game object containing the lights.")]
    [SerializeField] private GameObject _lights;
    [Tooltip("How fast the credits scroll.")]
    [SerializeField] private float _scrollSpeed;
    #endregion
    #region Private fields
    private float _initialPosition;
    private float _thankYouPosition;
    private bool _thankYouCentered;
    #endregion

    void Awake()
    {
        _creditsText.text = _creditsFile.text;
        _initialPosition = _creditsText.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        // moce credits
        _creditsText.transform.Translate(Vector3.up * _scrollSpeed * Time.deltaTime);

        // record the middle of the screen when the thank you text reaches it
        if(_thankYouPosition == 0 && _creditsText.GetComponent<RectTransform>().anchoredPosition.y - _creditsText.GetComponent<RectTransform>().rect.height 
            - _thankYouText.GetComponent<RectTransform>().rect.height/2.0f >= -GetComponent<RectTransform>().rect.height / 2.0f)
        {
            _thankYouPosition = _thankYouText.transform.position.y;
        }

        // Keep the thank you text centered once it reaches it
        if(_thankYouCentered || (_thankYouPosition != 0 && Mathf.Abs(_thankYouText.transform.position.y - _thankYouPosition) < 1))
        {
            _thankYouCentered = true;
            _thankYouText.transform.position = new Vector3(_thankYouText.transform.position.x, _thankYouPosition, _thankYouText.transform.position.z);

            // Fade out song then go back when its quiet
            if(AudioManager.Instance.FadeOutSong(0.2f * Time.deltaTime)) GoBack();
        }
    }

    /// <summary>
    /// Starts the credits sequence.
    /// </summary>
    public void StartCredits()
    {
        _creditsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _initialPosition);
        _thankYouText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -51);
        _thankYouCentered = false;
        AudioManager.Instance.PlayMusic("Credits");
    }

    /// <summary>
    /// Exits the credits sequence and returns to the title screen.
    /// </summary>
    public void GoBack()
    {
        // Credits accessed from the title screen
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach(GameObject go in gameObjects)
            {
                if(go != Camera.main.gameObject && go.GetComponent<EventSystem>() == null && go.GetComponent<MenuInput>() == null)
                {
                    go.SetActive(false);
                }
            }

            // Set the appropriate objects 
            _creditsButton.transform.parent.parent.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_creditsButton);
            _lights.SetActive(true);
            _backGround.SetActive(true);

            // Force volume to reset again in case the player skipped during the fade out
            AudioManager.Instance.FadeOutSong(1.0f);
            AudioManager.Instance.PlayMusic("TitleTheme");
        }
        else
        {
            // Credits shown from the ending
            SceneManager.LoadScene(0);
        }
    }
}
