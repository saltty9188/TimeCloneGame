using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// The ToolTips class is responsible for displaying tool tips to the player.
/// </summary>
public class ToolTips : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private Image _toolTipIcon;
    [SerializeField] private Image[] _physicsCycleIcons;
    [SerializeField] private Image _cancelRecordingImage;
    #endregion
    #region Private regions
    private PlayerControls _controls;
    private PlayerController _playerController;
    private Aim _aim;
    private Recorder _recorder;
    #endregion

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _controls = _playerController.CurrentControls;
        _recorder = GetComponent<Recorder>();
        _aim = transform.GetChild(0).GetComponent<Aim>();
    }

    void Update()
    {
        if(transform.localScale.x < 0)
        {
            Vector3 temp = _toolTipIcon.transform.localScale;
            temp.x = -Mathf.Abs(temp.x);        
            _toolTipIcon.transform.localScale = temp;
        }
        else
        {
            Vector3 temp = _toolTipIcon.transform.localScale;
            temp.x = Mathf.Abs(temp.x);        
            _toolTipIcon.transform.localScale = temp;
        }

        SetPhysicsIcons();
        if(_recorder.IsRecording && !_playerController.MovingMirrors)
        {
            _cancelRecordingImage.gameObject.SetActive(true);
            _cancelRecordingImage.sprite = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.Record));
        }
        else
        {
            _cancelRecordingImage.gameObject.SetActive(false);
        }
    }

    public void GrabToolTip(bool hit)
    {
        if(hit)
        {
            _toolTipIcon.gameObject.SetActive(true);
            _toolTipIcon.sprite = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.Grab));
        }
        else
        {
            _toolTipIcon.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "CloneDevice")
        {
            _toolTipIcon.gameObject.SetActive(true);
            _toolTipIcon.sprite = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.Record));
        }
        else if(other.tag == "MirrorMover")
        {
            _toolTipIcon.gameObject.SetActive(true);
            _toolTipIcon.sprite = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.Interact));
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "CloneDevice" || other.tag == "MirrorMover")
        {
            _toolTipIcon.gameObject.SetActive(false);
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
        sprites[0] = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.CycleObjects, 0));
        sprites[1] = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.CycleObjects, 1));
        mover.SetToolTips(sprites, ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.Cancel)));
    }

    void SetPhysicsIcons()
    {
        if(_aim.CurrentWeapon && typeof(PhysicsRay).IsInstanceOfType(_aim.CurrentWeapon))
        {
            foreach(Image icon in _physicsCycleIcons)
            {
                icon.gameObject.SetActive(true);
            }
            if(PlayerController.ControlScheme == "KeyboardMouse")
            {
                _physicsCycleIcons[1].sprite = ToolTipIcons.Instance.GetIcon("scroll-down");
                _physicsCycleIcons[0].sprite = ToolTipIcons.Instance.GetIcon("scroll-up");
            }   
            else
            {
                for(int i = 0; i < _physicsCycleIcons.Length; i++)
                {
                    _physicsCycleIcons[i].sprite = ToolTipIcons.Instance.GetIcon(GetToolTip(_controls.Gameplay.CyclePhysics, i));
                }
            }
        }
        else
        {
            foreach(Image icon in _physicsCycleIcons)
            {
               if(icon) icon.gameObject.SetActive(false);
            }
        }
    }
}
