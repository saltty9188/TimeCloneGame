using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The Button class can activate various <see cref="ButtonEvent">ButtonEvents</see> in the scene when stepped on.
/// </summary>
public class Button : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("What can activate this button?")]
    [SerializeField] private LayerMask _canActivateButton;
    [Tooltip("How long is the raycast sent up from the button.")]
    [SerializeField] private float _buttonSensitivity = 0.3f;
    [Tooltip("Minimum rigidbody mass needed to trigger this button.")]
    [SerializeField] private float _minWeight = 1;
    [Tooltip("Will the button stay down when pressed?")]
    [SerializeField] private bool _stayDown = false;
    [Tooltip("What button events does this activate?")]
    [SerializeField] private ButtonEvent[] _attachedEvents;
    #endregion

    #region Private fields
    private Light2D _light;
    private Vector3 _defaultPos;
    private float _buttonHeight;
    private bool _buttonDown;

    // array determining if corresponding event in the _attachedEvents array has been triggered
    private bool[] _eventsTriggered;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    #endregion

    void Start()
    {
        _light = GetComponentInChildren<Light2D>();
        _defaultPos = transform.localPosition;
        _buttonDown = false;
        _eventsTriggered = new bool[_attachedEvents.Length];

        _buttonHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localPosition.y;
        _startPoint = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        _endPoint = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
    }

    void FixedUpdate()
    {
        float width = Vector2.Distance(_startPoint, _endPoint);
        float divisor = 6;
        bool buttonDownThisFrame = false;
        // Do 6 raycasts across the face of the button
        for(float i = 0; i <= divisor; i += 1)
        {
            Vector2 point = _startPoint + new Vector2(transform.right.x, transform.right.y) * (i * width / divisor);
            
            RaycastHit2D hit =  Physics2D.Raycast(point, transform.up, _buttonSensitivity, _canActivateButton);
            if(hit && hit.collider.GetComponent<Rigidbody2D>() && hit.collider.GetComponent<Rigidbody2D>().mass >= _minWeight)
            {
                _buttonDown = true;
                buttonDownThisFrame = true;
            }
        }

        // if the button wasn't triggered this frame put it back up
        if(!buttonDownThisFrame)
        {
            _buttonDown = false;
        }

        // Move the button down if its triggered
        if(_buttonDown)
        {
            transform.position = _defaultPos - transform.up * _buttonHeight / 2;
            _light.enabled = true;

            for(int i = 0; i < _attachedEvents.Length; i++)
            {
                if(!_eventsTriggered[i]) 
                {
                    _attachedEvents[i].AddActivation();
                    _eventsTriggered[i] = true;
                }
            }
        }
        else if(!_stayDown)
        {
            transform.position = _defaultPos;
            _light.enabled = false;
            for(int i = 0; i < _attachedEvents.Length; i++)
            {
                if(_eventsTriggered[i]) 
                {
                    _attachedEvents[i].RemoveActivation();
                    _eventsTriggered[i] = false;
                }
            }
        }

    }

    /// <summary>
    /// Calls <see cref="ButtonEvent.ResetEvent">ResetEvent</see> on all of the <see cref="ButtonEvent">ButtonEvents</see> attached to this Button.
    /// </summary>
    public void ResetAttachedEvents()
    {
        transform.position = _defaultPos;
        _light.enabled = false;
        _buttonDown = false;
        for(int i = 0; i < _attachedEvents.Length; i++)
        {
            _attachedEvents[i].ResetEvent();
            _eventsTriggered[i] = false;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Vector2 v1 = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Vector2 v2 = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Gizmos.DrawLine(v1, v1 + (Vector2) transform.up * _buttonSensitivity);
        Gizmos.DrawLine(v2, v2 + (Vector2) transform.up * _buttonSensitivity);
    }
}
