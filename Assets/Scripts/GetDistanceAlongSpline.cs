using UnityEngine;
using UnityEngine.Splines;

public class GetDistanceAlongSpline : MonoBehaviour
{
    [SerializeField] SplineContainer path;
    [SerializeField] float t;

    private void OnValidate()
    {
        if (path == null)
        {
            return;
        }
        //path.Spline.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
