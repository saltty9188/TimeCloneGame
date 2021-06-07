using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolTipIcons : MonoBehaviour
{
    public static ToolTipIcons instance;
    [SerializeField] public Sprite[] keyboardIcons;
    [SerializeField] public Sprite[] xboxIcons;
    [SerializeField] public Sprite[] psIcons;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public Sprite GetIcon(string key)
    {
        if(PlayerController.ControlScheme == "KeyboardMouse")
        {
            foreach(Sprite sprite in keyboardIcons)
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
                foreach(Sprite sprite in psIcons)
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
                foreach(Sprite sprite in xboxIcons)
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
