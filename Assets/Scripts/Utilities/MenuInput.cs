using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The MenuInput class is responsible for handling player input on the title menu.
/// </summary>
public class MenuInput : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The options menu.")]
    [SerializeField] private OptionsMenu _optionsMenu;
    [Tooltip("The file view menu.")]
    [SerializeField] private FileViewer _fileViewer;
    [Tooltip("The level select menu.")]
    [SerializeField] private LevelSelect _levelSelect;
    [Tooltip("The credits sequence.")]
    [SerializeField] private Credits _credits;
    [Tooltip("The intro sequence.")]
    [SerializeField] private TitleScreenIntro _intro;
    #endregion

    #region Private fields
    private PlayerControls _controls;
    #endregion

    void Awake()
    {
        _controls = new PlayerControls();
        
        // sets what the go back button will do when each menu is active
        _controls.Menus.Back.performed += ctx =>
        {
            if(_intro.InIntro)
            {
                _intro.SkipIntro();
            }
            else if(_optionsMenu.gameObject.activeSelf)
            {
                _optionsMenu.GoBack();
            }
            else if(_fileViewer.gameObject.activeSelf)
            {
                _fileViewer.GoBack();
            }
            else if(_levelSelect.gameObject.activeSelf)
            {
                _levelSelect.GoBack();
            }
            else if(_credits.gameObject.activeSelf)
            {
                _credits.GoBack();
            }
        };
    }

    void OnEnable()
    {
        _controls.Menus.Enable();
    }

    void OnDisable()
    {
        _controls.Menus.Disable();
    }
}
