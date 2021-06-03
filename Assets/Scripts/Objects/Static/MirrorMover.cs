using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class MirrorMover : MonoBehaviour
{
    #region Inspector fields
    
    [SerializeField] public GameObject[] movableObjects;
    [SerializeField] private Camera moveCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float objectMoveSpeed = 0.4f;
    [SerializeField] private Material solidColour;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Image[] toolTipIcons;
    [SerializeField] private Image backImage;
    #endregion

    #region Private fields
    private GameObject currentObject;
    private bool usingCamera;
    private int index;
    private Rigidbody2D currentObjectRigidbody;
    #endregion

    void Start()
    {
        index = 0;
        currentObject = movableObjects[0];
        currentObjectRigidbody = currentObject.GetComponent<Rigidbody2D>();
        color.a = 1;
    }

    void Update()
    {
        if(currentObject.transform.parent.tag == "Enemy") UpdateRigidBodies();
    }

    void Initialise()
    {
        index = 0;
        currentObject = movableObjects[0];
        currentObjectRigidbody = currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    public void SetInitialPositions()
    {
        foreach(GameObject go in movableObjects)
        {
            go.GetComponent<MovableObject>().SetInitialPosition();
        }
    }

    public void ResetPositions()
    {
        foreach(GameObject go in movableObjects)
        {
            go.GetComponent<MovableObject>().ResetPosition();
            go.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void StartMover(bool switchCam = true)
    {
        usingCamera = switchCam;
        if(switchCam)
        {
            moveCamera.enabled = true;
            toolTipIcons[0].transform.parent.gameObject.SetActive(true);
            mainCamera.enabled = false;
        }
        Initialise();
    }

    public void ExitMover()
    {
        if(usingCamera)
        {
            moveCamera.enabled = false;
            toolTipIcons[0].transform.parent.gameObject.SetActive(false);
            mainCamera.enabled = true;
        }
        
        foreach(GameObject obj in movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.velocity = Vector2.zero;
            temp.isKinematic = true;
            temp.useFullKinematicContacts = false;
        }

        ResetOutline();
    }

    public void CycleNextObject()
    {
        currentObjectRigidbody.velocity = Vector2.zero;
        if(index < movableObjects.Length - 1)
        {
            currentObject = movableObjects[++index];
        }
        else
        {
            currentObject = movableObjects[0];
            index = 0;
        }
        currentObjectRigidbody = currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    public void CyclePrevObject()
    {
        currentObjectRigidbody.velocity = Vector2.zero;
        if(index > 0)
        {
            currentObject = movableObjects[--index];
        }
        else
        {
            currentObject = movableObjects[movableObjects.Length - 1];
            index = movableObjects.Length - 1;
        }
        currentObjectRigidbody = currentObject.GetComponent<Rigidbody2D>();
        UpdateCamera();
        UpdateRigidBodies();
        MakeOutline();
    }

    public void Move(Vector2 direction)
    {
        if(currentObject.transform.parent.tag != "Enemy") currentObjectRigidbody.velocity = direction * objectMoveSpeed;
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if(usingCamera) moveCamera.transform.position = currentObject.transform.position + new Vector3(0, 0, moveCamera.transform.position.z);
    }

    void UpdateRigidBodies()
    {
        foreach(GameObject obj in movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.isKinematic = true;
            temp.useFullKinematicContacts = true;
        }

        if(currentObject.transform.parent.tag != "Enemy")
        {
            currentObjectRigidbody.isKinematic = false;
            currentObjectRigidbody.useFullKinematicContacts = false;
            currentObjectRigidbody.gravityScale = 0;
        }
    }

    void MakeOutline()
    {
        ResetOutline();

        Light2D currentLight = currentObject.GetComponentInChildren<Light2D>();
        currentLight.color = Color.cyan;
    }

    void ResetOutline()
    {
        foreach(GameObject go in movableObjects)
        {    
            Light2D light = go.GetComponentInChildren<Light2D>();
            light.color = Color.white;
        }
    }

    public void SetToolTips(Sprite[] sprites, Sprite backSprite)
    {
        toolTipIcons[0].sprite = sprites[0];
        toolTipIcons[1].sprite = sprites[1];
        backImage.sprite = backSprite;
    }
}
