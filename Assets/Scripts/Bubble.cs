using UnityEngine;

public class Bubble : MonoBehaviour
{
    //[SerializeField] int id;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        // enables gpu instancing - only compatible with webgl 2.0 and above
        GetComponent<MeshRenderer>().SetPropertyBlock(new MaterialPropertyBlock());
        //id = GetComponent<MeshRenderer>().GetInstanceID();
    }
}
