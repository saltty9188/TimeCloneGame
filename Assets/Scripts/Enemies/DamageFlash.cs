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
    
    void Start()
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
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        SetFlash(flashMaterial, color);

        yield return new WaitForSeconds(duration);

        SetFlash(startingMaterial, startingColor);

        flashRoutine = null;
    }


    void SetFlash(Material material, Color color)
    {
        spriteRenderer.material = material;
        spriteRenderer.color = color;
        if(transform.childCount > 0) SetFlash(material, color, gameObject);
    }

    void SetFlash(Material material, Color color, GameObject parent)
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
            if(child.transform.childCount > 0) SetFlash(material, color, child);
        }
    }
}
