using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    public int checkpointIndex;
    public MeshRenderer meshRenderer1, meshRenderer2;

    private void Awake()
    {
        meshRenderer1.enabled = false;
        meshRenderer2.enabled = false;
    }
}
