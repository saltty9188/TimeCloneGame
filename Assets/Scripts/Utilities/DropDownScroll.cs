using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropDownScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform content;
    void Start()
    {
        content = transform.parent.GetComponent<RectTransform>();
        scrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {

            //adapted from https://www.alexandrow.org/blog/autoscroll-dropdowns-in-unity/

            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;
    
            // visible bounds of this object
            float centerLine = transform.localPosition.y;
            float upperBound = centerLine + (GetComponent<RectTransform>().rect.height / 2f); 
            float lowerBound = centerLine - (GetComponent<RectTransform>().rect.height / 2f); 
    
            // bounds of the currently visible area
            float lowerVisible = (contentHeight - viewportHeight) * scrollRect.normalizedPosition.y - contentHeight;
            float upperVisible = lowerVisible + viewportHeight;
    
            // check if item is currently visible
            float desiredLowerBound;
            if (upperBound > upperVisible) 
            {
                // need to scroll up to upperBound
                desiredLowerBound = upperBound - viewportHeight + GetComponent<RectTransform>().rect.height * 0.3f;
            } 
            else if (lowerBound < lowerVisible) 
            {
                // need to scroll down to lowerBound
                desiredLowerBound = lowerBound - GetComponent<RectTransform>().rect.height * 0.3f;
            } 
            else 
            {
                // item already visible - all good
                return;
            }
    
            // normalize and set the desired viewport
            float normalizedDesired = (desiredLowerBound + contentHeight) / (contentHeight - viewportHeight);
            scrollRect.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
        }
    }
}
