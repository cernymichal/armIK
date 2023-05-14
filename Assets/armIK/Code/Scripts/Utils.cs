using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

public static class Utils {
    public static void GizmosDrawArc(Matrix4x4 localToWorld, Vector3 normal, Vector3 middle, float angle, Color color) {
        var segments = 16;
        var center = localToWorld.MultiplyPoint(Vector3.zero);

        var arcPoint = localToWorld.MultiplyPoint(Quaternion.AngleAxis(-angle / 2, normal) * middle);
        if (angle != 360) Gizmos.Line(center, arcPoint, color);
        for (float i = -segments / 2f + 1; i <= segments / 2f; i++) {
            var nextArcPoint = localToWorld.MultiplyPoint(Quaternion.AngleAxis(angle / segments * i, normal) * middle);
            Gizmos.Line(arcPoint, nextArcPoint, color);
            arcPoint = nextArcPoint;
        }
        if (angle != 360) Gizmos.Line(center, arcPoint, color);
    }
}
