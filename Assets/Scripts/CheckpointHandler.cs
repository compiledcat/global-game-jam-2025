using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class CheckpointHandler : MonoBehaviour
{
    public static List<CheckpointScript>[] checkpoints;
    [SerializeField] SplineContainer path;
    [SerializeField] CheckpointScript checkpoint;
    [SerializeField] int checkpointCount;
    [SerializeField] float mergeDistance;


    [ContextMenu("Delete Checkpoints")]
    void Delete_Checkpoints()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // DestroyImmediate(transform.GetChild(i).gameObject);
            // change to work in both edit and play mode
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }


    [ContextMenu("Generate")]
    void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // DestroyImmediate(transform.GetChild(i).gameObject);
            // change to work in both edit and play mode
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        checkpoints = new List<CheckpointScript>[checkpointCount];
        if (path == null || checkpoint == null)
        {
            return;
        }

        for (int i = 0; i < checkpointCount; i++)
        {
            float t = i / (float)checkpointCount;
            var cp = Instantiate(checkpoint, (Vector3)path.Spline.EvaluatePosition(t) + path.transform.position,
                Quaternion.identity, transform);
            checkpoints[i] = new List<CheckpointScript> { cp };
            cp.checkpointIndex = i;
        }

        for (int i = 1; i < path.Splines.Count; i++)
        {
            float start, end;

            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, 0), out var links)) continue;

            var root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            start = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, path.Splines[i].Count - 1), out links))
                continue;
            root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            end = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            for (int j = 0; j < checkpointCount; j++)
            {
                float t = j / (float)checkpointCount;
                if (start < end && (t > start && t < end) || start > end && (t > start && t < 1 || t > 0 && t < end))
                {
                    var cp = Instantiate(checkpoint,
                        (Vector3)path.Splines[i].EvaluatePosition((t - start) / (end - start)) +
                        path.transform.position, Quaternion.identity, transform);
                    checkpoints[j].Add(cp);
                    cp.checkpointIndex = j;
                }
            }
        }

        for (int i = 0; i < checkpointCount; i++)
        {
            for (int j = 0; j < checkpoints[i].Count; j++)
            {
                for (int k = j + 1; k < checkpoints[i].Count; k++)
                {
                    if (Vector3.Distance(checkpoints[i][j].transform.position, checkpoints[i][k].transform.position) <
                        mergeDistance)
                    {
                        checkpoints[i][j].transform.position =
                            (checkpoints[i][j].transform.position + checkpoints[i][k].transform.position) / 2;
                        DestroyImmediate(checkpoints[i][k].gameObject);
                        checkpoints[i].RemoveAt(k);
                        k--;
                    }
                }
            }
        }
    }

    private void Awake()
    {
        /*for (int i = 0; i < checkpoints.GetLength(0); i++)
        {
            for (int j = 0; j < checkpoints[i].Length; j++)
            {
                var cp = checkpoints[i][j].GetComponent<CheckpointScript>();
                cp.checkpointIndex = i;
            }
        }*/
        
        Generate();
    }

    public int GetNext(int i)
    {
        return (i + 1) % checkpoints.Length;
    }

    public static Vector3 GetNearestCheckpoint(int index, Vector3 position)
    {
        index = index % checkpoints.Length;
        Vector3 nearest = checkpoints[index][0].transform.position;
        float minDist = Vector3.Distance(nearest, position);
        for (int i = 1; i < checkpoints[index].Count(); i++)
        {
            float dist = Vector3.Distance(checkpoints[index][i].transform.position, position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = checkpoints[index][i].transform.position;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        if (path == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < checkpointCount; i++)
        {
            float t = i / (float)checkpointCount;
            Gizmos.DrawSphere((Vector3)path.Spline.EvaluatePosition(t) + path.transform.position, 0.1f);
            Gizmos.DrawWireSphere((Vector3)path.Spline.EvaluatePosition(t) + path.transform.position, mergeDistance);
        }

        for (int i = 1; i < path.Splines.Count; i++)
        {
            float start, end;

            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, 0), out var links)) continue;

            // foreach (var link in links)
            // {
            //     Debug.Log(i + " " + link.Knot + " " + link.Spline);
            // }

            var root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            start = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, path.Splines[i].Count - 1), out links))
                continue;

            root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            end = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            for (int j = 0; j < checkpointCount; j++)
            {
                float t = j / (float)checkpointCount;
                if (start < end && (t > start && t < end) || start > end && (t > start && t < 1 || t > 0 && t < end))
                {
                    Gizmos.DrawSphere(
                        (Vector3)path.Splines[i].EvaluatePosition((t - start) / (end - start)) +
                        path.transform.position, 0.1f);
                    Gizmos.DrawWireSphere(
                        (Vector3)path.Splines[i].EvaluatePosition((t - start) / (end - start)) +
                        path.transform.position, mergeDistance);
                }
            }
        }
    }
}