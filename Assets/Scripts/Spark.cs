using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    #region Private fields
    private Animator animator;
    #endregion
    void Start()
    {
        float deviceWidth = transform.parent.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        transform.localPosition = new Vector3(Random.Range(-deviceWidth/2, deviceWidth/2), transform.localPosition.y, transform.localPosition.z);
        animator = GetComponent<Animator>();
        animator.SetFloat("startOffset", Random.Range(0.0f, 1.0f));
        animator.SetFloat("speed", Random.Range(0.5f, 2.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
