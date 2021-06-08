
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// The PauseMenu class is responsible for pausing the game and displaying the pause menu to the player.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The resume button in the menu.")]
    [SerializeField] private GameObject _resumeButton;
    [Tooltip("The player status script used to respawn the player when starting at the last checkpoint.")]
    [SerializeField] private PlayerStatus _player;
    [Tooltip("The options menu.")]
    [SerializeField] private OptionsMenu _optionsMenu;
    #endregion

    #region Private enum
    // Type of confirmation being asked, used for setting up button listeners
    private enum ConfirmationType 
    {
        Restart = 0,
        TitleScreen = 1,
        Exit = 2
    }
    #endregion

    #region Private fields
    private Scene _currentScene;
    private GameObject _confirmButton;
    #endregion

    void Awake()
    {
        _currentScene = SceneManager.GetActiveScene();
    }

    /// <summary>
    /// Pauses the game and opens the PauseMenu.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        SetSelectedGameObject(_resumeButton);
    }

    /// <summary>
    /// Resumes the game and closes the PauseMenu.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
        if(!transform.GetChild(1).gameObject.activeSelf) _optionsMenu.GoBack();
        transform.GetChild(3).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Restarts the current level.
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(_currentScene.name);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Respawns the player at the last CheckPoint.
    /// </summary>
    public void RestartCheckpoint()
    {
        if(CheckPoint.lastCheckpoint != null)
        {
            _player.Respawn();
            FindObjectOfType<TimeCloneController>().EmptyAllCloneDevices();
        }
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Opens the OptionsMenu.
    /// </summary>
    /// <seealso cref="OptionsMenu.OpenMenu"/>
    public void OptionsMenu()
    {
        _optionsMenu.OpenMenu();
        transform.GetChild(1).gameObject.SetActive(false);
    }

    /// <summary>
    /// Returns to the TitleMenu.
    /// </summary>
    public void TitleScreen()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    /// <summary>
    /// Asks the player for confirmation before performing the action they requested.
    /// </summary>
    /// <remarks>
    /// The <paramref name="confirmationNum"/> value is tied to an enum determining what type of action was requested.
    /// Its value must be limited to 0, 1, or 2.
    /// </remarks>
    /// <param name="confirmationNum">The number dictating which action is being validated.</param>
    public void AskConfirmation(int confirmationNum)
    {
        transform.GetChild(3).gameObject.SetActive(true);
        SetSelectedGameObject(transform.GetChild(3).GetChild(3).gameObject);

        UnityEngine.UI.Button button = transform.GetChild(3).GetChild(2).GetComponent<UnityEngine.UI.Button>();

        // Cast the number to enum
        ConfirmationType confirmationType = (ConfirmationType) confirmationNum;
        // assign the yes button listener
        switch(confirmationType)
        {
            case ConfirmationType.Restart:
                button.onClick.AddListener(RestartLevel);
                _confirmButton = transform.GetChild(1).GetChild(3).gameObject;
                break;
            case ConfirmationType.TitleScreen:
                button.onClick.AddListener(TitleScreen);
                _confirmButton = transform.GetChild(1).GetChild(5).gameObject;
                break;
            case ConfirmationType.Exit:
                button.onClick.AddListener(Exit);
                _confirmButton = transform.GetChild(1).GetChild(6).gameObject;
                break;
        }
    }

    /// <summary>
    /// Takes the player back to the PauseMenu after a no answer in the confirmation screen,
    /// </summary>
    /// <seealso cref="AskConfirmation(int)"/>
    public void BackToMenu()
    {
        transform.GetChild(3).gameObject.SetActive(false);
        SetSelectedGameObject(_confirmButton);
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
