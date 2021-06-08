using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// The LevelSelect class is responsible for displaying the available levels to play to the player.
/// </summary>
public class LevelSelect : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The load menu game object.")]
    [SerializeField] private GameObject _fileSelectMenu;
    [Tooltip("The template for creating level select buttons.")]
    [SerializeField] private GameObject _levelSelectTemplate;
    [Tooltip("Button that goes back to the file select screen.")]
    [SerializeField] private GameObject _backButton;
    [Tooltip("Array of the level icons.")]
    [SerializeField] private Sprite[] _levelIcons;
    #endregion

    #region Private fields
    private GameObject _saveFileButton;
    private ScrollRect _scrollRect;
    private float _originalHeight;
    #endregion

    void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _originalHeight = _scrollRect.content.rect.height;
    }

    /// <summary>
    /// Opens the LevelSelect menu.
    /// </summary>
    /// <param name="saveFileButton">The save button from the FileViewer that opened this menu.</param>
    public void OpenLevelSelectMenu(GameObject saveFileButton)
    {
        gameObject.SetActive(true);
        this._saveFileButton = saveFileButton;
        PopulateMenu();
        SetSelectedGameObject(_scrollRect.content.transform.GetChild(1).gameObject);
    }

    /// <summary>
    /// Goes back to the FileViewer load menu.
    /// </summary>
    public void GoBack()
    {
        _fileSelectMenu.SetActive(true);
        SetSelectedGameObject(_saveFileButton);

        foreach(Transform child in _scrollRect.content)
        {
            if(child.gameObject != _levelSelectTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        gameObject.SetActive(false);
    }

    // populates the menu with the available levels
    void PopulateMenu()
    {
        _levelSelectTemplate.SetActive(true);
        LevelSelectButton.ResetTotalLevelNumber();
        // reset the scrollrect height
        _scrollRect.content.sizeDelta = new Vector2(0, _originalHeight);

        int unlockedLevels = SaveData.currentSaveFile.LevelIndex;
        Vector3 buttonPos = _levelSelectTemplate.transform.localPosition;

        float totalHeight = unlockedLevels * _levelSelectTemplate.GetComponent<RectTransform>().rect.height;

        if(totalHeight > _scrollRect.content.rect.height)
        {
            _scrollRect.content.sizeDelta = new Vector2(0, totalHeight);
        }

        // populate the menu
        for(int i = 1; i <= unlockedLevels; i++)
        {
            string displayName = "Level " + i.ToString();
            if(i == 14) displayName = "Final Boss";

            // create the button
            GameObject button = Instantiate(_levelSelectTemplate, buttonPos, new Quaternion());
            button.name = displayName;
            button.transform.parent = _levelSelectTemplate.transform.parent;
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = buttonPos;
            buttonPos.y -= _levelSelectTemplate.GetComponent<RectTransform>().rect.height;

            // set the level name
            TextMeshProUGUI levelName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            levelName.text = displayName;

            // Set the level icon
            Image levelIcon = button.transform.GetChild(1).GetComponent<Image>();
            levelIcon.sprite = _levelIcons[i - 1];
        }

        // Set the back button navigation
        UnityEngine.UI.Button UIButton = _backButton.GetComponent<UnityEngine.UI.Button>();
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = _scrollRect.content.GetChild(_scrollRect.content.childCount - 1).GetComponent<Selectable>();
        
        nav.selectOnDown = _scrollRect.content.transform.GetChild(1).GetComponent<Selectable>();
        UIButton.navigation = nav;

        _levelSelectTemplate.SetActive(false);
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
