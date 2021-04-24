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
    private float startX;
    private float endX;
    private float yPos;
    #endregion

    void Start()
    {
        defaultY = transform.localPosition.y;
        buttonDown = false;
        buttonMovedDown = false;
        eventsTriggered = new bool[attachedEvents.Length];

        startX = transform.position.x - (GetComponent<SpriteRenderer>().bounds.size.x / 2f);
        endX = transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2f);
        yPos = transform.position.y + 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float width = endX - startX;
        float divisor = 6;
        bool buttonDownThisFrame = false;
        for(float i = 0; i <= divisor; i += 1)
        {
            float xPos = startX + (i * width / divisor);
            
            RaycastHit2D hit =  Physics2D.Raycast(new Vector2(xPos, yPos), transform.up, buttonSensitivity, canActivateButton);
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
}
