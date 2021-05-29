using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] private Transform[] _backgrounds;
    [SerializeField] private float smoothing;
    #endregion

    #region Private fields
    private float[] parallaxScales;
    private Vector3 previousCamPos;
    #endregion

    void Awake()
    {

    }

    void Start()
    {
        previousCamPos = Camera.main.transform.position;

        parallaxScales = new float[_backgrounds.Length];
        for(int i = 0; i < parallaxScales.Length; i++)
        {
            parallaxScales[i] = _backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _backgrounds.Length; i++)
        {
            // Parallax is opposite to the camera movement direction
            float parallax = (previousCamPos.x - Camera.main.transform.position.x) * parallaxScales[i];

            Vector3 newPos = new Vector3(_backgrounds[i].position.x + parallax, _backgrounds[i].position.y, _backgrounds[i].position.z);

            // Fade between two positions
            _backgrounds[i].position = Vector3.Lerp(_backgrounds[i].position, newPos, smoothing * Time.deltaTime);
        }

        // record the previous camera position
        previousCamPos = Camera.main.transform.position;
    }
}
