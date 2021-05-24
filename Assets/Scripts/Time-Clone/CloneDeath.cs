using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CloneDeath : MonoBehaviour
{

    #region Private fields
    private Animator animator;
    private Light2D light;
    private float lightIntensity;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        light = transform.GetChild(0).GetComponent<Light2D>();
        lightIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float animatorTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        light.intensity = lightIntensity * (1.0f - animatorTime);

        if(animatorTime >= 1)
        {
            Destroy(gameObject);
        }
    }

    public void SetColor(Color color)
    {
        Color temp = color;
        temp.a = 1;
        if(temp != Color.white)
        {
            GetComponent<SpriteRenderer>().color = color;
            light.color = color;
        }
    }
}
