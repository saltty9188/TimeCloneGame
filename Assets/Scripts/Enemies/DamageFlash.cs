using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The DamageFlash class causes the attached to flash opaque white when taking damage.
/// </summary>
public class DamageFlash : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The color the entity will flash.")]
    [SerializeField] private Color _flashColor = Color.white;
    [Tooltip("The duration of the flash.")]
    [SerializeField] private float _duration = 0.1f;
    #endregion

    #region Private fields
    private Material _flashMaterial;
    private SpriteRenderer _spriteRenderer;
    private Material _startingMaterial;
    private Color _startingColor;
    private Coroutine _flashRoutine;
    #endregion
    
    void Awake()
    {
        _flashMaterial = Resources.Load<Material>("Flash");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startingMaterial = _spriteRenderer.material;
        _startingColor = _spriteRenderer.color;
        _flashColor.a = 1;
    }

    /// <summary>
    /// Causes the entity to flash for the given duration then restores the original material.
    /// </summary>
    public void Flash()
    {
        if(_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
        }

        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    /// <summary>
    /// Resets the entites color and material to its default.
    /// </summary>
    public void ResetShader()
    {
        SetColour(_startingMaterial, _startingColor);
        if(_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
        }
    }

    IEnumerator FlashRoutine()
    {
        SetColour(_flashMaterial, _flashColor);

        yield return new WaitForSeconds(_duration);

        SetColour(_startingMaterial, _startingColor);

        _flashRoutine = null;
    }

    /// <summary>
    /// Sets the default tint for the given entity.
    /// </summary>
    /// <param name="color">The color for the entity to be tinted.</param>
    public void SetBaseColour(Color color)
    {
        _startingColor = color;
        SetColour(_startingMaterial, _startingColor);
    }

    void SetColour(Material material, Color color)
    {   
        // Sets the material and color recursively for the entity and all its children
        _spriteRenderer.material = material;
        _spriteRenderer.color = color;
        if(transform.childCount > 0) SetColour(material, color, gameObject);
    }

    void SetColour(Material material, Color color, GameObject parent)
    {
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if(sr)
            {
                sr.material = material;
                sr.color = color;
            }
            if(child.transform.childCount > 0) SetColour(material, color, child);
        }
    }
}
