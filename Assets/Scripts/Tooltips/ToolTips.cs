using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour
{

    [SerializeField] private Image toolTipIcon;
    [SerializeField] private Image[] physicsCycleIcons;
    [SerializeField] private Image cancelRecordingImage;
    private PlayerControls controls;
    private PlayerController playerController;
    private Aim aim;
    private Recorder recorder;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        controls = playerController.CurrentControls;
        recorder = GetComponent<Recorder>();
        aim = transform.GetChild(0).GetComponent<Aim>();
    }

    void Update()
    {
        if(transform.localScale.x < 0)
        {
            Vector3 temp = toolTipIcon.transform.localScale;
            temp.x = -Mathf.Abs(temp.x);        
            toolTipIcon.transform.localScale = temp;
        }
        else
        {
            Vector3 temp = toolTipIcon.transform.localScale;
            temp.x = Mathf.Abs(temp.x);        
            toolTipIcon.transform.localScale = temp;
        }

        SetPhysicsIcons();
        if(recorder.IsRecording && !playerController.MovingMirrors)
        {
            cancelRecordingImage.gameObject.SetActive(true);
            cancelRecordingImage.sprite = ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.Record));
        }
        else
        {
            cancelRecordingImage.gameObject.SetActive(false);
        }
    }

    public void GrabToolTip(bool hit)
    {
        if(hit)
        {
            toolTipIcon.gameObject.SetActive(true);
            toolTipIcon.sprite = ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.Grab));
        }
        else
        {
            toolTipIcon.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "CloneDevice")
        {
            toolTipIcon.gameObject.SetActive(true);
            toolTipIcon.sprite = FindObjectOfType<ToolTipIcons>().GetIcon(GetToolTip(controls.Gameplay.Record));
        }
        else if(other.tag == "MirrorMover")
        {
            toolTipIcon.gameObject.SetActive(true);
            toolTipIcon.sprite = FindObjectOfType<ToolTipIcons>().GetIcon(GetToolTip(controls.Gameplay.Interact));
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "CloneDevice" || other.tag == "MirrorMover")
        {
            toolTipIcon.gameObject.SetActive(false);
        }
    }

    string GetToolTip(InputAction action)
    {
        int bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(PlayerController.ControlScheme));
        string output = action.GetBindingDisplayString(bindingIndex).ToLower();

        if(action.bindings[bindingIndex].isPartOfComposite)
        {
            while (action.bindings[++bindingIndex].isPartOfComposite)
            {
                output += " " + action.GetBindingDisplayString(bindingIndex).ToLower();
            }
        }

        if(output.Contains("press")) output = output.Substring(6);

        return output;
    }

    string GetToolTip(InputAction action, int compositeIndex)
    {
        int bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(PlayerController.ControlScheme));
        string output = action.GetBindingDisplayString(bindingIndex).ToLower();

        if(action.bindings[bindingIndex].isPartOfComposite)
        {
            bindingIndex += compositeIndex;
            output = action.GetBindingDisplayString(bindingIndex).ToLower();
        }

        return output;
    }

    public void SetMoverToolTips(MirrorMover mover)
    {
        Sprite[] sprites = new Sprite[2];
        sprites[0] = ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.CycleObjects, 0));
        sprites[1] = ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.CycleObjects, 1));
        mover.SetToolTips(sprites, ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.Cancel)));
    }

    void SetPhysicsIcons()
    {
        if(aim.CurrentWeapon && typeof(PhysicsRay).IsInstanceOfType(aim.CurrentWeapon))
        {
            foreach(Image icon in physicsCycleIcons)
            {
                icon.gameObject.SetActive(true);
            }
            if(PlayerController.ControlScheme == "KeyboardMouse")
            {
                physicsCycleIcons[1].sprite = ToolTipIcons.instance.GetIcon("scroll-down");
                physicsCycleIcons[0].sprite = ToolTipIcons.instance.GetIcon("scroll-up");
            }   
            else
            {
                for(int i = 0; i < physicsCycleIcons.Length; i++)
                {
                    physicsCycleIcons[i].sprite = ToolTipIcons.instance.GetIcon(GetToolTip(controls.Gameplay.CyclePhysics, i));
                }
            }
        }
        else
        {
            foreach(Image icon in physicsCycleIcons)
            {
               if(icon) icon.gameObject.SetActive(false);
            }
        }
    }
}
