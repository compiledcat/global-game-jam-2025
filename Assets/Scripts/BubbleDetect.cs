using UnityEngine;

public class BubbleDetect : MonoBehaviour
{
    [SerializeField] Vector2 bounds;
    [SerializeField] float height, offset, scale, sizeVariance, spawnChance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector2Int resolution;
    [SerializeField] GameObject[] objectPrefab;
    [SerializeField] Transform objectParent;
    [SerializeField] bool autoGenerate, drawGizmos, insideTrack;
    //[SerializeField] bool[,] grid;


    [ContextMenu("Delete Objects")]
    void DeleteObjects()
    {
        for (int i = objectParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(objectParent.GetChild(i).gameObject);
        }
    }


    [ContextMenu("Generate Objects")]
    private void GenerateObjects()
    {
        if (Application.isPlaying)
        {
            return;
        }

        for (int i = objectParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(objectParent.GetChild(i).gameObject);
        }

        //grid = new bool[resolution.x, resolution.y];

        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                if (Random.value > spawnChance)
                {
                    continue;
                }

                Vector3 origin = new Vector3(
                    i * bounds.x / (resolution.x - 1) - bounds.x / 2 + transform.position.x + Random.Range(-offset, offset),
                    transform.position.z + height / 2,
                    j * bounds.y / (resolution.y - 1) - bounds.y / 2 + transform.position.z + Random.Range(-offset, offset)
                );
                //RaycastHit hit;
                bool hit = Physics.Raycast(origin, Vector3.down, out _, height, layerMask);
                if (hit == insideTrack)
                {
                    GameObject objectToSpawn = Instantiate(objectPrefab[Random.Range(0, objectPrefab.Length)], objectParent);
                    objectToSpawn.transform.localScale = Random.Range(1-sizeVariance, 1+sizeVariance) * scale * Vector3.one;
                    objectToSpawn.transform.position = new Vector3(
                        origin.x,
                        transform.position.y + objectToSpawn.transform.localScale.y / 2,
                        origin.z
                    );
                }
            }
        }
    }

    private void OnValidate()
    {
        if (autoGenerate)
        {
            GenerateObjects();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x, height, bounds.y));
        if (!drawGizmos)
        {
            return;
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                Vector3 origin = new Vector3(
                    i * bounds.x / (resolution.x-1) - bounds.x / 2 + transform.position.x,
                    transform.position.z + height / 2,
                    j * bounds.y / (resolution.y-1) - bounds.y / 2 + transform.position.z
                );

                Gizmos.DrawRay(origin, Vector3.down * height);
            }
        }
    }
}
