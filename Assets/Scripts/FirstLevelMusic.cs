using UnityEngine;

public class FirstLevelMusic : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other) 
    {
        AudioManager.instance.PlayLevelTheme(1);    
    }
}
