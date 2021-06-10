using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// The FileViewer class is responsible for displaying existing SaveData to the player.
/// </summary>
public class FileViewer : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The new game button on the title menu. Used to select after going back.")]
    [SerializeField] private GameObject _newGameButton;
    [Tooltip("The load game button on the title menu. Used to select after going back.")]
    [SerializeField] private GameObject _loadGameButton;
    [Tooltip("The template for creating file buttons.")]
    [SerializeField] private GameObject _fileSelectTemplate;
    [Tooltip("The button that creates new save files.")]
    [SerializeField] private GameObject _newFileButton;
    [Tooltip("The button that goes back to the title menu.")]
    [SerializeField] private GameObject _backButton;
    [Tooltip("The level select menu.")]
    [SerializeField] private LevelSelect _levelSelectMenu;
    [Tooltip("Tool tip image showing how to erase a file.")]
    [SerializeField] private Image _eraseFileIcon;
    [Tooltip("The menu input game object.")]
    [SerializeField] private MenuInput _input;
    #endregion

    public bool InConfirmation
    {
        get {return _inConfirmation;}
    }

    #region Private fields
    private ScrollRect _scrollRect;
    private float _originalHeight;
    private bool _newGame;
    private GameObject _confirmButton;
    private bool _inConfirmation;
    #endregion

    void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _originalHeight = _scrollRect.content.rect.height;
    }

    void Update()
    {
        // update erase tool tip
        int bindingIndex = _input.CurrentControls.Menus.Erase.GetBindingIndex(InputBinding.MaskByGroup(MenuInput.ControlScheme));
        string key = _input.CurrentControls.Menus.Erase.GetBindingDisplayString(bindingIndex).ToLower();
        _eraseFileIcon.sprite = ToolTipIcons.Instance.GetIcon(key);
    }

    /// <summary>
    /// Opens the FileViewer and primes it to create a new save file.
    /// </summary>
    public void OpenNewGameMenu()
    {   
        gameObject.SetActive(true);
        PopulateMenu(true);
    }

    /// <summary>
    /// Opens the FileViewer and primes it to load an exisiting save file.
    /// </summary>
    public void OpenLoadGameMenu()
    {
        gameObject.SetActive(true);
        PopulateMenu(false);
    }

    /// <summary>
    /// Returns the player back to the TitleMenu.
    /// </summary>
    public void GoBack()
    {
        _loadGameButton.transform.parent.parent.gameObject.SetActive(true);
        if(_newGame)
        {
            SetSelectedGameObject(_newGameButton);
        }
        else
        {
            SetSelectedGameObject(_loadGameButton);
        }

        // clear out the created buttons
        foreach(Transform child in _scrollRect.content)
        {
            if(child.gameObject != _fileSelectTemplate && child.gameObject != _newFileButton)
            {
                Destroy(child.gameObject);
            }
        }

        gameObject.SetActive(false);
    }

    // populates the file view
    void PopulateMenu(bool newGame)
    {
        this._newGame = newGame;
        _inConfirmation = false;

        _fileSelectTemplate.SetActive(true);
        // reset the scrollrect height
        _scrollRect.content.sizeDelta = new Vector2(0, _originalHeight);
        string[] fileNames = SaveSystem.AllFileNames();
        if(fileNames.Length > 0 || newGame)
        {
            // hide the "no save files present" text
            transform.GetChild(2).gameObject.SetActive(false);
            Vector3 buttonPos = _fileSelectTemplate.transform.localPosition;

            // calculate the new scrollrect height
            float totalHeight = fileNames.Length * _fileSelectTemplate.GetComponent<RectTransform>().rect.height;
            if(newGame) totalHeight += _fileSelectTemplate.GetComponent<RectTransform>().rect.height;

            if(totalHeight > _scrollRect.content.rect.height)
            {
                _scrollRect.content.sizeDelta = new Vector2(0, totalHeight);
            }

            // set title text depending on the context
            if(newGame)
            {
                _newFileButton.SetActive(true);
                _newFileButton.transform.localPosition = buttonPos;
                buttonPos.y -= _fileSelectTemplate.GetComponent<RectTransform>().rect.height;
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "New Game";
            }
            else
            {
                _newFileButton.SetActive(false);
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Load Game";
            }

            // populate the menu with the existing files
            foreach(string name in fileNames)
            {
                // load the file temporarily to get its current level number
                string strippedName = name.Remove(name.IndexOf("."));
                SaveData temp = SaveSystem.LoadGame(strippedName);

                // create the button
                GameObject button = Instantiate(_fileSelectTemplate, buttonPos, new Quaternion());
                button.name = temp.FileName;

                // set the transform
                button.transform.parent = _fileSelectTemplate.transform.parent;
                button.transform.localScale = Vector3.one;
                button.transform.localPosition = buttonPos;

                buttonPos.y -= _fileSelectTemplate.GetComponent<RectTransform>().rect.height;

                // Set the onClick listeners
                UnityEngine.UI.Button b = button.GetComponent<UnityEngine.UI.Button>();
                FileManager fm = button.GetComponent<FileManager>();
                if(newGame)
                {
                    b.onClick.AddListener(delegate{AskConfirmation(button, false);});
                }
                else
                {
                    b.onClick.AddListener(fm.LoadGame);
                }

                TextMeshProUGUI fileName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                fileName.text = temp.FileName;

                TextMeshProUGUI currentLevel = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                currentLevel.text = "Level: " + (temp.LevelIndex == 14 ? "Final Boss" : temp.LevelIndex.ToString());
            }

            // set the navigation
            UnityEngine.UI.Button UIButton = _backButton.GetComponent<UnityEngine.UI.Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = _scrollRect.content.GetChild(_scrollRect.content.childCount - 1).GetComponent<Selectable>();

            // new game button is default selected or first file for load screen
            if(newGame)
            {
                SetSelectedGameObject(_newFileButton);
                nav.selectOnDown = _newFileButton.GetComponent<Selectable>();
            }
            else
            {
                SetSelectedGameObject(_scrollRect.content.transform.GetChild(2).gameObject);
                nav.selectOnDown = _scrollRect.content.transform.GetChild(2).GetComponent<Selectable>();
            }
            UIButton.navigation = nav;
            
        }
        else
        {
            // remove all buttons and show the no files found text
            _newFileButton.SetActive(false);
            _fileSelectTemplate.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            SetSelectedGameObject(_backButton);
            UnityEngine.UI.Button button = _backButton.GetComponent<UnityEngine.UI.Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
        }
       

        _fileSelectTemplate.SetActive(false);
    }

    /// <summary>
    /// Opens the LevelSelect menu.
    /// </summary>
    /// <seealso cref="LevelSelect.OpenLevelSelectMenu(GameObject)"/>
    /// <param name="saveFileButton">The button used to open the LevelSelect menu.</param>
    public void OpenLevelSelect(GameObject saveFileButton)
    {
        _levelSelectMenu.OpenLevelSelectMenu(saveFileButton);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Brings up a screen asking the player for confirmation before performing an action on a file.
    /// </summary>
    /// <remarks>
    /// This screen can ask if the player wants to overwrite a save or erase one.
    /// </remarks>
    /// <param name="fileButton">The GameObject representing the save file.</param>
    /// <param name="eraseGame">True if the confirmation is to erase a save, false if it is to overwrite the save with a new file.</param>
    public void AskConfirmation(GameObject fileButton, bool eraseGame)
    {
        _inConfirmation = true;
        GameObject confirmScreen = transform.GetChild(4).gameObject;
        confirmScreen.SetActive(true);
        SetSelectedGameObject(confirmScreen.transform.GetChild(3).gameObject);
        _confirmButton = fileButton;

        UnityEngine.UI.Button button = confirmScreen.transform.GetChild(2).GetComponent<UnityEngine.UI.Button>();

        if(eraseGame)
        {
            confirmScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Erase Game?";
            button.onClick.AddListener(delegate{DeleteFile(fileButton);});
        }
        else
        {
            confirmScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Are You Sure?";
            FileManager fm = fileButton.GetComponent<FileManager>();
            button.onClick.AddListener(fm.NewGame);
        }
    }

    /// <summary>
    /// Returns the player to the FileViewer if they said no on the confirmation screen.
    /// </summary>
    public void BackToMenu()
    {
        _inConfirmation = false;
        transform.GetChild(4).gameObject.SetActive(false);
        SetSelectedGameObject(_confirmButton);
    }

    // sets the selected game object in the event system
    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }

    // deletes the file represented by the fileButton
    void DeleteFile(GameObject fileButton)
    {
        fileButton.GetComponent<FileManager>().DeleteGame();

        // clear out the created buttons
        foreach(Transform child in _scrollRect.content)
        {
            if(child.gameObject != _fileSelectTemplate && child.gameObject != _newFileButton)
            {
                Destroy(child.gameObject);
            }
        }
        BackToMenu();
        PopulateMenu(_newGame);
    }
}
