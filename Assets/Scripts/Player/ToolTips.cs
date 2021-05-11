using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ToolTips : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI toolTip;
    private PlayerControls controls;

    void Start()
    {
        controls = GetComponent<PlayerController>().CurrentControls;
    }

    void Update()
    {
        if(transform.localScale.x < 0)
        {
            Vector3 temp = toolTip.transform.localScale;
            temp.x = -Mathf.Abs(temp.x);
            toolTip.transform.localScale = temp;
        }
        else
        {
            Vector3 temp = toolTip.transform.localScale;
            temp.x = Mathf.Abs(temp.x);
            toolTip.transform.localScale = temp;
        }
    }

    public void GrabToolTip(bool hit)
    {
        if(hit)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.text = GetToolTip(controls.Gameplay.Grab);
        }
        else
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "CloneDevice")
        {
            toolTip.gameObject.SetActive(true);
            toolTip.text = GetToolTip(controls.Gameplay.Jump);
        }
        else if(other.tag == "MirrorMover")
        {
            toolTip.gameObject.SetActive(true);
            toolTip.text = GetToolTip(controls.Gameplay.Interact);
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        toolTip.gameObject.SetActive(false);
    }

    string GetToolTip(InputAction action)
    {
        int bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(PlayerController.controlScheme));
        string output = action.GetBindingDisplayString(bindingIndex).ToLower();

        if(output.Contains("press")) output = output.Substring(6);

        //temp mapping ps buttons to xinput
        if(output == "square") output = "X";
        if(output == "cross") output = "A";
        if(output == "circle") output = "B";
        if(output == "triangle") output = "Y";
        if(output == "L2") output = "LT";
        if(output == "L1") output = "LB";
        if(output == "R2") output = "RT";
        if(output == "R1") output = "RB";

        return output;
    }
}
