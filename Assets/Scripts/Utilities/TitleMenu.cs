using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// The TitleMenu class is responsible for displaying the title screen menu to the player.
/// </summary>
public class TitleMenu : MonoBehaviour
{
    #region Inpsector fields
    [SerializeField] private FileViewer _fileMenu;
    [SerializeField] private OptionsMenu _optionsMenu;
    [SerializeField] private Credits _credits;
    #endregion

    /// <summary>
    /// Opens the FileViewer to start a new game.
    /// </summary>
    /// <seealso cref="FileViewer.OpenNewGameMenu"/>
    public void NewGame()
    {
        _fileMenu.OpenNewGameMenu();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the FileViewer to load a previous save..
    /// </summary>
    /// <seealso cref="FileViewer.OpenLoadGameMenu"/>
    public void LoadGame()
    {
        _fileMenu.OpenLoadGameMenu();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Opens the OptionsMenu.
    /// </summary>
    /// <seealso cref="OptionsMenu.OpenMenu"/>
    public void Options()
    {
        _optionsMenu.OpenMenu();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Starts the Credits sequence.
    /// </summary>
    /// <seealso cref="Credits.StartCredits"/>
    public void RollCredits()
    {
        // hide everything
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if(go != _credits.gameObject && go != Camera.main.gameObject && go.GetComponent<EventSystem>() == null && go.GetComponent<MenuInput>() == null)
            {
                go.SetActive(false);
            }
        }

        _credits.gameObject.SetActive(true);
        _credits.StartCredits();
    }
}
