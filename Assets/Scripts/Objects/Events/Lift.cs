using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : ButtonEvent
{

    #region Inspector fields
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private bool startsUp = true;
    #endregion

    #region Private fields
    private float upY;
    private float downY;

    private bool goingUp;

    private bool goingDown;

    private Coroutine coroutine;
    #endregion

    void Start()
    {
        if(startsUp)
        {
            upY = transform.position.y;
            downY = transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y;
        }
        else
        {
            downY = transform.position.y;
            upY = transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y;
        }
    }

    protected override void Activate() 
    {
        if(startsUp)
       {
           goingDown = true;
           goingUp = false;
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown());
       }
       else
       {
           goingDown = false;
           goingUp = true;
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp());
       }
    }

    protected override void Deactivate()
    {
       if(startsUp)
       {
           goingDown = false;
           goingUp = true;
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp());
       }
       else
       {
           goingDown = true;
           goingUp = false;
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown());
       }
    }

    public override void ResetEvent()
    {
        if(startsUp)
        {
            transform.position = new Vector3(transform.position.x, upY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, downY, transform.position.z);
        }

        activations = 0;
        StopAllCoroutines();
    }

    IEnumerator GoUp()
    {
        while(transform.position.y < upY)
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0), Space.World);
            yield return null;
        }
    }

    IEnumerator GoDown()
    {
        while(transform.position.y > downY)
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0), Space.World);
            yield return null;
        }
    }
}
