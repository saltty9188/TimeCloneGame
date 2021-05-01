using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : ButtonEvent
{
    #region Inspector fields
    [SerializeField] private bool startsOn = false;
    [SerializeField] private bool startsUp = false;
    [SerializeField] private TextMeshProUGUI tempText;
    #endregion

    #region Private fields
    private Vector3 defaultPosition;
    private Vector3 activePosition;
    private Vector3 upPosition;
    private BoxCollider2D boxCollider2D;

    private SpriteRenderer spriteRenderer;
    #endregion

    protected virtual void Start()
    {

        if(startsUp)
        {
            defaultPosition = transform.position;
            activePosition = defaultPosition - transform.up * GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        }
        else
        {
            defaultPosition = transform.position;
            activePosition = defaultPosition + transform.up * GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        }

        upPosition = (startsUp ? defaultPosition : activePosition);
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Activate text showing number of required activations
        if(requiredActivations > 1) transform.GetChild(0).gameObject.SetActive(true);

        if(startsOn)
        {
            activations = requiredActivations;
            Activate();
        }
    }

    protected virtual void Update()
    {
        if(transform.position == upPosition)
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
    }

    //Replace with animation later
    protected override void Activate() 
    {
        transform.position = activePosition;
        if(tempText) tempText.gameObject.SetActive(false);
    }

    protected override void Deactivate()
    {
        transform.position = defaultPosition;
        if(tempText) tempText.gameObject.SetActive(true);
    }

    public override void ResetEvent()
    {
        transform.position = defaultPosition;
        if(tempText) tempText.gameObject.SetActive(true);
        activations = 0;
    }
}
