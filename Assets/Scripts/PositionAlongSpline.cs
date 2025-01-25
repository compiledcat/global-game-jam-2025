using UnityEngine;
using UnityEngine.Splines;

public class PositionAlongSpline : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField, Range(0, 1)] float percentage;
    [SerializeField] bool asPercentage;
    [SerializeField] SplineContainer path;

    private void OnValidate()
    {
        if (path == null)
        {
            return;
        }

        if (asPercentage)
        {
            transform.position = (Vector3)path.Spline.EvaluatePosition(percentage) + path.transform.position;
        }
        else
        {
            transform.position = (Vector3)path.Spline.EvaluatePosition(distance % path.Spline.GetLength() / path.Spline.GetLength()) + path.transform.position;
        }
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
