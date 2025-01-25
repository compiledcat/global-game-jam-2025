using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class GenerateCheckpoints : MonoBehaviour
{
    [SerializeField] SplineContainer path;
    [SerializeField] int checkpointCount;

    [ContextMenu("Generate")]
    void Generate()
    {

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
        }
        for (int i = 1; i < path.Splines.Count; i++)
        {
            float start, end;

            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, 0), out var links)) continue;

            foreach (var link in links)
            {
                Debug.Log(i + " " + link.Knot + " " + link.Spline);
            }

            var root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            start = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            if (!path.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(i, path.Splines[i].Count - 1), out links)) continue;

            root = links.Where(l => l.Spline == 0);

            if (root.Count() == 0) continue;

            end = path[0].ConvertIndexUnit(root.First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            for (int j = 0; j < checkpointCount; j++)
            {
                float t = j / (float)checkpointCount;
                if (start < end && (t > start && t < end) || start > end && (t > start && t < 1 || t > 0 && t < end))
                {
                    Gizmos.DrawSphere((Vector3)path.Splines[i].EvaluatePosition((t - start) / (end - start)) + path.transform.position, 0.1f);
                }
            }
        }
    }
}
