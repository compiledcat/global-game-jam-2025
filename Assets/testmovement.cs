using UnityEngine;

public class testmovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float height = WaterDisplacementManager.GetHeightAtPosition(new Vector2(transform.position.x, transform.position.z));
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
