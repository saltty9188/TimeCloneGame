using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Icon
{
    public Sprite sprite;
    public string name;

    public Icon(string name, Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}
