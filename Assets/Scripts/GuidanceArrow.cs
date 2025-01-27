using UnityEngine;

public class GuidanceArrow : MonoBehaviour
{
    [SerializeField] DuckController duck;

    private void Update()
    {
        transform.LookAt(CheckpointHandler.GetNearestCheckpoint(duck.NextCheckpointIndex, duck.transform.position));
    }
}
