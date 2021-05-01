using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    #region Inspector fields
    [SerializeField] private LayerMask canActivateButton;
    [SerializeField] private float buttonSensitivity;
    [SerializeField] private float minWeight = 1;
    [SerializeField] private ButtonEvent[] attachedEvents;
    #endregion

    #region Private fields
    private float defaultY;
    private bool buttonDown;
    private bool buttonMovedDown;
    private bool[] eventsTriggered;
    private Vector2 startPoint;
    private Vector2 endPoint;
    #endregion

    void Start()
    {
        defaultY = transform.localPosition.y;
        buttonDown = false;
        buttonMovedDown = false;
        eventsTriggered = new bool[attachedEvents.Length];

        startPoint = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        endPoint = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float width = Vector2.Distance(startPoint, endPoint);
        float divisor = 6;
        bool buttonDownThisFrame = false;
        for(float i = 0; i <= divisor; i += 1)
        {
            Vector2 point = startPoint + new Vector2(transform.right.x, transform.right.y) * (i * width / divisor);
            
            RaycastHit2D hit =  Physics2D.Raycast(point, transform.up, buttonSensitivity, canActivateButton);
            if(hit && hit.collider.GetComponent<Rigidbody2D>() && hit.collider.GetComponent<Rigidbody2D>().mass >= minWeight)
            {
                buttonDown = true;
                buttonDownThisFrame = true;
            }
        }

        if(!buttonDownThisFrame)
        {
            buttonDown = false;
        }

        if(buttonDown)
        {
            if(!buttonMovedDown)
            {
                transform.Translate(new Vector3(0, -(GetComponent<PolygonCollider2D>().bounds.size.y) / 2.0f, 0), Space.Self);
                buttonMovedDown = true;
            }

            for(int i = 0; i < attachedEvents.Length; i++)
            {
                if(!eventsTriggered[i]) 
                {
                    attachedEvents[i].AddActivation();
                    eventsTriggered[i] = true;
                }
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
            buttonMovedDown = false;
            for(int i = 0; i < attachedEvents.Length; i++)
            {
                if(eventsTriggered[i]) 
                {
                    attachedEvents[i].RemoveActivation();
                    eventsTriggered[i] = false;
                }
            }
        }

    }

    public void ResetAttachedEvents()
    {
        for(int i = 0; i < attachedEvents.Length; i++)
        {
            attachedEvents[i].ResetEvent();
            eventsTriggered[i] = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector2 v1 = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Vector2 v2 = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Gizmos.DrawLine(v1, v1 + (Vector2) transform.up * buttonSensitivity);
        Gizmos.DrawLine(v2, v2 + (Vector2) transform.up * buttonSensitivity);
    }
}
