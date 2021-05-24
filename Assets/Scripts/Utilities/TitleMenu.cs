using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{

    #region Inpsector fields
    [SerializeField] private FileViewer fileMenu;
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private Credits credits;
    #endregion

    public void NewGame()
    {
        fileMenu.OpenNewGameMenu();
        gameObject.SetActive(false);
    }

    public void LoadGame()
    {
        fileMenu.OpenLoadMenu();
        gameObject.SetActive(false);
    }

    public void Options()
    {
        optionsMenu.OpenMenu();
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void RollCredits()
    {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if(go != credits.gameObject && go != Camera.main.gameObject && go.GetComponent<EventSystem>() == null && go.GetComponent<MenuInput>() == null)
            {
                go.SetActive(false);
            }
        }

        credits.gameObject.SetActive(true);
        credits.StartCredits();
    }


    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
