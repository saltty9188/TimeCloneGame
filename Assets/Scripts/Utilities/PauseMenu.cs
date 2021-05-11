using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
   
    #region Inspector fields
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private PlayerStatus player;
    [SerializeField] private OptionsMenu optionsMenu;
    #endregion

    #region Private fields
    private Scene currentScene;
    #endregion

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        SetSelectedGameObject(resumeButton);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        if(!transform.GetChild(1).gameObject.activeSelf) optionsMenu.GoBack();
        gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1;
    }

    public void RestartCheckpoint()
    {
        player.Respawn();
        FindObjectOfType<TimeCloneController>().EmptyAllCloneDevices();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OptionsMenu()
    {
        optionsMenu.OpenMenu();
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
   
}
