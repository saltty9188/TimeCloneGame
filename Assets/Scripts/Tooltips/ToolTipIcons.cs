using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The ToolTipIcons singleton class supplies other classes with the tool tip icons needed throughout the game.
/// </summary>
public class ToolTipIcons : MonoBehaviour
{
    #region Public fields
    /// <summary>
    /// The static instance of this class.
    /// </summary>
    public static ToolTipIcons Instance;
    #endregion

    #region Inspector fields
    [Tooltip("The tool tip icons for keyboard and mouse.")]
    [SerializeField] private Sprite[] _keyboardIcons;
    [Tooltip("The tool tip icons for Xbox controllers.")]
    [SerializeField] private Sprite[] _xboxIcons;
    [Tooltip("The tool tip icons for PlayStation controllers.")]
    [SerializeField] private Sprite[] _psIcons;
    #endregion

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Returns the sprite corresponding to the given key string.
    /// </summary>
    /// <param name="key">The key of the tool tip sprite.</param>
    /// <returns>The required tool tip sprite.</returns>
    public Sprite GetIcon(string key)
    {
        string controlScheme = "";
        
        if(PlayerController.ControlScheme != null)
        {
            controlScheme = PlayerController.ControlScheme;
        }
        else if(MenuInput.ControlScheme != null)
        {
            controlScheme = MenuInput.ControlScheme;
        }
        else
        {
            Debug.LogError("No ControlScheme found.");
            return null;
        }

        // keyboard icon
        if(controlScheme == "KeyboardMouse")
        {
            foreach(Sprite sprite in _keyboardIcons)
            {
                if(sprite.name.ToLower() == key)
                {
                    return sprite;
                }
            }
        }
        else
        {
            //ps controller
            if(Gamepad.current.name.Contains("Dual"))
            {
                foreach(Sprite sprite in _psIcons)
                {
                    if(sprite.name.ToLower() == key)
                    {
                        return sprite;
                    }
                }
            }
            // xbox controller
            else
            {
                foreach(Sprite sprite in _xboxIcons)
                {
                    if(sprite.name.ToLower() == key)
                    {
                        return sprite;
                    }
                }
            }
        }

        Debug.LogError("Could not find icon with key " + key);

        return null;
    }
}
