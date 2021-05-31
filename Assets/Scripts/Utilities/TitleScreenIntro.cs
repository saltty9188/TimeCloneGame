using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreenIntro : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private CanvasGroup titleCanvas;
    [SerializeField] private CanvasGroup buttonCanvas;
    [SerializeField] private float cameraY;
    [SerializeField] private float panSpeed;
    [SerializeField] EventSystem eventSystem;
    #endregion

    #region Public fields
    public bool inIntro;
    #endregion

    #region Private fields
    private Coroutine coroutine;
    private Vector3 finalPosition;
    private float panDist;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        inIntro = true;
        Camera.main.transform.position = new Vector3(-0.5f, 11f, -10f);
        buttonCanvas.alpha = 0;
        finalPosition = new Vector3(Camera.main.transform.position.x, cameraY, Camera.main.transform.position.z);
        panDist = Camera.main.transform.position.y - cameraY;
        eventSystem.gameObject.SetActive(false);
        coroutine = StartCoroutine(Intro());
        AudioManager.Instance.PlayMusic("TitleTheme");
    }

    IEnumerator Intro()
    {
        while(Camera.main.transform.position.y != cameraY)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, finalPosition, panSpeed * Time.deltaTime);
            float distToFade = Camera.main.transform.position.y - cameraY;
            float fadeAlpha = distToFade / panDist;
            fadeCanvas.alpha = fadeAlpha;
            titleCanvas.alpha = 1.0f - fadeAlpha;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        while(buttonCanvas.alpha < 1.0f)
        {
            float alpha = buttonCanvas.alpha;
            alpha += 16 * Time.deltaTime;
            buttonCanvas.alpha = alpha;
            yield return null;
        }

        coroutine = null;
        inIntro = false;
        eventSystem.gameObject.SetActive(true);
    }

    public void SkipIntro()
    {
        StopCoroutine(coroutine);
        fadeCanvas.alpha = 0;
        titleCanvas.alpha = 1;
        buttonCanvas.alpha = 1;
        Camera.main.transform.position = finalPosition;
        inIntro = false;
        eventSystem.gameObject.SetActive(true);
    }
}
