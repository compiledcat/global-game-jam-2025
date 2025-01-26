using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public static DuckController[] GetSortedPlayerArray()
    {
        DuckController[] players = FindObjectsByType<DuckController>(FindObjectsSortMode.None);

        return players.OrderBy((player) =>
        {
            return -(player.NextCheckpointIndex - 1);
        }).ThenBy((player) =>
        {
            Vector3 nextCheckpointPosition = CheckpointHandler.GetNearestCheckpoint(player.NextCheckpointIndex, player.transform.position);
            return Vector3.Distance(nextCheckpointPosition, player.transform.position);
        }).ToArray();
    }
}