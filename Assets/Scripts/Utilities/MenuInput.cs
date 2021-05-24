using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{
    
    #region Inspector fields
    [SerializeField] private OptionsMenu optionsMenu;
    [SerializeField] private FileViewer fileViewer;
    [SerializeField] private LevelSelect levelSelect;
    [SerializeField] private Credits credits;
    [SerializeField] private TitleScreenIntro intro;
    #endregion

    #region Private fields
    private PlayerControls controls;
    #endregion

    void Awake()
    {
        controls = new PlayerControls();
        
        controls.Menus.Back.performed += ctx =>
        {
            if(intro.inIntro)
            {
                intro.SkipIntro();
            }
            else if(optionsMenu.gameObject.activeSelf)
            {
                optionsMenu.GoBack();
            }
            else if(fileViewer.gameObject.activeSelf)
            {
                fileViewer.GoBack();
            }
            else if(levelSelect.gameObject.activeSelf)
            {
                levelSelect.GoBack();
            }
            else if(credits.gameObject.activeSelf)
            {
                credits.GoBack();
            }
        };
    }

    void OnEnable()
    {
        controls.Menus.Enable();
    }

    void OnDisable()
    {
        controls.Menus.Disable();
    }
}
