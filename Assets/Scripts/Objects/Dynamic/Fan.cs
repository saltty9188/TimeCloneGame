using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{

    public LayerMask layerMask;
    public float fanDist = 10;
    public float fanPower = 200;
    [SerializeField] private GameObject windPrefab;
    private SpriteMask mask;
    private GameObject[] windSprites;
    Vector2 startPoint;
    Vector2 endPoint;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        endPoint = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        mask = transform.GetChild(1).GetComponent<SpriteMask>();
        AdjustMask(fanDist);

        //populate wind sprites
        float windWidth = windPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x * windPrefab.transform.localScale.x;
        int numWind = (int) (fanDist / windWidth);
        if(numWind == 0) numWind = 1;
        windSprites = new GameObject[numWind];
        windSprites[0] = windPrefab;
        if(numWind > 1)
        {
            Vector3 pos = windPrefab.transform.position + transform.up * windWidth;
            for(int i = 1; i < numWind; i++)
            {
                GameObject wind = Instantiate(windPrefab, pos, windPrefab.transform.rotation);
                windSprites[i] = wind;
                pos += transform.up * windWidth;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
                        if(go.tag == "Player" || go.tag == "Clone" || go.tag == "Enemy")
                        {
                            rb.AddForce(transform.up * 50);
                        }
                        else
                        {
                            rb.AddForce(transform.up * fanPower);
                        }
                    }
                    if(go.tag != "Player" && go.tag != "Clone" && go.tag != "Enemy")
                    {
                        AdjustMask(hit.distance);
                    }
                }
            }
        }
        if(objectsHit.Count == 0)
        {
            AdjustMask(fanDist);
        }

    }

    private void AdjustMask(float distToHit)
    {
        mask.transform.localScale = new Vector3(mask.transform.localScale.x, (distToHit / mask.sprite.bounds.size.y) / transform.localScale.y, 1);
        mask.transform.position = transform.position + transform.up * distToHit;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Vector2 v1 = transform.position - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        Vector2 v2 = transform.position + transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x * transform.localScale.x/2 - transform.right * GetComponent<SpriteRenderer>().sprite.bounds.size.x/24;
        Gizmos.DrawLine(v1, v1 + (Vector2) transform.up * fanDist);
        Gizmos.DrawLine(v2, v2 + (Vector2) transform.up * fanDist);
        float width = Vector2.Distance(v1, v2);
        for(float i = 0; i <= 6; i += 1)
        {
            Vector2 pos = v1 + new Vector2(transform.right.x, transform.right.y) * (i * width / 6);
            Gizmos.DrawLine(pos, pos + (Vector2) transform.up * fanDist);
        }
    }
}
