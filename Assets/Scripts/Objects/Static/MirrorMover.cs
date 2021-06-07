using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The MirrorMover class is responsible for allowing the player to adjust the position of <see cref="MovableObject">MovableObjects</see>.
/// </summary>
public class MirrorMover : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Array of the objects this mirror mover can move.")]
    [SerializeField] public GameObject[] _movableObjects;
    [Tooltip("The camera that will be used when moving objects.")]
    [SerializeField] private Camera _moveCamera;
    [Tooltip("The main gameplay camera.")]
    [SerializeField] private Camera _mainCamera;
    [Tooltip("How fast the objects move.")]
    [SerializeField] private float _objectMoveSpeed = 5f;
    [Tooltip("The controller tool tip icons for cycling objects.")]
    [SerializeField] private Image[] _toolTipIcons;
    [Tooltip("The image that displays the tool tip for exiting the mirror mover.")]
    [SerializeField] private Image _backImage;
    #endregion

    #region Private fields
    private GameObject _currentObject;
    private bool _usingCamera;
    private int _index;
    private Rigidbody2D _currentObjectRigidbody;
    #endregion

    void Start()
    {
        _index = 0;
        _currentObject = _movableObjects[0];
        _currentObjectRigidbody = _currentObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(_currentObject.transform.parent.tag == "Enemy") UpdateRigidBodies();
    }

    void Initialise()
    {
        _index = 0;
        _currentObject = _movableObjects[0];
        _currentObjectRigidbody = _currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    /// <summary>
    /// Sets the initial position of all the <see cref="MovableObject">MovableObjects</see> attached to this MirrorMover.
    /// </summary>
    /// <seealso cref="MovableObject.SetInitialPosition"/>
    public void SetInitialPositions()
    {
        foreach(GameObject go in _movableObjects)
        {
            go.GetComponent<MovableObject>().SetInitialPosition();
        }
    }

    /// <summary>
    /// Resets the position of all the <see cref="MovableObject">MovableObjects</see> attached to this MirrorMover.
    /// </summary>
    /// <seealso cref="MovableObject.ResetPosition"/>
    public void ResetPositions()
    {
        foreach(GameObject go in _movableObjects)
        {
            go.GetComponent<MovableObject>().ResetPosition();
            go.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Sets up the MirrorMover to be used by the player.
    /// </summary>
    /// <remarks>
    /// Swaps the active camera if the player is using it and initialises the MovableObject Rigidbodies.
    /// </remarks>
    /// <param name="switchCam">Whether or not the active camera should switch to the one that is used by the MirrorMover.</param>
    public void StartMover(bool switchCam = true)
    {
        _usingCamera = switchCam;
        if(switchCam)
        {
            _moveCamera.enabled = true;
            _toolTipIcons[0].transform.parent.gameObject.SetActive(true);
            _mainCamera.enabled = false;
        }
        Initialise();
    }

    /// <summary>
    /// Cleans up the MirrorMover for when the player stops using it.
    /// </summary>
    /// <remarks>
    /// Swaps the camera back to the main camera if needed and makes the attached <see cref="MovableObject">MovableObjects</see> kinematic again.
    /// </remarks>
    public void ExitMover()
    {
        if(_usingCamera)
        {
            _moveCamera.enabled = false;
            _toolTipIcons[0].transform.parent.gameObject.SetActive(false);
            _mainCamera.enabled = true;
        }
        
        foreach(GameObject obj in _movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.velocity = Vector2.zero;
            temp.isKinematic = true;
            temp.useFullKinematicContacts = false;
        }

        ResetOutline();
    }

    /// <summary>
    /// Cycles the current selected GameObject to the next one in the array.
    /// </summary>
    public void CycleNextObject()
    {
        _currentObjectRigidbody.velocity = Vector2.zero;
        if(_index < _movableObjects.Length - 1)
        {
            _currentObject = _movableObjects[++_index];
        }
        else
        {
            _currentObject = _movableObjects[0];
            _index = 0;
        }
        _currentObjectRigidbody = _currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    /// <summary>
    /// Cycles the current selected GameObject to the previous one in the array.
    /// </summary>
    public void CyclePrevObject()
    {
        _currentObjectRigidbody.velocity = Vector2.zero;
        if(_index > 0)
        {
            _currentObject = _movableObjects[--_index];
        }
        else
        {
            _currentObject = _movableObjects[_movableObjects.Length - 1];
            _index = _movableObjects.Length - 1;
        }
        _currentObjectRigidbody = _currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    /// <summary>
    /// Moves the current MovableObject in the direction specified by the direction parameter.
    /// </summary>
    /// <param name="direction">The direction for the MovableObject to move.</param>
    public void Move(Vector2 direction)
    {
        // Don't move the object if it's being carried by a SpiderBot
        if(_currentObject.transform.parent.tag != "Enemy") _currentObjectRigidbody.velocity = direction * _objectMoveSpeed;
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if(_usingCamera) _moveCamera.transform.position = _currentObject.transform.position + new Vector3(0, 0, _moveCamera.transform.position.z);
    }

    void UpdateRigidBodies()
    {
        // Set everything to be kinematic
        foreach(GameObject obj in _movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.isKinematic = true;
            temp.useFullKinematicContacts = true;
        }

        // Set the current object to be dynamic with no gravity if its not being grabbed by an enemy
        if(_currentObject.transform.parent.tag != "Enemy")
        {
            _currentObjectRigidbody.isKinematic = false;
            _currentObjectRigidbody.useFullKinematicContacts = false;
            _currentObjectRigidbody.gravityScale = 0;
        }
    }

    // Outline the curent object with a cyan light
    void MakeOutline()
    {
        ResetOutline();

        Light2D currentLight = _currentObject.GetComponentInChildren<Light2D>();
        currentLight.color = Color.cyan;
    }

    // reset the movable object lights
    void ResetOutline()
    {
        foreach(GameObject go in _movableObjects)
        {    
            Light2D light = go.GetComponentInChildren<Light2D>();
            light.color = Color.white;
        }
    }

    /// <summary>
    /// Sets the next, previous, and back tool tips to correspond to the appropriate control scheme.
    /// </summary>
    /// <param name="sprites">The next and previous tool tip sprites.</param>
    /// <param name="backSprite">The back tool tip sprite.</param>
    public void SetToolTips(Sprite[] sprites, Sprite backSprite)
    {
        _toolTipIcons[0].sprite = sprites[0];
        _toolTipIcons[1].sprite = sprites[1];
        _backImage.sprite = backSprite;
    }
}
