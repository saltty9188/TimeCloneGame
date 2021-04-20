using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private float lightMass;

    [SerializeField] private float heavyMass;

    [SerializeField] private PhysicsMaterial2D bounceMaterial;

    [SerializeField] private float floatHeight;

    [SerializeField] private float floatSpeed;

    #endregion

    #region  Private fields
    private static List<PhysicsObject> allPhysicsObjects;

    private Rigidbody2D rigidbody2D;

    private SpriteRenderer spriteRenderer;
    private float initialMass;
    private Vector3 recordingStartPosition;

    private PhysicsMaterial2D initialPhysicsMaterial;

    private float yPosOnGround;
    private float yPosInAir;

    private Coroutine floatRoutine;

    //Empty child that allows player to stand on a bouncing object without bouncing themselves
    private GameObject noBouncePlatform;

    #endregion

    void Awake()
    {
        if (allPhysicsObjects == null) allPhysicsObjects = new List<PhysicsObject>();
    }

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialMass = rigidbody2D.mass;
        recordingStartPosition = transform.position;
        initialPhysicsMaterial = rigidbody2D.sharedMaterial;
        noBouncePlatform = transform.GetChild(0).gameObject;
        yPosOnGround = transform.position.y;
        allPhysicsObjects.Add(this);
    }

    void Update()
    {
        if (rigidbody2D.gravityScale == 0 && floatRoutine == null)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        }
    }

    public void UpdateRecordingStartPosition()
    {
        recordingStartPosition = transform.position;
    }

    void ResetPhysics()
    {
        rigidbody2D.isKinematic = false;
        rigidbody2D.mass = initialMass;
        rigidbody2D.sharedMaterial = initialPhysicsMaterial;
        rigidbody2D.gravityScale = 1;
        noBouncePlatform.SetActive(false);
        if (floatRoutine != null) StopCoroutine(floatRoutine);

        spriteRenderer.color = Color.white;
    }

    public static void UpdateAllInitialPositions()
    {
        if (allPhysicsObjects != null)
        {
            foreach (PhysicsObject physicsObject in allPhysicsObjects)
            {
                physicsObject.UpdateRecordingStartPosition();
            }
        }
    }

    public static void ResetAllPhysics(bool resetPosition)
    {
        if (allPhysicsObjects != null)
        {
            foreach (PhysicsObject physicsObject in allPhysicsObjects)
            {
                physicsObject.ResetPhysics();
                if (resetPosition) physicsObject.transform.position = physicsObject.recordingStartPosition;
            }
        }
    }

    void MakeLight()
    {
        rigidbody2D.mass = lightMass;
        spriteRenderer.color = Color.cyan;
    }

    void MakeHeavy()
    {
        rigidbody2D.mass = heavyMass;
        spriteRenderer.color = Color.red;
    }

    void MakeBouncy()
    {
        rigidbody2D.sharedMaterial = bounceMaterial;
        rigidbody2D.drag = 0;
        noBouncePlatform.SetActive(true);

        //Starting bounce if the object was stationary
        if (Mathf.Abs(rigidbody2D.velocity.y) < 1 &&
            Mathf.Abs(transform.position.y - yPosOnGround) < 0.1f)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 10);
        }

        spriteRenderer.color = Color.green;
    }

    void MakeFloat()
    {
        yPosInAir = yPosOnGround + floatHeight;
        floatRoutine = StartCoroutine(GoUp());
        rigidbody2D.gravityScale = 0;
        rigidbody2D.isKinematic = true;
        rigidbody2D.useFullKinematicContacts = true;
        spriteRenderer.color = Color.yellow;
    }

    IEnumerator GoUp()
    {
        while (transform.position.y < yPosInAir)
        {
            Debug.Log("Going up");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, floatSpeed);
            yield return null;
        }

        floatRoutine = null;
    }

    public void AlterPhysics(PhysicsRay.RayType rayType)
    {
        ResetAllPhysics(false);
        switch (rayType)
        {
            case PhysicsRay.RayType.Light:
                {
                    MakeLight();
                    break;
                }

            case PhysicsRay.RayType.Heavy:
                {
                    MakeHeavy();
                    break;
                }

            case PhysicsRay.RayType.Bouncy:
                {
                    MakeBouncy();
                    break;
                }

            case PhysicsRay.RayType.Float:
                {
                    MakeFloat();
                    break;
                }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.GetContact(0).normal == Vector2.up)
        {
            yPosOnGround = transform.position.y;
        }
        else if (other.GetContact(0).normal == Vector2.down && other.GetContact(0).collider.tag != "Player"
             && other.GetContact(0).collider.tag != "Clone" && floatRoutine != null)
        {
            StopCoroutine(floatRoutine);
            floatRoutine = null;
        }
    }

    void OnDestroy()
    {
        allPhysicsObjects.Remove(this);
    }


}
