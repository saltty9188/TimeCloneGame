using UnityEngine;

/// <summary>
/// The Parallax class is responsible for making a psuedo parallax scrolling effect on the backgrounds during the ending.
/// </summary>
public class Parallax : MonoBehaviour
{
    #region Inspector fields
    [Tooltip("The background layers.")]
    [SerializeField] private Transform[] _backgrounds;
    [Tooltip("The parallax smoothing.")]
    [SerializeField] private float _smoothing = 2;
    #endregion

    #region Private fields
    private float[] _parallaxScales;
    private Vector3 _previousCamPos;
    #endregion

    void Start()
    {
        _previousCamPos = Camera.main.transform.position;

        _parallaxScales = new float[_backgrounds.Length];
        for(int i = 0; i < _parallaxScales.Length; i++)
        {
            _parallaxScales[i] = _backgrounds[i].position.z * -1;
        }
    }

    void Update()
    {
        for(int i = 0; i < _backgrounds.Length; i++)
        {
            // Parallax is opposite to the camera movement direction
            float parallax = (_previousCamPos.x - Camera.main.transform.position.x) * _parallaxScales[i];

            Vector3 newPos = new Vector3(_backgrounds[i].position.x + parallax, _backgrounds[i].position.y, _backgrounds[i].position.z);

            // Fade between two positions
            _backgrounds[i].position = Vector3.Lerp(_backgrounds[i].position, newPos, _smoothing * Time.deltaTime);
        }

        // record the previous camera position
        _previousCamPos = Camera.main.transform.position;
    }
}
