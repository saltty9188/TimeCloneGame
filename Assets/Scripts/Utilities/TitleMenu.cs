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


    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
