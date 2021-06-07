using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The CloneDeath class is responsible for playing the clone death animation then destroying itself.
/// </summary>
public class CloneDeath : MonoBehaviour
{
    #region Private fields
    private Animator _animator;
    private Light2D _light;
    private float _lightIntensity;
    #endregion

    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _light = transform.GetChild(0).GetComponent<Light2D>();
        _lightIntensity = _light.intensity;
    }

    
    void Update()
    {
        float animatorTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        _light.intensity = _lightIntensity * (1.0f - animatorTime);

        if(animatorTime >= 1)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the color of the CloneDeath.
    /// </summary>
    /// <param name="color">The color for the CloneDeath.</param>
    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        _light.color = color;
    }
}
