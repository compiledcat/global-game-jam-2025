using Unity.Mathematics;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public class DuckController
{
    public int indexOfNextCheckpoint;
    public void CheckpointAdvance() { }
};

public class Checkpoint : MonoBehaviour
{

    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DuckController dc = other.GetComponent<DuckController>();
            if (dc.indexOfNextCheckpoint == checkpointIndex)
            {
                dc.CheckpointAdvance();
            }
        }
    }
}
