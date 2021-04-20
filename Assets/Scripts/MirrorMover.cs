using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMover : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private GameObject[] movableObjects;
    [SerializeField] private Camera moveCamera;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float objectMoveSpeed = 0.4f;
    [SerializeField] private Material solidColour;
    [SerializeField] private Color color = Color.white;
    #endregion

    #region Private fields
    private GameObject currentObject;
    private int index;
    private Rigidbody2D currentObjectRigidbody;
    private GameObject outline;
    #endregion

    void Start()
    {
        index = 0;
        currentObject = movableObjects[0];
        currentObjectRigidbody = currentObject.GetComponent<Rigidbody2D>();
        color.a = 1;
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
    public void StartMover()
    {
        moveCamera.enabled = true;
        mainCamera.enabled = false;
        Initialise();
    }

    public void ExitMover()
    {
        moveCamera.enabled = false;
        mainCamera.enabled = true;
        foreach(GameObject obj in movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.isKinematic = false;
            temp.useFullKinematicContacts = false;
        }
        Destroy(outline);
    }

    public void CycleNextObject()
    {
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
        UpdateOutline();
        currentObjectRigidbody.velocity = direction * objectMoveSpeed;
        UpdateCamera();
        
    }

    void UpdateCamera()
    {
        moveCamera.transform.position = currentObject.transform.position + new Vector3(0, 0, moveCamera.transform.position.z);
    }

    void UpdateRigidBodies()
    {

        foreach(GameObject obj in movableObjects)
        {
            Rigidbody2D temp = obj.GetComponent<Rigidbody2D>();
            temp.isKinematic = true;
            temp.useFullKinematicContacts = true;
        }

        currentObjectRigidbody.isKinematic = false;
        currentObjectRigidbody.useFullKinematicContacts = false;
        currentObjectRigidbody.gravityScale = 0;
    }

    void MakeOutline()
    {
        if(outline != null) Destroy(outline);
        outline = Instantiate(currentObject);
        SetUpOutline(outline);
        if(outline.tag == "Reflective")
        {
            SetUpOutline(outline.transform.GetChild(0).gameObject);
        }
        Vector3 temp = outline.transform.localScale;
        temp.x += 0.2f;
        temp.y += 0.2f;
        outline.transform.localScale = temp;
    }

    void SetUpOutline(GameObject outlineObject)
    {
        outlineObject.GetComponent<BoxCollider2D>().enabled = false;
        SpriteRenderer sr = outlineObject.GetComponent<SpriteRenderer>(); 
        sr.material = solidColour;
        sr.color = color;
        sr.sortingOrder = 1;
    }

    void UpdateOutline()
    {
        outline.transform.position = currentObject.transform.position;
    }
}
