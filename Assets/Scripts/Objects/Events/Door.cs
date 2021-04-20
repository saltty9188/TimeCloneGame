using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : ButtonEvent
{
    #region Inspector fields
    [SerializeField] private bool startsUp = false;
    [SerializeField] private TextMeshProUGUI tempText;
    #endregion

    #region Private fields
    private float defaultY;
    private float activeY;
    private float upY;
    private BoxCollider2D boxCollider2D;

    private SpriteRenderer spriteRenderer;
    #endregion

    void Start()
    {
        if(startsUp)
        {
            defaultY = transform.position.y;
            activeY = defaultY - GetComponent<BoxCollider2D>().bounds.size.y;
        }
        else
        {
            defaultY = transform.position.y;
            activeY = defaultY + GetComponent<BoxCollider2D>().bounds.size.y;
        }

        upY = (defaultY > activeY ? defaultY : activeY);
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Activate text showing number of required activations
        if(requiredActivations > 1) transform.GetChild(0).gameObject.SetActive(true);

    }

    void Update()
    {
        if(transform.position.y >= upY)
        {
            boxCollider2D.enabled = false;
            spriteRenderer.enabled = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
        }

        if(tempText)
        {
            tempText.text = (requiredActivations - activations).ToString();
        }
        if(activations < 0) activations = 0;
    }

    //Replace with animation later
    protected override void Activate() 
    {
        transform.position = new Vector3(transform.position.x, activeY, transform.position.z);
        if(tempText) tempText.gameObject.SetActive(false);
    }

    protected override void Deactivate()
    {
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
        if(tempText) tempText.gameObject.SetActive(true);
    }

    public override void ResetEvent()
    {
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
        if(tempText) tempText.gameObject.SetActive(true);
        activations = 0;
    }
}
