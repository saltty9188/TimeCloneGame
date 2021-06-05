using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Lift class is a ButtonEvent that controls how the lifts in the game move up and down.
/// </summary>
public class Lift : ButtonEvent
{
    #region Inspector fields
    [Tooltip("How fast the lift moves.")]
    [SerializeField] private float _speed = 2.0f;
    [Tooltip("Does the lift start in the up posiiton?")]
    [SerializeField] private bool startsUp = true;
    #endregion

    #region Private fields
    private float _upY;
    private float _downY;
    private Rigidbody2D _rigidbody2D;
    private Coroutine _coroutine;
    #endregion

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if(startsUp)
        {
            _upY = transform.position.y;
            _downY = transform.position.y - GetComponent<SpriteRenderer>().bounds.size.y;
        }
        else
        {
            _downY = transform.position.y;
            _upY = transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y;
        }
    }

    /// <summary>
    /// Causes the Lift to move towards its activate position.
    /// </summary>
    protected override void Activate() 
    {
        if(startsUp)
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoDown());
       }
       else
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoUp());
       }
    }

    /// <summary>
    /// Causes the Lift to move towards its default position.
    /// </summary>
    protected override void Deactivate()
    {
       if(startsUp)
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoUp());
       }
       else
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoDown());
       }
    }

     /// <summary>
    /// Resets the Lift immediately back to its default position.
    /// </summary>
    public override void ResetEvent()
    {
        _rigidbody2D.velocity = Vector2.zero;
        if(startsUp)
        {
            transform.position = new Vector3(transform.position.x, _upY, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, _downY, transform.position.z);
        }

        Activations = 0;
        StopAllCoroutines();
    }

    IEnumerator GoUp()
    {
        while(transform.position.y < _upY)
        {
            //transform.Translate(new Vector3(0, speed * Time.deltaTime, 0), Space.World);
            _rigidbody2D.velocity = Vector2.up * _speed;
            yield return null;
        }
        _rigidbody2D.velocity = Vector2.zero;
    }

    IEnumerator GoDown()
    {
        while(transform.position.y > _downY)
        {
            //transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0), Space.World);
            _rigidbody2D.velocity = Vector2.up * -_speed;
            yield return null;
        }
        _rigidbody2D.velocity = Vector2.zero;
    }
}
