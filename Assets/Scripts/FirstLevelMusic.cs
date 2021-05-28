using UnityEngine;

public class FirstLevelMusic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) 
    {
        AudioManager.instance.PlayLevelTheme(1);    
    }
}
