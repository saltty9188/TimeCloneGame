using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextAsset creditsFile;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject lights;
    [SerializeField] private float scrollSpeed;

    private float initialPosition;

    void Awake()
    {
        creditsText.text = creditsFile.text;
        initialPosition = creditsText.GetComponent<RectTransform>().anchoredPosition.y;
        Debug.Log(initialPosition);
    }

    // Update is called once per frame
    void Update()
    {
        creditsText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if(creditsText.GetComponent<RectTransform>().anchoredPosition.y - creditsText.GetComponent<RectTransform>().rect.height > 10)
        {
            GoBack();
        }
         
    }

    public void StartCredits()
    {
        creditsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, initialPosition);
    }

    public void GoBack()
    {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if(go != Camera.main.gameObject && go.GetComponent<EventSystem>() == null && go.GetComponent<MenuInput>() == null)
            {
                go.SetActive(false);
            }
        }

        creditsButton.transform.parent.parent.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsButton);
        lights.SetActive(true);
        backGround.SetActive(true);

    }
}
