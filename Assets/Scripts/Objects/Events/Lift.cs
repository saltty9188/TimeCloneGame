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

    private Rigidbody2D rigidbody2D;

    private Coroutine coroutine;
    #endregion

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
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
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown());
       }
       else
       {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp());
       }
    }

    protected override void Deactivate()
    {
       if(startsUp)
       {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoUp());
       }
       else
       {
           if(coroutine != null) StopCoroutine(coroutine);
           coroutine = StartCoroutine(GoDown());
       }
    }

    public override void ResetEvent()
    {
        rigidbody2D.velocity = Vector2.zero;
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
            //transform.Translate(new Vector3(0, speed * Time.deltaTime, 0), Space.World);
            rigidbody2D.velocity = Vector2.up * speed;
            yield return null;
        }
        rigidbody2D.velocity = Vector2.zero;
    }

    IEnumerator GoDown()
    {
        while(transform.position.y > downY)
        {
            //transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0), Space.World);
            rigidbody2D.velocity = Vector2.up * -speed;
            yield return null;
        }
        rigidbody2D.velocity = Vector2.zero;
    }
}
