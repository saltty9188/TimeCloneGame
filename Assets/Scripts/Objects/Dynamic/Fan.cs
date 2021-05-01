using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{

    public LayerMask layerMask;
    public float fanDist = 10;
    public float fanPower = 10;
    Vector2 startPoint;
    Vector2 endPoint;
    //float yPos;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        endPoint = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> objectsHit = new List<GameObject>();
        float width = Vector2.Distance(startPoint, endPoint);
        float divisor = 6;
        for(float i = 0; i <= divisor; i += 1)
        {
            Vector2 pos = startPoint + new Vector2(transform.right.x, transform.right.y) * (i * width / divisor);
            RaycastHit2D hit =  Physics2D.Raycast(pos, transform.up, fanDist, layerMask);
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
                        if(go.tag == "Player" || go.tag == "Clone")
                        {
                            rb.AddForce(transform.up * 10);
                        }
                        else
                        {
                            rb.AddForce(transform.up * fanPower);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector2 v1 = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Vector2 v2 = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x / 2;
        Gizmos.DrawLine(v1, v1 + (Vector2) transform.up * fanDist);
        Gizmos.DrawLine(v2, v2 + (Vector2) transform.up * fanDist);
    }
}
