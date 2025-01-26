using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class CornerGenerator : MonoBehaviour
{
    [SerializeField] SplineContainer path;
    [SerializeField] Transform rightCorner, leftCorner;
    [SerializeField] float cornerGap = 1, guideAngle = 30, displaceDist = 1.5f;
    [SerializeField, Min(0.1f)] float minDisplacementsFromBranch = 3;

    [ContextMenu("Delete Corners")]
    void Delete_Corners()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("Generate")]
    void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        if (path == null || rightCorner == null || leftCorner == null || cornerGap <= 0)
        {
            return;
        }

        for (int spline_index = 0; spline_index < path.Splines.Count; spline_index++)
        {

            int lastBranch = -1, nextBranch = -1;
            float lastBranchT = 0, nextBranchT = 0;
            var links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, 0));
            if (links.Count() > 0)
            {
                lastBranch = 0;
            }
            for (int knot_index = 1; knot_index < path.Splines[spline_index].Count; knot_index++)
            {
                links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, knot_index));
                if (links.Count() > 1)
                {
                    nextBranch = knot_index;
                    nextBranchT = path[spline_index].ConvertIndexUnit(links.Where(link => link.Spline == spline_index).First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                    break;
                }
                else if (knot_index == path.Splines.Count - 1)
                {
                    nextBranch = -1;
                }
            }
            

            float length = path.Splines[spline_index].GetLength();
            float tDelta = cornerGap / length;
            Vector3 lastTanget = path.Splines[spline_index].EvaluateTangent(0);
            for (float t = tDelta; t <= 1; t += tDelta)
            {
                float lastT = t - tDelta * minDisplacementsFromBranch, nextT = t + tDelta * minDisplacementsFromBranch;
                if (lastBranch != -1 && (lastT < lastBranchT && nextT > lastBranchT) || nextBranch != -1 && (lastT < nextBranchT && nextT > nextBranchT))
                {
                    continue;
                }
                else if (lastBranch != -1) lastBranch = -1;
                else if (nextBranch != -1 && t > nextBranchT)
                {
                    lastBranch = nextBranch;
                    for (int knot_index = nextBranch; knot_index < path.Splines[spline_index].Count; knot_index++)
                    {
                        links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, knot_index));
                        if (links.Count() > 1)
                        {
                            nextBranch = knot_index;
                            nextBranchT = path[spline_index].ConvertIndexUnit(links.Where(link => link.Spline == spline_index).First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                            break;
                        }
                        else if (knot_index == path.Splines[spline_index].Count - 1)
                        {
                            nextBranch = -1;
                        }
                    }
                }

                Vector3 tangent = path.Spline.EvaluateTangent(t);
                float angle = Vector3.SignedAngle(lastTanget, tangent, Vector3.up);
                if (Mathf.Abs(angle) > guideAngle)
                {
                    Vector3 position = (Vector3)path.Splines[spline_index].EvaluatePosition(t) + path.transform.position + transform.localPosition;
                    Vector3 displace = Vector3.Cross(tangent, Vector3.up).normalized * displaceDist;
                    position -= displace * (angle < 0 ? 1 : -1);
                    Quaternion rotation = Quaternion.LookRotation(tangent, Vector3.up) * Quaternion.Euler(0, angle < 0 ? -90 : 90, 0);
                    Instantiate(angle > 0 ? rightCorner : leftCorner, position, rotation, transform);
                }
                lastTanget = tangent;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (path == null || cornerGap <= 0)
        {
            return;
        }
        for (int spline_index = 0; spline_index < path.Splines.Count; spline_index++)
        {
            float length = path.Splines[spline_index].GetLength();
            float tDelta = cornerGap / length;
            Vector3 nextTangent = path.Splines[spline_index].EvaluateTangent(0);

            int lastBranch = -1, nextBranch = -1;
            float lastBranchT = 0, nextBranchT = 0;
            var links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, 0));
            if (links.Count() > 0)
            {
                lastBranch = 0;
            }
            for (int knot_index = 1; knot_index < path.Splines[spline_index].Count; knot_index++) {
                links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, knot_index));
                if (links.Count() > 1)
                {
                    nextBranch = knot_index;
                    nextBranchT = path[spline_index].ConvertIndexUnit(links.Where(link => link.Spline == spline_index).First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                    break;
                }
                else if (knot_index == path.Splines.Count - 1)
                {
                    nextBranch = -1;
                }
            }

            for (float t = 0; t <= 1; t += tDelta)
            {

                float lastT = t - tDelta * minDisplacementsFromBranch, nextT = t + tDelta * minDisplacementsFromBranch;
                if (lastBranch != -1 && (lastT < lastBranchT && nextT > lastBranchT) || nextBranch != -1 && (lastT < nextBranchT && nextT > nextBranchT))
                {
                    continue;
                }
                else if (lastBranch != -1) lastBranch = -1;
                else if (nextBranch != -1 && t > nextBranchT)
                {
                    lastBranch = nextBranch;
                    for (int knot_index = nextBranch; knot_index < path.Splines[spline_index].Count; knot_index++)
                    {
                        links = path.KnotLinkCollection.GetKnotLinks(new SplineKnotIndex(spline_index, knot_index));
                        if (links.Count() > 1)
                        {
                            nextBranch = knot_index;
                            nextBranchT = path[spline_index].ConvertIndexUnit(links.Where(link => link.Spline == spline_index).First().Knot, PathIndexUnit.Knot, PathIndexUnit.Normalized);
                            break;
                        }
                        else if (knot_index == path.Splines[spline_index].Count - 1)
                        {
                            nextBranch = -1;
                        }
                    }
                }

                Vector3 tangent = nextTangent;
                nextTangent = path.Spline.EvaluateTangent(t);
                float angle = Vector3.SignedAngle(tangent, nextTangent, Vector3.up);
                if (Mathf.Abs(angle) > guideAngle)
                {
                    Vector3 position = (Vector3)path.Splines[spline_index].EvaluatePosition(t) + path.transform.position + transform.localPosition;
                    Vector3 displace = Vector3.Cross(tangent, Vector3.up).normalized * displaceDist;
                    position -= displace * (angle < 0 ? 1 : -1);
                    Gizmos.DrawSphere(position, 1f);
                    Quaternion rotation = Quaternion.LookRotation(tangent, Vector3.up);
                    Gizmos.DrawRay(position, rotation * Vector3.forward);
                }
            }
        }
    }
}
