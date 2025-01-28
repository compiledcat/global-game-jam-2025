using UnityEngine;
using System.Linq;

public static class LeaderboardManager
{
    public static IOrderedEnumerable<DuckController> GetSortedPlayerArray()
    {
        DuckController[] players = Object.FindObjectsByType<DuckController>(FindObjectsSortMode.None);

        return players.OrderBy(player => -player.lapCounter).ThenBy(player => -(player.NextCheckpointIndex - 1)).ThenBy(player =>
        {
            var nextCheckpointPosition = CheckpointHandler.GetNearestCheckpoint(player.NextCheckpointIndex, player.transform.position);
            return Vector3.Distance(nextCheckpointPosition, player.transform.position);
        });
    }
}