using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Credits : MonoBehaviour
{
    [SerializeField] private TextAsset creditsFile;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private GameObject thankYouText;
    [SerializeField] private GameObject creditsButton;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject lights;
    [SerializeField] private float scrollSpeed;

    private float initialPosition;
    private float thankYouPosition;
    private bool thankYouCentered;

    void Awake()
    {
        creditsText.text = creditsFile.text;
        initialPosition = creditsText.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        creditsText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        if(thankYouPosition == 0 && creditsText.GetComponent<RectTransform>().anchoredPosition.y - creditsText.GetComponent<RectTransform>().rect.height 
            - thankYouText.GetComponent<RectTransform>().rect.height/2.0f >= -GetComponent<RectTransform>().rect.height / 2.0f)
        {
            thankYouPosition = thankYouText.transform.position.y;
        }

        if(thankYouCentered || (thankYouPosition != 0 && Mathf.Abs(thankYouText.transform.position.y - thankYouPosition) < 1))
        {
            thankYouCentered = true;
            thankYouText.transform.position = new Vector3(thankYouText.transform.position.x, thankYouPosition, thankYouText.transform.position.z);

            if(AudioManager.Instance.FadeOutSong(0.2f * Time.deltaTime)) GoBack();
        }
    }

    public void StartCredits()
    {
        creditsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, initialPosition);
        thankYouText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -51);
        thankYouCentered = false;
        AudioManager.Instance.PlayMusic("Credits");
    }

    public void GoBack()
    {
        // Credits accessed from the title screen
        if(SceneManager.GetActiveScene().buildIndex == 0)
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
            // Force volume to reset again in case the player skipped during the fade out
            AudioManager.Instance.FadeOutSong(1.0f);
            AudioManager.Instance.PlayMusic("TitleTheme");
        }
        else
        {
            // Credits shown from the ending
            SceneManager.LoadScene(0);
        }
    }
}
