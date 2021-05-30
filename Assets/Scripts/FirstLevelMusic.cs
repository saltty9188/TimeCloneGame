using UnityEngine;

public class FirstLevelMusic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) 
    {
        AudioManager.Instance.PlayLevelTheme(1);    
    }
}
