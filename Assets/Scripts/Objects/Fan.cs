using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{

    public LayerMask layerMask;
    public float fanDist = 10;
    public float fanPower = 10;
    float startX;
    float endX;
    float yPos;

    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x - GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        endX = transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        yPos = transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y / 2f + 0.01f) * transform.up.y;

    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> objectsHit = new List<GameObject>();
        float width = endX - startX;
        float divisor = 6;
        for(float i = 0; i <= divisor; i += 1)
        {
            float xPos = startX + (i * width / divisor);
            RaycastHit2D hit =  Physics2D.Raycast(new Vector2(xPos, yPos), transform.up, fanDist, layerMask);
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
}
