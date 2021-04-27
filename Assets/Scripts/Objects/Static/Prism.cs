using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MovableObject
{
    #region Inspector fields
    [SerializeField] private Vector2[] outputDirections;
    #endregion

    // #region Private fields
    // List<LineRenderer> lines;
    // #endregion

    void Start()
    {
        // lines = new List<LineRenderer>();
        // foreach(Vector2 direction in outputDirections)
        // {
        //     direction.Normalize();
        //     GameObject go = new GameObject("Line " + (lines.Count + 1));
        //     go.transform.parent = gameObject.transform;
        //     LineRenderer line = go.AddComponent<LineRenderer>();
        //     line.useWorldSpace = true;
        //     line.material = new Material(Shader.Find("Unlit/Color"));
            
        //     Color lineCol = Color.green;
        //     lineCol.a = 0.5f;

        //     line.startColor = lineCol;
        //     line.endColor = lineCol;
        //     line.startWidth = 0.08f;
        //     line.endWidth = line.startWidth;
        //     line.receiveShadows = false;
        //     line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //     line.positionCount = 2;

        //     Vector3[] points = new Vector3[]{transform.position, transform.position + new Vector3(direction.x, direction.y, 0) * 1f};
        //     line.SetPositions(points);
        //     lines.Add(line);
        // }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        Projectile p = other.GetContact(0).collider.GetComponent<Projectile>();
        if(p && p.laser)
        {
            GameObject template = p.gameObject;

            foreach(Vector2 direction in outputDirections)
            {
                GameObject go = Instantiate(template, transform.position + new Vector3(direction.x, direction.y, 0) * 0.5f, Quaternion.identity);
                Projectile p1 = go.GetComponent<Projectile>();
                p1.direction = direction;
                p1.SetShooter(gameObject, true);
                p1.enabled = true;
                p1.GetComponent<Collider2D>().enabled = true;
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), go.GetComponent<Collider2D>());
            }

            Destroy(other.GetContact(0).collider.gameObject);            
        }
    }


}
