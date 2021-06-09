using UnityEngine;

/// <summary>
/// The Spark class is responsible for altering its size and position relative to the broken TimeCloneDevice it is a child of.
/// </summary>
public class Spark : MonoBehaviour
{
    #region Private fields
    private Animator _animator;
    #endregion
    void Start()
    {
        float deviceWidth = transform.parent.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        transform.localPosition = new Vector3(Random.Range(-deviceWidth/2, deviceWidth/2), transform.localPosition.y, transform.localPosition.z);
        _animator = GetComponent<Animator>();
        _animator.SetFloat("startOffset", Random.Range(0.0f, 1.0f));
        _animator.SetFloat("speed", Random.Range(0.5f, 2.0f));
    }
}
