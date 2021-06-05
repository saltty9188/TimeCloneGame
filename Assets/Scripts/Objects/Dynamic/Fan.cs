using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Fan class pushes <see cref="PhysicsObject">PhysicsObjects</see> and the player using wind.
/// </summary>
public class Fan : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("Which objects can the fan push?")]
    [SerializeField] private LayerMask _pushableObjects;
    [Tooltip("How far the fan's wind is effective.")]
    [SerializeField] private float _fanDist = 9;
    [Tooltip("The amount of power the fan exerts on objects.")]
    [SerializeField] private float _fanPower = 2000;
    [Tooltip("Prefab for the wind animation.")]
    [SerializeField] private GameObject _windPrefab;
    #endregion

    #region Private fields
    private SpriteMask _spriteMask;
    private GameObject[] _windSprites;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    #endregion

    void Start()
    {
        _startPoint = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        _endPoint = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        _spriteMask = transform.GetChild(1).GetComponent<SpriteMask>();
        AdjustMask(_fanDist);

        //populate wind sprites
        float windWidth = _windPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x * _windPrefab.transform.localScale.x;
        int numWind = (int) (_fanDist / windWidth);
        if(numWind == 0) numWind = 1;
        _windSprites = new GameObject[numWind];
        _windSprites[0] = _windPrefab;
        if(numWind > 1)
        {
            Vector3 pos = _windPrefab.transform.position + transform.up * windWidth;
            for(int i = 1; i < numWind; i++)
            {
                GameObject wind = Instantiate(_windPrefab, pos, _windPrefab.transform.rotation);
                _windSprites[i] = wind;
                pos += transform.up * windWidth;
            }
        }
    }

    
    void FixedUpdate()
    {
        // populate a list of objects hit this frame so they only have the force applied once
        List<GameObject> objectsHit = new List<GameObject>();

        // Do 6 raycasts across the face of the fan
        float width = Vector2.Distance(_startPoint, _endPoint);
        float divisor = 6;
        for(float i = 0; i <= divisor; i += 1)
        {
            Vector2 pos = _startPoint + new Vector2(transform.right.x, transform.right.y) * (i * width / divisor);
            RaycastHit2D hit =  Physics2D.Raycast(pos, transform.up, _fanDist, _pushableObjects);
            if(hit)
            {
                GameObject go = hit.collider.gameObject;
                if(!objectsHit.Contains(go))
                {
                    objectsHit.Add(go);
                    Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                    if(rb)
                    {
                        rb.drag = 0.5f;
                        // Add less force to the player and enemies
                        if(rb.mass == 1)
                        {
                            rb.AddForce(transform.up * 50);
                        }
                        else
                        {
                            rb.AddForce(transform.up * _fanPower);
                        }
                    }
                    // Adjust the wind sprite mask when a box is hit with the raycast
                    if(go.tag != "Player" && go.tag != "Clone" && go.tag != "Enemy")
                    {
                        AdjustMask(hit.distance);
                    }
                }
            }
        }
        // Set the sprite mask to the fan distance if nothing was hit
        if(objectsHit.Count == 0)
        {
            AdjustMask(_fanDist);
        }
    }

    // Shrinks/Grows the sprite mask that shows the wind animation
    private void AdjustMask(float distToHit)
    {
        _spriteMask.transform.localScale = new Vector3(_spriteMask.transform.localScale.x, (distToHit / _spriteMask.sprite.bounds.size.y) / transform.localScale.y, 1);
        _spriteMask.transform.position = transform.position + transform.up * distToHit;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Vector2 v1 = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        Vector2 v2 = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        Gizmos.DrawLine(v1, v1 + (Vector2) transform.up * _fanDist);
        Gizmos.DrawLine(v2, v2 + (Vector2) transform.up * _fanDist);
        float width = Vector2.Distance(v1, v2);
        for(float i = 0; i <= 6; i += 1)
        {
            Vector2 pos = v1 + new Vector2(transform.right.x, transform.right.y) * (i * width / 6);
            Gizmos.DrawLine(pos, pos + (Vector2) transform.up * _fanDist);
        }
    }
}
