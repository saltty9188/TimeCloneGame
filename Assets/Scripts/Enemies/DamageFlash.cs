using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private Material flashMaterial;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float duration = 0.1f;
    #endregion

    #region Private fields
    private SpriteRenderer spriteRenderer;
    private Material startingMaterial;

    private Color startingColor;
    private Coroutine flashRoutine;
    #endregion
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingMaterial = spriteRenderer.material;
        startingColor = spriteRenderer.color;
        color.a = 1;
    }

    public void Flash()
    {
        if(flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    public void ResetShader()
    {
        SetColour(startingMaterial, startingColor);
        if(flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
    }

    IEnumerator FlashRoutine()
    {
        SetColour(flashMaterial, color);

        yield return new WaitForSeconds(duration);

        SetColour(startingMaterial, startingColor);

        flashRoutine = null;
    }

    public void SetBaseColour(Color color)
    {
        startingColor = color;
        SetColour(startingMaterial, startingColor);
    }

    void SetColour(Material material, Color color)
    {
        spriteRenderer.material = material;
        spriteRenderer.color = color;
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
