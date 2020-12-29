using UnityEditor;
using UnityEngine;

public class UnityAPITestDrawGizmo : MonoBehaviour
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Pickable)]
    static void DrawGizmoForThis(UnityAPITestDrawGizmoComponentTest info,GizmoType gizmoType)
    {
        Vector3 position = info.transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawCube(position, Vector3.one);
    }
}
