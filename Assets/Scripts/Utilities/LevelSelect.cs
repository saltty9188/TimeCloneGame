using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject levelSelectTemplate;
    [SerializeField] private GameObject backButton;
    [SerializeField] private Sprite[] _levelIcons;
    #endregion

    #region Private fields
    private GameObject saveFileButton;
    private ScrollRect scrollRect;
    private float originalHeight;
    #endregion

    void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        originalHeight = scrollRect.content.rect.height;
    }

    public void OpenLoadMenu(GameObject saveFileButton)
    {
        gameObject.SetActive(true);
        this.saveFileButton = saveFileButton;
        PopulateMenu();
        SetSelectedGameObject(scrollRect.content.transform.GetChild(1).gameObject);
    }

    public void GoBack()
    {
        saveFileButton.transform.parent.parent.parent.parent.gameObject.SetActive(true);
        SetSelectedGameObject(saveFileButton);

        foreach(Transform child in scrollRect.content)
        {
            if(child.gameObject != levelSelectTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        gameObject.SetActive(false);
    }

    void PopulateMenu()
    {
        levelSelectTemplate.SetActive(true);
        LevelSelectButton.totalLevelNumber = 0;
        scrollRect.content.sizeDelta = new Vector2(0, originalHeight);

        int unlockedLevels = SaveData.currentSaveFile.LevelIndex;
        Vector3 buttonPos = levelSelectTemplate.transform.localPosition;

        float totalHeight = unlockedLevels * levelSelectTemplate.GetComponent<RectTransform>().rect.height;

        if(totalHeight > scrollRect.content.rect.height)
        {
            scrollRect.content.sizeDelta = new Vector2(0, totalHeight);
        }

        for(int i = 1; i <= unlockedLevels; i++)
        {
            string displayName = "Level " + i.ToString();
            if(i == 14) displayName = "Final Boss";
            GameObject button = Instantiate(levelSelectTemplate, buttonPos, new Quaternion());
            button.name = displayName;
            button.transform.parent = levelSelectTemplate.transform.parent;
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = buttonPos;
            buttonPos.y -= levelSelectTemplate.GetComponent<RectTransform>().rect.height;

            TextMeshProUGUI levelName = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            levelName.text = displayName;

            Image levelIcon = button.transform.GetChild(1).GetComponent<Image>();
            levelIcon.sprite = _levelIcons[i - 1];
        }

        UnityEngine.UI.Button UIButton = backButton.GetComponent<UnityEngine.UI.Button>();
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = scrollRect.content.GetChild(scrollRect.content.childCount - 1).GetComponent<Selectable>();
        
        nav.selectOnDown = scrollRect.content.transform.GetChild(1).GetComponent<Selectable>();
        UIButton.navigation = nav;

        levelSelectTemplate.SetActive(false);
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }
}
