using UnityEngine;

/// <summary>
/// The Explosion class is responsible for destroying itself after its animation has played.
/// </summary>
public class Explosion : MonoBehaviour
{
    
    #region Private fields
    private Animator _animator;
    #endregion

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }
}
