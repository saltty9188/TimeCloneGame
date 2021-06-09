using UnityEngine;

/// <summary>
/// The Intro class is responsible for playing the introduction cutscene.
/// </summary>
public class Intro : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject _player;
    #endregion  


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayMusic("Intro");
    }

    /// <summary>
    /// Animation event that allows the player to move.
    /// </summary>
    public void ShowPlayer()
    {
        _player.SetActive(true);
    }
}
