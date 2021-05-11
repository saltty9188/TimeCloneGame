using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FileViewer : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject loadGameButton;
    [SerializeField] private GameObject fileSelectTemplate;
    [SerializeField] private GameObject newFileButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private LevelSelect levelSelectMenu;
    #endregion

    #region Private fields
    private ScrollRect scrollRect;
    private float originalHeight;
    private bool newGame;
    #endregion

    void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        originalHeight = scrollRect.content.rect.height;
    }

    public void OpenNewGameMenu()
    {   
        gameObject.SetActive(true);
        PopulateMenu(true);
    }

    public void OpenLoadMenu()
    {
        gameObject.SetActive(true);
        PopulateMenu(false);
    }

    public void GoBack()
    {
        loadGameButton.transform.parent.gameObject.SetActive(true);
        if(newGame)
        {
            SetSelectedGameObject(newGameButton);
        }
        else
        {
            SetSelectedGameObject(loadGameButton);
        }

        foreach(Transform child in scrollRect.content)
        {
            if(child.gameObject != fileSelectTemplate && child.gameObject != newFileButton)
            {
                Destroy(child.gameObject);
            }
        }

        gameObject.SetActive(false);
    }

    void PopulateMenu(bool newGame)
    {
        this.newGame = newGame;
        fileSelectTemplate.SetActive(true);
        scrollRect.content.sizeDelta = new Vector2(0, originalHeight);

        string[] fileNames = SaveSystem.AllFileNames();
        if(fileNames.Length > 0 || newGame)
        {
            transform.GetChild(2).gameObject.SetActive(false);
            Vector3 buttonPos = fileSelectTemplate.transform.position;

            float totalHeight = fileNames.Length * fileSelectTemplate.GetComponent<RectTransform>().rect.height;
            if(newGame) totalHeight += fileSelectTemplate.GetComponent<RectTransform>().rect.height;

            if(totalHeight > scrollRect.content.rect.height)
            {
                scrollRect.content.sizeDelta = new Vector2(0, totalHeight);
            }

            if(newGame)
            {
                newFileButton.SetActive(true);
                newFileButton.transform.position = buttonPos;
                buttonPos.y -= fileSelectTemplate.GetComponent<RectTransform>().rect.height;
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "New Game";
            }
            else
            {
                newFileButton.SetActive(false);
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Load Game";
            }

            foreach(string name in fileNames)
            {
                string strippedName = name.Remove(name.IndexOf("."));
                SaveData temp = SaveSystem.LoadGame(strippedName);
                GameObject button = Instantiate(fileSelectTemplate, buttonPos, new Quaternion());
                button.name = temp.fileName;
                button.transform.parent = fileSelectTemplate.transform.parent;
                buttonPos.y -= fileSelectTemplate.GetComponent<RectTransform>().rect.height;

                UnityEngine.UI.Button b = button.GetComponent<UnityEngine.UI.Button>();
                FileManager fm = button.GetComponent<FileManager>();
                if(newGame)
                {
                    b.onClick.AddListener(fm.NewGame);
                }
                else
                {
                    b.onClick.AddListener(fm.LoadGame);
                }

                TextMeshProUGUI fileName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                fileName.text = temp.fileName;

                TextMeshProUGUI currentLevel = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                currentLevel.text = "Level: " + (temp.levelIndex == 14 ? "Final Boss" : temp.levelIndex.ToString());
            }

            UnityEngine.UI.Button UIButton = backButton.GetComponent<UnityEngine.UI.Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = scrollRect.content.GetChild(scrollRect.content.childCount - 1).GetComponent<Selectable>();

            if(newGame)
            {
                SetSelectedGameObject(newFileButton);
                nav.selectOnDown = newFileButton.GetComponent<Selectable>();
            }
            else
            {
                SetSelectedGameObject(scrollRect.content.transform.GetChild(2).gameObject);
                nav.selectOnDown = scrollRect.content.transform.GetChild(2).GetComponent<Selectable>();
            }
            UIButton.navigation = nav;
            
        }
        else
        {
            newFileButton.SetActive(false);
            fileSelectTemplate.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            SetSelectedGameObject(backButton);
            UnityEngine.UI.Button button = backButton.GetComponent<UnityEngine.UI.Button>();
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
        }
       

        fileSelectTemplate.SetActive(false);
    }

    public void OpenLevelSelect(GameObject saveFileButton)
    {
        levelSelectMenu.OpenLoadMenu(saveFileButton);
        gameObject.SetActive(false);
    }
    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
