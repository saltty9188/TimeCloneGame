using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The Door class is a ButtonEvent that controls how the doors in the game open and close.
/// </summary>
public class Door : ButtonEvent
{
    #region Inspector fields
    [Tooltip("How fast does the door move when opening and closing.")]
    [SerializeField] private float _speed = 10;
    [Tooltip("Does the door start on?")]
    [SerializeField] private bool _startsOn = false;
    [Tooltip("Does the door start in the up position?")]
    [SerializeField] private bool _startsUp = false;
    [SerializeField] private TextMeshProUGUI _doorText;
    #endregion

    #region Private fields
    private Vector3 _downPosition;
    private Vector3 _upPosition;
    private Coroutine _coroutine;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    private GameObject _spriteMask;
    private float _distFromDown;
    #endregion

    protected virtual void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();


        if(_startsUp)
        {
            _upPosition = transform.position;
            _downPosition = _upPosition - transform.up * _spriteRenderer.sprite.bounds.size.y;
            _distFromDown = _spriteRenderer.sprite.bounds.size.y;
        }
        else
        {
            _downPosition = transform.position;
            _upPosition = _downPosition + transform.up * _spriteRenderer.sprite.bounds.size.y;
            _distFromDown = 0;
        }

        //Activate text showing number of required activations
        if(RequiredActivations > 1) transform.GetChild(0).gameObject.SetActive(true);

        // Activate the door if it starts on
        if(_startsOn)
        {
            Activations = RequiredActivations;
            Activate();
        }

        _spriteMask = transform.GetChild(1).gameObject;
    }

    protected virtual void Update()
    {
        // disable the sprite renderer and collider when the door is up
        if(transform.position == _upPosition)
        {
            _boxCollider2D.enabled = false;
            _spriteRenderer.enabled = false;
        }
        else
        {
            _spriteRenderer.enabled = true;
            _boxCollider2D.enabled = true;
        }

        if(_doorText)
        {
            _doorText.text = (RequiredActivations - Activations).ToString();
        }

       _spriteMask.transform.position = _downPosition;

    }

    /// <summary>
    /// Causes the Door to move from its default position to its activated position.
    /// </summary>
    protected override void Activate() 
    {
        if(_startsUp)
        {
            if(_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(GoDown(_speed));
        }
        else
        {
            if(_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(GoUp(_speed));
        }
    }

    /// <summary>
    /// Causes the Door to move from its activated position back to its default position.
    /// </summary>
    protected override void Deactivate()
    {
       if(_startsUp)
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoUp(_speed));
       }
       else
       {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoDown(_speed));
       }
    }

    /// <summary>
    /// Resets the dDoor immediately back to its default position.
    /// </summary>
    public override void ResetEvent()
    {
        if(_startsUp)
        {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoUp(_speed*5));
        }
        else
        {
           if(_coroutine != null) StopCoroutine(_coroutine);
           _coroutine = StartCoroutine(GoDown(_speed*5));
        }
        Activations = 0;
    }

    IEnumerator GoUp(float moveSpeed)
    {
        while(transform.position != _upPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, _upPosition, moveSpeed * Time.deltaTime);
            _distFromDown += moveSpeed * Time.deltaTime;
            ShrinkCollider();
            yield return null;
        }
        _distFromDown = _spriteRenderer.sprite.bounds.size.y;
        ShrinkCollider();
        if(_doorText) _doorText.gameObject.SetActive(false);
    }

    IEnumerator GoDown(float moveSpeed)
    {
        while(transform.position != _downPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, _downPosition, moveSpeed * Time.deltaTime);
            _distFromDown -= moveSpeed * Time.deltaTime;
            ShrinkCollider();
            yield return null;
        }
        _distFromDown = 0;
        ShrinkCollider();
        if(_doorText) _doorText.gameObject.SetActive(true);
    }

    void ShrinkCollider()
    {
        // Shrink door hitbox relative to how far up it is from the ground
        _boxCollider2D.size = new Vector2(_boxCollider2D.size.x, _spriteRenderer.sprite.bounds.size.y - _distFromDown);
        _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x, _boxCollider2D.size.y / 2.0f);
    }
}
