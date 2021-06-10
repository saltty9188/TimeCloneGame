using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    #region Public fields
    /// <value>A string representing the currently used control scheme.</value>
    public static string ControlScheme
    {
        get {return _controlScheme;}
    }

    /// <value>The PlayerControls in use by the player.</value>
    public PlayerControls CurrentControls
    {
        get {return _controls;}
    }
    #endregion

    #region Private fields
    private PlayerControls _controls;
    private static string _controlScheme;
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

        _controls.Menus.Erase.performed += ctx =>
            {
                if(_fileViewer.gameObject.activeSelf && !_fileViewer.InConfirmation)
                {
                    _fileViewer.AskConfirmation(EventSystem.current.currentSelectedGameObject, true);
                }
                
            };
        
        //Detect Input Device
        InputSystem.onActionChange += (obj, change) =>
            {
                if (change == InputActionChange.ActionPerformed)
                {
                    var inputAction = (InputAction) obj;
                    var lastControl = inputAction.activeControl;
                    var lastDevice = lastControl.device;
                    SetControlScheme(lastDevice);
                }
            };
    }

    void SetControlScheme(InputDevice device)
    {
        if(device.displayName == "Mouse" || device.displayName == "Keyboard")
        {
            _controlScheme = "KeyboardMouse";
        }
        else
        {
            _controlScheme = "Gamepad";
        }
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
