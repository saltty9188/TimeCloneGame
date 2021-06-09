using UnityEngine;

/// <summary>
/// The FirstLevelMusic class is a trigger responsible for starting the main level music after the game intro.
/// </summary>
public class FirstLevelMusic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) 
    {
        AudioManager.Instance.PlayLevelTheme(1);    
    }
}
