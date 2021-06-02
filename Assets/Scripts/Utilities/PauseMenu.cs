using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
   
    #region Inspector fields
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private PlayerStatus player;
    [SerializeField] private OptionsMenu optionsMenu;
    #endregion

    #region Public enum
    [System.Serializable]
    public enum ConfirmationType 
    {
        Restart = 0,
        TitleScreen = 1,
        Exit = 2
    }
    #endregion

    #region Private fields
    private Scene currentScene;
    private GameObject _confirmButton;
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
        transform.GetChild(3).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1;
    }

    public void RestartCheckpoint()
    {
        if(CheckPoint.lastCheckpoint != null)
        {
            player.Respawn();
            FindObjectOfType<TimeCloneController>().EmptyAllCloneDevices();
        }
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
        Debug.Log("Exit");
        Application.Quit();
    }

    public void AskConfirmation(int confirmationNum)
    {
        transform.GetChild(3).gameObject.SetActive(true);
        SetSelectedGameObject(transform.GetChild(3).GetChild(3).gameObject);

        UnityEngine.UI.Button button = transform.GetChild(3).GetChild(2).GetComponent<UnityEngine.UI.Button>();
        ConfirmationType confirmationType = (ConfirmationType) confirmationNum;
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
