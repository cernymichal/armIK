using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static void GizmosDrawArc(Matrix4x4 localToWorld, Vector3 normal, Vector3 middle, float angle) {
        var segments = 16;
        var center = localToWorld.MultiplyPoint(Vector3.zero);

        var arcPoint = localToWorld.MultiplyPoint(Quaternion.AngleAxis(-angle / 2, normal) * middle);
        if (angle != 360) Gizmos.DrawLine(center, arcPoint);
        for (float i = -segments / 2f + 1; i <= segments / 2f; i++) {
            var nextArcPoint = localToWorld.MultiplyPoint(Quaternion.AngleAxis(angle / segments * i, normal) * middle);
            Gizmos.DrawLine(arcPoint, nextArcPoint);
            arcPoint = nextArcPoint;
        }
        if (angle != 360) Gizmos.DrawLine(center, arcPoint);
    }
}
