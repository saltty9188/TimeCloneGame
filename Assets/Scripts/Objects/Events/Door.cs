using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : ButtonEvent
{
    #region Inspector fields
    [SerializeField] private float speed = 20;
    [SerializeField] private bool startsOn = false;
    [SerializeField] private bool startsUp = false;
    [SerializeField] private TextMeshProUGUI tempText;
    #endregion

    #region Private fields
    private Vector3 downPosition;
    private Vector3 upPosition;
    private Coroutine coroutine;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private GameObject mask;
    private float distFromDown;
    #endregion

    protected virtual void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(startsUp)
        {
            upPosition = transform.position;
            downPosition = upPosition - transform.up * spriteRenderer.sprite.bounds.size.y;
            distFromDown = spriteRenderer.sprite.bounds.size.y;
        }
        else
        {
            downPosition = transform.position;
            upPosition = downPosition + transform.up * spriteRenderer.sprite.bounds.size.y;
            distFromDown = 0;
        }

        //Activate text showing number of required activations
        if(requiredActivations > 1) transform.GetChild(0).gameObject.SetActive(true);

        if(startsOn)
        {
            activations = requiredActivations;
            Activate();
        }

        mask = transform.GetChild(1).gameObject;
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

       mask.transform.position = downPosition;

    }

    protected override void Activate() 
    {
        if(startsUp)
        {
            if(coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(GoDown(speed));
        }
        else
        {
            if(coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(GoUp(speed));
        }
    }

    protected override void Deactivate()
    {
       if(startsUp)
       {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp(speed));
       }
       else
       {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown(speed));
       }
    }

    public override void ResetEvent()
    {
        if(startsUp)
        {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp(speed*5));
        }
        else
        {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown(speed*5));
        }
        activations = 0;
    }

    IEnumerator GoUp(float moveSpeed)
    {
        while(transform.position != upPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPosition, moveSpeed * Time.deltaTime);
            distFromDown += moveSpeed * Time.deltaTime;
            ShrinkCollider();
            yield return null;
        }
        distFromDown = spriteRenderer.sprite.bounds.size.y;
        ShrinkCollider();
        if(tempText) tempText.gameObject.SetActive(false);
    }

    IEnumerator GoDown(float moveSpeed)
    {
        while(transform.position != downPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPosition, moveSpeed * Time.deltaTime);
            distFromDown -= moveSpeed * Time.deltaTime;
            ShrinkCollider();
            yield return null;
        }
        distFromDown = 0;
        ShrinkCollider();
        if(tempText) tempText.gameObject.SetActive(true);
    }

    void ShrinkCollider()
    {
        // Shrink door hitbox relative to how far up it is from the ground
        boxCollider2D.size = new Vector2(boxCollider2D.size.x, spriteRenderer.sprite.bounds.size.y - distFromDown);
        boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, boxCollider2D.size.y / 2.0f);
    }
}
