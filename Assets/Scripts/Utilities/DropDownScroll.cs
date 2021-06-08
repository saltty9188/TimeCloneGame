using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// The DropDownScroll class is attached to items in ScrollViews so that they remain on screen when navigated with controller.
/// </summary>
public class DropDownScroll : MonoBehaviour
{
    #region Private fields
    private ScrollRect _scrollRect;
    private RectTransform _content;
    #endregion

    void Start()
    {
        _content = transform.parent.GetComponent<RectTransform>();
        _scrollRect = transform.parent.parent.parent.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {

            //adapted from https://www.alexandrow.org/blog/autoscroll-dropdowns-in-unity/

            float contentHeight = _scrollRect.content.rect.height;
            float viewportHeight = _scrollRect.viewport.rect.height;
    
            // visible bounds of this object
            float centerLine = transform.localPosition.y;
            float upperBound = centerLine + (GetComponent<RectTransform>().rect.height / 2f); 
            float lowerBound = centerLine - (GetComponent<RectTransform>().rect.height / 2f); 
    
            // bounds of the currently visible area
            float lowerVisible = (contentHeight - viewportHeight) * _scrollRect.normalizedPosition.y - contentHeight;
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
            _scrollRect.normalizedPosition = new Vector2(0f, Mathf.Clamp01(normalizedDesired));
        }
    }
}
